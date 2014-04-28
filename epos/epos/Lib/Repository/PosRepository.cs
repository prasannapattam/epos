﻿using epos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using epos.Lib.Shared;
using System.Web.Mvc;

namespace epos.Lib.Repository
{
    public static class PosRepository
    {
        public static ProfileModel ProfileGet(LoginModel login)
        {
            using(var db = new PosEntities())
            {
                var query = from dbUser in db.Users
                            where dbUser.UserName == login.UserName && dbUser.Password == login.UserPassword
                            select new ProfileModel
                            {
                                UserID = dbUser.UserID,
                                FirstName = dbUser.FirstName,
                                LastName = dbUser.LastName,
                                UserName = dbUser.UserName,
                                PhotoUrl = dbUser.PhotoUrl
                            };

                return query.FirstOrDefault();
            }
        }

        public static List<LookUpModel> LookUpGet()
        {
            using(var db = new PosEntities())
            {
                var query = from dbLookup in db.LookUps
                            orderby dbLookup.FieldName, dbLookup.SortOrder
                            select new LookUpModel
                            {
                                LookUpID = dbLookup.LookupID,
                                FieldName = dbLookup.FieldName,
                                FieldValue = dbLookup.FieldValue,
                                FieldDescription = dbLookup.FieldDescription,
                                SortOrder = dbLookup.SortOrder
                            };

                return query.ToList();
            }
        }

        public static List<SelectListItem> ExamLookUpGet()
        {
            using(var db = new PosEntities())
            {
                var query = from dbExamLookUp in db.ExamLookUps
                            orderby dbExamLookUp.ControlName
                            select new SelectListItem
                            {
                                Text = dbExamLookUp.ControlName,
                                Value = dbExamLookUp.FieldName
                            };

                return query.ToList();
            }
        }

        public static List<UserModel> UserSearch(string criteria)
        {
            string firstName = criteria;
            string lastName = criteria;
            string userName = criteria;

            if (criteria.IndexOf(' ') > -1)
            {
                var arr = criteria.Split(' ');
                firstName = arr[0];
                lastName = arr[1];

                using (var db = new PosEntities())
                {
                    var query = from dbUser in db.Users
                                where dbUser.FirstName.StartsWith(firstName)
                                && dbUser.LastName.StartsWith(lastName)
                                orderby dbUser.FirstName ascending
                                select new UserModel
                                {
                                    UserID = dbUser.UserID,
                                    FirstName = dbUser.FirstName,
                                    LastName = dbUser.LastName,
                                    UserName = dbUser.UserName,
                                    PhotoUrl = dbUser.PhotoUrl
                                };

                    return query.Take(20).ToList();
                }
            }
            else
            {

                using (var db = new PosEntities())
                {
                    var query = from dbUser in db.Users
                                where dbUser.UserName.StartsWith(userName)
                                || dbUser.FirstName.StartsWith(firstName)
                                || dbUser.LastName.StartsWith(lastName)
                                orderby dbUser.FirstName ascending
                                select new UserModel
                                {
                                    UserID = dbUser.UserID,
                                    FirstName = dbUser.FirstName,
                                    LastName = dbUser.LastName,
                                    UserName = dbUser.UserName,
                                    PhotoUrl = dbUser.PhotoUrl
                                };

                    return query.Take(20).ToList();
                }
            }
        }

        public static UserModel UserGet(int userID)
        {
            using (var db = new PosEntities())
            {
                var userQuery = from dbUser in db.Users
                                where dbUser.UserID == userID
                                select dbUser;

                UserModel userModel = Mapper.Map<User, UserModel>(userQuery.FirstOrDefault());

                var defaultQuery = from dbDefault in db.ExamDefaults
                                   where dbDefault.DoctorUserID == userID
                                   orderby dbDefault.DefaultName ascending
                                   select new ExamDefaultModel
                                   {
                                       ExamDefaultID = dbDefault.ExamDefaultID,
                                       DefaultName = dbDefault.DefaultName,
                                       AgeStart = dbDefault.AgeStart,
                                       AgeEnd = dbDefault.AgeEnd,
                                       PrematureBirth = dbDefault.PrematureBirth,
                                       DoctorUserID = dbDefault.DoctorUserID,
                                       ExamText = ""
                                   };

                userModel.Defaults = defaultQuery.ToList();

                return userModel;
            }
        }

