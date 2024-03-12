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
using static MCU_GroupTen.Models.Product;

namespace MCU_GroupTen.Controllers
{
    public class BackProductController : Controller
    {
        string ConnStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
        public ActionResult UploadProduct()
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
            int Vid = 1;
            SqlConnection conn = null;
            try
            {
                // 資料庫連線
                string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;

                // 註冊資料新增至資料庫
                string sql = "SELECT * FROM Store_Data";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                var model = new List<Proshop>();
                conn.Open();
                SqlDataReader s = cmd.ExecuteReader();
                while (s.Read())
                {
                    var Store = new Proshop();
                    Session["" + Vid] = (string)s["Store_Name"];
                    Vid++;
                }
                ViewData["upmsg"] = TempData["upmsg"];
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadProduct(HttpPostedFileBase File, Proshop inModel, string select, string SelectStore)
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
            //存到資料夾
            var FileName = Path.GetFileName(File.FileName);
            var FilePath = Path.Combine(Server.MapPath("~/Images/"), FileName);
            File.SaveAs(FilePath);
            DateTime upd = DateTime.Now;
            byte[] FileBytes;
            var ProductId = "";
            int RProductId = 0;
            int ProductSt=0;
            var Sid = "";
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
                string sql = "SELECT Top 1* FROM Products_Data ORDER BY Product_ID DESC";
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
                    ProductId = dt.Rows[0]["Product_ID"].ToString();
                    RProductId = int.Parse(ProductId) + 1;
                }
                else
                {
                    RProductId=1;
                }
                sql = @"Select Store_ID From Store_Data where Store_Name = @Store_Name";
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@Store_Name", SelectStore);
                Ret = cmd.ExecuteNonQuery();

                adpt = new SqlDataAdapter();
                adpt.SelectCommand = cmd;
                ds = new DataSet();
                adpt.Fill(ds);
                dt = ds.Tables[0];
                Model = new Proshop();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Sid = dt.Rows[0]["Store_ID"].ToString();
                }
                sql = @"INSERT INTO Products_Data (Product_ID,Product_Name,Product_Stock,Product_Price,Product_Picture,Product_Introduce,Product_Sales,Product_Origin,Product_Spec,Product_Type,Store_ID,Product_StDate,Product_Status) VALUES (@Product_ID,@Product_Name,@Product_Stock,@Product_Price,@Product_Picture,@Product_Introduce,@Product_Sales,@Product_Origin,@Product_Spec,@Product_Type,@Store_ID,@Product_StDate,@Product_Status)";
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                var sales = 0;

                // 使用參數化填值
                cmd.Parameters.AddWithValue("@Product_ID", RProductId);
                cmd.Parameters.AddWithValue("@Product_Name", inModel.Product_Name);
                cmd.Parameters.AddWithValue("@Product_Stock", inModel.Product_Stock);
                cmd.Parameters.AddWithValue("@Product_Price", inModel.Product_Price);
                cmd.Parameters.AddWithValue("@Product_Picture", FileBytes);
                cmd.Parameters.AddWithValue("@Product_Introduce", inModel.Product_Introduce);
                cmd.Parameters.AddWithValue("@Product_Sales", sales);
                cmd.Parameters.AddWithValue("@Product_Origin", inModel.Product_Origin);
                cmd.Parameters.AddWithValue("@Product_Spec", inModel.Product_Spec);
                cmd.Parameters.AddWithValue("@Product_Type", select);
                cmd.Parameters.AddWithValue("@Store_ID", Sid);
                cmd.Parameters.AddWithValue("@Product_StDate", upd);
                cmd.Parameters.AddWithValue("@Product_Status", ProductSt);

