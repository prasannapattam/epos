using epos.Lib.Repository;
using epos.Lib.Shared;
using epos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace epos.Controllers
{
    public class PatientController : ApiController
    {
        public AjaxModel<PatientModel> Get(int id)
        {
            AjaxModel<PatientModel> ajax = null;

            PatientModel result = PosRepository.PatientGet(id, true);

            if(result == null)
            {
                ajax = new AjaxModel<PatientModel>() { Success = false, Message = PosMessage.PatientInvalid, Model = null };
            }
            else
            {
                ajax = new AjaxModel<PatientModel>(){Success = true, Message = "", Model = result };
            }

            return ajax;
        }

        public AjaxModel<string> Post([FromBody] PatientModel model)
        {
            AjaxModel<string> ajax = new AjaxModel<string>() { Success = true, Message = PosMessage.PatientSaveSuccessful, Model = PosMessage.Blank };
            try
            {
                PosRepository.PatientSave(model);
            }
            catch(Exception exp)
            {
                ajax.Success = false;
                ajax.Message = exp.Message;
            }


            return ajax;
        }
    }
}
