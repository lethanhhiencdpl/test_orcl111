﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test_orcl.Models
{
    public class PatientDataImage:BTDBenhNhan
    {
        public string ID { get; set; }
        public string MABN { get; set; }
        public string MABS { get; set; }

        public string MOTA { get; set; }

        public string KETLUAN { get; set; }

        public string BSCHIDINH { get; set; }

        public string BSTHUCHIEN { get; set; }

        public string TENKYTHUAT { get; set; }

        public string TENLOAIKT { get; set; }

        public string NGAYCHUP { get; set; }

        public DateTime NGAYKETQUA { get; set; }

        public double DONGIA { get; set; }

        public string CHANDOAN { get; set; }

        //public List<PatientImage> Image { get; set; }
        public List<string> ImageBase64List { get; set; } // Danh sách hình ảnh dạng base64
    }
}