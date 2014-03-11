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
    public class PrintController : ApiController
    {
        public HttpResponseMessage Get(int id, int type)
        {
            string filename;

            byte[] bytes = null;

            if(type == 1)
                bytes = new PatientLetter().Generate(id.ToString(), out filename);
            else
                bytes = new PatientPrint().Generate(id.ToString(), out filename);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(bytes);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-word");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = filename + ".docx";
            return result;
        }
    }
}
