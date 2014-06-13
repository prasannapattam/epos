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
        public string FieldValue { get; set; }
        public string Header
        {
            get
            {
                return ExamDate.ToString("d/M/yyyy") + (CorrectExamID.HasValue ? " (Corrected on " + ExamCorrectDate.Value.ToString("d/M/yyyy") + ")" : "");

            }
        }
    }
}