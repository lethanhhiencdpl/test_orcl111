using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using test_orcl.Models;
using test_orcl.Models.DataAccess;
using Oracle.ManagedDataAccess.Client;
using static test_orcl.Models.Ganeral;
using System.Net.NetworkInformation;
using System.Net;

namespace test_orcl.Controllers
{
    public class DoctorController : Controller
    {
        private ListBenhAnPhongKham _repository = new ListBenhAnPhongKham();
        // GET: Doctor
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public  ActionResult ChiTietXetNghiem(DateTime Date, string code, string doctorCode)
        {

            if (code == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<Analysis> medicalRecords = _repository.KetQuaXN_BS( doctorCode, Date, code);
            return View(medicalRecords.OrderBy(n=>n.MAXN).ToList());
        }

        [HttpGet]
        public PartialViewResult _NgayXetNghiemPartial(string code, string doctorCode)
        {
            List<CountNgayXetNghiem> medicalRecords = _repository.NgayXetNghiem(doctorCode, code);
            return PartialView("_NgayXetNghiemPartial", medicalRecords.OrderByDescending(n=>n.NGAYXETNGHIEM));
        }

        [HttpGet]
        public PartialViewResult _HienThiTTBenhNhanPartial(string code)
        {
            List<BTDBenhNhan> TTBenhNhan = _repository.GetThongTinBN(code);
            return PartialView("_HienThiTTBenhNhanPartial", TTBenhNhan);
        }


        //CDHA
        public ActionResult ChiTietCHDA(DateTime Date, string code, string doctorCode)
        {
            if (code == null  || Date ==null || doctorCode==null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            List<PatientDataImage> medicalRecords = _repository.GetPatientDataImageByCode(code, Date, doctorCode);
            return View(medicalRecords.ToList());
        }

        [HttpGet]
        public PartialViewResult _NgayCDHAPartial(string code, string doctorCode)
        {
            List<CountNgayCDHA> medicalRecords = _repository.NgayCDHA(doctorCode, code);
            return PartialView("_NgayCDHAPartial", medicalRecords.Distinct().OrderByDescending(n => n.NGAYCDHA));
        }
    }
}