using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace epos.Models
{
	public class NotesViewModel
	{
		public NotesModel Notes { get; set; }
	
		//dropdown for doctors
		public List<SelectListItem> Doctors { get; set; }

		//AutoComplete
		public string[] AutoComplete { get; set; }
	}
}