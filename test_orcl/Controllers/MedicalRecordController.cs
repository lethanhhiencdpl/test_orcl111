using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using test_orcl.Models;
using test_orcl.Models.DataAccess;

namespace test_orcl.Controllers
{
    public class MedicalRecordController : Controller
    {
        // GET: MedicalRecord
        private MedicalRecordRepository _repository = new MedicalRecordRepository();

        public ActionResult Index(int patientId)
        {
            List<MedicalRecord> medicalRecords = _repository.GetMedicalRecordsByPatientId(patientId);

            return View(medicalRecords);
        }
    }
}