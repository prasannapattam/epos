using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using epos.Lib.Repository;
using epos.Lib.Shared;
using epos.Models;
using epos.Lib.Domain;

namespace epos.Controllers
{
    public class CompatController : ApiController
    {
        [Route("api/compatlastvisit")]
        [HttpPost]
        public AjaxModel<string> LastVisitDateUpdate()
        {
            AjaxModel<string> ajax = new AjaxModel<string>() { Success = true, Message = PosMessage.LastVisitDateUpdateSuccess, Model = null };

            try
            {
                PosRepository.UpdateLastVisitDate();
            }
            catch (Exception exp)
            {
                ajax.Success = false;
                ajax.Message = exp.Message;
            }

            return ajax;
        }

        [Route("api/compatpatientids")]
        [HttpPost]
        public AjaxModel<List<int>> GetPatientIds()
        {
            AjaxModel<List<int>> ajax = new AjaxModel<List<int>>() { Success = true, Message = "", Model = null };
    
            try
            {
                System.Threading.Thread.Sleep(100);
                ajax.Model = PatientRepository.PatientGetAllIds();
            }
            catch (Exception exp)
            {
                ajax.Success = false;
                ajax.Message = exp.Message;
            }

            return ajax;
        }
        [Route("api/compatupdatehistory")]
        [HttpPost]
        public AjaxModel<List<int>> UpdatePatientHistory(int patientID)
        {
            AjaxModel<List<int>> ajax = new AjaxModel<List<int>>() { Success = true, Message = "", Model = null };

            try
            {
                System.Threading.Thread.Sleep(100);
                //NotesDomain domain = new NotesDomain();
                //domain.PatientExamDataSave(patientID);
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
