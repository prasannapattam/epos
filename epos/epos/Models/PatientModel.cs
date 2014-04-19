using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epos.Models
{
    public class PatientModel
    {
        public int PatientID { get; set; }
        public string PatientNumber { get; set; }
        public string Greeting { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PatientName { get { return FirstName + ' ' + LastName; } }
        public string NickName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Sex { get; set; }
        public string Occupation { get; set; }
        public string HxFrom { get; set; }
        public string ReferredFrom { get; set; }
        public string ReferredDoctor { get; set; }
        public string Allergies { get; set; }
        public string Medications { get; set; }
        public bool? PrematureBirth { get; set; }
        public List<PatientHistoryModel> History { get; set; }
    }
}