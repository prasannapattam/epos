using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using FontSize = DocumentFormat.OpenXml.Wordprocessing.FontSize;
using System.Data;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Data.SqlClient;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;

namespace epos.Lib.Shared
{
    public class PatientPrint
    {
        Dictionary<string, string> dict;
        private DataTable lookUp;

        Body body;
        Paragraph paragraph;
        bool firstLine = false;

        //TableRow trLetter;
        //TableCell tcLetter;

        public byte[] Generate(string examID, out string filename)
        {

            string cmdText;

            //checking whether the examid exists
            if (examID == null)
                examID = "";
            if (examID != "")
            {
                cmdText = "SELECT ExamID FROM Exam WHERE ExamID = " + examID;
                object retValue = DBUtil.ExecuteScalar(cmdText);
                if (retValue == null)
                    examID = "";
            }

            if (examID == "")
                throw new ApplicationException("Invalid Exam Notes");

            //getting the Exam details from database 
            cmdText = "SELECT p.FirstName + ' ' + p.LastName as PatientName, ExamDate, ExamText, DateOfBirth, u.FirstName + ' ' + u.LastName as DoctorName FROM Patient p JOIN EXAM e on e.PatientID = p.PatientID JOIN [User] u ON u.UserName = e.UserName WHERE ExamID = " + examID;
            SqlDataReader drExam = DBUtil.ExecuteReader(cmdText);

            drExam.Read();
            DateTime dtExam = Convert.ToDateTime(drExam["ExamDate"]);
            DateTime dob = Convert.ToDateTime(drExam["DateOfBirth"]);
            
            filename = drExam["PatientName"].ToString() + " - " + dtExam.ToString("yyyyMMdd");
            string xml = drExam["ExamText"].ToString();
            string examDate = dtExam.ToShortDateString();
            string doctorName = drExam["DoctorName"].ToString();

            drExam.Close();
            drExam.Dispose();

            //populating the drop downs
            cmdText = "SELECT ControlName, FieldValue, RTrim(lu.FieldDescription) as FieldDescription, SortOrder";
            cmdText += ", null as DefaultValue FROM ExamLookUp elu inner join LookUp lu on elu.FieldName = lu.FieldName";
            lookUp = DBUtil.ExecuteDataTable(cmdText);

            using(MemoryStream ms = new MemoryStream())
            {

                using (WordprocessingDocument package = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
                {

                    MainDocumentPart mainPart = package.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    body = new Body();
                    SectionProperties secprop = new SectionProperties();
                    PageMargin pm = new PageMargin() { Top = 395, Right = 395, Bottom = 395, Left = 395 };
                    secprop.Append(pm);
                    body.Append(secprop);

                    mainPart.Document.Append(body);

                    //LetterHeader(package);
                    GetLetter(xml, dtExam, dob, doctorName);
                    package.Close();

                }

                return ms.ToArray();
            }
        }


        private Run GetTextRun(string text, string fontName, int size, bool bold, bool italic, int breaks)
        {
            Run run = new Run();
            RunProperties runProperties = new RunProperties();

            runProperties.Append(new RunFonts() { Ascii = fontName });
            runProperties.Append(new FontSize() { Val = size.ToString() });

            RunFonts fnts = new RunFonts();

            if (bold)
                runProperties.Append(new Bold());
            if (italic)
                runProperties.Append(new Italic());

            run.Append(runProperties);
            run.Append(new Text(text));

            for (int ibreak = 0; ibreak < breaks; ibreak++)
                run.Append(new Break());

            return run;
        }

        private Run GetTableCellRun(TableRow tr, int width, int gridSpan, JustificationValues justification, int fontSize, bool globalTC)
        {
            TableCell tcRun = new TableCell();
            TableCellProperties properties = new TableCellProperties();
            if (width > 0)
                properties.Append(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = StringValue.FromString(width.ToString()) });

            if (gridSpan > 0)
                properties.Append(new GridSpan() { Val = gridSpan });

            tcRun.Append(properties);

            paragraph = new Paragraph();
            ParagraphProperties paraProperties = new ParagraphProperties();
            paraProperties.Append(new Justification() { Val = justification });
            paraProperties.Append(new SpacingBetweenLines() { After = "10", Line = "240", LineRule = LineSpacingRuleValues.Auto });

            paragraph.Append(paraProperties);
            Run run = new Run();

            if (fontSize != 0)
            {
                RunProperties runProperties = new RunProperties();
                FontSize size = new FontSize();
                size.Val = StringValue.FromString(fontSize.ToString());
                runProperties.Append(size);
                run.Append(runProperties);
            }

            paragraph.Append(run);
            tcRun.Append(paragraph);
            tr.Append(tcRun);

            //if (globalTC)
            //    tcLetter = tcRun;
            firstLine = false;
            return run;
        }

