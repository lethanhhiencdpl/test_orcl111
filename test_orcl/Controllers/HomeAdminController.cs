using Antlr.Runtime.Misc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;
using test_orcl.Models;
using test_orcl.Models.DataAccess;

namespace test_orcl.Controllers
{
    public class HomeAdminController : Controller
    {
        // GET: HomeAdmin
        private string connectionString = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;

        // GET: MedicalRecord
        private ListBenhAnPhongKham _repository = new ListBenhAnPhongKham();
        public ActionResult Index(string mabs)
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            //List<BenhAnPK> medicalRecords = _repository.GetBenhAn(mabs/*, startDate*/);
            //List<DanhChiDinhBSTrongNgay> medicalRecords = _repository.GetDanhChiDinhBSTrongNgay(mabs, startDate, endDate);
            //return View(medicalRecords);
            return View();

            //return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserDocTor user)
        {
            AccountDataAccess dataAccess = new AccountDataAccess(connectionString);
            //UserDocTor authenticatedUser = dataAccess.GetUser(user.ID, user.DIENTHOAI);
            UserDocTor authenticatedUser = dataAccess.GetUser(user.ID, user.DIENTHOAI);

            //1. ktra tên đn & mk có trống --> trở về trang đăng nhập: Thông báo thiếu thông tin
            if (string.IsNullOrEmpty(user.ID) == true | string.IsNullOrEmpty(user.DIENTHOAI) == true)
            {
                ViewBag.thongbao = "Thông báo thiếu thông tin";
                return View();
            }

            //3. Kiểm tra tồn tại tài khoản --> nếu k tồn tại --> trở về trang đăng nhập: TK hoặc MK không đúng 
            if (authenticatedUser == null)
            {
                ViewBag.thongbao = "Tài khoản hoặc mật khẩu không đúng";
                ViewBag.Username = user.ID.ToString();
                return View();
            }
            if (authenticatedUser != null)
            {
                // Lưu session
                Session["ID"] = authenticatedUser.ID;
                Session["HOTEN"] = authenticatedUser.HOTEN;

                // Truyền giá trị đến View thông qua ViewBag hoặc Model
                string ID = HttpContext.Session["ID"] as string;
                ViewBag.ID = ID;
                //ViewBag.TENUSER = authenticatedUser.TENBACSI;

                // Đăng nhập thành công
                // Redirect hoặc thực hiện hành động mong muốn
                return RedirectToAction("Index", "HomeAdmin");
            }
            else
            {
                // Đăng nhập thất bại
                // Hiển thị thông báo hoặc thực hiện hành động mong muốn
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(user);
            }
        }

        public ActionResult Logout()
        {
            // Xóa session khi đăng xuất
            Session.Remove("ID");
            FormsAuthentication.SignOut();
            //Session.Clear();
            return RedirectToAction("Login", "HomeAdmin");
        }
    }
}