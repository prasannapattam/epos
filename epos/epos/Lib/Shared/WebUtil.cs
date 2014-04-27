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
using epos.Models;
using System.Reflection;
using System.Web.Mvc;
using epos.Lib.Repository;

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

        public static string GetXml(NotesModel notes, bool acceptDefaults, Dictionary<string, string> dict)
        {
            dict = (dict == null) ? new Dictionary<string, string>() : dict;

            StringWriter stringWriter = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CheckCharacters = false;
            XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings);

            xmlWriter.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"");
            xmlWriter.WriteStartElement("patient");

            //getting the properties from the model
            PropertyInfo[] notesFields = notes.GetType().GetProperties();
            Field field;

            foreach (var pi in notesFields)
            {
                if (pi.PropertyType == typeof(Field))
                {
                    field = (Field)pi.GetValue(notes);

                    if (field == null)
                    {
                        field = new Field() { Name = pi.Name, Value = String.Empty, ColourType = (int)PosConstants.ColourType.Normal };
                    }

                    if (field.Value == null)
                        field.Value = String.Empty;

                    if (dict.ContainsKey(field.Name) && dict[field.Name] != field.Value.Trim())
                    {
                        field.ColourType = (int)PosConstants.ColourType.Correct;
                    }
                    else if (acceptDefaults)
                    {
                        field.ColourType = (int)PosConstants.ColourType.Normal;
                    }

                    xmlWriter.WriteStartElement(field.Name);
                    xmlWriter.WriteAttributeString("CustomColourType", field.ColourType.ToString());
                    xmlWriter.WriteCData(field.Value.Trim());
                    xmlWriter.WriteEndElement();

                }

            }

            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            xmlWriter.Close();
            stringWriter.Flush();
            string xml = stringWriter.ToString();
            stringWriter.Dispose();
            return xml;
        }


    }
}