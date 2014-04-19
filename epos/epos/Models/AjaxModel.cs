using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epos.Models
{
    public class AjaxModel<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Model { get; set; }
    }
}