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

namespace epos.Controllers
{
    public class NotesController : ApiController
    {

        //patientid = 1203
        public AjaxModel<NotesModel> Get(int patientID, int? examID)
        {
            AjaxModel<NotesModel> ajax = new AjaxModel<NotesModel>() { Success = true };

            try
            {
                //get the last examid when no examid is passed
                ExamModel exam = PosRepository.ExamGet(patientID, examID);
                NotesModel notes = GetNotes(exam);
                //getting the defaults -------------------------------

                if (exam.ExamDate.Date == DateTime.Today.Date)
                    notes.NotesType = PosConstants.NotesType.Correct;

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

        private NotesModel GetNotes(ExamModel exam)
        {
            NotesModel notes = new NotesModel() { NotesType = PosConstants.NotesType.New };

            PropertyInfo[] notesFields = notes.GetType().GetProperties();
            Field value;
            
            //looping through the xml and setting the Notes
            StringReader stringReader = new StringReader(exam.ExamText);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CheckCharacters = false;
            XmlReader reader = XmlReader.Create(stringReader, settings);
            //XmlTextReader reader = new XmlTextReader(stringReader);
            //reader.WhitespaceHandling = WhitespaceHandling.None;

            string fieldName = "";
            string fieldValue = "";
            string fieldAttr = "";
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        fieldName = reader.Name;
                        fieldAttr = reader.GetAttribute("CustomColourType");
                        break;
                    case XmlNodeType.Text:
                        fieldValue = reader.Value;
                        break;
                    case XmlNodeType.CDATA:
                        fieldValue = reader.Value;
                        break;
                    case XmlNodeType.EndElement:
                        if(fieldName != "")
                        {
                            //SetControlValue(fieldName, fieldValue, fieldAttr);

                            PropertyInfo pi = notesFields.FirstOrDefault(p => p.Name == fieldName);

                            if(pi != null)
                            {
                                value = new Field() { Value = fieldValue, ColourType = Convert.ToInt32(fieldAttr) };
                                pi.SetValue(notes, value);
                            }

                            fieldName = "";
                            fieldValue = "";
                            fieldAttr = "";
                        }
                        break;
                }
            }

            reader.Close();
            stringReader.Close();
            stringReader.Dispose();

            return notes;
        }
    }


}
