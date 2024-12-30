using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using test_orcl.Models;

namespace test_orcl.ViewModels
{
    public class ChiTietCDHAViewModels
    {
        public List<PatientDataImage> Records { get; set; }
        public string Message { get; set; }

        public ChiTietCDHAViewModels()
        {
            Records = new List<PatientDataImage>();
        }
    }
}