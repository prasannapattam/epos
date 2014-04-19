using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epos.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PhotoUrl { get; set; }

        public List<ExamDefaultModel> Defaults { get; set; }
        public int ID { get { return UserID; } }
        public string FullName { get { return FirstName + ' ' + LastName;  } }
    }
}