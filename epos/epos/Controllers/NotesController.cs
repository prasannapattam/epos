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

        public AjaxModel<NotesModel> Get(int userID, int patientID, int? examID)
        {
            AjaxModel<NotesModel> ajax = new AjaxModel<NotesModel>() { Success = true };

            try
            {
                NotesDomain domain = new NotesDomain();
                ajax.Model = domain.GetNotes(userID, patientID, examID);

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
            string message;
            AjaxModel<string> ajax = new AjaxModel<string>() { Success = true, Model = PosMessage.Blank };
            try
            {
                ExamModel exam = new ExamModel()
                {
                    ExamID = model.hdnExamID != null ? Convert.ToInt32(model.hdnExamID.Value) : 0,
                    ExamDate = Convert.ToDateTime(model.ExamDate.Value),
                    PatientID = Convert.ToInt32(model.hdnPatientID.Value),
                    UserName = model.User.Value,
                    SaveInd = 0,
                    LastUpdatedDate = DateTime.Now,
                    ExamCorrectDate = DateTime.Now,
                    CorrectExamID = null,
                };
                switch (saveType)
                {
                    case PosConstants.NotesSaveType.Save:
                        message = PosMessage.NotesSaveSuccessful;
                        exam.ExamText = WebUtil.GetXml(model, false, null);
                        exam.SaveInd = 1;
                        break;
                    case PosConstants.NotesSaveType.SignOff:
                        message = PosMessage.NotesSignOffSuccessful;
                        exam.ExamText = WebUtil.GetXml(model, true, null);
                        break;
                    case PosConstants.NotesSaveType.Correct:
                        message = PosMessage.NotesCorrectSuccessful;
                        exam.CorrectExamID = exam.ExamID;
                        exam.ExamID = 0;
                        //getting the original exam
                        ExamModel orginalExam = PatientRepository.ExamGet(exam.PatientID, exam.CorrectExamID);
                        Dictionary<string, string> dict = WebUtil.GetDictionary(orginalExam.ExamText, false);
                        exam.ExamText = WebUtil.GetXml(model, true, dict);
                        break;
                    default:
                        message = String.Empty;
                        break;
                }

                PatientRepository.ExamSave(exam);

                //removing & creating print queue
                if (saveType == PosConstants.NotesSaveType.Correct)
                {
                    PosRepository.PrintQueueRemove(exam.CorrectExamID.Value);
                }

                PosRepository.PrintQueueAdd(new PrintQueueItem() { ExamID = exam.ExamID, UserName = exam.UserName, PrintExamNote = null });
                PosRepository.PrintQueueAdd(new PrintQueueItem() { ExamID = exam.ExamID, UserName = exam.UserName, PrintExamNote = true });

                ajax.Message = message;
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
