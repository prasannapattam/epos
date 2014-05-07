using epos.Lib.Repository;
using epos.Lib.Shared;
using epos.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
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


        public async Task<HttpResponseMessage> Post()
        {
            AjaxModel<string> ajax = new AjaxModel<string>() { Success = true, Message = PosMessage.UserSaveSuccessful, Model = PosMessage.Blank };

            try
            {
                string root = HttpContext.Current.Server.MapPath("~/Data");
                var provider = new MultipartFormDataStreamProvider(root);
                await Request.Content.ReadAsMultipartAsync(provider);

                //getting the user details
                UserModel model = new UserModel();
                model.UserID = Convert.ToInt32(provider.FormData["UserID"]);
                model.FirstName = provider.FormData["FirstName"];
                model.LastName = provider.FormData["LastName"];
                model.UserName = provider.FormData["UserName"];
                model.Password = provider.FormData["Password"];
                model.PhotoUrl = provider.FormData["PhotoUrl"];
                model.PhotoUrl = model.PhotoUrl == "" ? null : model.PhotoUrl;

                //checking whether the user exists
                if (PosRepository.UserExists(model))
                {
                    ajax.Success = false;
                    ajax.Message = PosMessage.UserNameExists;
                }
                else
                {
                    if (provider.FileData.Count > 0)
                    {
                        MultipartFileData fileData = provider.FileData[0];
                        FileInfo fi = new FileInfo(fileData.LocalFileName);

                        //getting the file saving path
                        string clientFileName = fileData.Headers.ContentDisposition.FileName.Replace(@"""", "");
                        if (clientFileName != "")
                        {
                            string clientExtension = clientFileName.Substring(clientFileName.LastIndexOf('.'));
                            string serverFileName = fi.DirectoryName + @"\" + model.UserName + clientExtension;
                            model.PhotoUrl = model.UserName;

                            FileInfo fiOld = new FileInfo(serverFileName);
                            if (fiOld.Exists)
                                fiOld.Delete();
                            //if (File.Exists())
                            fi.MoveTo(serverFileName);
                        }
                        else
                        {
                            if (fi.Exists)
                                fi.Delete();
                        }
                    }

                    PosRepository.UserSave(model);
                }
            }
            catch (Exception exp)
            {
                ajax.Success = false;
                ajax.Message = exp.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, ajax, "application/json"); ;
        }
    }
}
