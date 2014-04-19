using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epos.Lib.Shared
{
    public static class PosMessage
    {
        public const string LoginError = "Invalid username or password";

        //patient
        public const string PatientSearchNoRecords = "No patients found for the search criteria";
        public const string PatientInvalid = "Invalid Patient ID";
        public const string PatientNumberExists = "Patient Number already exists";
        public const string PatientSaveSuccessful = "Patient saved successfully";

        //user
        public const string UserSearchNoRecords = "No Users found for the search criteria";
        public const string UserInvalid = "Invalid User ID";
        public const string UserSaveSuccessful = "User saved successfully";
        public const string UserNameExists = "User Name already exists";

        //PrintQueue
        public const string PrintQueueError = "Unable to retrieve print queue";
        public const string PrintQueueDeleteSuccessful = "Removed queue item";
        public const string PrintQueueDeleteError = "Error in removing queue item, please try again";


        public const string LookUpGetError = "Unable to retrieve Drop Down data";

        public const string Blank = "There is no data to pass back to the client";
    }
}