        protected void GetLetter(string xml, DateTime dtExam, DateTime dob, string doctorName)
        {
            Run run;

            dict = WebUtil.GetDictionary(xml, false);


            bool premature = false;

            if (dict["BirthHist1"] == "Premature")
                premature = true;

            if (dict.ContainsKey("Premature"))
                premature = Convert.ToBoolean(dict["Premature"] == "" ? "false" : dict["Premature"]);


            int patientMonths = monthDifference(dob, DateTime.Now);

            if (patientMonths >= 6)
                premature = false;

            paragraph = new Paragraph();
            ParagraphProperties paraProperties = new ParagraphProperties();
            paraProperties.Append(new Justification() { Val = JustificationValues.Left });
            paraProperties.Append(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto });
            paraProperties.Append(new ParagraphBorders(new BottomBorder() { Val = BorderValues.Single, Size = UInt32Value.FromUInt32(12) }));
            paragraph.Append(paraProperties);

            paragraph.Append(GetTextRun(doctorName, "Cambria", 24, true, false, 1));
            //paragraph.Append(GetTextRun("", "Cambria", 10, false, false, 1));
            paragraph.Append(GetTextRun("LONG ISLAND PEDIATRIC OPHTHALMOLOGY AND STRABISMUS, PC", "Cambria", 30, true, false, 0));
            //paragraph.Append(GetTextRun("", "Century Gothic", 3, false, false, 2));
            body.Append(paragraph);

            //EXAMDATE
            run = GetParaRun();
            run.Append(new Text(dtExam.ToString("MMMM dd, yyyy")));

            TableRow trLetter;

            trLetter = GetTable(false);

            AppendTableCell(trLetter, "Patient Name", "Greeting", "FirstName", "MiddleName", "LastName");
            AppendTableCell(trLetter, "DOB", "DOB");
            AppendTableCell(trLetter, "Age", "tbAge");
            AppendTableCell(trLetter, "Sex", "Sex");
            AppendTableCell(trLetter, "Premature", "Premature");

            trLetter = GetTable(true);
            AppendTableCell(trLetter, "Hx From", "HxFrom");
            AppendTableCell(trLetter, "Ref'd By", "Refd");
            AppendTableCell(trLetter, "Ref'd Dr", "RefDoctor");

            trLetter = GetTable(true);
            AppendTableCell(trLetter, "Allergies", "Allergies");
            AppendTableCell(trLetter, "Grade Level/Occupation", "Occupation");

            run = GetParaRun();
            //CHIEFCO & subjhx
            AppendLine("Chief Complaint", "Compliant");
            AppendLine("History", "SubjectiveHistory");
            AppendLine("Mentation", "Mentation1", "Mentation2");

            //Past Ocular/Medical History
            AppendHeadLine("Past Ocular/Medical History", true, true, 4);
            trLetter = GetTable(true);
            AppendTableCell(trLetter, "", "");
            AppendLine("Glasses since", "Glasses1", "Glasses2");
            AppendLine("Last Exam", "LastExam", "LastExamElse");
            AppendLine("Contact Lens Hx", "ContactLens1", "ContactLens2");
            AppendLine("Disease/Trauma", "DiseaseTrauma1", "DiseaseTrauma2", "DiseaseTrauma3", "DiseaseTrauma4", "DiseaseTrauma5", "DiseaseTrauma6");
            AppendLine("Surgery/Treatment", "SurgeryTreatment1", "SurgeryTreatment2", "SurgeryTreatment3", "SurgeryTreatment4", "SurgeryTreatment5");
            AppendTableCell(trLetter, "", "");
            AppendLine("Medications", "Medications");
            AppendLine("PMH", "PMH1", "PMH2", "PMH3", "PHM4");
            AppendLine("Birth Hx", "BirthHist1", "BirthHist2");
            AppendLine("GA", "GA", "BirthHist3");
            AppendLine("PC Age", "PCA");
            AppendLine("Birth Wt", "BirthWt");
            AppendLine("Development Hx", "DevelopHist1", "DevelopHist2", "DevelopHist3", "DevelopHist4");
            AppendLine("Family Hx", "FH1", "FH2", "FH3", "FH4", "FH5", "FH6");


