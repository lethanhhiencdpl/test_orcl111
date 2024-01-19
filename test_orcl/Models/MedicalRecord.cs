using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test_orcl.Models
{
    public class MedicalRecord
    {
        public int RecordId { get; set; }
        public int PatientId { get; set; }
        public DateTime VisitDate { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        // Thêm các thuộc tính khác nếu cần
    }
}