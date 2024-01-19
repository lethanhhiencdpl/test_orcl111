using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace test_orcl.Models
{
    public class Ganeral
    {
        public class PrescribeDetailModel
        {
            public decimal ID { get; set; }
            public string TEN { get; set; }
        }


        public class Phone
        {
            public string MABN { get; set; }

            public string DIDONG { get; set; }
 
            public string HOTEN { get; set; }
        }

        public class TrieuChung
        {
            public string MAQL { get; set; }


            [Display(Name = "TÊN")]
            public string TEN { get; set; }

            public string MA { get; set; }
        }

        public class PrescribeModel
        {
            public decimal MAVP { get; set; }

            public decimal MAQL { get; set; }

            public string TENCHIDINH { get; set; }
        }

        public class CDHABNLL
        {
            public string ID { get; set; }

            public string MABS { get; set; }

            public string MAKT { get; set; }
        }

        public class CDHABNLLCT
        {
            public string ID { get; set; }

            public string MOTA { get; set; }

            public string KETLUAN { get; set; }

            public string MA { get; set; }

            public string HOTENBS { get; set; }

            public string IDLOAIKYTHUAT { get; set; }
        }

        public class Prescription
        {
            public string ID { get; set; }

            public string NGAY { get; set; }

            public string MABD { get; set; }

            public int SLYEUCAU { get; set; }

            public string CACHDUNG { get; set; }

            public string GHICHU { get; set; }

        }

        public class DDMBD
        {
            public string ID { get; set; }

            public string TEN { get; set; }

            public string TENHC { get; set; }

            public string HAMLUONG { get; set; }

            public string DANG { get; set; }

            public string DUONGDUNG { get; set; }

            public string DONVIDUNG { get; set; }

            public int SLYEUCAU { get; set; }

            public string Use { get; set; }

        }    
    }
}