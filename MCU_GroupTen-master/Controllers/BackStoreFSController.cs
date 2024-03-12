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
    public class BackStoreFSController : Controller
    {
        string ConnStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
        // GET: BackStoreFS
        public ActionResult SelectforStore()
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "商家")
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


                String sql = "select * from Store_Data where Member_Account = @Member_Account";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                cmd.Parameters.AddWithValue("@Member_Account", Session["Member_Account"]);
                var model = new List<Proshop>();
                conn.Open();
                SqlDataReader s = cmd.ExecuteReader();
                while (s.Read())
                {
                    var Store = new Proshop();
                    Store.Store_ID = (int)s["Store_ID"];
                    Store.Store_Status = (int)s["Store_Status"];
                    Store.Store_Name = (string)s["Store_Name"];
                    Store.Store_Introduce = (string)s["Store_Introduce"];
                    Store.Store_Startdate = (DateTime)s["Store_Startdate"];
                    Store.Store_Picture = (byte[])s["Store_Picture"];
                    if ((string)s["Member_Account"] != null)
                    {
                        Store.Member_Account = (string)s["Member_Account"];
                    }

                    model.Add(Store);
                }
                ViewData["usmsg"] = TempData["usmsg"];
                ViewData["dels"] = TempData["dels"];
                ViewData["error_back"] = TempData["error_back"];
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

        public ActionResult chgdnStore(int Sid)
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "商家")
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

                String sql = "Update Store_Data SET Store_Status=1 where Store_ID=@Store_ID";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@Store_ID", Sid);
                cmd.ExecuteNonQuery();
                TempData["usmsg"] = "成功下架！";
                return RedirectToAction("SelectforStore");
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
        public ActionResult chgupStore(int Sid)
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "商家")
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

                String sql = "Update Store_Data SET Store_Status=0 where Store_ID=@Store_ID";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@Store_ID", Sid);
                cmd.ExecuteNonQuery();
                TempData["usmsg"] = "成功恢復上架！";
                return RedirectToAction("SelectforStore");
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
        public ActionResult UploadforStore(HttpPostedFileBase File, Proshop inModel)
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "商家")
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
            //存到資料夾
            var FileName = Path.GetFileName(File.FileName);
            var FilePath = Path.Combine(Server.MapPath("~/Images/"), FileName);
            File.SaveAs(FilePath);
            var StoreId = "";
            int RStoreId = 0;
            int StoreSt=0;
            byte[] FileBytes;
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
                string sql = "SELECT Top 1* FROM Store_Data ORDER BY Store_ID DESC";
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
                if (ds.Tables[0].Rows.Count > 0)
                {
                    StoreId = dt.Rows[0]["Store_ID"].ToString();
                    RStoreId = int.Parse(StoreId) + 1;
                }
                else
                {
                    RStoreId = 1;
                }
                // 註冊資料新增至資料庫
                sql = @"INSERT INTO Store_Data (Store_ID,Store_Name,Store_Startdate,Store_Picture,Store_Introduce,Member_Account,Store_Status) VALUES (@Store_ID,@Store_Name,@Store_Startdate,@Store_Picture,@Store_Introduce,@Member_Account,@Store_Status)";
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                // 使用參數化填值
                cmd.Parameters.AddWithValue("@Store_ID", RStoreId);
                cmd.Parameters.AddWithValue("@Store_Name", inModel.Store_Name);
                cmd.Parameters.AddWithValue("@Store_Picture", FileBytes);
                cmd.Parameters.AddWithValue("@Store_Status", StoreSt);
                cmd.Parameters.AddWithValue("@Store_Introduce", inModel.Store_Introduce);
                cmd.Parameters.AddWithValue("@Store_Startdate", inModel.Store_Startdate);
                cmd.Parameters.AddWithValue("@Member_Account", Session["Member_Account"]);

                // 執行資料庫更新動作
                cmd.ExecuteNonQuery();
                TempData["usmsg"] = "";
                TempData["usmsg"] = "新增成功";
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
            return RedirectToAction("UploadforStore");
        }
        public ActionResult UploadforStore()
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "商家")
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


                String sql = "select * from Store_Data where Member_Account = @Member_Account";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                cmd.Parameters.AddWithValue("@Member_Account", Session["Member_Account"]);
                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    TempData["error_back"] = "";
                    TempData["error_back"] = "您只能有一個商家!";
                    return RedirectToAction("SelectforStore", "BackStoreFS");
                }
                ViewData["usmsg"] = TempData["usmsg"];
                return View();
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
        public JsonResult check_Store_Name_Edit(Proshop model)
        {
            string result;
            SqlConnection conn = null;
            try
            {
                // 資料庫連線
                string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;
                conn.Open();

                // 檢查帳號、密碼是否正確
                string sql = "select * from Store_Data where Store_Name = @Store_Name";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                // 使用參數化填值
                cmd.Parameters.AddWithValue("@Store_Name", model.Store_Name);

                // 執行資料庫查詢動作
                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                DataTable dt = ds.Tables[0];
                Proshop models = new Proshop();
                if (ds.Tables[0].Rows.Count == 0)
                {
                    result = "Y";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    sql = "select * from Store_Data where Store_Name = @Store_Name AND Store_ID = @Store_ID";
                    cmd = new SqlCommand();
                    cmd.CommandText = sql;
                    cmd.Connection = conn;

                    // 使用參數化填值
                    cmd.Parameters.AddWithValue("@Store_Name", model.Store_Name);
                    cmd.Parameters.AddWithValue("@Store_ID", model.Store_ID);
                    // 執行資料庫查詢動作
                    adpt = new SqlDataAdapter();
                    adpt.SelectCommand = cmd;
                    ds = new DataSet();
                    adpt.Fill(ds);
                    dt = ds.Tables[0];
                    models = new Proshop();
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        result = "N";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        result = "Y";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
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
        public JsonResult check_Store_Name(Proshop model)
        {
            string result;
            SqlConnection conn = null;
            try
            {
                // 資料庫連線
                string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;
                conn.Open();

                // 檢查帳號、密碼是否正確
                string sql = "select * from Store_Data where Store_Name = @Store_Name";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                // 使用參數化填值
                cmd.Parameters.AddWithValue("@Store_Name", model.Store_Name);// 雜湊運算後密碼

                // 執行資料庫查詢動作
                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                DataTable dt = ds.Tables[0];
                MemberModel models = new MemberModel();
                if (ds.Tables[0].Rows.Count == 0)
                {
                    result = "Y";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    result = "N";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
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
        public ActionResult uptForStore(HttpPostedFileBase File, Proshop Store, DateTime Date)
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "商家")
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

                    string sql = "UPDATE Store_Data SET Store_Name = @Store_Name, Store_Picture = @Store_Picture, Store_Startdate = @Store_Startdate, Store_Introduce = @Store_Introduce WHERE Store_ID = @Store_ID";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@Store_Name", Store.Store_Name);
                    cmd.Parameters.AddWithValue("@Store_Introduce", Store.Store_Introduce);
                    cmd.Parameters.AddWithValue("@Store_Startdate", Date);
                    cmd.Parameters.AddWithValue("@Store_ID", Session["Store_ID"]);
                    cmd.Parameters.AddWithValue("@Store_Picture", FileBytes);
                    cmd.ExecuteNonQuery();
                    TempData["usmsg"] = "";
                    TempData["usmsg"] = "更新成功";
                    return RedirectToAction("SelectforStore");
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

                    string sql = "UPDATE Store_Data SET Store_Name = @Store_Name, Store_Startdate = @Store_Startdate, Store_Introduce = @Store_Introduce WHERE Store_ID = @Store_ID";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@Store_Name", Store.Store_Name);
                    cmd.Parameters.AddWithValue("@Store_Introduce", Store.Store_Introduce);
                    cmd.Parameters.AddWithValue("@Store_Startdate", Date);
                    cmd.Parameters.AddWithValue("@Store_ID", Session["Store_ID"]);
                    cmd.ExecuteNonQuery();

                    TempData["usmsg"] = "";
                    TempData["usmsg"] = "更新成功";
                    return RedirectToAction("SelectforStore");
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
        public ActionResult uptForStore(string Store_ID)
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "商家")
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
            if (Store_ID == null)
            {
                return RedirectToAction("SelectStore");
            }
            SqlConnection conn = null;
            try
            {
                string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;


                string sql = "select * from Store_Data where Store_ID = @Store_ID";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@Store_ID", Store_ID);

                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                DataTable dt = ds.Tables[0];
                Proshop model = new Proshop();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (dt.Rows[0]["Store_Picture"] != null)
                    {
                        model.Product_Picture = (byte[])dt.Rows[0]["Store_Picture"];
                    }
                    model.Store_ID = (int)dt.Rows[0]["Store_ID"];
                    model.Store_Name = dt.Rows[0]["Store_Name"].ToString();
                    model.Store_Introduce = dt.Rows[0]["Store_Introduce"].ToString();
                    model.Store_Startdate = (DateTime)dt.Rows[0]["Store_Startdate"];
                    Session["Store_ID"] = dt.Rows[0]["Store_ID"].ToString();
                    return View(model);
                }


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
        public ActionResult delForStore(int Store_ID)
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "商家")
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
            BackStoreFSController FSStoreController = new BackStoreFSController();
            FSStoreController.delForStoreById(Store_ID);
            TempData["dels"] = "刪除成功";
            return RedirectToAction("SelectforStore");
        }

        public void delForStoreById(int Store_ID)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("DELETE FROM Products_Data WHERE Store_ID=@Store_ID DELETE FROM Store_Data WHERE Store_ID=@Store_ID2");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Store_ID", Store_ID));
            sqlCommand.Parameters.Add(new SqlParameter("@Store_ID2", Store_ID));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
}