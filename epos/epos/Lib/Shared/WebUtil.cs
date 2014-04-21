using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Xml;

namespace epos.Lib.Shared
{
    /// <summary>
    /// Summary description for WebUtil
    /// </summary>
    public static class WebUtil
    {
        public static Dictionary<string, string> GetDictionary(string xml, bool removeDupsSuffix)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            StringReader stringReader = new StringReader(xml);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CheckCharacters = false;
            XmlReader reader = XmlReader.Create(stringReader, settings);
            //XmlTextReader reader = new XmlTextReader(stringReader);
            //reader.WhitespaceHandling = WhitespaceHandling.None;

            string fieldName = "";
            string fieldValue = "";
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        fieldName = reader.Name;
                        break;
                    case XmlNodeType.Text:
                        fieldValue = reader.Value;
                        break;
                    case XmlNodeType.CDATA:
                        fieldValue = reader.Value;
                        break;
                    case XmlNodeType.EndElement:
                        if (fieldName != "")
                        {
                            if (removeDupsSuffix & fieldValue.Contains("~"))
                            {
                                string dupNumber = fieldValue.Substring(fieldValue.LastIndexOf('~') + 1);
                                int dupNumberOut;
                                if (Int32.TryParse(dupNumber, out dupNumberOut))
                                {
                                    fieldValue = fieldValue.Remove(fieldValue.LastIndexOf('~'));
                                }
                            }
                            dict.Add(fieldName, fieldValue);
                        }
                        fieldName = "";
                        break;
                }
            }

            reader.Close();
            stringReader.Close();
            stringReader.Dispose();

            return dict;
        }


    }
}