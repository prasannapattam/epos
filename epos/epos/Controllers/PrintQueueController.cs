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
    public class PrintQueueController : ApiController
    {
        public AjaxModel<PrintQueueModel> Get()
        {
            AjaxModel<PrintQueueModel> ajax = null;

            PrintQueueModel model = PosRepository.PrintQueueGet();
            if (model == null)
            {
                ajax = new AjaxModel<PrintQueueModel>() { Success = false, Message = PosMessage.PrintQueueError, Model = null };
            }
            else
            {
                ajax = new AjaxModel<PrintQueueModel>() { Success = true, Message = "", Model = model };
            }
            return ajax;
        }

        public AjaxModel<PrintQueueModel> Post(PrintQueueItem item)
        {
            AjaxModel<PrintQueueModel> ajax = null;

            try
            {
                PosRepository.PrintQueueRemove(item);
                ajax = new AjaxModel<PrintQueueModel>() { Success = true, Message = PosMessage.PrintQueueDeleteSuccessful, Model = null };
            }
            catch (Exception e)
            {
                ajax = new AjaxModel<PrintQueueModel>() { Success = false, Message = PosMessage.PrintQueueDeleteError + " - " + e.Message, Model = null };
            }

            return ajax;
        }
    }
}
