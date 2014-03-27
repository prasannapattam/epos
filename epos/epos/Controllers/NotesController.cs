using epos.Models;
using epos.Lib.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace epos.Controllers
{
    public class NotesController : ApiController
    {

        public AjaxModel<dynamic> Get(PosConstants.NotesType notesType, int patientID, int? examID)
        {
            AjaxModel<dynamic> ajax = new AjaxModel<dynamic>() { Success = true };

            try
            {
                //get the last examid when no examid is passed

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
