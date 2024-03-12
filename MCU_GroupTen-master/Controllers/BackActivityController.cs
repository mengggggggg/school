using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MCU_GroupTen.Models;

namespace MCU_GroupTen.Controllers
{
    public class BackActivityController : Controller
    {
        string ConnStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
        // GET: Activity
        public ActionResult SelectActivity()
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "平台管理員"||Session["Member_identity"].ToString() == "開發者")
                {

                }
                else
                {
                    TempData["error_MI"] = "權限不足!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (Session["Member_identity"] == null)
            {
                TempData["error_MI"] = "權限不足!";
                return RedirectToAction("Index", "Home");
            }
            SqlConnection conn = null;
            try
            {
                String connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;


                String sql = "SELECT * FROM Activity_Data";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                var model = new List<Act>();
                conn.Open();
                SqlDataReader s = cmd.ExecuteReader();
                if (s == null)
                {
                    return View();
                }
                else
                {
                    while (s.Read())
                    {
                        var Act = new Act();
                        Act.Activity_ID = (int)s["Activity_ID"];
                        Act.Activity_Status = (int)s["Activity_Status"];
                        Act.Activity_Information = (string)s["Activity_Information"];
                        Act.Activity_URL = (string)s["Activity_URL"];
                        Act.Activity_StartDate = (DateTime)s["Activity_StartDate"];
                        Act.Activity_Picture = (byte[])s["Activity_Picture"];

                        model.Add(Act);
                    }
                }
                ViewData["uamsg"] = TempData["uamsg"];
                ViewData["dela"] = TempData["dela"];
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    //關閉資料庫連線
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        public ActionResult UploadAct()
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "平台管理員"||Session["Member_identity"].ToString() == "開發者")
                {

                }
                else
                {
                    TempData["error_MI"] = "權限不足!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (Session["Member_identity"] == null)
            {
                TempData["error_MI"] = "權限不足!";
                return RedirectToAction("Index", "Home");
            }

            ViewData["uamsg"] = TempData["uamsg"];
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadAct(HttpPostedFileBase File, Act inModel)
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "平台管理員"||Session["Member_identity"].ToString() == "開發者")
                {

                }
                else
                {
                    TempData["error_MI"] = "權限不足!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (Session["Member_identity"] == null)
            {
                TempData["error_MI"] = "權限不足!";
                return RedirectToAction("Index", "Home");
            }
            var FileName = Path.GetFileName(File.FileName);
            var FilePath = Path.Combine(Server.MapPath("~/Images/"), FileName);
            File.SaveAs(FilePath);
            DateTime upd = DateTime.Now;
            byte[] FileBytes;
            var ActivityId = "";
            int RActivityId = 0;
            //轉成byte 方法一 直接轉
            using (MemoryStream ms = new MemoryStream())
            {
                File.InputStream.CopyTo(ms);
                FileBytes = ms.GetBuffer();
            }
            SqlConnection conn = null;
            try
            {
                // 資料庫連線
                string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;
                conn.Open();

                // 註冊資料新增至資料庫
                string sql = "SELECT Top 1* FROM Activity_Data ORDER BY Activity_ID DESC";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                int Ret = cmd.ExecuteNonQuery();

                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                DataTable dt = ds.Tables[0];
                Proshop Model = new Proshop();
                int Status=0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ActivityId = dt.Rows[0]["Activity_ID"].ToString();
                    RActivityId = int.Parse(ActivityId) + 1;
                }
                else
                {
                    RActivityId = 1;
                }
                sql = @"INSERT INTO Activity_Data (Activity_ID,Activity_Information,Activity_URL,Activity_StartDate,Activity_Picture,Activity_Status) VALUES (@Activity_ID,@Activity_Information,@Activity_URL,@Activity_StartDate,@Activity_Picture,@Activity_Status)";
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                // 使用參數化填值
                cmd.Parameters.AddWithValue("@Activity_ID", RActivityId);
                cmd.Parameters.AddWithValue("@Activity_Information", inModel.Activity_Information);
                cmd.Parameters.AddWithValue("@Activity_URL", inModel.Activity_URL);
                cmd.Parameters.AddWithValue("@Activity_StartDate", inModel.Activity_StartDate);
                cmd.Parameters.AddWithValue("@Activity_Picture", FileBytes);
                cmd.Parameters.AddWithValue("@Activity_Status", Status);

                // 執行資料庫更新動作
                cmd.ExecuteNonQuery();
                TempData["uamsg"] = "";
                TempData["uamsg"] = "新增成功";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    //關閉資料庫連線
                    conn.Close();
                    conn.Dispose();
                }
            }
            return RedirectToAction("UploadAct");
        }
        public ActionResult delAct(int Activity_ID)
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "平台管理員"||Session["Member_identity"].ToString() == "開發者")
                {

                }
                else
                {
                    TempData["error_MI"] = "權限不足!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (Session["Member_identity"] == null)
            {
                TempData["error_MI"] = "權限不足!";
                return RedirectToAction("Index", "Home");
            }
            BackActivityController ActivityController = new BackActivityController();
            ActivityController.delActById(Activity_ID);
            TempData["dela"] = "刪除成功";
            return RedirectToAction("SelectActivity");
        }

        public void delActById(int Activity_ID)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("DELETE FROM Activity_Data WHERE Activity_ID=@Activity_ID");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Activity_ID", Activity_ID));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }
        public ActionResult uptAct(string Activity_ID)
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "平台管理員"||Session["Member_identity"].ToString() == "開發者")
                {

                }
                else
                {
                    TempData["error_MI"] = "權限不足!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (Session["Member_identity"] == null)
            {
                TempData["error_MI"] = "權限不足!";
                return RedirectToAction("Index", "Home");
            }
            if (Activity_ID == null)
            {
                return RedirectToAction("SelectActivity");
            }
            SqlConnection conn = null;
            try
            {
                string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;


                string sql = "Select * from Activity_Data where Activity_ID=@Activity_ID";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@Activity_ID", Activity_ID);

                Session["Back_Activity_ID"] = Activity_ID;

                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                DataTable dt = ds.Tables[0];
                Act model = new Act();
                model.Activity_ID = (int)dt.Rows[0]["Activity_ID"];
                model.Activity_Information = (string)dt.Rows[0]["Activity_Information"];
                model.Activity_URL = (string)dt.Rows[0]["Activity_URL"];
                model.Activity_StartDate = (DateTime)dt.Rows[0]["Activity_StartDate"];
                model.Activity_Picture = (byte[])dt.Rows[0]["Activity_Picture"];
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    //關閉資料庫連線
                    conn.Close();
                    conn.Dispose();
                }
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult uptAct(HttpPostedFileBase File, Act inModel,DateTime Date)
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "平台管理員"||Session["Member_identity"].ToString() == "開發者")
                {

                }
                else
                {
                    TempData["error_MI"] = "權限不足!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (Session["Member_identity"] == null)
            {
                TempData["error_MI"] = "權限不足!";
                return RedirectToAction("Index", "Home");
            }
            SqlConnection conn = null;
            if (File != null)
            {
                var FileName = Path.GetFileName(File.FileName);
                var FilePath = Path.Combine(Server.MapPath("~/Images/"), FileName);
                File.SaveAs(FilePath);
                byte[] FileBytes;
                //轉成byte 方法一 直接轉
                using (MemoryStream ms = new MemoryStream())
                {
                    File.InputStream.CopyTo(ms);
                    FileBytes = ms.GetBuffer();
                }
                try
                {
                    // 資料庫連線
                    string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                    conn = new SqlConnection();
                    conn.ConnectionString = connStr;
                    conn.Open();

                    string sql = "UPDATE Activity_Data SET Activity_Information = @Activity_Information, Activity_URL = @Activity_URL, Activity_StartDate = @Activity_StartDate, Activity_Picture = @Activity_Picture WHERE Activity_ID = @Activity_ID";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@Activity_Information", inModel.Activity_Information);
                    cmd.Parameters.AddWithValue("@Activity_URL", inModel.Activity_URL);
                    cmd.Parameters.AddWithValue("@Activity_StartDate", Date);
                    cmd.Parameters.AddWithValue("@Activity_Picture", FileBytes);
                    cmd.Parameters.AddWithValue("@Activity_ID", Session["Back_Activity_ID"]);
                    cmd.ExecuteNonQuery();

                    TempData["uamsg"] = "";
                    TempData["uamsg"] = "更新成功";
                    return RedirectToAction("SelectActivity");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conn != null)
                    {
                        //關閉資料庫連線
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            else
            {
                try
                {
                    // 資料庫連線
                    string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                    conn = new SqlConnection();
                    conn.ConnectionString = connStr;
                    conn.Open();

                    string sql = "UPDATE Activity_Data SET Activity_Information = @Activity_Information, Activity_URL = @Activity_URL, Activity_StartDate = @Activity_StartDate WHERE Activity_ID = @Activity_ID";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@Activity_Information", inModel.Activity_Information);
                    cmd.Parameters.AddWithValue("@Activity_URL", inModel.Activity_URL);
                    cmd.Parameters.AddWithValue("@Activity_StartDate", Date);
                    cmd.Parameters.AddWithValue("@Activity_ID", Session["Back_Activity_ID"]);
                    cmd.ExecuteNonQuery();

                    TempData["uamsg"] = "";
                    TempData["uamsg"] = "更新成功";
                    return RedirectToAction("SelectActivity");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conn != null)
                    {
                        //關閉資料庫連線
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
        }

        public ActionResult chgdnAct(int Aid)
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "平台管理員" || Session["Member_identity"].ToString() == "開發者")
                {

                }
                else
                {
                    TempData["error_MI"] = "權限不足!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (Session["Member_identity"] == null)
            {
                TempData["error_MI"] = "權限不足!";
                return RedirectToAction("Index", "Home");
            }
            SqlConnection conn = null;
            try
            {
                String connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;
                conn.Open();

                String sql = "Update Activity_Data SET Activity_Status=1 where Activity_ID = @Activity_ID";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@Activity_ID", Aid);
                cmd.ExecuteNonQuery();
                TempData["uamsg"] = "成功結束活動！";
                return RedirectToAction("SelectActivity");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    //關閉資料庫連線
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        public ActionResult chgupAct(int Aid)
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "平台管理員" || Session["Member_identity"].ToString() == "開發者")
                {

                }
                else
                {
                    TempData["error_MI"] = "權限不足!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (Session["Member_identity"] == null)
            {
                TempData["error_MI"] = "權限不足!";
                return RedirectToAction("Index", "Home");
            }

            SqlConnection conn = null;
            try
            {
                String connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;
                conn.Open();

                String sql = "Update Activity_Data SET Activity_Status=0 where Activity_ID=@Activity_ID";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@Activity_ID", Aid);
                cmd.ExecuteNonQuery();
                TempData["uamsg"] = "成功恢復開始活動！";
                return RedirectToAction("SelectActivity");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    //關閉資料庫連線
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
    }
}