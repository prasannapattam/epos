using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using epos.Lib.Repository;
using epos.Lib.Shared;
using epos.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace epos.Controllers
{
    public class PatientSearchController : ApiController
    {
        public AjaxModel<List<PatientSearchResultModel>> Post([FromBody] string criteria)
        {
            AjaxModel<List<PatientSearchResultModel>> ajax = null;

            List<PatientSearchResultModel> result = PosRepository.PatientSearch(criteria.Trim());

            if (result.Count == 0)
            {
                ajax = new AjaxModel<List<PatientSearchResultModel>>() { Success = false, Message = PosMessage.PatientSearchNoRecords, Model = null };
            }
            else
            {
                ajax = new AjaxModel<List<PatientSearchResultModel>>() { Success = true, Message = "", Model = result };
            }

            return ajax;
        }
    }
}