            //Acuity/VF/Pupils
            //run = GetParaRun();
            AppendHeadLine("Acuity", true, true, 4);
            if (DataExists("VisualAcuity", "Binocsc1", "Binocsc2"))
            {
                trLetter = GetTable(false);
                AppendTableCell(trLetter, "", "");
                AppendLine("Tested with", "VisualAcuity");
                AppendTableCell(trLetter, "", "");
                AppendLine("BINOC sc Dist", "Binocsc1", "Binocsc2");
            }
            if (DataExists("VAscOD1", "VAscOD2", "DistOS1", "DistOS2", "VAccOD1", "VAccOD2", "DistOS3", "DistOS4", "VAOD1", "VAOD2", "NearOS1", "NearOS2"))
            {
                trLetter = GetTable(true);
                AppendTableCell(trLetter, "", "");
                AppendVALine("VA sc Dist", "VAscOD1", "VAscOD2", "DistOS1", "DistOS2", "NoPref");
                AppendTableCell(trLetter, "", "");
                AppendVALine("VA cc Dist", "VAccOD1", "VAccOD2", "DistOS3", "DistOS4", "");
                AppendTableCell(trLetter, "", "");
                AppendVALine("VA Near", "VAOD1", "VAOD2", "NearOS1", "NearOS2", "");
            }
            if (DataExists("SpcWr1OD", "SpcWr1OS", "SpcWr2OD", "SpcWr2OS", "SpcWr3OD", "SpcWr3OS"))
            {
                trLetter = GetTable(true);
                AppendTableCell(trLetter, "", "");
                AppendVALine("Spc Wr1", "SpcWr1OD", "", "SpcWr1OS", "", "");
                AppendTableCell(trLetter, "", "");
                AppendVALine("Spc Wr2", "SpcWr2OD", "", "SpcWr2OS", "", "");
                AppendTableCell(trLetter, "", "");
                AppendVALine("Spc Wr3", "SpcWr3OD", "", "SpcWr3OS", "", "");
            }

            if (DataExists("ManRfxOD1", "ManRfxOD2", "ManRfxOS1", "ManRfxOS2", "ManVAOD1", "ManVAOD2", "ManVSOS1", "ManVSOS2"
                        , "CycRfxOD", "CycRfxOS", "CycVAOD3", "CycVAOD4", "CycVSOS1", "CycVSOS2"))
            {
                trLetter = GetTable(true);
                AppendTableCell(trLetter, "", "");
                AppendVALine("Man Rfx", "ManRfxOD1", "ManRfxOD2", "ManRfxOS1", "ManRfxOS2", "");
                AppendTableCell(trLetter, "", "");
                AppendVALine("VA", "ManVAOD1", "ManVAOD2", "ManVSOS1", "ManVSOS2", "");
                AppendTableCell(trLetter, "", "");
                AppendVALine("Cyc Rfx", "CycRfxOD", "", "CycRfxOS", "", "");
                AppendTableCell(trLetter, "", "");
                AppendVALine("VA", "CycVAOD3", "CycVAOD4", "CycVSOS1", "CycVSOS2", "");
            }

            if (DataExists("LastManRfxOD1", "LastManRfxOD2", "LastManRfxOS1", "LastManRfxOS2"
                , "LastManVAOD1", "LastManVAOD2", "LastManVSOS1", "LastManVSOS2", "LastCycRfxOD", "LastCycRfxOS"
                , "LastCycVAOD3", "LastCycVAOD4", "LastCycVSOS1", "LastCycVSOS2"))
            {
                AppendHeadLine("PriorExam", true, true, 4);
                trLetter = GetTable(true);
                AppendTableCell(trLetter, "", "");
                AppendVALine("Man Rfx", "LastManRfxOD1", "LastManRfxOD2", "LastManRfxOS1", "LastManRfxOS2", "");
                AppendTableCell(trLetter, "", "");
                AppendVALine("VA", "LastManVAOD1", "LastManVAOD2", "LastManVSOS1", "LastManVSOS2", "");
                AppendTableCell(trLetter, "", "");
                AppendVALine("Cyc Rfx", "LastCycRfxOD", "", "LastCycRfxOS", "", "");
                AppendTableCell(trLetter, "", "");
                AppendVALine("VA", "LastCycVAOD3", "LastCycVAOD4", "LastCycVSOS1", "LastCycVSOS2", "");
            }

            AppendODOSLine("Rx given", new string[] { "RxOD1", "RxOD2" }, new string[] { "RXOS1", "RXOS2", "RXOS3", "RXOS4" });
            AppendODOSLine("CTL Rx", new string[] { "CTLRxOD1", "CTLRxOD2", "CTLRxOD3" }, new string[] { "CTLRxOS1", "CTLRxOS2", "CTLRxOS3" });

            run = GetParaRun();
            AppendLine("Visual Fields", "Confront1", "Confront2", "Confront3");
            AppendLine("Pupils", "PupilOD1", "PupilOD2", "PupilOS1", "PupilOS2", "Pupil");

            AppendHeadLine("", true, true, 4);
            //run = GetParaRun();
            //Ocular Motility      
            AppendLine("Ocular motility testing", "OcularMotility6", "OcularMotility1", "OcularMotility5", "OcularMotility2", "OcularMotility4");

