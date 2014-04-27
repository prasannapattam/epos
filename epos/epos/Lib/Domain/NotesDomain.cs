using epos.Lib.Repository;
using epos.Lib.Shared;
using epos.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace epos.Lib.Domain
{
    public class NotesDomain
    {
        private int patientID;
        private int? examID;

        public NotesModel GetNotes(int patientID, int? examID)
        {
            this.patientID = patientID;
            this.examID = examID;

            PatientModel patient = PosRepository.PatientGet(patientID, false);

            //get the last examid or the passed in exam id
            ExamModel exam = PosRepository.ExamGet(patientID, examID);
            if (exam == null)
            {
                //getting the defaults

                //finally setting the blank exam record
                exam = new ExamModel();
            }

            PosConstants.NotesType notesType = GetNotesType(exam);

            NotesModel notes = GetNotesFromXml(exam.ExamText, notesType);

            SetOverrides(exam, notes);
            SetPatientInfo(patient, notes);

            return notes;
        }

        public NotesModel GetDefaultNotes(int doctorUserID, int? examDefaultID)
        {
            ExamDefaultModel examDefault = PosRepository.ExamDefaultGet(examDefaultID.Value);

            NotesModel notes = GetNotesFromXml(examDefault.ExamText, PosConstants.NotesType.Default);

            notes.hdnExamDefaultID = new Field() { Name = "hdnExamDefaultID", Value = examDefault.ExamDefaultID.ToString() };
            notes.DefaultName = new Field() { Name = "DefaultName", Value = examDefault.DefaultName };
            notes.AgeStart = new Field() { Name = "AgeStart", Value = examDefault.AgeStart.ToString() };
            notes.AgeEnd = new Field() { Name = "AgeEnd", Value = examDefault.AgeEnd.ToString() };
            notes.PrematureBirth = new Field() { Name = "PrematureBirth", Value = examDefault.PrematureBirth.ToString(), LookUpFieldName = "Premature" };
            notes.DoctorName = new Field() { Name = "DoctorName", Value = examDefault.DoctorName };

            return notes;
        }

        private PosConstants.NotesType GetNotesType(ExamModel exam)
        {
            //setting the notes type & examid
            PosConstants.NotesType notesType = PosConstants.NotesType.New;

            if (exam.SaveInd == 1)
            {
                notesType = PosConstants.NotesType.Saved;
            }
            else if (examID == null && exam.ExamID > 0 && exam.ExamDate.Date == DateTime.Today.Date)
            {
                notesType = PosConstants.NotesType.Correct;
            }
            else if (examID == null)
            {
                notesType = PosConstants.NotesType.New;
            }
            else
            {
                notesType = PosConstants.NotesType.Correct;
            }

            return notesType;
        }

        private void SetOverrides(ExamModel exam, NotesModel notes)
        {
            if (notes.NotesType == PosConstants.NotesType.New)
                exam.ExamID = 0;
            notes.hdnPatientID = new Field() { Name = "hdnPatientID", Value = patientID.ToString() };
            if (exam.ExamID > 0)
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
            if (notes.NotesType == PosConstants.NotesType.Correct && exam.CorrectExamID != null)
            {
                notes.ExamCorrectDate = new Field() { Name = "ExamCorrectDate", Value = exam.ExamCorrectDate.Value.ToShortDateString() };
            }
            else
            {
                notes.ExamCorrectDate = null;
            }
            if (notes.NotesType == PosConstants.NotesType.Saved)
            {
                notes.ExamSaveDate = new Field() { Name = "ExamSaveDate", Value = exam.LastUpdatedDate.ToString() };
            }
            else
            {
                notes.ExamSaveDate = null;
            }
        }

        private void SetPatientInfo(PatientModel patient, NotesModel notes)
        {
            SetPatientField(notes.patientID, patient.PatientID.ToString());
            SetPatientField(notes.hdnPatientID, patient.PatientID.ToString());
            SetPatientField(notes.Greeting, patient.Greeting);
            SetPatientField(notes.FirstName, patient.FirstName);
            SetPatientField(notes.MiddleName, patient.MiddleName);
            SetPatientField(notes.LastName, patient.LastName);
            SetPatientField(notes.PatientName, patient.FirstName + " " + patient.LastName);
            SetPatientField(notes.DOB, patient.DateOfBirth.Value.ToShortDateString());
            SetPatientField(notes.Sex, patient.Sex);
            SetPatientField(notes.PrematureBirth, patient.PrematureBirth == null ? false.ToString() : patient.PrematureBirth.Value.ToString());
            SetPatientField(notes.HxFrom, patient.HxFrom);
            SetPatientField(notes.RefDoctor, patient.ReferredDoctor);
            SetPatientField(notes.Refd, patient.ReferredFrom);
            SetPatientField(notes.Allergies, patient.Allergies);
            SetPatientField(notes.Occupation, patient.Occupation);
            notes.tbAge.ColourType = (int)PosConstants.ColourType.Normal;
        }

        private void SetPatientField(Field field, string value)
        {
            field.Value = value;
            field.ColourType = (int) PosConstants.ColourType.Normal;
        }

        private NotesModel GetNotesFromXml(string examText, PosConstants.NotesType notesType)
        {
            NotesModel notes = GetBlankNotes();
            if (examText == null)
                return notes;

            notes.NotesType = notesType;

            PropertyInfo[] notesFields = notes.GetType().GetProperties();

            //looping through the xml and setting the Notes
            StringReader stringReader = new StringReader(examText);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CheckCharacters = false;
            XmlReader reader = XmlReader.Create(stringReader, settings);

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
                        if (fieldName != "")
                        {
                            //SetControlValue(fieldName, fieldValue, fieldAttr);
                            PropertyInfo pi = notesFields.FirstOrDefault(p => p.Name == fieldName);
                            SetPropertyValue(notes, pi, fieldValue, Convert.ToInt32(fieldAttr));

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

        private NotesModel GetBlankNotes()
        {
            NotesModel notes = new NotesModel() { NotesType = PosConstants.NotesType.New };
            notes.Doctors = PosRepository.DoctorsGet();
            List<SelectListItem> examLookUp = PosRepository.ExamLookUpGet();

            PropertyInfo[] notesFields = notes.GetType().GetProperties();
            foreach (var pi in notesFields)
            {
                SetProperty(notes, pi, pi.Name, "", (int)PosConstants.ColourType.Normal, examLookUp);
            }

            return notes;
        }

        private void SetProperty(NotesModel notes, PropertyInfo pi, string fieldName, string fieldValue, int colourType, List<SelectListItem> examLookUp)
        {
            if (pi != null && pi.PropertyType == typeof(Field))
            {
                Field value = new Field() { Name = fieldName };
                //setting the colour type

                //setting the loopup
                var lookupItem = examLookUp.FirstOrDefault(m => m.Text.Trim() == fieldName.Trim());
                if (lookupItem != null)
                {
                    value.LookUpFieldName = lookupItem.Value;
                }

                pi.SetValue(notes, value);

                SetPropertyValue(notes, pi, fieldValue, colourType);
            }
        }

        private void SetPropertyValue(NotesModel notes, PropertyInfo pi, string fieldValue, int colourType)
        {
            if (pi == null) return;

            Field field = (Field)pi.GetValue(notes);
            field.Value = fieldValue;
            field.ColourType = colourType;

            if (notes.NotesType == PosConstants.NotesType.New)
            {
                if (fieldValue != "" && fieldValue != "OU")
                    field.ColourType = (int)PosConstants.ColourType.New;
            }

        }
    }
}