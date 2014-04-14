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

        //patientid = 1203
        //http://localhost:49337/api/notes?patientID=1203&examID=
        public AjaxModel<NotesModel> Get(int type, int patientID, int? examID)
        {
            AjaxModel<NotesModel> ajax = new AjaxModel<NotesModel>() { Success = true };

            try
            {
                //get the last examid when no examid is passed
                ExamModel exam = PosRepository.ExamGet(patientID, examID);
                List<SelectListItem> examLookUp = PosRepository.ExamLookUpGet();
               
                //setting the notes type
                PosConstants.NotesType notesType = PosConstants.NotesType.New;
                if (exam.ExamDate.Date == DateTime.Today.Date)
                    notesType = PosConstants.NotesType.Correct;

                if (exam.SaveID == 1)
                    notesType = PosConstants.NotesType.Saved;


                NotesModel notes = GetNotes(exam, examLookUp, notesType);
                notes.Doctors = PosRepository.DoctorsGet();
                //getting the defaults -------------------------------

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

        private NotesModel GetNotes(ExamModel exam, List<SelectListItem> examLookUp, PosConstants.NotesType notesType)
        {
            NotesModel notes = new NotesModel() { NotesType = notesType };

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
                                value = new Field() {Name = fieldName, Value = fieldValue, ColourType = Convert.ToInt32(fieldAttr) };
                                //setting the colour type
                                if(notesType == PosConstants.NotesType.New)
                                {
                                    if (fieldValue != "" && fieldValue != "OU")
                                        value.ColourType = (int) PosConstants.ColourType.New;
                                }
                                //setting the loopup
                                var lookupItem = examLookUp.FirstOrDefault(m => m.Text.Trim() == fieldName.Trim());
                                if(lookupItem != null)
                                {
                                    value.LookUpFieldName = lookupItem.Value;
                                }

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