            trLetter = GetTable(false);
            AppendTableCell(trLetter, "", "");
            AppendOcularMotility("OcMot1a", "OcMot1b");
            AppendTableCell(trLetter, "", "");
            AppendOcularMotility("OcMot2a", "OcMot2b", true);
            AppendTableCell(trLetter, "", "");
            AppendOcularMotility("OcMot3a", "OcMot3b");
            trLetter = GetTable(false);
            AppendTableCell(trLetter, "", "");
            AppendOcularMotility("OcMot4a", "OcMot4b");
            AppendTableCell(trLetter, "", "");
            AppendOcularMotility("OcMot5a", "OcMot5b", true);
            AppendTableCell(trLetter, "", "");
            AppendOcularMotility("OcMot6a", "OcMot6b");
            trLetter = GetTable(false);
            AppendTableCell(trLetter, "", "");
            AppendOcularMotility("OcMot7a", "OcMot7b");
            AppendTableCell(trLetter, "", "");
            AppendOcularMotility("OcMot8a", "OcMot8b", true);
            AppendTableCell(trLetter, "", "");
            AppendOcularMotility("OcMot9a", "OcMot9b");


            trLetter = GetTable(true);
            AppendTableCell(trLetter, "", "");
            //run = GetParaRun();
            AppendLine("Head Tilt Right", "HeadTiltRight1", "HeadTiltRight2");
            AppendLine("Head Tilt Left", "HeadTiltLeft1", "HeadTiltLeft2");
            AppendLine("Adaptive Head Positions", "HeadPosition1", "HeadPosition2");
            AppendLine("Double Maddox Rod", "DMR2");
            AppendLine("Preference", "Preference");

            //OCVE
            AppendTableCell(trLetter, "", "");
            AppendLine("Ocular versions", "OcularVersions1", "OcularVersions11", "OcularVersions2", "OcularVersions3", "OcularVersions31", "OcularVersions4", "OcularVersions5");
            AppendLine("Nystagmus", "Nystagmus1", "Nystagmus2");

            AppendTableCell(trLetter, "", "");
            Paragraph para = paragraph;
            AppendH("OcularVersionOD1", "OcularVersionOD2", "OcularVersionOD3", "OcularVersionOD4", "OcularVersionOD5", "OcularVersionOD6");
            paragraph = para;
            AppendTableCell(trLetter, "", "");
            AppendH("OcularVersionOS1", "OcularVersionOS2", "OcularVersionOS3", "OcularVersionOS4", "OcularVersionOS5", "OcularVersionOS6");


            if (DataExists("FusAmpBO", "FusAmpBI", "FusAmpBU", "FusAmpBD", "Convergence", "Accommodation", "LVRR1", "LVRR2", "LVRL1", "LVRL2"))
            {
                run = GetParaRun();
                AppendHeadLine("FUSIONAL AMPLITUDES", true, false, 4);
                trLetter = GetTable(false);
                AppendTableCell(trLetter, "", "");
                AppendLine("BO", "FusAmpBO");
                AppendTableCell(trLetter, "", "");
                AppendLine("BI", "FusAmpBI");
                AppendTableCell(trLetter, "", "");
                AppendLine("Convergence", "Convergence");
                trLetter = GetTable(false);
                AppendTableCell(trLetter, "", "");
                AppendLine("BU", "FusAmpBU");
                AppendTableCell(trLetter, "", "");
                AppendLine("BD", "FusAmpBD");
                AppendTableCell(trLetter, "", "");
                AppendLine("Accommodation", "Accommodation");
                trLetter = GetTable(true);
                AppendTableCell(trLetter, "", "");
                AppendLine("LVR", "LVRR1-R", "LVRR2");
                AppendTableCell(trLetter, "", "");
                AppendLine("", "LVRL1-L", "LVRL2");
            }

            run = GetParaRun();
            AppendLine("BINOCULARITY", "Binocularity1", "Binocularity2", "Binocularity3", "Binocularity4");
            if (DataExists("W4DNear1", "W4DNear2", "W4DDistance1", "W4DDistance2"))
            {
                trLetter = GetTable(false);
                AppendTableCell(trLetter, "", "");
                AppendLine("W4D Near", "W4DNear1", "W4DNear2");
                AppendTableCell(trLetter, "", "");
                AppendLine("W4D  Distance", "W4DDistance1", "W4DDistance2");
                run = GetParaRun();
            }
            AppendLine("Stereoacuity (Titmus)", "Stereo1", "Stereo2");
            AppendHeadLine("", true, true, 4);

            //ANTSEG4
            string antSegExam = "Ant Seg Exam";
            if (GetKeyText("SLE") == "True")
                antSegExam += " (SLE)";
            else if (GetKeyText("PenLight") == "True")
                antSegExam += " (Pen-light)";
            AppendLine(antSegExam, "AnteriorSegment");
            //trLetter = GetTable(false);
            //AppendTableCell(trLetter, "", "");
            //AppendLine("SLE", "SLE");
            //AppendTableCell(trLetter, "", "");
            //AppendLine("Pen-light", "PenLight");

