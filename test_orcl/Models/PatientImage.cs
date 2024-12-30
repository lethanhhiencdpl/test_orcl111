using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test_orcl.Models
{
    public class PatientImage
    {
        public string ID { get; set; }

        public byte[] Imagee { get; set; }

        public string Value { get; set; }
    }
}