        public static bool UserSave(UserModel user)
        {
            using (var db = new PosEntities())
            {
                var dbUser = (from dbusr in db.Users where dbusr.UserID == user.UserID select dbusr).FirstOrDefault();
                if (dbUser == null)
                {
                    //checking for unique patient number
                    dbUser = (from dbusr in db.Users where dbusr.UserName == user.UserName select dbusr).FirstOrDefault();
                    if (dbUser != null)
                    {
                        throw new ApplicationException(PosMessage.UserNameExists);
                    }
                    dbUser = new User();
                    db.Users.Add(dbUser);
                }


                Mapper.Map<UserModel, User>(user, dbUser);

                int result = db.SaveChanges();

                if (result > 0)
                    return true;
                else
                    return false;
            }
        }

        public static List<SelectListItem> DoctorsGet()
        {
            using (var db = new PosEntities())
            {
                var userQuery = from user in db.Users
                                join queue in db.PrintQueues on user.UserName equals queue.UserName
                                orderby user.FirstName, user.LastName
                                select new SelectListItem
                                {
                                    Text = user.FirstName + " " + user.LastName,
                                    Value = user.UserName
                                };
                return userQuery.Distinct().ToList();
            }
        }

        public static PrintQueueModel PrintQueueGet()
        {
            //first populating the users

            using(var db = new PosEntities())
            {
                
                PrintQueueModel model = new PrintQueueModel();
                model.Doctors = DoctorsGet();

                var itemQuery = from queue in db.PrintQueues
                                join exam in db.Exams on queue.ExamID equals exam.ExamID
                                join patient in db.Patients on exam.PatientID equals patient.PatientID
                                join user in db.Users on queue.UserName equals user.UserName
                                orderby exam.LastUpdatedDate
                                select new PrintQueueItem
                                {
                                    PatientName = patient.FirstName + " " + patient.LastName,
                                    ExamDate = exam.ExamDate,
                                    ExamID = exam.ExamID,
                                    PrintQueueID = queue.PrintQueueID,
                                    PrintExamNote = queue.PrintExamNote,
                                    CorrectExamID = exam.CorrectExamID,
                                    ExamCorrectDate = exam.ExamCorrectDate,
                                    DoctorName = user.FirstName + " " + user.LastName,
                                    LastUpdatedDate = exam.LastUpdatedDate,
                                    UserName = queue.UserName
                                };
                model.Items = itemQuery.ToList();

                return model;
            }
        }

        public static void PrintQueueRemove(PrintQueueItem item)
        {
            using (var db = new PosEntities())
            {
                var queueItem = (from queue in db.PrintQueues where queue.PrintQueueID == item.PrintQueueID select queue).First();
                db.PrintQueues.Remove(queueItem);
                db.SaveChanges();
            }
        }

        public static void PrintQueueRemove(int examID)
        {
            using(var db = new PosEntities())
            {
                var queueItems = (from queue in db.PrintQueues where queue.ExamID == examID select queue).ToList();
                foreach(var item in queueItems)
                {
                    db.PrintQueues.Remove(item);
                }
                db.SaveChanges();
            }
        }

        public static void PrintQueueAdd(PrintQueueItem item)
        {
            using(var db = new PosEntities())
            {
                PrintQueue queue = new PrintQueue() { ExamID = item.ExamID, UserName = item.UserName, PrintExamNote = item.PrintExamNote };
                db.PrintQueues.Add(queue);
                db.SaveChanges();
            }
        }

    }
}



