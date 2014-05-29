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