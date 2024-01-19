using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static test_orcl.Models.Ganeral;

namespace test_orcl.Models
{
    public class DetailPatientViewModel
    {
        public List<Analysis> Analyses { get; set; }



        public string KETQUA { get; set; }


        public string DONVI { get; set; }


        public string TENXETNGHIEM { get; set; }


        public string GHICHU { get; set; }

        public string CSBT_NU { get; set; }

        public string CSBT_NAM { get; set; }


        public string CSBT { get; set; }


        public string NGAYXETNGHIEM { get; set; }

        public string GIODANGKY { get; set; }


        public string HOTEN { get; set; }

        public int PHAI { get; set; }

        public string Sex { get; set; }

        public int NAMSINH { get; set; }


        public string MAXN { get; set; }

        public string MABN { get; set; }

        public string DIDONG { get; set; }

        public string DIACHI { get; set; }

        public string CHUANDOAN { get; set; }

        public string BSCHIDINH { get; set; }
    }

    public class DetailPatientImageViewModel
    {
        //public List<PatientDataImage> Data { get; set; }

        public string KETQUA { get; set; }

        public string DONVI { get; set; }


        public string TENXETNGHIEM { get; set; }

        public string GHICHU { get; set; }

        public string CSBT_NU { get; set; }

        public string CSBT_NAM { get; set; }

        public string CSBT { get; set; }

        public string NGAYXETNGHIEM { get; set; }

        public string GIODANGKY { get; set; }

        public string HOTEN { get; set; }

        public int PHAI { get; set; }

        public string Sex { get; set; }

        public int NAMSINH { get; set; }

        public string MAXN { get; set; }

        public string MABN { get; set; }

        public string DIDONG { get; set; }

        public string DIACHI { get; set; }

        public string CHUANDOAN { get; set; }

        public string BSCHIDINH { get; set; }
    }
}