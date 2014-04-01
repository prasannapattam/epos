using epos.Lib.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace epos.Models
{
    public class NotesModel
    {
        //default fields
        public Field hdnExamDefaultID { get; set; }
        public Field DefaultName { get; set; }
        public Field AgeStart { get; set; }
        public Field AgeEnd { get; set; }
        public Field PrematureBirth { get; set; }
        public Field DoctorID { get; set; }
        public List<SelectListItem> Doctors { get; set; }

        //patient fields
        public Field hdnPatientID { get; set; }
        public Field hdnExamID { get; set; }
        public Field hdnNoteType { get; set; }
        public Field hdnColourType { get; set; }
        public Field hdnDefaultInd { get; set; }
        public Field Greeting { get; set; }
        public Field FirstName { get; set; }
        public Field MiddleName { get; set; }
        public Field LastName { get; set; }
        public Field DOB { get; set; }
        public Field Age { get; set; }
        public Field tbAge { get; set; }
        public Field PatientMonths { get; set; }
        public Field Sex { get; set; }
        public Field BoyGirlAdult { get; set; }
        public Field Premature { get; set; }
        public Field ExamDate { get; set; }
        public Field User { get; set; }
        public Field HxFrom { get; set; }
        public Field ddlHxFrom { get; set; }
        public Field tbHxOther { get; set; }
        public Field Refd { get; set; }
        public Field RefDoctor { get; set; }
        public Field Allergies { get; set; }
        public Field Occupation { get; set; }

        //Compliant Tab
        public Field Compliant { get; set; }
        public Field SubjectiveHistory { get; set; }
        public Field Mentation1 { get; set; }
        public Field Mentation2 { get; set; }
        public Field Glasses1 { get; set; }
        public Field Glasses2 { get; set; }
        public Field LastExam { get; set; }
        public Field LastExamElse { get; set; }
        public Field ContactLens1 { get; set; }
        public Field ContactLens2 { get; set; }
        public Field DiseaseTrauma1 { get; set; }
        public Field DiseaseTrauma2 { get; set; }
        public Field DiseaseTrauma3 { get; set; }
        public Field DiseaseTrauma4 { get; set; }
        public Field DiseaseTrauma5 { get; set; }
        public Field DiseaseTrauma6 { get; set; }
        public Field SurgeryTreatment1 { get; set; }
        public Field SurgeryTreatment2 { get; set; }
        public Field SurgeryTreatment3 { get; set; }
        public Field SurgeryTreatment4 { get; set; }
        public Field SurgeryTreatment5 { get; set; }
        public Field Medications { get; set; }
        public Field PMH1 { get; set; }
        public Field PMH2 { get; set; }
        public Field PMH3 { get; set; }
        public Field PHM4 { get; set; }
        public Field BirthHist1 { get; set; }
        public Field BirthHist2 { get; set; }
        public Field BirthHist3 { get; set; }
        public Field GA { get; set; }
        public Field PCA { get; set; }
        public Field BirthWt { get; set; }
        public Field DevelopHist1 { get; set; }
        public Field DevelopHist2 { get; set; }
        public Field DevelopHist3 { get; set; }
        public Field DevelopHist4 { get; set; }
        public Field FH1 { get; set; }
        public Field FH2 { get; set; }
        public Field FH3 { get; set; }
        public Field FH4 { get; set; }
        public Field FH5 { get; set; }
        public Field FH6 { get; set; }

        //Visual
        public Field VisualAcuity { get; set; }
        public Field Binocsc1 { get; set; }
        public Field Binocsc2 { get; set; }
        public Field VAscOD1 { get; set; }
        public Field VAscOD2 { get; set; }
        public Field VAccOD1 { get; set; }
        public Field VAccOD2 { get; set; }
        public Field VAOD1 { get; set; }
        public Field VAOD2 { get; set; }
        public Field NoPref { get; set; }
        public Field DistOS1 { get; set; }
        public Field DistOS2 { get; set; }
        public Field DistOS3 { get; set; }
        public Field DistOS4 { get; set; }
        public Field NearOS1 { get; set; }
        public Field NearOS2 { get; set; }
        public Field SpcWr1OD { get; set; }
        public Field SpcWr2OD { get; set; }
        public Field SpcWr3OD { get; set; }
        public Field SpcWr1OS { get; set; }
        public Field SpcWr2OS { get; set; }
        public Field SpcWr3OS { get; set; }
        public Field ManRfxOD1 { get; set; }
        public Field ManRfxOD2 { get; set; }
        public Field ManVAOD1 { get; set; }
        public Field ManVAOD2 { get; set; }
        public Field CycRfxOD { get; set; }
        public Field CycVAOD3 { get; set; }
        public Field CycVAOD4 { get; set; }
        public Field ManRfxOS1 { get; set; }
        public Field ManRfxOS2 { get; set; }
        public Field ManVSOS1 { get; set; }
        public Field ManVSOS2 { get; set; }
        public Field CycRfxOS { get; set; }
        public Field CycVSOS1 { get; set; }
        public Field CycVSOS2 { get; set; }
        public Field Greeting { get; set; } //prior exam
        public Field Greeting { get; set; }
        public Field Greeting { get; set; }
        public Field Greeting { get; set; }


        public PosConstants.NotesType NotesType { get; set; }
    }

    public class Field
    {
        public string Value { get; set; }
        public int ColourType { get; set; }
    }
}
/*
public static IList<dynamic> GetExpandoFromXml(String file) { 
  var persons = new List<dynamic>();

  var doc = XDocument.Load(file);
  var nodes = from node in doc.Root.Descendants("Person")
              select node;
  foreach (var n in nodes) {
    dynamic person = new ExpandoObject();
    foreach (var child in n.Descendants()) {
      var p = person as IDictionary<String, object>);
      p[child.Name] = child.Value.Trim();
    }

    persons.Add(person);
  }

  return persons;
}
*/