using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test_orcl.Models
{
    public class V_ChiDinh
    {
        public Int64 ID { get; set; }
        public string MABN { get; set; }
        public Int64 MAVAOVIEN { get; set; }
        public Int64 MAQL { get; set; }
        public DateTime NGAY { get; set; }
        public int MAKP { get; set; }
        public int MADOITUONG { get; set; }
        public int MAVP { get; set; }
        public int SOLUONG { get; set; }
        public int DONGIA { get; set; }
        public int PAID { get; set; }
        public int DONE { get; set; }
        public string CHANDOAN { get; set; }
        public string MABS { get; set; }
    }
}