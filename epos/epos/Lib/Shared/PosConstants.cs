using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epos.Lib.Shared
{
    public static class PosConstants
    {
        public const bool Obscure = false;

        public enum NotesType
        {
            New = 1,
            Correct = 2,
            Saved = 3,
            Default = 4
        }

        public enum ColourType
        {
            Normal = 0,
            New = 1,
            Correct = 2
        }

        public enum NotesSaveType
        {
            Save = 1,
            SignOff = 2,
            Correct = 3
        }

        public enum FieldType
        {
            Notes = 0,
            Patient = 1,
            Default = 2
        }

        public enum Sex
        {
            Male = 0,
            Female = 1
        }
    }
}