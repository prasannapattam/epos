using epos.Lib.Repository;
using epos.Lib.Shared;
using epos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace epos.Controllers
{
    public class ExamDefaultController : ApiController
    {
        public AjaxModel<NotesModel> Get(int doctorUserID, int? examDefaultID)
        {
            AjaxModel<NotesModel> ajax = new AjaxModel<NotesModel>() { Success = true };

            try
            {
                ExamDefaultModel examDefault = PosRepository.ExamDefaultGet(examDefaultID.Value);

                NotesModel notes = WebUtil.GetNotes(examDefault.ExamText, PosConstants.NotesType.Default);

                notes.hdnExamDefaultID = new Field() { Name = "hdnExamDefaultID", Value = examDefault.ExamDefaultID.ToString() };
                notes.DefaultName = new Field() { Name = "DefaultName", Value = examDefault.DefaultName };
                notes.AgeStart = new Field() { Name = "AgeStart", Value = examDefault.AgeStart.ToString() };
                notes.AgeEnd = new Field() { Name = "AgeEnd", Value = examDefault.AgeEnd.ToString() };
                notes.PrematureBirth = new Field() { Name = "PrematureBirth", Value = examDefault.PrematureBirth.ToString(), LookUpFieldName = "Premature" };
                notes.DoctorName = new Field() { Name = "DoctorName", Value = examDefault.DoctorName };

                ajax.Model = notes;

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
