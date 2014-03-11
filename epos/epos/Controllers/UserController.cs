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
    public class UserController : ApiController
    {
        public AjaxModel<UserModel> Get(int id)
        {
            AjaxModel<UserModel> ajax = null;

            UserModel result = PosRepository.UserGet(id);

            if (result == null)
            {
                ajax = new AjaxModel<UserModel>() { Success = false, Message = PosMessage.UserInvalid, Model = null };
            }
            else
            {
                ajax = new AjaxModel<UserModel>() { Success = true, Message = "", Model = result };
            }

            return ajax;
        }

        public AjaxModel<string> Post([FromBody] UserModel model)
        {
            PosRepository.UserSave(model);

            AjaxModel<string> ajax = new AjaxModel<string>() { Success = true, Message = PosMessage.UserSaveSuccessful, Model = PosMessage.Blank };

            return ajax;
        }
    }
}
