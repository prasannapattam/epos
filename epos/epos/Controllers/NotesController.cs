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

namespace epos.Controllers
{
    public class NotesController : ApiController
    {

        public AjaxModel<NotesModel> Get(int patientID, int? examID)
        {
            AjaxModel<NotesModel> ajax = new AjaxModel<NotesModel>() { Success = true };

            try
            {
                //get the last examid or the passed in exam id
                ExamModel exam = PosRepository.ExamGet(patientID, examID);
               
                //setting the notes type
                PosConstants.NotesType notesType = PosConstants.NotesType.New;
                if (exam.ExamDate.Date == DateTime.Today.Date)
                    notesType = PosConstants.NotesType.Correct;
                else if (examID == null)
                    exam.ExamID = 0;

                if (exam.SaveInd == 1)
                    notesType = PosConstants.NotesType.Saved;
                else if (exam.ExamID > 0)
                {
                    notesType = PosConstants.NotesType.Correct;
                }

                NotesModel notes = WebUtil.GetNotes(exam.ExamText, notesType);

                notes.hdnPatientID = new Field() { Name = "hdnPatientID", Value = patientID.ToString() };
                if(exam.ExamID > 0)
                {
                    notes.ExamDate = new Field() { Name = "ExamDate", Value = exam.ExamDate.ToShortDateString() };
                    notes.hdnExamID = new Field() { Name = "hdnExamID", Value = exam.ExamID.ToString() };
                }
                else
                {
                    notes.ExamDate = new Field() { Name = "ExamDate", Value = DateTime.Now.ToShortDateString() };
                    notes.hdnExamID = null;
                }
                //setting ExamDate & Correct Date
                if(notesType == PosConstants.NotesType.Correct && exam.CorrectExamID != null)
                {
                    notes.ExamCorrectDate = new Field() { Name = "ExamCorrectDate", Value = exam.ExamCorrectDate.Value.ToShortDateString() };
                }
                else
                {
                    notes.ExamCorrectDate = null;
                }
                if(notesType == PosConstants.NotesType.Saved)
                {
                    notes.ExamSaveDate = new Field() { Name = "ExamSaveDate", Value = exam.LastUpdatedDate.ToString() };
                }
                else
                {
                    notes.ExamSaveDate = null;
                }

                notes.PatientName = new Field() { Name = "PatientName", Value = notes.FirstName.Value + ' ' + notes.LastName.Value };

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
                        ExamModel orginalExam = PosRepository.ExamGet(exam.PatientID, exam.CorrectExamID);
                        Dictionary<string, string> dict = WebUtil.GetDictionary(orginalExam.ExamText, false);
                        exam.ExamText = WebUtil.GetXml(model, true, dict);
                        break;
                    default:
                        message = String.Empty;
                        break;
                }

                PosRepository.ExamSave(exam);

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
