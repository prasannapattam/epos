using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epos.Models
{
    public class ExamDefaultModel
    {
        public int ExamDefaultID { get; set; }
        public string DefaultName { get; set; }
        public int AgeStart { get; set; }
        public int AgeEnd { get; set; }
        public bool PrematureBirth { get; set; }
        public int DoctorUserID { get; set; }
        public string ExamText { get; set; }
    }
}