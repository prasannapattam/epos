using epos.Lib.Repository;
using epos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace epos.Controllers
{
    public class NotesAutoCompleteController : ApiController
    {
		public AjaxModel<string[]> Get(string userName)
		{
		AjaxModel<string[]> ajax = new AjaxModel<string[]>() { Success = true };

			try
				{
				ajax.Model = PosRepository.AutoCorrectGet(userName);
				}
			catch (Exception exp)
				{
				ajax.Success = false;
				ajax.Message = exp.Message;
				ajax.Model = null;
				}

			return ajax;
		}

	}
}
