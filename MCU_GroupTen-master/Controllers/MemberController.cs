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
using static MCU_GroupTen.Models.MemberModel;

namespace MCU_GroupTen.Controllers
{
	public class MemberController : Controller
	{

		// GET: Member
		public ActionResult Register()
		{
			ViewData["RGmsg"] = TempData["RGmsg"];
			//ViewData["Message"] = TempData["Message"];
			return View();
		}
		public ActionResult Member_Order()
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

				string sql = "select Order_Details_ID,Order_Details_Data.Store_ID,Store_Name,Order_Details_Data.Product_ID,Order_Details_Data.Product_Name,Order_Details_Data.Product_Spec,Order_Details_Data.Product_Price,Product_Count,Order_Amount,Order_Details_Data.Order_ID,Order_Details_Data.Member_Account,Order_Total_Amount,Order_Status,Order_Date,Order_Delivery_Status,Order_Payment_Status,Order_Address,Order_Pay_Way,Order_Delivery_Way,Order_Note,Products_Data.Product_Picture from Order_Details_Data left join Order_Data on Order_Details_Data.Order_ID = Order_Data.Order_ID left join Products_Data on Order_Details_Data.Product_ID=Products_Data.Product_ID where Order_Details_Data.Member_Account = @Member_Account AND Order_Details_Data.Order_ID is not null order by Order_Details_Data.Order_ID";

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
					TempData["ctmsg"] = "";
					TempData["ctmsg"] = "沒有任何訂單紀錄";
					return RedirectToAction("Member_Center", "Member");
				}
				while (s.Read())
				{
					if (s["Order_Status"].ToString()== "0")
					{
						continue;
					}
					var Cart = new Cart();
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
					Cart.Order_ID = (int)s["Order_ID"];
					Cart.Order_Total_Amount = (int)s["Order_Total_Amount"];
					Cart.Order_Status = s["Order_Status"].ToString();
					Cart.Order_Address = s["Order_Address"].ToString();
					Cart.Order_Delivery_Way = s["Order_Delivery_Way"].ToString();
					Cart.Order_Pay_Way = s["Order_Pay_Way"].ToString();
					Cart.Order_Note = s["Order_Note"].ToString();
                    Cart.Product_Picture = (byte[])s["Product_Picture"];

					model.Add(Cart);

				}
				if (model.Count == 0)
				{
					TempData["ctmsg"] = "";
					TempData["ctmsg"] = "沒有任何訂單紀錄";
					return RedirectToAction("Member_Center", "Member");
				}
				ViewData["cartmsg"] = TempData["cartmsg"];
				ViewData["ummsg"] = TempData["ummsg"];
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
		public ActionResult Member_Questions()
		{
			if (Session["Member_Account"] == null || Session["Member_Account"].ToString() == "")
			{
				TempData["msg"] = "";
				TempData["msg"] = "您尚未登入";
				return RedirectToAction("Index", "Home");
			}
			//ViewData["Message"] = TempData["Message"];
			return View();
		}
		public ActionResult Member_Change()
		{
			if (Session["Member_Account"] == null || Session["Member_Account"].ToString() == "")
			{
				TempData["msg"] = "";
				TempData["msg"] = "您尚未登入";
				return RedirectToAction("Index", "Home");
			}
			//ViewData["Message"] = TempData["Message"];
			return View();
		}
		public JsonResult Check_Original_FS(MemberModel model)
		{
			string result;
			model.New_Password = Request["member_password"];
			SqlConnection conn = null;
			try
			{
				// 資料庫連線
				string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
				conn = new SqlConnection();
				conn.ConnectionString = connStr;
				conn.Open();

				// 將密碼轉為 SHA256 雜湊運算(不可逆)
				string salt = Session["Member_Account"].ToString().Substring(0, 1).ToLower(); //使用帳號前一碼當作密碼鹽
				SHA256 sha256 = SHA256.Create();
				byte[] bytes = Encoding.UTF8.GetBytes(salt + model.New_Password); //將密碼鹽及原密碼組合
				byte[] hash = sha256.ComputeHash(bytes);
				StringBuilder results = new StringBuilder();
				for (int i = 0; i < hash.Length; i++)
				{
					results.Append(hash[i].ToString("X2"));
				}
				string CheckPwd = results.ToString(); // 雜湊運算後密碼

				// 檢查帳號、密碼是否正確
				string sql = "select * from Member_Data where Member_Account = @Member_Account";
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
					// 有查詢到資料，表示帳號密碼正確
					if (CheckPwd == dt.Rows[0]["Member_Password"].ToString())
					{
						result = "N";
						return Json(result, JsonRequestBehavior.AllowGet);
					}
					else if (CheckPwd != dt.Rows[0]["Member_Password"].ToString())
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
		public JsonResult Check_Original_PW(MemberModel model)
		{
			string result;
			model.Before_Password = Request["user_passwd"];
			SqlConnection conn = null;
			try
			{
				// 資料庫連線
				string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
				conn = new SqlConnection();
				conn.ConnectionString = connStr;
				conn.Open();

				// 將密碼轉為 SHA256 雜湊運算(不可逆)
				string salt = Session["Member_Account"].ToString().Substring(0, 1).ToLower(); //使用帳號前一碼當作密碼鹽
				SHA256 sha256 = SHA256.Create();
				byte[] bytes = Encoding.UTF8.GetBytes(salt + model.Before_Password); //將密碼鹽及原密碼組合
				byte[] hash = sha256.ComputeHash(bytes);
				StringBuilder results = new StringBuilder();
				for (int i = 0; i < hash.Length; i++)
				{
					results.Append(hash[i].ToString("X2"));
				}
				string CheckPwd = results.ToString(); // 雜湊運算後密碼

				// 檢查帳號、密碼是否正確
				string sql = "select * from Member_Data where Member_Account = @Member_Account";
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
					// 有查詢到資料，表示帳號密碼正確
					if (CheckPwd != dt.Rows[0]["Member_Password"].ToString())
					{
						result = "N";
						return Json(result, JsonRequestBehavior.AllowGet);
					}
					else if (CheckPwd == dt.Rows[0]["Member_Password"].ToString())
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
		public ActionResult Member_Change(MemberModel Pwd)
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
				// 資料庫連線
				string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
				conn = new SqlConnection();
				conn.ConnectionString = connStr;
				conn.Open();

				// 檢查帳號、密碼是否正確
				string salt = Session["Member_Account"].ToString().Substring(0, 1).ToLower(); //使用帳號前一碼當作密碼鹽
				SHA256 sha256 = SHA256.Create();
				byte[] bytes = Encoding.UTF8.GetBytes(salt + Pwd.Member_Password); //將密碼鹽及新密碼組合
				byte[] hash = sha256.ComputeHash(bytes);
				StringBuilder result = new StringBuilder();
				for (int i = 0; i < hash.Length; i++)
				{
					result.Append(hash[i].ToString("X2"));
				}
				string NewPwd = result.ToString();

				string sql = @"UPDATE Member_Data SET Member_Password = @Member_Password WHERE Member_Account = @Member_Account";
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = conn;
				cmd.CommandText = sql;
				// 使用參數化填值
				cmd.Parameters.AddWithValue("@Member_Account", Session["Member_Account"]);
				cmd.Parameters.AddWithValue("@Member_Password", NewPwd);


				// 執行資料庫查詢動作
				int Ret = cmd.ExecuteNonQuery();

				if (Ret > 0)
				{
					TempData["msg"] = "";
					TempData["msg"] = "修改密碼完成";
					return RedirectToAction("Member_Center", "Member");
				}
				else
				{
					TempData["msg"] = "";
					TempData["msg"] = "修改密碼失敗";
					return RedirectToAction("Member_Center", "Member");
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
			//ViewData["Message"] = TempData["Message"];
		}
		public ActionResult Member_Center_Change()
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
				// 資料庫連線
				string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
				conn = new SqlConnection();
				conn.ConnectionString = connStr;
				conn.Open();
				string sql = "select * from Member_Data where Member_Account = @Member_Account";
				SqlCommand cmd = new SqlCommand();
				cmd.CommandText = sql;
				cmd.Connection = conn;

				// 使用參數化填值
				cmd.Parameters.AddWithValue("@Member_Account", Session["Member_Account"]);

				// 執行資料庫查詢動作
				SqlDataAdapter adpt = new SqlDataAdapter();
				adpt.SelectCommand = cmd;
				DataSet ds = new DataSet();
				adpt.Fill(ds);
				DataTable dt = ds.Tables[0];
				MemberModel model = new MemberModel();
				if (ds.Tables[0].Rows.Count > 0)
				{
					model.Member_Account = dt.Rows[0]["Member_Account"].ToString();
					model.Member_Password = dt.Rows[0]["Member_Password"].ToString();
					model.Member_Name = dt.Rows[0]["Member_Name"].ToString();
					model.Member_Mail = dt.Rows[0]["Member_Mail"].ToString();
					model.Member_Phone = dt.Rows[0]["Member_Phone"].ToString();
					model.Member_Birth = (DateTime)dt.Rows[0]["Member_Birth"];
					model.Member_Sex = dt.Rows[0]["Member_Sex"].ToString();
					model.Member_Address = dt.Rows[0]["Member_Address"].ToString();
					// 有查詢到資料，表示帳號密碼正確
					TempData["Message"] = "登入成功";
					return View(model);
					//outModel.ResultMsg = "登入成功";
				}
				else
				{
					// 查無資料，帳號或密碼錯誤
					TempData["Message"] = "帳號錯誤";
					return RedirectToAction("Index");
					//outModel.ErrMsg = "帳號或密碼錯誤";
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
		public ActionResult Member_Center_Change(MemberModel inModel,DateTime Date)
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
				// 資料庫連線
				string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
				conn = new SqlConnection();
				conn.ConnectionString = connStr;
				conn.Open();

				// 檢查帳號、密碼是否正確
				string sql = "UPDATE Member_Data SET Member_Name = @Member_Name, Member_Mail = @Member_Mail, Member_Birth = @Member_Birth, Member_Address = @Member_Address, Member_Phone = @Member_Phone WHERE Member_Account = @Member_Account";
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = conn;
				cmd.CommandText = sql;

				// 使用參數化填值
				cmd.Parameters.AddWithValue("@Member_Account", Session["Member_Account"]);
				cmd.Parameters.AddWithValue("@Member_Name", inModel.Member_Name);
				cmd.Parameters.AddWithValue("@Member_Mail", inModel.Member_Mail);
				cmd.Parameters.AddWithValue("@Member_Birth", Date);
				cmd.Parameters.AddWithValue("@Member_Address", inModel.Member_Address);
				cmd.Parameters.AddWithValue("@Member_Phone", inModel.Member_Phone);


				// 執行資料庫查詢動作
				int Ret = cmd.ExecuteNonQuery();

				if (Ret > 0)
				{
					TempData["msg"] = "";
					TempData["msg"] = "更新成功";
                    Session["Member_Name"]=inModel.Member_Name;
					return RedirectToAction("Member_Center", "Member");
				}
				else
				{
					TempData["msg"] = "";
					TempData["msg"] = "更新失敗";
					return RedirectToAction("Member_Center", "Member");
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
		public ActionResult Member_Center()
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
				// 資料庫連線
				string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
				conn = new SqlConnection();
				conn.ConnectionString = connStr;
				conn.Open();
				string sql = "select * from Member_Data where Member_Account = @Member_Account";
				SqlCommand cmd = new SqlCommand();
				cmd.CommandText = sql;
				cmd.Connection = conn;

				// 使用參數化填值
				cmd.Parameters.AddWithValue("@Member_Account", Session["Member_Account"]);

				// 執行資料庫查詢動作
				SqlDataAdapter adpt = new SqlDataAdapter();
				adpt.SelectCommand = cmd;
				DataSet ds = new DataSet();
				adpt.Fill(ds);
				DataTable dt = ds.Tables[0];
				MemberModel model = new MemberModel();
				if (ds.Tables[0].Rows.Count > 0)
				{
					model.Member_Account = dt.Rows[0]["Member_Account"].ToString();
					model.Member_Password = dt.Rows[0]["Member_Password"].ToString();
					model.Member_Name = dt.Rows[0]["Member_Name"].ToString();
					model.Member_Mail = dt.Rows[0]["Member_Mail"].ToString();
					model.Member_Phone = dt.Rows[0]["Member_Phone"].ToString();
					model.Member_Birth = (DateTime)dt.Rows[0]["Member_Birth"];
					model.Member_Sex = dt.Rows[0]["Member_Sex"].ToString();
					model.Member_Address = dt.Rows[0]["Member_Address"].ToString();
					// 有查詢到資料，表示帳號密碼正確
					ViewData["msg"] = TempData["msg"];
					ViewData["ctmsg"] = TempData["ctmsg"];
					return View(model);
					//outModel.ResultMsg = "登入成功";
				}
				else
				{
					// 查無資料，帳號或密碼錯誤
					TempData["msg"] = "";
					TempData["msg"] = "帳號錯誤";
					return RedirectToAction("Index", "Home");
					//outModel.ErrMsg = "帳號或密碼錯誤";
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

		/// <summary>
		/// 執行註冊
		/// </summary>
		/// <param name="inModel"></param>
		/// <returns></returns>
		/// 
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Register(MemberModel inModel)
		{
			//         DoRegisterOut outModel = new DoRegisterOut();

			//         if (string.IsNullOrEmpty(inModel.Member_Account) || string.IsNullOrEmpty(inModel.Member_Password) ||
			//             string.IsNullOrEmpty(inModel.Member_Mail) || string.IsNullOrEmpty(inModel.Member_Phone) ||
			//             string.IsNullOrEmpty(inModel.Member_Name) || string.IsNullOrEmpty(inModel.Member_Sex) ||
			//             string.IsNullOrEmpty(inModel.Member_Address))
			//         {

			//             outModel.ErrMsg = "請確認輸入資料";
			//         }
			//         else
			//{
			//}
			SqlConnection conn = null;
			try
			{
				// 資料庫連線
				string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
				conn = new SqlConnection();
				conn.ConnectionString = connStr;
				conn.Open();

				// 檢查帳號是否存在
				string sql = "select * from Member_Data where Member_Account = @Member_Account";
				SqlCommand cmd = new SqlCommand();
				cmd.CommandText = sql;
				cmd.Connection = conn;
				// 使用參數化填值
				cmd.Parameters.AddWithValue("@Member_Account", inModel.Member_Account);


				// 執行資料庫查詢動作
				DbDataAdapter adpt = new SqlDataAdapter();
				adpt.SelectCommand = cmd;
				DataSet ds = new DataSet();
				adpt.Fill(ds);

				if (ds.Tables[0].Rows.Count > 0)
				{
					TempData["RGmsg"] = "";
					TempData["RGmsg"] = "此帳號已存在";
					return RedirectToAction("Register", "Member");
					//outModel.ErrMsg = "此登入帳號已存在";
				}
				else
				{
					// 將密碼使用 SHA256 雜湊運算(不可逆)
					string salt = inModel.Member_Account.Substring(0, 1).ToLower(); //使用帳號前一碼當作密碼鹽
					SHA256 sha256 = SHA256.Create();
					byte[] bytes = Encoding.UTF8.GetBytes(salt + inModel.Member_Password); //將密碼鹽及原密碼組合
					byte[] hash = sha256.ComputeHash(bytes);
					StringBuilder result = new StringBuilder();
					for (int i = 0; i < hash.Length; i++)
					{
						result.Append(hash[i].ToString("X2"));
					}
					string NewPwd = result.ToString(); // 雜湊運算後密碼

					// 註冊資料新增至資料庫
					sql = @"INSERT INTO Member_Data (Member_Account,Member_Password,Member_Mail,Member_Phone,Member_Birth,Member_Name,Member_Sex,Member_Address,Member_identity,Member_Status) VALUES (@Member_Account,@Member_Password,@Member_Mail,@Member_Phone,@Member_Birth,@Member_Name,@Member_Sex,@Member_Address,@Member_identity,@Member_Status)";
					cmd = new SqlCommand();
					cmd.Connection = conn;
					cmd.CommandText = sql;
					string identity = "會員";
					var status = 0;

					// 使用參數化填值
					cmd.Parameters.AddWithValue("@Member_Account", inModel.Member_Account);
					cmd.Parameters.AddWithValue("@Member_Password", NewPwd); // 雜湊運算後密碼
					cmd.Parameters.AddWithValue("@Member_Mail", inModel.Member_Mail);
					cmd.Parameters.AddWithValue("@Member_Phone", inModel.Member_Phone);
					cmd.Parameters.AddWithValue("@Member_Birth", inModel.Member_Birth);
					cmd.Parameters.AddWithValue("@Member_Name", inModel.Member_Name);
					cmd.Parameters.AddWithValue("@Member_Sex", inModel.Member_Sex);
					cmd.Parameters.AddWithValue("@Member_Address", inModel.Member_Address);
					cmd.Parameters.AddWithValue("@Member_identity", identity);
					cmd.Parameters.AddWithValue("@Member_Status", status); 

					// 執行資料庫更新動作
					cmd.ExecuteNonQuery();
					TempData["RGmsg"] = "註冊成功";
					//outModel.ResultMsg = "註冊完成";
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

			//// 輸出json
			//TempData["Message"] = outModel.ResultMsg;
			return RedirectToAction("Register");
		}
		/// <summary>
		/// 執行登入
		/// </summary>
		/// <param name="inModel"></param>
		/// <returns></returns>
		/// 
		public ActionResult Login()
		{
			ViewData["Message"] = TempData["Message"];
			return View();
		}
		public JsonResult check_Member_Account(string Member_Account)
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
				string sql = "select * from Member_Data where Member_Account = @Member_Account";
				SqlCommand cmd = new SqlCommand();
				cmd.CommandText = sql;
				cmd.Connection = conn;

				// 使用參數化填值
				cmd.Parameters.AddWithValue("@Member_Account", Member_Account);// 雜湊運算後密碼

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
		public ActionResult Logout()
		{
			Session.Clear();
			Session.RemoveAll();
			Response.Cookies.Clear();
			TempData["Message"] = "";
			TempData["logout"] = "登出成功！";
			return RedirectToAction("Index", "Home");
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Login(MemberModel inModel)
		{
			//DoLoginOut outModel = new DoLoginOut();

			//// 檢查輸入資料
			//if (string.IsNullOrEmpty(inModel.Member_Account) || string.IsNullOrEmpty(inModel.Member_Password))
			//{
			//	outModel.ErrMsg = "請輸入資料";
			//}
			//else
			//{
			//}
			SqlConnection conn = null;
			try
			{
				// 資料庫連線
				string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
				conn = new SqlConnection();
				conn.ConnectionString = connStr;
				conn.Open();

				// 將密碼轉為 SHA256 雜湊運算(不可逆)
				string salt = inModel.Member_Account.Substring(0, 1).ToLower(); //使用帳號前一碼當作密碼鹽
				SHA256 sha256 = SHA256.Create();
				byte[] bytes = Encoding.UTF8.GetBytes(salt + inModel.Member_Password); //將密碼鹽及原密碼組合
				byte[] hash = sha256.ComputeHash(bytes);
				StringBuilder result = new StringBuilder();
				for (int i = 0; i < hash.Length; i++)
				{
					result.Append(hash[i].ToString("X2"));
				}
				string CheckPwd = result.ToString(); // 雜湊運算後密碼

				// 檢查帳號、密碼是否正確
				string sql = "select * from Member_Data where Member_Account = @Member_Account and Member_Password = @Member_Password";
				SqlCommand cmd = new SqlCommand();
				cmd.CommandText = sql;
				cmd.Connection = conn;

				// 使用參數化填值
				cmd.Parameters.AddWithValue("@Member_Account", inModel.Member_Account);
				cmd.Parameters.AddWithValue("@Member_Password", CheckPwd); // 雜湊運算後密碼

				// 執行資料庫查詢動作
				SqlDataAdapter adpt = new SqlDataAdapter();
				adpt.SelectCommand = cmd;
				DataSet ds = new DataSet();
				adpt.Fill(ds);
				DataTable dt = ds.Tables[0];
				MemberModel model = new MemberModel();
				if (ds.Tables[0].Rows.Count > 0)
				{
					if (dt.Rows[0]["Member_Status"].ToString() == "1")
					{
						TempData["Message"] = "帳號已被停權！若有疑問請聯絡平台管理員！";
						return RedirectToAction("Login");
					}
					// 有查詢到資料，表示帳號密碼正確
					Session["Member_Account"] = dt.Rows[0]["Member_Account"].ToString();
					Session["Member_Name"] = dt.Rows[0]["Member_Name"].ToString();
					Session["Member_identity"] = dt.Rows[0]["Member_identity"].ToString();
					Session["Member_Status"] = dt.Rows[0]["Member_Status"].ToString();
					TempData["msg"] = "登入成功";
					return RedirectToAction("Index", "Home");
					//outModel.ResultMsg = "登入成功";
				}
				else
				{
					// 查無資料，帳號或密碼錯誤
					TempData["Message"] = "帳號或密碼錯誤";
					return RedirectToAction("Login");
					//outModel.ErrMsg = "帳號或密碼錯誤";
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
			// 輸出json
		}
		public ActionResult Member_uptOrder(string Order_ID)
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
		public ActionResult Member_uptOrder(Cart inModel)
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
				// 資料庫連線
				string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["WFDB"].ConnectionString;
				conn = new SqlConnection();
				conn.ConnectionString = connStr;
				conn.Open();

				// 檢查帳號、密碼是否正確
				string sql = "UPDATE Order_Data SET Order_Note = @Order_Note WHERE Order_ID = @Order_ID";
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = conn;
				cmd.CommandText = sql;

				// 使用參數化填值
				cmd.Parameters.AddWithValue("@Order_ID", Session["Back_Order_ID"]);
				cmd.Parameters.AddWithValue("@Order_Note", inModel.Order_Note);


				// 執行資料庫查詢動作
				int Ret = cmd.ExecuteNonQuery();

				if (Ret > 0)
				{
					TempData["ummsg"] = "";
					TempData["ummsg"] = "更新成功";
					return RedirectToAction("Member_Order", "Member");
				}
				else
				{
					TempData["ummsg"] = "";
					TempData["ummsg"] = "更新失敗";
					return RedirectToAction("Member_Order", "Member");
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
}