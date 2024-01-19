using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test_orcl.Models
{
    public partial class DangNhap
    {
        public string ID { get; set; }
        public string TENUSER { get; set; }
        public string PASSWORD { get; set; }
        public string TENBACSI { get; set; }
        public int HIDE { get; set; }
        public DateTime NGAYUD { get; set; }
    }
}