            run = GetParaRun(false);
            AppendLine("Lids/Lashes/Lacrimal", "LidLashLacrimal1", "LidLashLacrimal2", "LidLashLacrimal3", "LidLashLacrimal4", "LidLashLacrimal5");
            if (DataExists("Exophthalmometry", "ExophthalmometryOD", "ExophthalmometryOS"))
            {
                trLetter = GetTable(false);
                AppendTableCell(trLetter, "", "");
                AppendLine("Exophthalmometry", "Exophthalmometry");
                AppendTableCell(trLetter, "", "");
                AppendLine("OD", "ExophthalmometryOD");
                AppendTableCell(trLetter, "", "");
                AppendLine("OS", "ExophthalmometryOS");
            }

            if (DataExists("Exophthalmometry", "ExophthalmometryOD", "ExophthalmometryOS"))
            {
                trLetter = GetTable(false);
                AppendTableCell(trLetter, "", "");
                AppendLine("OD PF", "ODPF");
                AppendTableCell(trLetter, "", "");
                AppendLine("MRD1", "ODMRD1");
                AppendTableCell(trLetter, "", "");
                AppendLine("MRD2", "ODMRD2");
                AppendTableCell(trLetter, "", "");
                AppendLine("Levator Fxn", "ODLevatorFxn1", "ODLevatorFxn2");

                trLetter = GetTable(false);
                AppendTableCell(trLetter, "", "");
                AppendLine("OS PF", "OSPF");
                AppendTableCell(trLetter, "", "");
                AppendLine("MRD1", "OSMRD1");
                AppendTableCell(trLetter, "", "");
                AppendLine("MRD2", "OSMRD2");
                AppendTableCell(trLetter, "", "");
                AppendLine("Levator Fxn", "OSLevatorFxn1", "OSLevatorFxn2");
            }

            trLetter = GetTable(false);
            AppendTableCell(trLetter, "", "");
            AppendLine("Conj/Sclera", "ConjSclera1", "ConjSclera2", "ConjSclera3", "ConjSclera4", "ConjSclera5");
            AppendLine("Cornea", "Cornea1", "Cornea2", "Cornea3", "Cornea4", "Cornea5");
            AppendLine("Ant Chamber", "AntChamber1", "AntChamber2", "AntChamber3", "AntChamber4", "AntChamber5");
            AppendTableCell(trLetter, "", "");
            AppendLine("Iris", "Iris1", "Iris2", "Iris3", "Iris4", "Iris5");
            AppendLine("Lens", "Lens1", "Lens2", "Lens3", "Lens4", "Lens5");
            AppendLine("Tono", "Tono1", "Tono2", "Tono3-OD", "Tono4-OS");

            run = GetParaRun();
            if (dict["Fundus1"] != "" && dict["Fundus2"] != "")
            {
                AppendLine("Dilated", "Dilate3");
                AppendLine("Fundus Exam: Optic nerve heads are", "Fundus1");
                Append(" with", "Fundus2");
                AppendLine("Retina and Mac OU", "RetinaOU");
            }
            if (DataExists("RetOD", "RetOS"))
            {
                trLetter = GetTable(false);
                AppendTableCell(trLetter, "", "");
                AppendLine("Ret Vessels OD", "RetOD");
                AppendTableCell(trLetter, "", "");
                AppendLine("Ret Vessels OS", "RetOS");
            }
            if (DataExists("MaculaOD", "MaculaOS"))
            {
                trLetter = GetTable(false);
                AppendTableCell(trLetter, "", "");
                AppendLine("Macula OD", "MaculaOD");
                AppendTableCell(trLetter, "", "");
                AppendLine("Macula OS", "MaculaOS");
            }

            AppendHeadLine("", true, true, 4);
            AppendLine("Summary", "Summary");
            if (dict.ContainsKey("Discussed"))
            {
                AppendLine("Discussed", "Discussed");
            }
            AppendLine("Advised", "Advised");
            AppendLine("Follow-up", "FollowUp1", "FollowUp2", "FollowUp3", "FollowUp4");
            AppendLine("Letter To", "CopyTo");
            if (dict.ContainsKey("ExamNoteTo"))
            {
                AppendLine("Exam Note To", "ExamNoteTo");
            }
            AppendLine("Notes", "Notes");
        }

