using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epos.Models
{
    public class PatientSearchResultModel
    {
        public int PatientID { get; set; }
        public string PatientNumber { get; set; }
        public string PatientName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? LastExamDate { get; set; }

        public int ID { get { return PatientID;  } }
    }
}