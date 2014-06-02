using epos.Lib.Repository;
using epos.Lib.Shared;
using epos.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace epos.Controllers
{
    public class LookUpController : ApiController
    {
        public AjaxModel<Dictionary<string, List<LookUpModel>>> Get()
        {
            AjaxModel<Dictionary<string, List<LookUpModel>>> ajax = null;

            List<LookUpModel> lookupList = PosRepository.LookUpGet();
            if (lookupList == null)
            {
                ajax = new AjaxModel<Dictionary<string, List<LookUpModel>>>() { Success = false, Message = PosMessage.LookUpGetError, Model = null };
            }
            else
            {
                var model = GetLookUpDictionary(lookupList);
                ajax = new AjaxModel<Dictionary<string, List<LookUpModel>>>() { Success = true, Message = "", Model = model };
            }
            return ajax; 
        }

        public AjaxModel<String> PostSaveLookUps(List<LookUpModel> lookupList)
        {

            AjaxModel<string> ajax = new AjaxModel<string>();
            try
            {

                if (lookupList != null)
                {
                    foreach (var lookupModel in lookupList)
                    {
                        if (lookupModel.IsNew)
                        {
                            PosRepository.AddLookUpModel(lookupModel);
                        }

                        else if (lookupModel.IsDeleted)
                        {
                            PosRepository.RemoveLookUpModel(lookupModel.LookUpID);
                        }
                        else
                        {
                            PosRepository.UpdateLookUpModel(lookupModel);
                        }
                    }
                    ajax.Success = true;
                ajax.Message = PosMessage.LookupsSavedSuccessfully  ;

                }
            }
            catch (Exception exp)
            {

                ajax.Success = false;
                ajax.Message = String.Format("Message {0}  Error {1}" ,PosMessage.LookUpsSaveError , exp.Message) ;
            }

            return ajax;
        }

        private Dictionary<string, List<LookUpModel>> GetLookUpDictionary(List<LookUpModel> list)
        {
            var dic = new Dictionary<string, List<LookUpModel>>();

            string fieldName = String.Empty;
            int dupCount = 1;

            List<LookUpModel> fieldList = new List<LookUpModel>();
            foreach(var item in list)
            {
                if(item.FieldName != fieldName)
                {
                    fieldName = item.FieldName;
                    fieldList = new List<LookUpModel>();
                    dic[fieldName] = fieldList;
                    dupCount = 1;
                }

                if(fieldList.Exists(listItem => listItem.FieldDescription == item.FieldDescription))
                {
                    item.FieldDescription += "~" + dupCount.ToString();
                    dupCount++;
                }

                fieldList.Add(item);
            }


            return dic;
        }
    }
}