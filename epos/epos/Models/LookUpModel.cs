using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epos.Models
{
    public class LookUpModel
    {
        public int LookUpID { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string FieldDescription { get; set; }
        public int? SortOrder { get; set; }
        public bool  IsNew { get; set; }
        public bool IsDeleted { get; set; }
    }
}
