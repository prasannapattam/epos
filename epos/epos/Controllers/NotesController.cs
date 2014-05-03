using epos.Models;
using epos.Lib.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using epos.Lib.Repository;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Web.Mvc;
using epos.Lib.Domain;

namespace epos.Controllers
{
    public class NotesController : ApiController
    {

        public AjaxModel<NotesModel> Get(string userName, int patientID, int? examID)
        {
            AjaxModel<NotesModel> ajax = new AjaxModel<NotesModel>() { Success = true };

            try
            {
                ajax.Model = new NotesDomain().GetNotes(userName, patientID, examID);
            }
            catch (Exception exp)
            {
                ajax.Success = false;
                ajax.Message = exp.Message;
                ajax.Model = null;
            }

            return ajax;
        }

        public AjaxModel<string> Post([FromUri] int type, [FromBody] NotesModel model)
        {
            PosConstants.NotesSaveType saveType = (PosConstants.NotesSaveType) type;
            AjaxModel<string> ajax = new AjaxModel<string>() { Success = true, Model = PosMessage.Blank };
            try
            {
                ajax.Message = new NotesDomain().Save(saveType, model);
            }
            catch (Exception exp)
            {
                ajax.Success = false;
                ajax.Message = exp.Message;
            }


            return ajax;
        }
    
    }


}