        private void AppendH(string ov1, string ov2, string ov3, string ov4, string ov5, string ov6)
        {
            Paragraph para = paragraph;
            TableRow trLetter;
            trLetter = GetTable(false, para);
            AppendTableCell(trLetter, JustificationValues.Center);
            AppendTableCell(trLetter, JustificationValues.Center);
            Append("", ov1);
            AppendTableCell(trLetter, JustificationValues.Center);
            AppendTableCell(trLetter, JustificationValues.Center);
            Append("", ov2);
            AppendTableCell(trLetter, JustificationValues.Center);
            trLetter = GetTable(false, para);
            AppendTableCell(trLetter, JustificationValues.Center);
            Append("", ov3);
            AppendTableCell(trLetter, 3, "", "");
            AppendText("", " ----------");
            //Run run = GetRun();
            ////run.Append(new Text("-----"));

            //Line line = new Line();
            //line.From = "10";
            //line.To = "50";
            //line.Horizontal = true;
            //paragraph.Append(line);

            //AppendTableCell(trLetter, "", "");
            //AppendTableCell(trLetter, "", "");
            AppendTableCell(trLetter, JustificationValues.Center);
            Append("", ov4);
            trLetter = GetTable(false, para);
            AppendTableCell(trLetter, JustificationValues.Center);
            AppendTableCell(trLetter, JustificationValues.Center);
            Append("", ov5);
            AppendTableCell(trLetter, JustificationValues.Center);
            AppendTableCell(trLetter, JustificationValues.Center);
            Append("", ov6);
        }

        private void AppendHeadLine(string title, bool show, bool underline, uint lineSize, params string[] keys)
        {
            Run run;
            if (!show)
            {
                foreach (string key in keys)
                {
                    if (key == "")
                        continue;
                    if (GetKeyText(key) != "")
                    {
                        show = true;
                        break;
                    }
                }
            }
            if (show)
            {
                if (underline)
                {
                    //run = GetParaRun(false);
                    paragraph = new Paragraph();
                    //ParagraphProperties paraProperties = paragraph.GetFirstChild<ParagraphProperties>();
                    ParagraphProperties paraProperties = new ParagraphProperties();
                    paraProperties.Append(new SpacingBetweenLines() { Before = "0", After = "3", Line = "200", LineRule = LineSpacingRuleValues.Auto });
                    paraProperties.Append(new ParagraphBorders(new BottomBorder() { Val = BorderValues.Single, Size = UInt32Value.FromUInt32(lineSize) }));
                    paragraph.Append(paraProperties);
                    run = new Run();
                    RunProperties runProperties = new RunProperties();
                    runProperties.Append(new RunFonts() { Ascii = "Cambria" });
                    runProperties.Append(new FontSize() { Val = "8" });
                    run.Append(runProperties);
                    run.Append(new Text("."));
                    paragraph.Append(run);
                    body.Append(paragraph);
                }

                run = GetParaRun(false);
                if (title != "")
                    AppendText(title, "");
            }
        }

        private void AppendTableCell(TableRow tr, string title, params string[] keys)
        {
            int gridSpan = 0;
            Run run = GetTableCellRun(tr, 0, gridSpan, JustificationValues.Left, 0, false);
            Append(title, keys);
        }

        private void AppendTableCell(TableRow tr, int gridSpan, string title, params string[] keys)
        {
            Run run = GetTableCellRun(tr, 0, gridSpan, JustificationValues.Left, 0, false);
            Append(title, keys);
        }

        private void AppendTableCell(TableRow tr, JustificationValues align)
        {
            Run run = GetTableCellRun(tr, 0, 0, align, 0, false);
        }

        private Run GetParaRun(bool linebreak = true)
        {
            paragraph = new Paragraph();
            ParagraphProperties paraProperties = new ParagraphProperties();
            paraProperties.Append(new SpacingBetweenLines() { Before = "0", After = "0", Line = "200", LineRule = LineSpacingRuleValues.Auto });
            paragraph.Append(paraProperties);
            body.Append(paragraph);

            Run run = GetRun();
            if (linebreak)
            {
                RunProperties runProperties = new RunProperties();
                runProperties.Append(new RunFonts() { Ascii = "Cambria" });
                runProperties.Append(new FontSize() { Val = "5" });
                run.Append(runProperties);
                run.Append(new Break());
            }
            firstLine = false;
            return GetRun();
        }

        private Run GetRun()
        {
            Run run = new Run();
            RunProperties runProperties = new RunProperties();
            runProperties.Append(new RunFonts() { Ascii = "Century Gothic" });
            runProperties.Append(new FontSize() { Val = "20" });
            run.Append(runProperties);
            paragraph.Append(run);
            return run;
        }

        private void AppendText(string title, string text)
        {
            Run run;
            if (title != "")
            {
                run = GetRun();
                RunProperties runProperties = new RunProperties();
                runProperties.Bold = new Bold();
                if (text == "")
                    runProperties.Underline = new Underline() { Val = UnderlineValues.Single };
                else
                    runProperties.Italic = new Italic();
                run.Append(runProperties);
                run.Append(new Text(title) { Space = SpaceProcessingModeValues.Preserve });
            }
            run = GetRun();
            run.Append(new Text(text) { Space = SpaceProcessingModeValues.Preserve });
            firstLine = true; //once we write a line then it is not a first line
        }

