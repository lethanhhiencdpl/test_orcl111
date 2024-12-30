using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using test_orcl.Models;

namespace test_orcl.ViewModels
{
    public class ChiTietXetNghiemViewModels
    {

            public List<Analysis> Records { get; set; }
            public string Message { get; set; }

            public ChiTietXetNghiemViewModels()
            {
                Records = new List<Analysis>();
            }

    }
}