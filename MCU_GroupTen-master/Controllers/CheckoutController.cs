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
    public class CheckoutController : Controller
    {
        // GET: Checkout
        public ActionResult Checkoutsend(string checkBoxValue, FormCollection collection)
        {

            SqlConnection conn;
            string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
            conn = new SqlConnection();
            conn.ConnectionString = connStr;
            conn.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            //cmd.Transaction = conn.BeginTransaction();

            string[] checkboxarray = checkBoxValue.Split(',');

            string[] sncompare = new string[checkboxarray.Length];

            string[] order = new string[checkboxarray.Length];

            string[] amount = new string[checkboxarray.Length];

            string[] pay = new string[checkboxarray.Length];

            string[] way = new string[checkboxarray.Length];

            string[] add = new string[checkboxarray.Length];

            string[] note = new string[checkboxarray.Length];

            int sncount = 0;

            var OrderId = "";
            int rOrderId = 0;
            var sOrderId = "";


            try
            {
                //判斷有沒有選商品
                if (string.IsNullOrEmpty(checkBoxValue))
                {
                    return RedirectToAction("Index", "Home");
                }

                //更新庫存
                foreach (string checkbox in checkboxarray)
                {
                    string sql4 = @"SELECT Member_identity,Store_Status,Order_Details_ID, Products_Data.Product_ID, Product_Status, Product_Stock, Product_Sales, Products_Data.Product_Name, Product_Count FROM  Products_Data left join Order_Details_Data ON Products_Data.Product_ID = Order_Details_Data.Product_ID left join Store_Data ON Order_Details_Data.Store_ID=Store_Data.Store_ID left join Member_Data ON Store_Data.Member_Account=Member_Data.Member_Account WHERE Member_identity='商家' and Order_Details_Data.Order_Details_ID =" + checkbox;
                    cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql4;
                    int Pid=0;
                    string Pname="";
                    int Pstock=0;
                    int Pcount=0;
                    int Psales=0;
                    SqlDataReader s = cmd.ExecuteReader();
                    s.Read();
                    if (s.HasRows.Equals(true))
                    {
                        if (s["Product_Status"].ToString() == "1")
                        {
                            TempData["chkerror"] = "";
                            TempData["chkerror"] = "所選商品中已有商家或商品下架!請重新選取!";
                            return RedirectToAction("Index", "Home");
                        }
                        if (s["Store_Status"].ToString() == "1")
                        {
                            TempData["chkerror"] = "";
                            TempData["chkerror"] = "所選商品中已有商家或商品下架!請重新選取!";
                            return RedirectToAction("Index", "Home");
                        }
                        if (s["Product_Stock"].ToString() == "0")
                        {
                            TempData["chkerror"] = "";
                            TempData["chkerror"] = "所選商品中已有商品庫存不夠!請重新選取!";
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            Pid = (int)s["Product_ID"];
                            Pname = s["Product_Name"].ToString();
                            Pstock = (int)s["Product_Stock"];
                            Pcount = (int)s["Product_Count"];
                            Psales = (int)s["Product_Sales"];
                        }
                    }
                    else
                    {
                        TempData["chkerror"] = "";
                        TempData["chkerror"] = "所選商品中已有商家或商品下架!請重新選取!";
                        return RedirectToAction("Index", "Home");
                    }


                    s.Close();

                    string sql5 = "UPDATE Products_Data SET Product_Stock=@Product_Stock,Product_Sales=@Product_Sales WHERE Product_ID=@Product_ID";
                    cmd.CommandText = sql5;

                    if (Pcount > Pstock)
                    {
                        TempData["msg"] = "";
                        TempData["msg"] = "[" + Pname + "]" + " 商品庫存不夠只剩下數量:" + Pstock + "，請重新選擇";
                        return RedirectToAction("Index", "Home");
                    }

                    cmd.Parameters.AddWithValue("@Product_ID", Pid);
                    cmd.Parameters.AddWithValue("@Product_Stock", Pstock - Pcount);
                    cmd.Parameters.AddWithValue("@Product_Sales", Psales + Pcount);

                    cmd.ExecuteNonQuery();
                }


                //將訂單編號更新到Order_Details
                foreach (string checkbox in checkboxarray)
                {

                    string sql = @"SELECT Store_ID FROM Order_Details_Data where Order_Details_ID=@Order_Details_ID";
                    cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql;


                    cmd.Parameters.AddWithValue("@Order_Details_ID", checkbox);

                    SqlDataReader s = cmd.ExecuteReader();
                    s.Read();

                    sncompare[sncount] = s["Store_ID"].ToString();
                    //拿表單資料
                    pay[sncount] = collection[s["Store_ID"].ToString() + "pay"];
                    way[sncount] = collection[s["Store_ID"].ToString() + "way"];
                    add[sncount] = collection[s["Store_ID"].ToString() + "add"];
                    note[sncount] = collection[s["Store_ID"].ToString() + "note"];
                    int one = collection[s["Store_ID"].ToString() + "total"].ToString().LastIndexOf(",");
                    amount[sncount] = collection[s["Store_ID"].ToString() + "total"].ToString().Substring((one + 1));

                    s.Close();

                    //給訂單編號
                    string sql2 = "SELECT Top 1* FROM Order_Data ORDER BY Order_ID DESC";
                    cmd.CommandText = sql2;

                    SqlDataAdapter adpt = new SqlDataAdapter();
                    adpt.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    adpt.Fill(ds);
                    DataTable dt = ds.Tables[0];
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        OrderId = dt.Rows[0]["Order_ID"].ToString();
                        rOrderId = int.Parse(OrderId) + 1;
                    }
                    else
                    {
                        rOrderId = 1;
                    }

                    //找相同相家的最後一筆訂單
                    if (sncount > 0)
                    {
                        string sql3 = "SELECT Top 1* FROM Order_Details_Data where Store_ID=@Store_ID ORDER BY Order_ID DESC";
                        cmd.CommandText = sql3;

                        cmd.Parameters.AddWithValue("@Store_ID", sncompare[sncount - 1]);

                        SqlDataReader t = cmd.ExecuteReader();
                        t.Read();

                        sOrderId = t["Order_ID"].ToString();

                        t.Close();
                    }

                    //更新購物車商品內的Order_ID
                    string sql6 = "UPDATE Order_Details_Data SET Order_ID=@Order_ID WHERE Order_Details_ID=@Order_Details_ID";
                    cmd.CommandText = sql6;


                    //第一筆訂單不用比較直接更新資料庫
                    if (sncount == 0)
                    {
                        cmd.Parameters.AddWithValue("@Order_ID", rOrderId);
                    }
                    //有多筆訂單要比較才能更新
                    if (sncount > 0)
                    {
                        if (sncompare[sncount] != sncompare[sncount - 1])
                        {
                            cmd.Parameters.AddWithValue("@Order_ID", rOrderId);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@Order_ID", sOrderId);
                        }

                    }
                    cmd.ExecuteNonQuery();

                    //第一筆訂單不用比較直接更新資料庫
                    if (sncount == 0)
                    {
                        string sql7 = "INSERT INTO Order_Data(Order_ID, Order_Total_Amount, Order_Status, Order_Date, Order_Address, Order_Pay_Way, Order_Delivery_Way, Member_Account, Order_Note)VALUES(@Order_ID1, @Order_Total_Amount, @Order_Status, @Order_Date, @Order_Address, @Order_Pay_Way, @Order_Delivery_Way, @Member_Account, @Order_Note)";
                        cmd.CommandText = sql7;

                        cmd.Parameters.AddWithValue("@Order_ID1", rOrderId);
                        cmd.Parameters.AddWithValue("@Order_Date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Member_Account", Session["Member_Account"]);
                        cmd.Parameters.AddWithValue("@Order_Total_Amount", amount[sncount]);
                        cmd.Parameters.AddWithValue("@Order_Status", 1);// Order_Status = 1代表以建立訂單但未完成
                        cmd.Parameters.AddWithValue("@Order_Address", add[sncount]);
                        cmd.Parameters.AddWithValue("@Order_Pay_Way", pay[sncount]);
                        cmd.Parameters.AddWithValue("@Order_Delivery_Way", way[sncount]);
                        if (string.IsNullOrEmpty(note[sncount]))
                        {
                            cmd.Parameters.AddWithValue("@Order_Note", "無備註");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@Order_Note", note[sncount]);
                        }
                    }
                    //不同商家的商品要新增新的Order
                    else
                    {
                        if (sncompare[sncount] != sncompare[sncount - 1])
                        {
                            string sql8 = "INSERT INTO Order_Data(Order_ID, Order_Total_Amount, Order_Status, Order_Date, Order_Address, Order_Pay_Way, Order_Delivery_Way, Member_Account, Order_Note)VALUES(@Order_ID2, @Order_Total_Amount, @Order_Status, @Order_Date, @Order_Address, @Order_Pay_Way, @Order_Delivery_Way, @Member_Account, @Order_Note)";
                            cmd.CommandText = sql8;
                            cmd.Parameters.AddWithValue("@Order_ID2", rOrderId);
                            cmd.Parameters.AddWithValue("@Order_Date", DateTime.Now);
                            cmd.Parameters.AddWithValue("@Member_Account", Session["Member_Account"]);
                            cmd.Parameters.AddWithValue("@Order_Total_Amount", amount[sncount]);
                            cmd.Parameters.AddWithValue("@Order_Status", 1);// Order_Status = 1代表以建立訂單但未完成
                            cmd.Parameters.AddWithValue("@Order_Address", add[sncount]);
                            cmd.Parameters.AddWithValue("@Order_Pay_Way", pay[sncount]);
                            cmd.Parameters.AddWithValue("@Order_Delivery_Way", way[sncount]);
                            if (string.IsNullOrEmpty(note[sncount]))
                            {
                                cmd.Parameters.AddWithValue("@Order_Note", "無備註");
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@Order_Note", note[sncount]);
                            }
                        }
                    }
                    cmd.ExecuteNonQuery();

                    sncount++;
                }


                //cmd.Transaction.Commit();
            }





            catch (Exception ex)
            {
                //cmd.Transaction.Rollback();
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
            TempData["msg"] = "";
            TempData["msg"] = "訂單已建立";
            return RedirectToAction("Index", "Home");
        }






        public ActionResult Checkout(string checkBoxValue)//點購物車結帳按鈕執行的動作
        {
            SqlConnection conn = null;
            var model = new List<Checkout>();
            int number = 0;
            int totally = 0; //所有商品的總價相加

            try
            {
                //判斷有沒有選商品
                if (string.IsNullOrEmpty(checkBoxValue))
                {
                    TempData["cartmsg"] = "";
                    TempData["cartmsg"] = "請選擇商品";
                    return RedirectToAction("Cart", "Cart");
                }
                string[] checkboxarray = checkBoxValue.Split(',');

                string[] sncompare = new string[checkboxarray.Length];



                if (string.IsNullOrEmpty(checkBoxValue))
                {
                    TempData["cartmsg"] = "";
                    TempData["cartmsg"] = "請選擇商品";
                    return RedirectToAction("Cart", "Cart");
                }



                foreach (string checkbox in checkboxarray)
                {
                    string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
                    conn = new SqlConnection();
                    conn.ConnectionString = connStr;
                    conn.Open();

                    string sql = "select Order_Details_ID,Order_Details_Data.Product_ID,Order_Details_Data.Product_Name,Order_Details_Data.Store_ID,Order_Details_Data.Store_Name,Order_Details_Data.Member_Account,Order_Details_Data.Product_Price,Order_Details_Data.Product_Spec,Order_Details_Data.Product_Count,Order_Details_Data.Order_Amount,Products_Data.Product_Picture from Order_Details_Data left join Products_Data ON Products_Data.Product_ID = Order_Details_Data.Product_ID where Order_Details_ID =@Order_Details_ID order by Store_ID";

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = sql;
                    cmd.Connection = conn;

                    cmd.Parameters.AddWithValue("@Order_Details_ID", checkbox);
                    number++;
                    //每查詢完商品就number++，不然查不到下一筆資料就會return，沒辦法繼續跑foreach，最後讓number=需要查詢的筆數才return

                    SqlDataAdapter adpt = new SqlDataAdapter();
                    adpt.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    adpt.Fill(ds);
                    DataTable dt = ds.Tables[0];



                    SqlDataReader s = cmd.ExecuteReader();

                    while (s.Read())
                    {
                        var Checkout = new Checkout();

                        totally += (int)s["Order_Amount"];

                        Checkout.Order_Details_ID = s["Order_Details_ID"].ToString();
                        Checkout.Product_ID = (int)s["Product_ID"];
                        Checkout.Store_ID = (int)s["Store_ID"];
                        Checkout.Member_Account = s["Member_Account"].ToString();
                        Checkout.Store_Name = s["Store_Name"].ToString();
                        Checkout.Product_Name = s["Product_Name"].ToString();
                        Checkout.Product_Price = (int)s["Product_Price"];
                        Checkout.Product_Spec = s["Product_Spec"].ToString();
                        Checkout.Product_Count = (int)s["Product_Count"];
                        Checkout.Order_Amount = (int)s["Order_Amount"];
                        Checkout.Product_Picture = (byte[])s["Product_Picture"];

                        model.Add(Checkout);
                    }

                    ViewBag.totally = totally;

                    if (number == checkboxarray.Length)
                    {
                        return View(model);
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

            return View();
        }
    }
}