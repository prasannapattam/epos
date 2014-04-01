using epos.Models;
using epos.Lib.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using epos.Lib.Repository;

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
                ExamModel exam = PosRepository.ExamGet(patientID, examID);
                NotesModel notes = new NotesModel() { NotesType = PosConstants.NotesType.New };
                if (exam.SaveID == 1)
                    notes.NotesType = PosConstants.NotesType.Saved;

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
