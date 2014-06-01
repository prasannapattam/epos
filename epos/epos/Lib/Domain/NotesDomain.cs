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
        private string[] patientFields =  { "patientID", "hdnPatientID", "Greeting", "FirstName", "MiddleName", "LastName", "PatientName", "NickName", "DOB", "Sex", "PrematureBirth", "HxFrom", "RefDoctor", "Refd", "Allergies", "Occupation", "tbAge" };

        public NotesViewModel GetNotes(string userName, int patientID, int? examID)
        {
            PatientModel patient = PatientRepository.PatientGet(patientID, false);

            bool defaultNotes = false;

            //get the last examid or the passed in exam id
            ExamModel exam = PatientRepository.ExamGet(patientID, examID);
            if (exam == null)
            {
                exam = new ExamModel();
                //getting the defaults if exists
                exam.ExamText = GetDefaultNotesText(userName, patient);
                defaultNotes = true;
            }

            PosConstants.NotesType notesType = GetNotesType(exam, examID);

			NotesViewModel notesVM = GetNotesFromXml(exam.ExamText, notesType, userName);
			NotesModel notes = notesVM.Notes;

            SetIdDates(exam, notes, patientID);

            if (defaultNotes)
                notes.DefaultInd = true;
            else
                notes.DefaultInd = false;
            if (notes.NotesType == PosConstants.NotesType.New)
            {
                SetPatientInfo(patient, notes);
                //SetOverides(notes);
                notes.User.Value = userName;
            }

            //In old code sometimes Patient Number & name is empty. This is causing problem. Setting that
            if (notes.PatientNumber.Value == "")
                notes.PatientNumber.Value = patient.PatientNumber;
            if (notes.PatientName.Value == "")
                notes.PatientNumber.Value = patient.FirstName + " " + patient.LastName;

			return notesVM;
        }

        public NotesViewModel GetDefaultNotes(int doctorUserID, int? examDefaultID)
        {
            ExamDefaultModel examDefault = PatientRepository.ExamDefaultGet(examDefaultID.Value);

			NotesViewModel notesVM = GetNotesFromXml(examDefault.ExamText, PosConstants.NotesType.Default, string.Empty);
			NotesModel notes = notesVM.Notes;

            notes.hdnExamDefaultID = new Field() { Name = "hdnExamDefaultID", Value = examDefault.ExamDefaultID.ToString() };
            notes.DefaultName = new Field() { Name = "DefaultName", Value = examDefault.DefaultName };
            notes.AgeStart = new Field() { Name = "AgeStart", Value = examDefault.AgeStart.ToString() };
            notes.AgeEnd = new Field() { Name = "AgeEnd", Value = examDefault.AgeEnd.ToString() };
            notes.PrematureBirth = new Field() { Name = "PrematureBirth", Value = examDefault.PrematureBirth.ToString(), LookUpFieldName = "Premature" };
            notes.DoctorName = new Field() { Name = "DoctorName", Value = examDefault.DoctorName };

			return notesVM;
        }

        private PosConstants.NotesType GetNotesType(ExamModel exam, int? examID)
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

        private void SetIdDates(ExamModel exam, NotesModel notes, int patientID)
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
                notes.LastExam = new Field() { Name = "LastExam", Value = exam.ExamDate != DateTime.MinValue ? exam.ExamDate.ToShortDateString() : null };
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
            SetPatientField(notes.PatientNumber, patient.PatientNumber.ToString());
            SetPatientField(notes.Greeting, patient.Greeting);
            SetPatientField(notes.FirstName, patient.FirstName);
            SetPatientField(notes.MiddleName, patient.MiddleName);
            SetPatientField(notes.LastName, patient.LastName);
            SetPatientField(notes.NickName, patient.NickName);
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
            field.FieldType = (int)PosConstants.FieldType.Patient;
        }

        private NotesViewModel GetNotesFromXml(string examText, PosConstants.NotesType notesType, string userName)
        {
			NotesViewModel notesVM = GetBlankNotes(userName);
			NotesModel notes = notesVM.Notes;
            if (examText == null)
				return notesVM;

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
                        fieldAttr = fieldAttr == "" ? "0" : fieldAttr;
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

			return notesVM;
        }

        private NotesViewModel GetBlankNotes(string userName)
        {
			NotesViewModel notesVM = new NotesViewModel();
            NotesModel notes = new NotesModel() { NotesType = PosConstants.NotesType.New };
			notesVM.Notes = notes;
			notesVM.Doctors = PosRepository.DoctorsGet();
			notesVM.AutoComplete = PosRepository.AutoCorrectGet(userName);
            List<SelectListItem> examLookUp = PosRepository.ExamLookUpGet();

            PropertyInfo[] notesFields = notes.GetType().GetProperties();
            foreach (var pi in notesFields)
            {
                SetProperty(notes, pi, pi.Name, "", (int)PosConstants.ColourType.Normal, examLookUp);
            }

			return notesVM;
        }

        private void SetProperty(NotesModel notes, PropertyInfo pi, string fieldName, string fieldValue, int colourType, List<SelectListItem> examLookUp)
        {
            if (pi != null && pi.PropertyType == typeof(Field))
            {
                Field field = new Field() { Name = fieldName };
                //setting the colour type

                //setting the loopup
                var lookupItem = examLookUp.FirstOrDefault(m => m.Text.Trim() == fieldName.Trim());
                if (lookupItem != null)
                {
                    field.LookUpFieldName = lookupItem.Value;
                }

                if(patientFields.Contains(fieldName))
                {
                    field.FieldType = (int) PosConstants.FieldType.Patient;
                }
                else
                {
                    field.FieldType = (int) PosConstants.FieldType.Notes;
                }

                pi.SetValue(notes, field);

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

        private string GetDefaultNotesText(string userName, PatientModel patient)
        {
            int patientAge = (int) DateTime.Now.Subtract(patient.DateOfBirth.Value).Days / 30; //in months
            bool prematureBirth = patient.PrematureBirth != null && patient.PrematureBirth.Value && patientAge < 6;
            string examText = PatientRepository.ExamDefaultNotesText(userName, patientAge, prematureBirth);

            //replacing the special fields
            if (examText != null)
            {
                examText = examText.Replace("[PatientName]", patient.PatientName);
                examText = examText.Replace("[FirstName]", patient.FirstName);
                examText = examText.Replace("[LastName]", patient.LastName);
                examText = examText.Replace("[Age]", GetPatientAge(patient.DateOfBirth.Value));
                examText = examText.Replace("[Sex]", GetSex(patient.DateOfBirth.Value, patient.Sex));
            }
            return examText;
        }

        private string GetPatientAge(DateTime dob)
        {
                TimeSpan ts = DateTime.Now.Subtract(dob);
                int totalDays = (int) ts.TotalDays;
                int totalWeeks = totalDays / 7;
                int totalMonths = totalDays / 30;
                int totalYears = totalMonths / 12;
                int months = totalMonths - (totalYears * 12);

                string age = string.Empty;
                if (totalMonths <= 6)
                    age = totalWeeks.ToString() + " weeks";
                else if (totalMonths < 12)
                    age = totalMonths.ToString() + " month-old";
                else if (totalYears <= 10)
                    age = totalYears.ToString() + '.' + months.ToString() + " year-old";
                else
                    age = totalYears.ToString() + " year-old";

                return age;
        }

        private string GetSex(DateTime dob, string sex)
        {
            TimeSpan ts = DateTime.Now.Subtract(dob);
            int totalDays = (int)ts.TotalDays;
            int totalMonths = totalDays / 30;
                int totalYears = totalMonths / 12;

            string displaySex = string.Empty;
            if (totalMonths <= 6)
                displaySex = "gestation";
            else if (totalYears <= 16)
            {
                if(sex.ToLower() == PosConstants.Sex.Female.ToString().ToLower())
                    displaySex = "girl";
                else
                    displaySex = "boy";
            }
            else if (totalYears <= 24)
            {
                if(sex.ToLower() == PosConstants.Sex.Female.ToString().ToLower())
                    displaySex = "young lady";
                else
                    displaySex = "young man";
            }
            else 
            {
                if (sex.ToLower() == PosConstants.Sex.Female.ToString().ToLower())
                    displaySex = "lady";
                else
                    displaySex = "gentleman";
            }


            return displaySex;

        }

        public string Save(PosConstants.NotesSaveType saveType, NotesModel model)
        {
            string message = String.Empty;

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

            if(saveType == PosConstants.NotesSaveType.SignOff || saveType == PosConstants.NotesSaveType.Correct)
            {
                if(model.cbPrintQueue.Value == true.ToString())
                    PosRepository.PrintQueueAdd(new PrintQueueItem() { ExamID = exam.ExamID, UserName = exam.UserName, PrintExamNote = null });
                if(model.ExamNoteTo.Value != "")
                    PosRepository.PrintQueueAdd(new PrintQueueItem() { ExamID = exam.ExamID, UserName = exam.UserName, PrintExamNote = true });
            }

            return message;
        }
    }
}