        private bool Append(string title, params string[] keys)
        {
            return AppendLine(false, title, keys);
        }

        private bool AppendLine(string title, params string[] keys)
        {
            return AppendLine(firstLine, title, keys);
        }

        private bool AppendLine(bool newLine, string title, params string[] keys)
        {
            string str = "";
            string strKey;
            string suffix;
            foreach (string key in keys)
            {
                if (key == "")
                    continue;

                //checking for prefix
                if (key.IndexOf("-") > 0)
                {
                    strKey = dict[key.Substring(0, key.IndexOf("-"))].Trim();
                    suffix = key.Substring(key.IndexOf("-") + 1);
                }
                else
                {
                    suffix = "";
                    strKey = GetKeyText(key);
                }

                //checking for 
                if (strKey != "")
                {
                    switch (strKey)
                    {
                        case "True":
                            str += "Yes ";
                            break;
                        case "False":
                            str += "No ";
                            break;
                        default:
                            str += strKey + " ";
                            if (suffix != "")
                                str += suffix + " ";
                            break;
                    }
                }
            }

            if (str != "")
            {
                if (newLine)
                {
                    Run run = GetRun();
                    run.Append(new Break());
                }
                title = title == "" ? "" : title + ": ";
                AppendText(title, str.Trim());
                return true;
            }
            else
                return false;
        }

        private void AppendVALine(string title, String od1, string od2, string os1, string os2, string noPref)
        {
            bool bNoPref = false;

            if (noPref != "" & dict.ContainsKey(noPref))
                bNoPref = Convert.ToBoolean(dict[noPref]);

            string od = dict[od1];
            if (od2 != "")
                od = (od + " " + dict[od2]).Trim();
            string os = dict[os1];
            if (os2 != "")
                os = (os + " " + dict[os2]).Trim();
            string ou = "";

            if (od == os)
                ou = od;

            od = od == "" ? od : od + " OD ";
            os = os == "" ? os : os + " OS";
            ou = ou == "" ? ou : ou + " OU";

            if (bNoPref && ou != "")
                ou += ". There is no preference demonstrated for either eye.";

            if (od != "" || os != "")
            {
                if (firstLine)
                {
                    Run run = GetRun();
                    run.Append(new Break());
                }

                if (ou != "")
                    AppendText(title + ": ", ou);
                else
                    AppendText(title + ": ", od + os);
            }
        }

        public void AppendODOSLine(string title, string[] odList, string[] osList)
        {

            string od = "";
            string os = "";

            foreach (string str in odList)
            {
                if (str != "" && dict.ContainsKey(str))
                    od = (od + " " + dict[str]).Trim();
            }

            foreach (string str in osList)
            {
                if (str != "" && dict.ContainsKey(str))
                    os = (os + " " + dict[str]).Trim();
            }


            string ou = "";

            if (od == os)
                ou = od;

            od = od == "" ? od : od + " OD ";
            os = os == "" ? os : os + " OS";
            ou = ou == "" ? ou : ou + " OU";

            if (od != "" || os != "")
            {
                if (firstLine)
                {
                    Run run = GetRun();
                    run.Append(new Break());
                }

                if (ou != "")
                    AppendText(title + ": ", ou);
                else
                    AppendText(title + ": ", od + os);
            }
        }

        private void AppendOcularMotility(string OcMota, string OcMotb, bool large = false)
        {
            Table tbl = new Table();
            TableProperties tableProperties = new TableProperties();
            //tableProperties.Append(new TableWidth() { Type = TableWidthUnitValues.Pct, Width = "5000" });
            tableProperties.Append(new TableBorders()
            {
                TopBorder = new TopBorder() { Val = BorderValues.Single },
                BottomBorder = new BottomBorder() { Val = BorderValues.Single },
                LeftBorder = new LeftBorder() { Val = BorderValues.Single },
                RightBorder = new RightBorder() { Val = BorderValues.Single }
            });

            tbl.Append(tableProperties);
            paragraph.Append(tbl);

            TableRow tr = new TableRow();
            tbl.Append(tr);

            TableCell tc = new TableCell();
            tr.Append(tc);
            TableCellProperties properties = new TableCellProperties();
            int width = 1500;
            if (large)
                width = 2500;
            properties.Append(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = width.ToString() });
            properties.Append(new TableCellBorders()
            {
                TopBorder = new TopBorder() { Val = BorderValues.Single },
                BottomBorder = new BottomBorder() { Val = BorderValues.Single },
                LeftBorder = new LeftBorder() { Val = BorderValues.Single },
                RightBorder = new RightBorder() { Val = BorderValues.Single }
            });
            tc.Append(properties);

            paragraph = new Paragraph();
            ParagraphProperties paraProperties = new ParagraphProperties();
            //paraProperties.Append(new Justification() { Val = justification });
            paraProperties.Append(new SpacingBetweenLines() { After = "10", Line = "240", LineRule = LineSpacingRuleValues.Auto });

