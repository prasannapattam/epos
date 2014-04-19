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
        public static void BindLookupDropDown(DropDownList ddl, DataTable dataSource, string name)
        {
            dataSource.DefaultView.RowFilter = "FieldName='" + name + "'";
            dataSource.DefaultView.Sort = "SortOrder, FieldValue";

            ddl.DataSource = dataSource.DefaultView;
            ddl.DataTextField = "FieldValue";
            ddl.DataValueField = "FieldValue";
            ddl.DataBind();
            ddl.Items.Insert(0, "");
        }

        public static void BindLookupDropDown(DropDownList ddl, SqlDataReader dataSource, string dataTextField, string dataValueField)
        {
            ddl.DataSource = dataSource;
            ddl.DataTextField = dataTextField;
            ddl.DataValueField = dataValueField;
            ddl.DataBind();
            ddl.Items.Insert(0, "");
        }
        public static void SelectDropDownTextBox(DropDownList ddl, TextBox tb, string value)
        {
            WebUtil.SelectDropDownValue(ddl, value);
            if (ddl.SelectedIndex == 0 || ddl.SelectedIndex == -1)
                tb.Text = value;
            else
                tb.Text = "";
        }

        public static void SelectDropDownValue(DropDownList ddl, string value)
        {
            if (value == "")
                return;
            ListItem li = ddl.Items.FindByValue(value);
            if (li != null)
            {
                ddl.ClearSelection();
                li.Selected = true;
            }
        }

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

        public static void BindDoctorDropDown(DropDownList ddlDoctor, string selectedDoctor)
        {
            string cmdText = "SELECT FirstName + ' ' + LastName as DoctorName, UserName FROM [User]";

            SqlDataReader drUser = DBUtil.ExecuteReader(cmdText);

            WebUtil.BindLookupDropDown(ddlDoctor, drUser, "DoctorName", "UserName");

            WebUtil.SelectDropDownValue(ddlDoctor, selectedDoctor);

            drUser.Close();
            drUser.Dispose();
        }

    }
}