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
    public class BackMemberController : Controller
    {
        string ConnStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
        // GET: MemberBack
        public ActionResult SelectMember()
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


                String sql = "select * from Member_Data";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                var model = new List<MemberModel>();
                conn.Open();
                SqlDataReader s = cmd.ExecuteReader();
                while (s.Read())
                {
                    var Member = new MemberModel();
                    Member.Member_Account = (string)s["Member_Account"];
                    Member.Member_Mail = (string)s["Member_Mail"];
                    Member.Member_Phone = (string)s["Member_Phone"];
                    Member.Member_Birth = (DateTime)s["Member_Birth"];
                    Member.Member_Name = (string)s["Member_Name"];
                    Member.Member_Sex = (string)s["Member_Sex"];
                    Member.Member_Address = (string)s["Member_Address"];
                    Member.Member_identity = (string)s["Member_identity"];
                    Member.Member_Status = (string)s["Member_Status"];

                    model.Add(Member);
                }
                ViewData["ummsg"] = TempData["ummsg"];
                ViewData["delm"] = TempData["delm"];
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
        public ActionResult DetailMember(string Member_Account)
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
            if (Member_Account == null)
            {
                return RedirectToAction("SelectMember");
            }
            SqlConnection conn = null;
            try
            {
                String connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;


                string sql = "select * from Member_Data where Member_Account = @Member_Account";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                cmd.Parameters.AddWithValue("@Member_Account", Member_Account);
                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                DataTable dt = ds.Tables[0];
                MemberModel model = new MemberModel();
                model.Member_Account = (string)dt.Rows[0]["Member_Account"];
                model.Member_Mail = (string)dt.Rows[0]["Member_Mail"];
                model.Member_Phone = (string)dt.Rows[0]["Member_Phone"];
                model.Member_Birth = (DateTime)dt.Rows[0]["Member_Birth"];
                model.Member_Name = (string)dt.Rows[0]["Member_Name"];
                model.Member_Sex = (string)dt.Rows[0]["Member_Sex"];
                model.Member_Address = (string)dt.Rows[0]["Member_Address"];
                model.Member_identity = (string)dt.Rows[0]["Member_identity"];
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
        public ActionResult uptMember(string Member_Account)
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
            if (Member_Account == null)
            {
                return RedirectToAction("SelectMember");
            }
            SqlConnection conn = null;
            try
            {
                string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;


                string sql = "Select * from Member_Data where Member_Account=@Member_Account";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@Member_Account", Member_Account);

                Session["Back_Member_Account"] = Member_Account;

                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                DataTable dt = ds.Tables[0];
                MemberModel model = new MemberModel();
                model.Member_Account = (string)dt.Rows[0]["Member_Account"];
                model.Member_Mail = (string)dt.Rows[0]["Member_Mail"];
                model.Member_Phone = (string)dt.Rows[0]["Member_Phone"];
                model.Member_Birth = (DateTime)dt.Rows[0]["Member_Birth"];
                model.Member_Name = (string)dt.Rows[0]["Member_Name"];
                model.Member_Sex = (string)dt.Rows[0]["Member_Sex"];
                model.Member_Address = (string)dt.Rows[0]["Member_Address"];
                model.Member_identity = (string)dt.Rows[0]["Member_identity"];
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
        public ActionResult uptMember(MemberModel inModel, string select,DateTime Date)
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
                // 資料庫連線
                string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;
                conn.Open();

                // 檢查帳號、密碼是否正確
                string sql = "UPDATE Member_Data SET Member_Name = @Member_Name, Member_Mail = @Member_Mail, Member_Birth = @Member_Birth, Member_Address = @Member_Address, Member_Phone = @Member_Phone, Member_Sex = @Member_Sex, Member_identity = @Member_identity WHERE Member_Account = @Member_Account";
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                // 使用參數化填值
                cmd.Parameters.AddWithValue("@Member_Account", Session["Back_Member_Account"]);
                cmd.Parameters.AddWithValue("@Member_Name", inModel.Member_Name);
                cmd.Parameters.AddWithValue("@Member_Mail", inModel.Member_Mail);
                cmd.Parameters.AddWithValue("@Member_Birth", Date);
                cmd.Parameters.AddWithValue("@Member_Address", inModel.Member_Address);
                cmd.Parameters.AddWithValue("@Member_Phone", inModel.Member_Phone);
                cmd.Parameters.AddWithValue("@Member_Sex", inModel.Member_Sex);
                cmd.Parameters.AddWithValue("@Member_identity", select);


                // 執行資料庫查詢動作
                int Ret = cmd.ExecuteNonQuery();

                if (Ret > 0)
                {
                    TempData["ummsg"] = "";
                    TempData["ummsg"] = "更新成功";
                    return RedirectToAction("SelectMember", "BackMember");
                }
                else
                {
                    TempData["ummsg"] = "";
                    TempData["ummsg"] = "更新失敗";
                    return RedirectToAction("SelectMember", "BackMember");
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
        public ActionResult updMember(string Member_Account)
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
                DateTime dt = DateTime.Now;

                String sql = "Update Member_Data SET Member_Status=0 where Member_Account=@Member_Account";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@Member_Account", Member_Account);
                cmd.ExecuteNonQuery();
                TempData["delm"] = "成功恢復權限！";
                return RedirectToAction("SelectMember");
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
            //BackMemberController MemberController = new BackMemberController();
            //MemberController.delMemberById(Member_Account);
            //TempData["delm"] = "刪除成功";
            //return RedirectToAction("SelectMember");
        }
        public ActionResult delMember(string Member_Account)
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
                conn.Open();
                DateTime dt = DateTime.Now;

                String sql = "Update Member_Data SET Member_Status=1 where Member_Account=@Member_Account";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@Member_Account", Member_Account);
                cmd.ExecuteNonQuery();
                TempData["delm"] = "成功停權！";
                return RedirectToAction("SelectMember");
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
            //BackMemberController MemberController = new BackMemberController();
            //MemberController.delMemberById(Member_Account);
            //TempData["delm"] = "刪除成功";
            //return RedirectToAction("SelectMember");
        }

        //public void delMemberById(string Member_Account)
        //{
        //    SqlConnection sqlConnection = new SqlConnection(ConnStr);
        //    SqlCommand sqlCommand = new SqlCommand("DELETE FROM Member_Data WHERE Member_Account=@Member_Account");
        //    sqlCommand.Connection = sqlConnection;
        //    sqlCommand.Parameters.Add(new SqlParameter("@Member_Account", Member_Account));
        //    sqlCommand = new SqlCommand("DELETE FROM Store_Data WHERE Member_Account=@Member_Account");
        //    sqlCommand.Connection = sqlConnection;
        //    sqlCommand.Parameters.Add(new SqlParameter("@Member_Account", Member_Account));
        //    sqlConnection.Open();
        //    sqlCommand.ExecuteNonQuery();
        //    sqlConnection.Close();
        //}
    }
}