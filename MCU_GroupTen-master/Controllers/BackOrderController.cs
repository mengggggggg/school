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
    public class BackOrderController : Controller
    {
        string ConnStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;

        // GET: BackOrder
        public ActionResult SelectOrder()
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


                String sql = "select * from Order_Data";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                var model = new List<Cart>();
                conn.Open();
                SqlDataReader s = cmd.ExecuteReader();
                while (s.Read())
                {
                    var Cart = new Cart();
                    Cart.Order_ID = (int)s["Order_ID"];
                    Cart.Order_Total_Amount = (int)s["Order_Total_Amount"];
                    Cart.Order_Status = (string)s["Order_Status"];
                    Cart.Order_Date = (DateTime)s["Order_Date"];
                    Cart.Order_Address = (string)s["Order_Address"];
                    Cart.Order_Pay_Way = (string)s["Order_Pay_Way"];
                    Cart.Order_Delivery_Way = (string)s["Order_Delivery_Way"];
                    Cart.Order_Note = (string)s["Order_Note"];
                    Cart.Member_Account = (string)s["Member_Account"];

                    model.Add(Cart);
                }
                ViewData["uomsg"] = TempData["uomsg"];
                ViewData["delo"] = TempData["delo"];
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

        public ActionResult DetailOrder(string Order_ID)
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
            if (Order_ID == null)
            {
                return RedirectToAction("SelectOrder");
            }
            SqlConnection conn = null;
            try
            {
                String connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;


                string sql = "select * from Order_Details_Data where Order_ID = @Order_ID";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                cmd.Parameters.AddWithValue("@Order_ID", Order_ID);

                Session["Back_Order_ID"] = Order_ID;
                var model = new List<Cart>();
                conn.Open();
                SqlDataReader s = cmd.ExecuteReader();
                while (s.Read())
                {
                    var Cart = new Cart();
                    Cart.Order_Details_ID = (string)s["Order_Details_ID"];
                    Cart.Store_ID = (int)s["Store_ID"];
                    Cart.Store_Name = (string)s["Store_Name"];
                    Cart.Product_Name = (string)s["Product_Name"];
                    Cart.Product_ID = (int)s["Product_ID"];
                    Cart.Product_Spec = (string)s["Product_Spec"];
                    Cart.Product_Price = (int)s["Product_Price"];
                    Cart.Product_Count = (int)s["Product_Count"];
                    Cart.Order_Amount = (int)s["Order_Amount"];
                    Cart.Member_Account = (string)s["Member_Account"];
                    Cart.Order_ID = (int)s["Order_ID"];

                    model.Add(Cart);
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
        public ActionResult uptOrder(string Order_ID)
        {
            if(Session["Member_identity"] != null)
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
            if (Order_ID == null)
            {
                return RedirectToAction("SelectOrder");
            }
            SqlConnection conn = null;
            try
            {
                string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;


                string sql = "Select * from Order_Data where Order_ID=@Order_ID";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@Order_ID", Order_ID);

                Session["Back_Order_ID"] = Order_ID;

                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                DataTable dt = ds.Tables[0];
                Cart model = new Cart();
                model.Order_ID = (int)dt.Rows[0]["Order_ID"];
                model.Order_Total_Amount = (int)dt.Rows[0]["Order_Total_Amount"];
                model.Order_Status = (string)dt.Rows[0]["Order_Status"];
                model.Order_Date = (DateTime)dt.Rows[0]["Order_Date"];
                model.Order_Address = (string)dt.Rows[0]["Order_Address"];
                model.Order_Pay_Way = (string)dt.Rows[0]["Order_Pay_Way"];
                model.Order_Delivery_Way = (string)dt.Rows[0]["Order_Delivery_Way"];
                model.Order_Note = (string)dt.Rows[0]["Order_Note"];
                model.Member_Account = (string)dt.Rows[0]["Member_Account"];
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
        public ActionResult uptOrder(Cart inModel, string SelectStatus, string SelectPay_Way, string SelectDelivery_Way)
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
                string sql = "UPDATE Order_Data SET Order_Status = @Order_Status, Order_Address = @Order_Address, Order_Pay_Way = @Order_Pay_Way, Order_Delivery_Way = @Order_Delivery_Way, Order_Note = @Order_Note WHERE Order_ID = @Order_ID";
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                // 使用參數化填值
                cmd.Parameters.AddWithValue("@Order_ID", Session["Back_Order_ID"]);
                cmd.Parameters.AddWithValue("@Order_Status", SelectStatus);
                cmd.Parameters.AddWithValue("@Order_Address", inModel.Order_Address);
                cmd.Parameters.AddWithValue("@Order_Pay_Way", SelectPay_Way);
                cmd.Parameters.AddWithValue("@Order_Delivery_Way", SelectDelivery_Way);
                cmd.Parameters.AddWithValue("@Order_Note", inModel.Order_Note);


                // 執行資料庫查詢動作
                int Ret = cmd.ExecuteNonQuery();

                if (Ret > 0)
                {
                    TempData["uomsg"] = "";
                    TempData["uomsg"] = "更新成功";
                    return RedirectToAction("SelectOrder", "BackOrder");
                }
                else
                {
                    TempData["uomsg"] = "";
                    TempData["uomsg"] = "更新失敗";
                    return RedirectToAction("SelectOrder", "BackOrder");
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
        public ActionResult delOrder(string Order_ID)
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
            BackOrderController OrderController = new BackOrderController();
            OrderController.delOrderById(Order_ID);
            TempData["delo"] = "刪除成功";
            return RedirectToAction("SelectOrder");
        }

        public void delOrderById(string Order_ID)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("DELETE FROM Order_Data WHERE Order_ID=@Order_ID2 DELETE FROM Order_Details_Data WHERE Order_ID=@Order_ID");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Order_ID2", Order_ID));
            sqlCommand.Parameters.Add(new SqlParameter("@Order_ID", Order_ID));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
}