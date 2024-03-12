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

namespace MCU_GroupTen.Controllers
{
    public class ProshopController : Controller
    {
        // GET: ProShop 
        public ActionResult Product(string Pid, string Sid)
        {

            if (Sid == null || Pid == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["psmsg"] = TempData["psmsg"];
            SqlConnection conn = null;
            try
            {
                string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;


                string sql = "select * from Products_Data ,Store_Data left join Member_Data on Store_Data.Member_Account=Member_Data.Member_Account where Product_ID = @Product_ID and Store_Data.Store_ID = @Store_ID and Product_Status=0 and Store_Status=0 and Member_identity='商家' and Member_Status=0";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@Product_ID", Pid);
                cmd.Parameters.AddWithValue("@Store_ID", Sid);

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
                        model.Store_Picture = (byte[])dt.Rows[0]["Store_Picture"];
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
                    model.Store_Startdate = (DateTime)dt.Rows[0]["Store_Startdate"];
                    model.Store_Introduce = dt.Rows[0]["Store_Introduce"].ToString();
                    model.Product_Picture = (byte[])dt.Rows[0]["Product_Picture"];
                    return View(model);
                }
                else if (ds.Tables[0].Rows.Count == 0)
                {
                    TempData["PSerror"] = "商家或商品已下架";
                    return RedirectToAction("Index", "Home");
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
        public ActionResult Addcart(string Pn, string Sn, string Pid, string Sid, int Pc, string Ps, int Pp)
        {

            //庫存量
            int stock = 0;

            if (Session["Member_Account"] == null || Session["Member_Account"].ToString() == "")
            {
                TempData["msg"] = "";
                TempData["msg"] = "您尚未登入";
                return RedirectToAction("Index", "Home");
            }

            SqlConnection conn = null;
            try
            {
                //判斷庫存
                string connStr2 = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr2;
                conn.Open();
                string sql2 = "select Product_Stock from Products_Data where Product_ID=" + Pid;

                SqlCommand cmd2 = new SqlCommand();
                cmd2.CommandText = sql2;
                cmd2.Connection = conn;
                SqlDataReader s = cmd2.ExecuteReader();

                s.Read();
                stock = (int)s["Product_Stock"];
                if (stock <= 0 || stock < Pc)
                {
                    TempData["psmsg"] = "";
                    TempData["psmsg"] = "此商品庫存不足";
                    return RedirectToAction("Product", "Proshop", new { Pid = Pid, Sid = Sid });
                }

                //加入購物車
                string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;
                conn.Open();
                string sql = @"INSERT INTO Order_Details_Data(Order_Details_ID,Store_ID,Store_Name,Product_ID,Product_Name,Product_Spec,Product_Price,Product_Count,Order_Amount,Member_Account)VALUES(@Order_Details_ID,@Store_ID,@Store_Name,@Product_ID,@Product_Name,@Product_Spec,@Product_Price,@Product_Count,@Order_Amount,@Member_Account)";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;


                Random rand = new Random();
                int shu2 = rand.Next(10000, 99999);

                //產生亂數ID，使ID不重複
                string randId = DateTime.Now.ToString("yyyyMMddHHmm") + shu2;
                //總價Pa=商品價格Pp*商品數量Pc
                int Pa = Pp * Pc;
                Cart model = new Cart();

                cmd.Parameters.AddWithValue("@Order_Details_ID", randId);
                cmd.Parameters.AddWithValue("@Store_Name", Sn);
                cmd.Parameters.AddWithValue("@Store_ID", Sid);
                cmd.Parameters.AddWithValue("@Product_Name", Pn);
                cmd.Parameters.AddWithValue("@Product_ID", Pid);
                cmd.Parameters.AddWithValue("@Product_Spec", Ps);
                cmd.Parameters.AddWithValue("@Product_Price", Pp);
                cmd.Parameters.AddWithValue("@Product_Count", Pc);
                cmd.Parameters.AddWithValue("@Order_Amount", Pa);
                cmd.Parameters.AddWithValue("@Member_Account", Session["Member_Account"]);



                cmd.ExecuteNonQuery();
                TempData["psmsg"] = "";
                TempData["psmsg"] = "加入購物車成功";
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

            return RedirectToAction("Product", "Proshop", new { Pid = Pid, Sid = Sid });
        }
        public ActionResult Deletecart(string checkBoxValue)
        {
            SqlConnection conn = null;
            try
            {
                if (string.IsNullOrEmpty(checkBoxValue))
                {
                    TempData["cartmsg"] = "";
                    TempData["cartmsg"] = "請選擇商品";
                    return RedirectToAction("Cart", "Cart");
                }

                string[] checkboxarray = checkBoxValue.Split(',');

                foreach (string checkbox in checkboxarray)
                {

                    string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                    conn = new SqlConnection();
                    conn.ConnectionString = connStr;
                    conn.Open();
                    string sql = @"Delete from Order_Details_Data where Order_Details_ID = @Order_Details_ID";
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = sql;
                    cmd.Connection = conn;

                    Cart model = new Cart();

                    cmd.Parameters.AddWithValue("@Order_Details_ID", checkbox);
                    TempData["cartmsg"] = "";
                    TempData["cartmsg"] = "已刪除所選商品";

                    cmd.ExecuteNonQuery();
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

            return RedirectToAction("Cart", "Cart");
        }
        public ActionResult Store(string Sid)
        {
            if (Sid == null)
            {
                return RedirectToAction("Index", "Home");
            }

            SqlConnection conn = null;
            try
            {
                string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;


                string sql = "select * from Products_Data A,Store_Data B left join Member_Data on B.Member_Account=Member_Data.Member_Account where A.Product_Status=0 and B.Store_Status=0 and A.Store_ID = B.Store_ID and B.Store_ID=" + Sid;

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                DataTable dt = ds.Tables[0];

                var model = new List<Proshop>();
                conn.Open();
                SqlDataReader s = cmd.ExecuteReader();

                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "select * from Store_Data left join Member_Data on Store_Data.Member_Account=Member_Data.Member_Account where Store_Data.Store_ID=" + Sid;

                    cmd = new SqlCommand();
                    cmd.CommandText = sql;
                    cmd.Connection = conn;

                    adpt = new SqlDataAdapter();
                    adpt.SelectCommand = cmd;
                    ds = new DataSet();
                    adpt.Fill(ds);

                    dt = ds.Tables[0];
                    model = new List<Proshop>();
                    s = cmd.ExecuteReader();
                    while (s.Read())
                    {
                        var Proshop = new Proshop();
                        if (s["Store_Status"].ToString()=="1")
                        {
                            TempData["PSerror"] = "商家已下架";
                            return RedirectToAction("Index", "Home");
                        }
                        else if (s["Member_Status"].ToString() == "1")
                        {
                            TempData["PSerror"] = "商家已下架";
                            return RedirectToAction("Index", "Home");
                        }
                        else if (s["Member_identity"].ToString() != "商家")
                        {
                            TempData["PSerror"] = "商家已下架";
                            return RedirectToAction("Index", "Home");
                        }
                        Proshop.Store_Picture = (byte[])s["Store_Picture"];
                        Proshop.Store_ID = (int)s["Store_ID"];
                        Proshop.Store_Name = s["Store_Name"].ToString();
                        Proshop.Store_Startdate = (DateTime)s["Store_Startdate"];
                        Proshop.Store_Introduce = s["Store_Introduce"].ToString();
                        model.Add(Proshop);
                    }
                }
                else if (dt.Rows[0]["Product_ID"] != null)
                {

                    while (s.Read())
                    {
                        var Proshop = new Proshop();
                        if (s["Store_Status"].ToString() == "1")
                        {
                            TempData["PSerror"] = "商家已下架";
                            return RedirectToAction("Index", "Home");
                        }
                        else if (s["Member_Status"].ToString() == "1")
                        {
                            TempData["PSerror"] = "商家已下架";
                            return RedirectToAction("Index", "Home");
                        }
                        else if (s["Member_identity"].ToString() != "商家")
                        {
                            TempData["PSerror"] = "商家已下架";
                            return RedirectToAction("Index", "Home");
                        }
                        if (dt.Rows[0]["Store_Picture"] != null)
                        {
                            Proshop.Store_Picture = (byte[])dt.Rows[0]["Store_Picture"];
                        }
                        Proshop.Product_ID = (int)s["Product_ID"];
                        Proshop.Product_Name = (string)s["Product_Name"];
                        Proshop.Product_Stock = (int)s["Product_Stock"];
                        Proshop.Product_Price = (int)s["Product_Price"];
                        Proshop.Product_Picture = (byte[])s["Product_Picture"];
                        Proshop.Product_Introduce = (string)s["Product_Introduce"];
                        Proshop.Product_Sales = (int)s["Product_Sales"];
                        Proshop.Product_Origin = (string)s["Product_Origin"];
                        Proshop.Product_Spec = (string)s["Product_Spec"];
                        Proshop.Product_Type = (string)s["Product_Type"];
                        Proshop.Store_ID = (int)s["Store_ID"];
                        Proshop.Store_Name = s["Store_Name"].ToString();
                        Proshop.Store_Startdate = (DateTime)s["Store_Startdate"];
                        Proshop.Store_Introduce = s["Store_Introduce"].ToString();
                        model.Add(Proshop);
                    }
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
        public ActionResult Storesort(string Sid, string type, string price, string date, string sale)
        {
            if (Sid == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string sql = "";
            Session["sort"] = "";
            SqlConnection conn = null;
            try
            {
                string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;
                //商品類型
                if (type == "fruit")
                {
                    sql = "select * from Products_Data,Store_Data left join Member_Data on Store_Data.Member_Account=Member_Data.Member_Account where Products_Data.Product_Status=0 and Store_Data.Store_Status=0 and Member_identity='商家' and Member_Status=0 and Products_Data.Store_ID = Store_Data.Store_ID and Store_Data.Store_ID=" + Sid + "and Products_Data.Product_Type='水果'";
                }
                else if (type == "veg")
                {
                    sql = "select * from Products_Data,Store_Data left join Member_Data on Store_Data.Member_Account=Member_Data.Member_Account where Products_Data.Product_Status=0 and Store_Data.Store_Status=0 and Member_identity='商家' and Member_Status=0 and Products_Data.Store_ID = Store_Data.Store_ID and Store_Data.Store_ID=" + Sid + "and Products_Data.Product_Type='蔬菜'";
                }
                else if (type == "meat")
                {
                    sql = "select * from Products_Data,Store_Data left join Member_Data on Store_Data.Member_Account=Member_Data.Member_Account where Products_Data.Product_Status=0 and Store_Data.Store_Status=0 and Member_identity='商家' and Member_Status=0 and Products_Data.Store_ID = Store_Data.Store_ID and Store_Data.Store_ID=" + Sid + "and Products_Data.Product_Type='肉類'";
                }

                //商品價格大到小
                if (price == "pdesc")
                {
                    sql = "select * from Products_Data,Store_Data left join Member_Data on Store_Data.Member_Account=Member_Data.Member_Account where Products_Data.Product_Status=0 and Store_Data.Store_Status=0 and Member_identity='商家' and Member_Status=0 and Products_Data.Store_ID = Store_Data.Store_ID and Store_Data.Store_ID=" + Sid + " order by Product_Price DESC ";
                }
                //商品價格小到大
                else if (price == "pasc")
                {
                    sql = "select * from Products_Data,Store_Data left join Member_Data on Store_Data.Member_Account=Member_Data.Member_Account where Products_Data.Product_Status=0 and Store_Data.Store_Status=0 and Member_identity='商家' and Member_Status=0 and Products_Data.Store_ID = Store_Data.Store_ID and Store_Data.Store_ID=" + Sid + " order by Product_Price ASC ";
                }
                //上架日期晚到早
                if (date == "ddesc")
                {
                    sql = "select * from Products_Data,Store_Data left join Member_Data on Store_Data.Member_Account=Member_Data.Member_Account where Products_Data.Product_Status=0 and Store_Data.Store_Status=0 and Member_identity='商家' and Member_Status=0 and Products_Data.Store_ID = Store_Data.Store_ID and Store_Data.Store_ID=" + Sid + " order by Product_StDate DESC ";
                }
                //上架日期早到晚
                else if (date == "dasc")
                {
                    sql = "select * from Products_Data,Store_Data left join Member_Data on Store_Data.Member_Account=Member_Data.Member_Account where Products_Data.Product_Status=0 and Store_Data.Store_Status=0 and Member_identity='商家' and Member_Status=0 and Products_Data.Store_ID = Store_Data.Store_ID and Store_Data.Store_ID=" + Sid + " order by Product_StDate ASC ";
                }
                //銷量多到少
                if (sale == "sdesc")
                {
                    sql = "select * from Products_Data,Store_Data left join Member_Data on Store_Data.Member_Account=Member_Data.Member_Account where Products_Data.Product_Status=0 and Store_Data.Store_Status=0 and Member_identity='商家' and Member_Status=0 and Products_Data.Store_ID = Store_Data.Store_ID and Store_Data.Store_ID=" + Sid + " order by Product_Sales DESC ";
                }
                //銷量少到多
                else if (sale == "sasc")
                {
                    sql = "select * from Products_Data,Store_Data left join Member_Data on Store_Data.Member_Account=Member_Data.Member_Account where Products_Data.Product_Status=0 and Store_Data.Store_Status=0 and Member_identity='商家' and Member_Status=0 and Products_Data.Store_ID = Store_Data.Store_ID and Store_Data.Store_ID=" + Sid + " order by Product_Sales ASC ";
                }


                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                DataTable dt = ds.Tables[0];

                var model = new List<Proshop>();
                conn.Open();
                SqlDataReader s = cmd.ExecuteReader();

                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "select * from Products_Data,Store_Data left join Member_Data on Store_Data.Member_Account=Member_Data.Member_Account where Products_Data.Product_Status=0 and Store_Data.Store_Status=0 and Member_identity='商家' and Member_Status=0 and Products_Data.Store_ID = Store_Data.Store_ID and Store_Data.Store_ID=" + Sid;
                    cmd = new SqlCommand();
                    cmd.CommandText = sql;
                    cmd.Connection = conn;
                    Session["sort"] = "error";

                    adpt = new SqlDataAdapter();
                    adpt.SelectCommand = cmd;
                    ds = new DataSet();
                    adpt.Fill(ds);
                    dt = ds.Tables[0];

                    s = cmd.ExecuteReader();
                    if (s.HasRows.Equals(false))
                    {
                        TempData["PSerror"] = "商家已下架";
                        return RedirectToAction("Index", "Home");
                    }
                    while (s.Read())
                    {
                        var Proshop = new Proshop();
                        if (dt.Rows[0]["Store_Status"].ToString() == "1")
                        {
                            TempData["PSerror"] = "商家已下架";
                            return RedirectToAction("Index", "Home");
                        }
                        Proshop.Store_Picture = (byte[])dt.Rows[0]["Store_Picture"];
                        Proshop.Product_ID = (int)s["Product_ID"];
                        Proshop.Product_Name = (string)s["Product_Name"];
                        Proshop.Product_Stock = (int)s["Product_Stock"];
                        Proshop.Product_Price = (int)s["Product_Price"];
                        Proshop.Product_Picture = (byte[])s["Product_Picture"];
                        Proshop.Product_Introduce = (string)s["Product_Introduce"];
                        Proshop.Product_Sales = (int)s["Product_Sales"];
                        Proshop.Product_Origin = (string)s["Product_Origin"];
                        Proshop.Product_Spec = (string)s["Product_Spec"];
                        Proshop.Product_Type = (string)s["Product_Type"];
                        Proshop.Store_ID = (int)s["Store_ID"];
                        Proshop.Store_Name = s["Store_Name"].ToString();
                        Proshop.Store_Startdate = (DateTime)s["Store_Startdate"];
                        Proshop.Store_Introduce = s["Store_Introduce"].ToString();
                        model.Add(Proshop);
                    }
                }
                else
                {
                    while (s.Read())
                    {
                        var Proshop = new Proshop();
                        if (dt.Rows[0]["Store_Picture"] != null)
                        {
                            Proshop.Store_Picture = (byte[])dt.Rows[0]["Store_Picture"];
                        }
                        Proshop.Product_ID = (int)s["Product_ID"];
                        Proshop.Product_Name = (string)s["Product_Name"];
                        Proshop.Product_Stock = (int)s["Product_Stock"];
                        Proshop.Product_Price = (int)s["Product_Price"];
                        Proshop.Product_Picture = (byte[])s["Product_Picture"];
                        Proshop.Product_Introduce = (string)s["Product_Introduce"];
                        Proshop.Product_Sales = (int)s["Product_Sales"];
                        Proshop.Product_Origin = (string)s["Product_Origin"];
                        Proshop.Product_Spec = (string)s["Product_Spec"];
                        Proshop.Product_Type = (string)s["Product_Type"];
                        Proshop.Store_ID = (int)s["Store_ID"];
                        Proshop.Store_Name = s["Store_Name"].ToString();
                        Proshop.Store_Startdate = (DateTime)s["Store_Startdate"];
                        Proshop.Store_Introduce = s["Store_Introduce"].ToString();
                        model.Add(Proshop);
                    }
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
    }
}