            paragraph.Append(paraProperties);
            tc.Append(paragraph);

            Run run = GetRun();
            run.Append(new Text(dict[OcMota]));

            tc = new TableCell();
            tr.Append(tc);
            properties = new TableCellProperties();
            properties.Append(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = width.ToString() });
            properties.Append(new TableCellBorders()
            {
                TopBorder = new TopBorder() { Val = BorderValues.Single },
                BottomBorder = new BottomBorder() { Val = BorderValues.Single },
                LeftBorder = new LeftBorder() { Val = BorderValues.Single },
                RightBorder = new RightBorder() { Val = BorderValues.Single }
            });
            tc.Append(properties);

            paragraph = new Paragraph();
            paraProperties = new ParagraphProperties();
            //paraProperties.Append(new Justification() { Val = justification });
            paraProperties.Append(new SpacingBetweenLines() { After = "10", Line = "240", LineRule = LineSpacingRuleValues.Auto });

            paragraph.Append(paraProperties);
            tc.Append(paragraph);

            run = GetRun();
            run.Append(new Text(dict[OcMotb]));

        }

        private int monthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }

        private TableRow GetTable(bool blankLine, Paragraph para = null)
        {
            if (blankLine)
            {
                paragraph = new Paragraph();
                ParagraphProperties paraProperties = new ParagraphProperties();
                paraProperties.Append(new Justification() { Val = JustificationValues.Left });
                paraProperties.Append(new SpacingBetweenLines() { After = "10", Line = "240", LineRule = LineSpacingRuleValues.Auto });
                paragraph.Append(paraProperties);
                body.Append(paragraph);
                Run run = new Run();
                RunProperties runProperties = new RunProperties();
                runProperties.Append(new RunFonts() { Ascii = "Cambria" });
                runProperties.Append(new FontSize() { Val = "3" });
                run.Append(runProperties);
                run.Append(new Text(".") { Space = SpaceProcessingModeValues.Preserve });
                paragraph.Append(run);
            }

            Table tbl = new Table();
            TableProperties tableProperties = new TableProperties();
            if (para != null)  // this is used only for drawing the H
                tableProperties.Append(new TableWidth() { Type = TableWidthUnitValues.Auto, Width = "0" });
            else
                tableProperties.Append(new TableWidth() { Type = TableWidthUnitValues.Pct, Width = "5000" });
            tableProperties.Append(new TableBorders()
            {
                TopBorder = new TopBorder() { Val = BorderValues.None },
                BottomBorder = new BottomBorder() { Val = BorderValues.None },
                LeftBorder = new LeftBorder() { Val = BorderValues.None },
                RightBorder = new RightBorder() { Val = BorderValues.None }
            });
            tableProperties.Append(new AutofitToFirstFixedWidthCell());
            tbl.Append(tableProperties);
            if (para != null)
                para.Append(tbl);
            else
                body.Append(tbl);
            TableRow trLetter = new TableRow();
            tbl.Append(trLetter);

            return trLetter;
        }

        private bool DataExists(params string[] keys)
        {
            bool ret = false;

            foreach (string key in keys)
            {
                if (key == "")
                    continue;
                if (GetKeyText(key) != "")
                {
                    ret = true;
                    break;
                }
            }

            return ret;
        }

        private string GetKeyText(string key)
        {
            if (key == "" || !dict.ContainsKey(key))
                return "";


            string keyText = dict[key].Trim();

            //checking for drop downs, showing the selected text
            string filter = "ControlName='" + key + "' AND FieldDescription='" + keyText.Replace("'", "''") + "'";
            DataView dv = new DataView(lookUp, filter, "", DataViewRowState.CurrentRows);
            if (dv.Count > 0)
                keyText = dv[0]["FieldValue"].ToString();

            //checking for dups
            if (keyText.IndexOf("~") > 0)
            {
                string fieldDescription;
                filter = "ControlName='" + key + "'";
                dv = new DataView(lookUp, filter, "", DataViewRowState.CurrentRows);
                if (dv.Count > 0)
                {
                    //checking for duplicate values
                    Dictionary<string, string> dups = new Dictionary<string, string>();
                    int dupCount = 1;
                    for (int iCounter = 0; iCounter < dv.Count; iCounter++)
                    {
                        if (dups.ContainsKey(dv[iCounter]["fieldDescription"].ToString()))
                        {
                            fieldDescription = dv[iCounter]["fieldDescription"].ToString() + "~" + dupCount.ToString();
                            if (fieldDescription == keyText)
                                keyText = dv[iCounter]["fieldValue"].ToString();
                            dupCount++;
                        }
                        else
                        {
                            dups.Add(dv[iCounter]["fieldDescription"].ToString(), "");
                        }
                    }

                }
            }

            return keyText;
        }
    }
}