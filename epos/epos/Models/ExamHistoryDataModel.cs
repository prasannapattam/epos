using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epos.Models
{
    public class ExamHistoryDataModel
    {
        public int ExamID { get; set; }
        public DateTime ExamDate { get; set; }
        public int? CorrectExamID { get; set; }
        public DateTime? ExamCorrectDate { get; set; }
        public string FieldName { get; set; }
        public dynamic FieldValue { get; set; }
        public string Header
        {
            get
            {
                return "Notes taken on " + ExamDate.ToString("MM/dd/yyyy") + (CorrectExamID.HasValue ? " (Corrected on " + ExamCorrectDate.Value.ToString("MM/dd/yyyy") + ")" : "");
            }
        }
    }
}