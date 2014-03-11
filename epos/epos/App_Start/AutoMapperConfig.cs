using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using epos.Lib.Repository;
using epos.Models;

namespace epos.App_Start
{
    public class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.CreateMap<Patient, PatientModel>();
            Mapper.CreateMap<PatientModel, Patient>();
            Mapper.CreateMap<User, UserModel>();
            Mapper.CreateMap<UserModel, User>();
        }
    }
}