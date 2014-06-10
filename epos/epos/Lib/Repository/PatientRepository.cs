using AutoMapper;
using epos.Lib.Shared;
using epos.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace epos.Lib.Repository
{
    public class PatientRepository
    {
        public static List<SearchResultModel> PatientSearch(string criteria)
        {
            string patientNumber = criteria;
            string firstName = criteria;
            string lastName = criteria;

            if (criteria.IndexOf(' ') > -1)
            {
                var arr = criteria.Split(' ');
                firstName = arr[0];
                lastName = arr[1];

                using (var db = new PosEntities())
                {
                    var query = from dbPatient in db.Patients
                                where dbPatient.FirstName.StartsWith(firstName)
                                && dbPatient.LastName.StartsWith(lastName)
                                orderby dbPatient.LastExamDate descending
                                select new SearchResultModel
                                {
                                    ID = dbPatient.PatientID,
                                    PatientNumber = dbPatient.PatientNumber,
                                    PatientName = dbPatient.FirstName + " " + dbPatient.LastName,
                                    DateOfBirth = dbPatient.DateOfBirth,
                                    LastExamDate = dbPatient.LastExamDate
                                };

                    return query.Take(20).ToList();
                }
            }
            else
            {

                using (var db = new PosEntities())
                {
                    var query = from dbPatient in db.Patients
                                where dbPatient.PatientNumber.StartsWith(patientNumber)
                                || dbPatient.FirstName.StartsWith(firstName)
                                || dbPatient.LastName.StartsWith(lastName)
                                orderby dbPatient.LastExamDate descending
                                select new SearchResultModel
                                {
                                    ID = dbPatient.PatientID,
                                    PatientNumber = dbPatient.PatientNumber,
                                    PatientName = dbPatient.FirstName + " " + dbPatient.LastName,
                                    DateOfBirth = dbPatient.DateOfBirth,
                                    LastExamDate = dbPatient.LastExamDate
                                };

                    return query.Take(20).ToList();
                }
            }
        }

        public static PatientModel PatientGet(int patientID, bool includeHistory)
        {
            using (var db = new PosEntities())
            {
                var patientQuery = from dbPatient in db.Patients
                                   where dbPatient.PatientID == patientID
                                   select dbPatient;

                PatientModel patientModel = Mapper.Map<Patient, PatientModel>(patientQuery.FirstOrDefault());

                if (includeHistory)
                {
                    var historyQuery = from dbExam in db.Exams
                                       where dbExam.PatientID == patientID
                                       orderby dbExam.ExamDate descending, dbExam.ExamID descending
                                       select new PatientHistoryModel
                                       {
                                           ExamID = dbExam.ExamID,
                                           ExamDate = dbExam.ExamDate,
                                           ExamCorrectDate = dbExam.ExamCorrectDate,
                                           CorrectExamID = dbExam.CorrectExamID,
                                           SavedInd = dbExam.SavedInd,
                                           LastUpdatedDate = dbExam.LastUpdatedDate
                                       };

                    patientModel.History = historyQuery.ToList();
                }

                return patientModel;
            }
        }

        public static bool PatientSave(PatientModel patient)
        {
            if(PosRepository.PatientExists(patient))
            {
                throw new ApplicationException(PosMessage.PatientNumberExists);
            }

            using (var db = new PosEntities())
            {
                var dbPatient = (from dbPat in db.Patients where dbPat.PatientID == patient.PatientID select dbPat).FirstOrDefault();
                if (dbPatient == null)
                {
                    //checking for unique patient number
                    dbPatient = new Patient();
                    db.Patients.Add(dbPatient);
                }

                Mapper.Map<PatientModel, Patient>(patient, dbPatient);

                int result = db.SaveChanges();

                if (result > 0)
                    return true;
                else
                    return false;
            }
        }


        public static ExamModel ExamGet(int patientID, int? examID)
        {
            using (var db = new PosEntities())
            {
                ExamModel model = null;
                if (examID != null)
                {
                    var examQuery = from exam in db.Exams
                                    where exam.ExamID == examID.Value && exam.PatientID == patientID
                                    select new ExamModel
                                    {
                                        ExamID = exam.ExamID,
                                        ExamText = exam.ExamText,
                                        ExamDate = exam.ExamDate,
                                        SaveInd = exam.SavedInd.Value,
                                        CorrectExamID = exam.CorrectExamID,
                                        ExamCorrectDate = exam.ExamCorrectDate,
                                        LastUpdatedDate = exam.LastUpdatedDate.Value
                                    };

                    model = examQuery.FirstOrDefault();
                }

                if (model == null) //getting the previous exam
                {
                    var examQuery = from exam in db.Exams
                                    where exam.PatientID == patientID
                                    orderby exam.SavedInd descending, exam.ExamDate descending, exam.ExamID descending
                                    select new ExamModel
                                    {
                                        ExamID = exam.ExamID,
                                        ExamText = exam.ExamText,
                                        ExamDate = exam.ExamDate,
                                        SaveInd = exam.SavedInd.Value,
                                        LastUpdatedDate = exam.LastUpdatedDate.Value
                                    };

                    model = examQuery.Take(1).FirstOrDefault();
                }

                return model;
            }
        }

        public static void ExamSave(ExamModel exam)
        {
            using (var db = new PosEntities())
            {
                Exam dbExam;
                if (exam.ExamID > 0)
                {
                    dbExam = (from e in db.Exams where e.ExamID == exam.ExamID select e).First();
                }
                else
                {
                    dbExam = new Exam();
                    db.Exams.Add(dbExam);
                }

                dbExam.ExamText = exam.ExamText;
                dbExam.ExamDate = exam.ExamDate;
                dbExam.PatientID = exam.PatientID;
                dbExam.UserName = exam.UserName;
                dbExam.SavedInd = exam.SaveInd;
                dbExam.LastUpdatedDate = DateTime.Now;
                dbExam.ExamCorrectDate = exam.ExamCorrectDate;
                dbExam.CorrectExamID = exam.CorrectExamID;

                var patient = (from pat in db.Patients where pat.PatientID == exam.PatientID select pat).First();
                if (!patient.LastExamDate.HasValue || patient.LastExamDate < exam.ExamDate)
                {
                    patient.LastExamDate = exam.ExamDate;
                }

                db.SaveChanges();
                exam.ExamID = dbExam.ExamID;
            }

        }

        public static void ExamDataSave(int examID, int patientID, NotesModel notes)
        {
            PropertyInfo[] notesFields = notes.GetType().GetProperties();
            Field field;
            PropertyInfo pi;
            Dictionary<string, string> dicJson;
            ExamData data = new ExamData();

            using(var db = new PosEntities())
            {
                var dataConfigurationQuery = from dbConfig in db.ExamDataConfigurations select dbConfig;

                foreach(var config in dataConfigurationQuery)
                {
                    dicJson = new Dictionary<string, string>();
                    if (config.FieldDataType == (int)PosConstants.FieldDataType.Json)
                    {
                        string[] examFields = config.Field.Replace(" ", "") .Split(',');
                        
                        foreach(string examField in examFields)
                        {
                            pi = notesFields.First(f => f.Name == examField);
                            field = (Field) pi.GetValue(notes);
                            dicJson.Add(field.Name, field.Value);
                        }

                        data = new ExamData()
                                {
                                    PatientID = patientID,
                                    ExamID = examID,
                                    ExamDataConfigurationID = config.ExamDataConfigurationID,
                                    FieldName = config.Name,
                                    FieldValue = JsonConvert.SerializeObject(dicJson)
                                };
                    }

                    db.ExamDatas.Add(data);
                    
                }

                //finally saving
                db.SaveChanges();
            }
        }

        public static ExamDefaultModel ExamDefaultGet(int examDefaultID)
        {
            using (var db = new PosEntities())
            {
                var defaultQuery = from dbDefault in db.ExamDefaults
                                   join user in db.Users on dbDefault.DoctorUserID equals user.UserID
                                   where dbDefault.ExamDefaultID == examDefaultID
                                   select new ExamDefaultModel
                                   {
                                       ExamDefaultID = dbDefault.ExamDefaultID,
                                       DefaultName = dbDefault.DefaultName,
                                       AgeStart = dbDefault.AgeStart,
                                       AgeEnd = dbDefault.AgeEnd,
                                       PrematureBirth = dbDefault.PrematureBirth,
                                       DoctorUserID = dbDefault.DoctorUserID,
                                       ExamText = dbDefault.ExamText,
                                       DoctorName = user.FirstName + " " + user.LastName,
                                       DoctorUserName = user.UserName
                                   };

                return defaultQuery.First();
            }
        }

        public static string ExamDefaultNotesText(string userName, int patientAge, bool prematureBirth)
        {
            using(var db = new PosEntities())
            {
                var defaultQuery = from dbDefault in db.ExamDefaults
                                   join dbUser in db.Users on dbDefault.DoctorUserID equals dbUser.UserID
                                   where dbUser.UserName.ToLower() == userName.ToLower()
                                   && dbDefault.PrematureBirth == prematureBirth
                                   && dbDefault.AgeStart <= patientAge
                                   && dbDefault.AgeEnd >= patientAge
                                   select dbDefault.ExamText;

                return defaultQuery.FirstOrDefault();
            }
        }

        public static List<int> PatientGetAllIds()
        {
            using (var db = new PosEntities())
            {
                var query = from dbPatient in db.Patients
                            select dbPatient.PatientID;
                return query.ToList();
            }
        }

        public static List<ExamModel> PatientGetExams(int patientID)
        {
            using (var db = new PosEntities())
            {
                var examQuery = from exam in db.Exams
                                where exam.PatientID == patientID
                                select new ExamModel
                                {
                                    ExamID = exam.ExamID,
                                    ExamText = exam.ExamText,
                                    ExamDate = exam.ExamDate,
                                    SaveInd = exam.SavedInd.Value,
                                    CorrectExamID = exam.CorrectExamID,
                                    ExamCorrectDate = exam.ExamCorrectDate,
                                    LastUpdatedDate = exam.LastUpdatedDate.Value
                                };

                return examQuery.ToList();
            }
        }

        public static void PatientDeleteExamData(int patientID)
        {
            using (var db = new PosEntities())
            {
                db.Database.ExecuteSqlCommand("DELETE FROM ExamData WHERE PatientID = " + patientID.ToString());
            }
        }
    }
}