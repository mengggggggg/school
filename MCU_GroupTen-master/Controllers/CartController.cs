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
    public class CartController : Controller
    {
        // GET: Shopcar
            public ActionResult Cart()
        {

            if (Session["Member_Account"] == null || Session["Member_Account"].ToString() == "")
            {
                TempData["msg"] = "";
                TempData["msg"] = "您尚未登入";
                return RedirectToAction("Index", "Home");
            }

            SqlConnection conn = null;
            try
            {
                string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                conn = new SqlConnection();
                conn.ConnectionString = connStr;
                conn.Open();

                string sql = "select Product_Stock,Product_Status,Store_Status,Order_Details_ID,Order_Details_Data.Product_ID,Order_Details_Data.Product_Name,Order_Details_Data.Store_ID,Order_Details_Data.Store_Name,Order_Details_Data.Member_Account,Order_Details_Data.Product_Price,Order_Details_Data.Product_Spec,Order_Details_Data.Product_Count,Order_Details_Data.Order_Amount,Products_Data.Product_Picture from Order_Details_Data left join Products_Data ON Products_Data.Product_ID = Order_Details_Data.Product_ID left join Store_Data on Store_Data.Store_ID = Order_Details_Data.Store_ID  where Order_Details_Data.Member_Account = @Member_Account AND Order_ID is null order by Store_Name";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;


                cmd.Parameters.AddWithValue("@Member_Account", Session["Member_Account"]);


                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                DataTable dt = ds.Tables[0];

                var model = new List<Cart>();

                SqlDataReader s = cmd.ExecuteReader();



                if (ds.Tables[0].Rows.Count == 0)
                {
                    TempData["msg"] = "";
                    TempData["msg"] = "沒有商品在購物車";
                    TempData["cartmsg"] = "";
                    return RedirectToAction("Index", "Home");
                }



                while (s.Read())
                {
                    var Cart = new Cart();
                    if (s["Product_Status"].ToString() == "1")
                    {
                        ViewData["carerror"] = "商家或商品已下架或庫存不足，所以某些商品已移除!";
                        continue;
                    }
                    else if (s["Store_Status"].ToString() == "1")
                    {
                        ViewData["carerror"] = "商家或商品已下架或庫存不足，所以某些商品已移除!";
                        continue;
                    }
                    else if (s["Product_Stock"].ToString() == "0")
                    {
                        ViewData["carerror"] = "商家或商品已下架或庫存不足，所以某些商品已移除!";
                        continue;
                    }
                    Cart.Order_Details_ID = s["Order_Details_ID"].ToString();
                    Cart.Product_ID = (int)s["Product_ID"];
                    Cart.Store_ID = (int)s["Store_ID"];
                    Cart.Member_Account = s["Member_Account"].ToString();
                    Cart.Store_Name = s["Store_Name"].ToString();
                    Cart.Product_Name = s["Product_Name"].ToString();
                    Cart.Product_Price = (int)s["Product_Price"];
                    Cart.Product_Spec = s["Product_Spec"].ToString();
                    Cart.Product_Count = (int)s["Product_Count"];
                    Cart.Order_Amount = (int)s["Order_Amount"];
                    Cart.Product_Picture = (byte[])s["Product_Picture"];

                    model.Add(Cart);

                }
                ViewData["cartmsg"] = TempData["cartmsg"];
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