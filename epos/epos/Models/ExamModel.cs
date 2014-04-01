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
        public int SaveID { get; set; }
        public DateTime ExamDate { get; set; }
    }
}