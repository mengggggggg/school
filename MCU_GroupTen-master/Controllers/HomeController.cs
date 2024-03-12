using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MCU_GroupTen.Models;
using static MCU_GroupTen.Models.Sort;

namespace MCU_GroupTen.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["error_MI"] = TempData["error_MI"];
            SqlConnection conn = null;
            try
            {
                String connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;
                DateTime dt = DateTime.Now;

                String sql = "Select Product_ID,Product_Name,Product_Stock,Product_Price,Product_Picture,Product_Introduce,Product_Sales,Product_Origin,Product_Spec,Product_Type,Product_StDate,A.Store_ID,B.Store_Name from Products_Data A, Store_Data B,Member_Data C where C.Member_Account=B.Member_Account and B.Store_ID=A.Store_ID and C.Member_Status='0' and C.Member_identity='商家' and A.Product_Status=0 and B.Store_Status=0 and A.Product_Sales>5 order by A.Product_Sales DESC";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                var model = new List<Products_Data>();
                conn.Open();
                SqlDataReader s = cmd.ExecuteReader();
                if (s.HasRows.Equals(true))
                {
                    Session["sales"] = "";
                    while (s.Read())
                    {
                        var Product = new Products_Data();
                        Product.Product_ID = (int)s["Product_ID"];
                        Product.Product_Name = (string)s["Product_Name"];
                        Product.Product_Stock = (int)s["Product_Stock"];
                        Product.Product_Price = (int)s["Product_Price"];
                        Product.Product_Picture = (byte[])s["Product_Picture"];
                        Product.Product_Introduce = (string)s["Product_Introduce"];
                        Product.Product_Sales = (int)s["Product_Sales"];
                        Product.Product_Origin = (string)s["Product_Origin"];
                        Product.Product_Spec = (string)s["Product_Spec"];
                        Product.Product_Type = (string)s["Product_Type"];
                        Product.Product_StDate = (DateTime)s["Product_StDate"];
                        Product.Store_ID = (int)s["Store_ID"];
                        Product.Store_Name = (string)s["Store_Name"];
                        model.Add(Product);
                    }
                }
                else {
                    Session["sales"] = "error";
                }
                sql = "select * from Activity_Data WHERE Activity_StartDate<= @Activity_StartDate and Activity_Status=0  order by Activity_StartDate";
                cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@Activity_StartDate", dt);
                s = cmd.ExecuteReader();
                var actmodel = new List<Act_Data>();
                if (s.HasRows.Equals(true))
                {
                    Session["act"] = "";
                    while (s.Read())
                    {
                        var Act = new Act_Data();
                        Act.Activity_ID = (int)s["Activity_ID"];
                        Act.Activity_Information = (string)s["Activity_Information"];
                        Act.Activity_URL = (string)s["Activity_URL"];
                        Act.Activity_StartDate = (DateTime)s["Activity_StartDate"];
                        Act.Activity_Picture = (byte[])s["Activity_Picture"];
                        actmodel.Add(Act);
                    }
                }
                else
                {
                    Session["act"] = "error";
                }
                sql = "Select * from Store_Data B,Member_Data C where C.Member_Account=B.Member_Account and B.Store_Status=0 and C.Member_Status='0' and C.Member_identity='商家'";
                cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                s = cmd.ExecuteReader();
                var stmodel = new List<Store_Data>();
                if (s.HasRows.Equals(true))
                {
                    Session["store"] = "";
                    while (s.Read())
                    {
                        var Store = new Store_Data();
                        Store.Store_ID = (int)s["Store_ID"];
                        Store.Store_Name = (string)s["Store_Name"];
                        Store.Store_Picture = (byte[])s["Store_Picture"];
                        Store.Store_Introduce = (string)s["Store_Introduce"];
                        stmodel.Add(Store);
                    }
                }
                else
                {
                    Session["store"] = "error";
                }
                

                sql = "Select Product_ID,Product_Name,Product_Stock,Product_Price,Product_Picture,Product_Introduce,Product_Sales,Product_Origin,Product_Spec,Product_Type,Product_StDate,A.Store_ID,B.Store_Name,DATEDIFF(day, Product_StDate, @DateTimeNow) AS minustime from Products_Data A, Store_Data B,Member_Data C where C.Member_Account=B.Member_Account and B.Store_ID=A.Store_ID and C.Member_Status='0' and A.Product_Status=0 and B.Store_Status=0 and C.Member_identity='商家' order by Product_StDate DESC";
                cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@DateTimeNow", dt);
                s = cmd.ExecuteReader();
                var dtmodel = new List<ProductsDate_Data>();
                while (s.Read())
                {
                    var Date = new ProductsDate_Data();
                    if ((int)s["minustime"] < 30)
                    {
                        DateTime dt2 = (DateTime)s["Product_StDate"];
                        Date.Product_ID = (int)s["Product_ID"];
                        Date.Product_Name = (string)s["Product_Name"];
                        Date.Product_Stock = (int)s["Product_Stock"];
                        Date.Product_Price = (int)s["Product_Price"];
                        Date.Product_Picture = (byte[])s["Product_Picture"];
                        Date.Product_Introduce = (string)s["Product_Introduce"];
                        Date.Product_Sales = (int)s["Product_Sales"];
                        Date.Product_Origin = (string)s["Product_Origin"];
                        Date.Product_Spec = (string)s["Product_Spec"];
                        Date.Product_Type = (string)s["Product_Type"];
                        Date.Store_ID = (int)s["Store_ID"];
                        Date.Store_Name = (string)s["Store_Name"];
                        dtmodel.Add(Date);
                    }
                    else
                    {
                        continue;
                    }
                }
                Session["date"] = "";
                if (dtmodel.Count == 0)
                {
                    Session["date"] = "error";
                }
                var rlmodel = new Sort();
                rlmodel.Index_Act_Data = actmodel;
                rlmodel.Date_Products_Data = dtmodel;
                rlmodel.Index_Products_Data = model;
                rlmodel.Index_Store_Data = stmodel;
                ViewData["logout"] = TempData["logout"];
                ViewData["msg"] = TempData["msg"];
                ViewData["chkerror"] = TempData["chkerror"];
                ViewData["error_acc"] = TempData["error_acc"];
                ViewData["PSerror"] = TempData["PSerror"];
                return View(rlmodel);
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
        public JsonResult check_identity(string identity)
        {
            var result = "";
            SqlConnection conn = null;
            if (Session["Member_Account"] == null || Session["Member_Account"].ToString() == "")
            {
                result = "Y";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else {

                try
                {
                    // 資料庫連線
                    string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                    conn = new SqlConnection();
                    conn.ConnectionString = connStr;
                    conn.Open();

                    // 檢查帳號、密碼是否正確
                    string sql = "select Member_identity,Member_Status from Member_Data where Member_Account = @Member_Account";
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = sql;
                    cmd.Connection = conn;

                    // 使用參數化填值
                    cmd.Parameters.AddWithValue("@Member_Account", Session["Member_Account"]);// 雜湊運算後密碼

                    // 執行資料庫查詢動作
                    SqlDataAdapter adpt = new SqlDataAdapter();
                    adpt.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    adpt.Fill(ds);
                    DataTable dt = ds.Tables[0];
                    MemberModel models = new MemberModel();

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (Session["Member_identity"].ToString() != dt.Rows[0]["Member_identity"].ToString())
                        {
                            Session.Clear();
                            Session.RemoveAll();
                            Response.Cookies.Clear();
                            TempData["error_acc"] = "";
                            TempData["error_acc"] = "帳號出現異常變更，請重新登入!";
                            result = "N";
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                        else if (Session["Member_Status"].ToString() != dt.Rows[0]["Member_Status"].ToString())
                        {
                            Session.Clear();
                            Session.RemoveAll();
                            Response.Cookies.Clear();
                            TempData["error_acc"] = "";
                            TempData["error_acc"] = "帳號出現異常變更，請重新登入!";
                            result = "N";
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            result = "Y";
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        result = "Y";
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
        }
        public ActionResult Search(string keyword)
        {
            SqlConnection conn = null;
            try
            {
                String connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;


                String sql = "Select Product_ID,Product_Name,Product_Stock,Product_Price,Product_Picture,Product_Introduce,Product_Sales,Product_Origin,Product_Spec,Product_Type,Product_StDate,A.Store_ID,B.Store_Name from Products_Data A inner join Store_Data B on A.Store_ID = B.Store_ID inner join Member_Data C on C.Member_Account=B.Member_Account and B.Store_ID=A.Store_ID and C.Member_Status='0' and C.Member_identity='商家' and A.Product_Status=0 and B.Store_Status=0 where A.Product_Name like '%' + @Product_Name + '%' or A.Product_Type like '%' + @Product_Type + '%' or B.Store_Name like '%' + @Store_Name + '%'";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                if (keyword == null || keyword == "")
                {
                    return RedirectToAction("Index");
                }
                Session["keyword"]= keyword;
                cmd.Parameters.AddWithValue("@Product_Name", keyword);
                cmd.Parameters.AddWithValue("@Product_Type", keyword);
                cmd.Parameters.AddWithValue("@Store_Name", keyword); 
                var model = new List<Proshop>();
                conn.Open();
                SqlDataReader s = cmd.ExecuteReader();
                while (s.Read())
                {
                    var Product = new Proshop();
                    Product.Product_Name = (string)s["Product_Name"];
                    Product.Store_ID = (int)s["Store_ID"];
                    Product.Product_ID = (int)s["Product_ID"];
                    Product.Product_Spec = (string)s["Product_Spec"];
                    Product.Product_Introduce = (string)s["Product_Introduce"];
                    Product.Product_Picture = (byte[])s["Product_Picture"];
                    Product.Product_Price = (int)s["Product_Price"];
                    Product.Store_Name = (string)s["Store_Name"];

                    model.Add(Product);
                }
                ViewData["msg"] = TempData["msg"];
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

        public ActionResult Searchtype(string Ptype)
        {
            SqlConnection conn = null;
            try
            {
                String connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;


                String sql = "Select Product_ID,Product_Name,Product_Stock,Product_Price,Product_Picture,Product_Introduce,Product_Sales,Product_Origin,Product_Spec,Product_Type,Product_StDate,A.Store_ID,B.Store_Name from Products_Data A, Store_Data B,Member_Data C where C.Member_Account=B.Member_Account and B.Store_ID=A.Store_ID and C.Member_Status='0' and C.Member_identity='商家' and A.Product_Status=0 and B.Store_Status=0 and Product_Type=@Product_Type";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                if (Ptype == null || Ptype == "")
                {
                    return RedirectToAction("Index");
                }
                Session["keyword"] = Ptype;
                cmd.Parameters.AddWithValue("@Product_Type", Ptype);
                var model = new List<Proshop>();
                conn.Open();
                SqlDataReader s = cmd.ExecuteReader();
                while (s.Read())
                {
                    var Product = new Proshop();
                    Product.Product_Name = (string)s["Product_Name"];
                    Product.Store_ID = (int)s["Store_ID"];
                    Product.Product_ID = (int)s["Product_ID"];
                    Product.Product_Price = (int)s["Product_Price"];
                    Product.Product_Introduce = (string)s["Product_Introduce"];
                    Product.Product_Spec = (string)s["Product_Spec"];
                    Product.Product_Picture = (byte[])s["Product_Picture"];
                    Product.Store_Name = (string)s["Store_Name"];

                    model.Add(Product);
                }
                ViewData["msg"] = TempData["msg"];
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
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Notice()
        {
            ViewBag.Message = "Your Notice page.";

            return View();
        }
    }
}