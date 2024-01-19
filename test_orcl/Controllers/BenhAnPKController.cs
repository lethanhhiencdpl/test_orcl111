using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using test_orcl.Models;
using test_orcl.Models.DataAccess;


namespace test_orcl.Controllers
{
    public class BenhAnPKController : Controller
    {
        // GET: BenhAnPK

        // GET: MedicalRecord
        //private ListBenhAnPhongKham _repository = new ListBenhAnPhongKham();
        private ListBenhAnPhongKham _repository = new ListBenhAnPhongKham();

        //[HttpGet]
        public ActionResult Index(string mabs)
        {
            List<BenhAnPK> medicalRecords = _repository.GetBenhAn(mabs);

            return View(medicalRecords);
        }

        //[HttpGet]
        public ActionResult Chidinh(string mabs, DateTime startDate, DateTime endDate)
        {
            List<DanhChiDinhBSTrongNgay> medicalRecords = _repository.GetDanhChiDinhBSTrongNgay(mabs, startDate , endDate);           

            return View(medicalRecords);
        }


        //[HttpGet]
        public ActionResult XemChiTiet(string code)
        {
            List<V_ChiDinh> medicalRecords = _repository.GetThongTinBN_new(code);

            return View(medicalRecords.Where(n => n.MABN == code));
        }


        #region Xem chi tiết
        // GET: AdminHoaDon/Details/5
        public ActionResult Details(string code)
        {
            if (code == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<V_ChiDinh> chiDinh = _repository.GetThongTinBN_new(code);
            if (chiDinh == null)
            {
                return HttpNotFound();
            }
            return View(chiDinh.Where(n=>n.MABN==code));
        }
        #endregion

    }
}