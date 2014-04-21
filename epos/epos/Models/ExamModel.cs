using epos.Lib.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epos.Models
{
    public class ExamModel
    {
        public int ExamID { get; set; }
        public string ExamText { get; set; }
        public int SaveInd { get; set; }
        public DateTime ExamDate { get; set; }
        public int? CorrectExamID { get; set; }
        public DateTime? ExamCorrectDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public int PatientID { get; set; }
        public string UserName { get; set; }
    }
}