                // 執行資料庫更新動作
                cmd.ExecuteNonQuery();
                TempData["upmsg"] = "";
                TempData["upmsg"] = "新增成功";
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
            return RedirectToAction("UploadProduct");
        }
        // GET: Product
        public ActionResult SelectProduct()
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


                String sql = "select Product_ID,Product_Name,Product_Stock,Product_Price,Product_Picture,Product_Introduce,Product_Sales,Product_Origin,Product_Spec,Product_Type,Store_Name,Product_Status from Products_Data A left join Store_Data B on A.Store_ID=B.Store_ID order by B.Store_ID ASC";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                var model = new List<Proshop>();
                conn.Open();
                SqlDataReader s = cmd.ExecuteReader();
                while (s.Read())
                {
                    var Product = new Proshop();
                    Product.Product_ID = (int)s["Product_ID"];
                    Product.Product_Status = (int)s["Product_Status"];
                    Product.Product_Name = (string)s["Product_Name"];
                    Product.Product_Stock = (int)s["Product_Stock"];
                    Product.Product_Price = (int)s["Product_Price"];
                    Product.Product_Picture = (byte[])s["Product_Picture"];
                    Product.Product_Introduce = (string)s["Product_Introduce"];
                    Product.Product_Sales = (int)s["Product_Sales"];
                    Product.Product_Origin = (string)s["Product_Origin"];
                    Product.Product_Spec = (string)s["Product_Spec"];
                    Product.Product_Type = (string)s["Product_Type"];
                    Product.Store_Name = (string)s["Store_Name"];

                    model.Add(Product);
                }
                ViewData["upmsg"] = TempData["upmsg"];
                ViewData["delp"] = TempData["delp"];
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
        public ActionResult uptProduct(string Product_ID)
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "開發者")
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
            if (Product_ID == null)
            {
                return RedirectToAction("SelectProduct");
            }
            SqlConnection conn = null;
            try
            {
                string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;


                string sql = "select Product_ID,Product_Name,Product_Stock,Product_Price,Product_Picture,Product_Introduce,Product_Sales,Product_Origin,Product_Spec,Product_Type,A.Store_ID,Store_Name from Products_Data A left join Store_Data B on A.Store_ID=B.Store_ID where A.Product_ID=@Product_ID";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@Product_ID", Product_ID);

                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                DataTable dt = ds.Tables[0];
                Proshop model = new Proshop();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (dt.Rows[0]["Product_Picture"] != null)
                    {
                        model.Product_Picture = (byte[])dt.Rows[0]["Product_Picture"];
                    }
                    model.Product_ID = (int)dt.Rows[0]["Product_ID"];
                    model.Product_Name = dt.Rows[0]["Product_Name"].ToString();
                    model.Product_Stock = (int)dt.Rows[0]["Product_Stock"];
                    model.Product_Price = (int)dt.Rows[0]["Product_Price"];
                    model.Product_Introduce = dt.Rows[0]["Product_Introduce"].ToString();
                    model.Product_Sales = (int)dt.Rows[0]["Product_Sales"];
                    model.Product_Origin = dt.Rows[0]["Product_Origin"].ToString();
                    model.Product_Spec = dt.Rows[0]["Product_Spec"].ToString();
                    model.Product_Type = dt.Rows[0]["Product_Type"].ToString();
                    model.Store_ID = (int)dt.Rows[0]["Store_ID"];
                    model.Store_Name = dt.Rows[0]["Store_Name"].ToString();
                    Session["Product_ID"] = dt.Rows[0]["Product_ID"].ToString();
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult uptProduct(HttpPostedFileBase File, Proshop product, string select)
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "開發者")
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

                    string sql = "UPDATE Products_Data SET Product_Name = @Product_Name, Product_Stock = @Product_Stock, Product_Price = @Product_Price, Product_Introduce = @Product_Introduce, Product_Origin = @Product_Origin, Product_Spec = @Product_Spec, Product_Type = @Product_Type, Product_Picture= @Product_Picture WHERE Product_ID = @Product_ID";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@Product_Name", product.Product_Name);
                    cmd.Parameters.AddWithValue("@Product_Stock", product.Product_Stock);
                    cmd.Parameters.AddWithValue("@Product_Price", product.Product_Price);
                    cmd.Parameters.AddWithValue("@Product_Introduce", product.Product_Introduce);
                    cmd.Parameters.AddWithValue("@Product_Origin", product.Product_Origin);
                    cmd.Parameters.AddWithValue("@Product_Spec", product.Product_Spec);
                    cmd.Parameters.AddWithValue("@Product_Type", select);
                    cmd.Parameters.AddWithValue("@Product_ID", Session["Product_ID"]);
                    cmd.Parameters.AddWithValue("@Product_Picture", FileBytes);
                    cmd.ExecuteNonQuery();

                    TempData["upmsg"] = "";
                    TempData["upmsg"] = "更新成功";
                    return RedirectToAction("SelectProduct");
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

                    string sql = "UPDATE Products_Data SET Product_Name = @Product_Name, Product_Stock = @Product_Stock, Product_Price = @Product_Price, Product_Introduce = @Product_Introduce, Product_Origin = @Product_Origin, Product_Spec = @Product_Spec, Product_Type = @Product_Type WHERE Product_ID = @Product_ID";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@Product_Name", product.Product_Name);
                    cmd.Parameters.AddWithValue("@Product_Stock", product.Product_Stock);
                    cmd.Parameters.AddWithValue("@Product_Price", product.Product_Price);
                    cmd.Parameters.AddWithValue("@Product_Introduce", product.Product_Introduce);
                    cmd.Parameters.AddWithValue("@Product_Origin", product.Product_Origin);
                    cmd.Parameters.AddWithValue("@Product_Spec", product.Product_Spec);
                    cmd.Parameters.AddWithValue("@Product_Type", select);
                    cmd.Parameters.AddWithValue("@Product_ID", Session["Product_ID"]);
                    cmd.ExecuteNonQuery();

                    TempData["upmsg"] = "";
                    TempData["upmsg"] = "新增成功";
                    return RedirectToAction("SelectProduct");
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
        public ActionResult chgdnProduct(int Pid)
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

                String sql = "Update Products_Data SET Product_Status=1 where Product_ID = @Product_ID";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@Product_ID", Pid);
                cmd.ExecuteNonQuery();
                TempData["upmsg"] = "成功下架！";
                return RedirectToAction("SelectProduct");
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
        public ActionResult chgupProduct(int Pid)
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

                String sql = "Update Products_Data SET Product_Status=0 where Product_ID=@Product_ID";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@Product_ID", Pid);
                cmd.ExecuteNonQuery();
                TempData["upmsg"] = "成功恢復上架！";
                return RedirectToAction("SelectProduct");
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
        public ActionResult delProduct(int Product_ID)
        {
            if (Session["Member_identity"] != null)
            {
                if (Session["Member_identity"].ToString() == "開發者")
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
            BackProductController productController = new BackProductController();
            productController.delProductById(Product_ID);
            TempData["delp"] = "刪除成功";
            return RedirectToAction("SelectProduct");
        }

        public void delProductById(int Product_ID)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("DELETE FROM Products_Data WHERE Product_ID=@Product_ID");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Product_ID", Product_ID));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

    }
}