using epos.Lib.Repository;
using epos.Lib.Shared;
using epos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace epos.Controllers
{
    public class UserSearchController : ApiController
    {
        public AjaxModel<List<UserModel>> Post([FromBody] string criteria)
        {
            AjaxModel<List<UserModel>> ajax = null;

            List<UserModel> result = PosRepository.UserSearch(criteria.Trim());

            if (result.Count == 0)
            {
                ajax = new AjaxModel<List<UserModel>>() { Success = false, Message = PosMessage.UserSearchNoRecords, Model = null };
            }
            else
            {
                ajax = new AjaxModel<List<UserModel>>() { Success = true, Message = "", Model = result };
            }

            return ajax;
        }
    }
}