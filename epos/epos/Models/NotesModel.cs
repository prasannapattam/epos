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
        //defaults
        public Field hdnExamDefaultID { get; set; }
        public Field DefaultName { get; set; }
        public Field AgeStart { get; set; }
        public Field AgeEnd { get; set; }
        public Field PrematureBirth { get; set; }
        public Field DoctorName { get; set; }

        //Hidden fields
        public Field hdnPatientID { get; set; }
        public Field hdnExamID { get; set; }
        public Field hdnNoteType { get; set; }
        public Field hdnColourType { get; set; }

        //Patient
        public Field PatientNumber { get; set; }
        public Field Greeting { get; set; }
        public Field FirstName { get; set; }
        public Field MiddleName { get; set; }
        public Field LastName { get; set; }
        public Field NickName { get; set; }
        public Field DOB { get; set; }
        public Field Age { get; set; }
        public Field tbAge { get; set; }
        public Field PatientMonths { get; set; }
        public Field Sex { get; set; }
        public Field BoyGirlAdult { get; set; }
        public Field Premature { get; set; }
        public Field User { get; set; }
        public Field HxFrom { get; set; }
        public Field Refd { get; set; }
        public Field RefDoctor { get; set; }
        public Field Allergies { get; set; }
        public Field Occupation { get; set; }
        
        //Compliant tab
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
        public Field GA { get; set; }
        public Field BirthHist3 { get; set; }
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

        //Acuity
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
        public Field LastManRfxOD1 { get; set; }
        public Field LastManRfxOD2 { get; set; }
        public Field LastManVAOD1 { get; set; }
        public Field LastManVAOD2 { get; set; }
        public Field LastCycRfxOD { get; set; }
        public Field LastCycVAOD3 { get; set; }
        public Field LastCycVAOD4 { get; set; }
        public Field LastManRfxOS1 { get; set; }
        public Field LastManRfxOS2 { get; set; }
        public Field LastManVSOS1 { get; set; }
        public Field LastManVSOS2 { get; set; }
        public Field LastCycRfxOS { get; set; }
        public Field LastCycVSOS1 { get; set; }
        public Field LastCycVSOS2 { get; set; }
        public Field RxOD1 { get; set; }
        public Field RxOD2 { get; set; }
        public Field PopulateRxGiven { get; set; }
        public Field CTLRxOD1 { get; set; }
        public Field CTLRxOD2 { get; set; }
        public Field CTLRxOD3 { get; set; }
        public Field PopulateCtlRx { get; set; }
        public Field RXOS1 { get; set; }
        public Field RXOS2 { get; set; }
        public Field RXOS3 { get; set; }
        public Field RXOS4 { get; set; }
        public Field CTLRxOS1 { get; set; }
        public Field CTLRxOS2 { get; set; }
        public Field CTLRxOS3 { get; set; }
        public Field Confront1 { get; set; }
        public Field Confront2 { get; set; }
        public Field Confront3 { get; set; }
        public Field PupilOD1 { get; set; }
        public Field PupilOD2 { get; set; }
        public Field PupilOS1 { get; set; }
        public Field PupilOS2 { get; set; }
        public Field Pupil { get; set; }

        //Ocular Motility
        public Field OcularMotility6 { get; set; }
        public Field OcularMotility1 { get; set; }
        public Field OcularMotility5 { get; set; }
        public Field OcularMotility2 { get; set; }
        public Field OcularMotility4 { get; set; }
        public Field OMDefaults { get; set; }
        public Field OcMot1a { get; set; }
        public Field OcMot1b { get; set; }
        public Field OcMot2a { get; set; }
        public Field OcMot2b { get; set; }
        public Field OcMot3a { get; set; }
        public Field OcMot3b { get; set; }
        public Field OcMot4a { get; set; }
        public Field OcMot4b { get; set; }
        public Field OcMot5a { get; set; }
        public Field OcMot5b { get; set; }
        public Field OcMot6a { get; set; }
        public Field OcMot6b { get; set; }
        public Field OcMot7a { get; set; }
        public Field OcMot7b { get; set; }
        public Field OcMot8a { get; set; }
        public Field OcMot8b { get; set; }
        public Field OcMot9a { get; set; }
        public Field OcMot9b { get; set; }
        public Field HeadTiltRight1 { get; set; }
        public Field HeadTiltRight2 { get; set; }
        public Field HeadTiltLeft1 { get; set; }
        public Field HeadTiltLeft2 { get; set; }
        public Field HeadPosition1 { get; set; }
        public Field HeadPosition2 { get; set; }
        public Field DMR2 { get; set; }
        public Field Preference { get; set; }
        public Field OcularVersions1 { get; set; }
        public Field OcularVersions11 { get; set; }
        public Field OcularVersions2 { get; set; }
        public Field OcularVersions3 { get; set; }
        public Field OcularVersions31 { get; set; }
        public Field OcularVersions4 { get; set; }
        public Field OcularVersions5 { get; set; }
        public Field Nystagmus1 { get; set; }
        public Field Nystagmus2 { get; set; }
        public Field OcularVersionOD1 { get; set; }
        public Field OcularVersionOD2 { get; set; }
        public Field OcularVersionOD3 { get; set; }
        public Field OcularVersionOD4 { get; set; }
        public Field OcularVersionOD5 { get; set; }
        public Field OcularVersionOD6 { get; set; }
        public Field OcularVersionOS1 { get; set; }
        public Field OcularVersionOS2 { get; set; }
        public Field OcularVersionOS3 { get; set; }
        public Field OcularVersionOS4 { get; set; }
        public Field OcularVersionOS5 { get; set; }
        public Field OcularVersionOS6 { get; set; }
        public Field FusAmpBO { get; set; }
        public Field FusAmpBI { get; set; }
        public Field FusAmpBU { get; set; }
        public Field FusAmpBD { get; set; }
        public Field Convergence { get; set; }
        public Field Accommodation { get; set; }
        public Field LVRR1 { get; set; }
        public Field LVRR2 { get; set; }
        public Field LVRL1 { get; set; }
        public Field LVRL2 { get; set; }
        public Field Binocularity1 { get; set; }
        public Field Binocularity2 { get; set; }
        public Field Binocularity3 { get; set; }
        public Field Binocularity4 { get; set; }
        public Field W4DNear1 { get; set; }
        public Field W4DNear2 { get; set; }
        public Field W4DDistance1 { get; set; }
        public Field W4DDistance2 { get; set; }
        public Field Stereo1 { get; set; }
        public Field Stereo2 { get; set; }

        //Anteior Segnment
        public Field AnteriorSegment { get; set; }
        public Field SLE { get; set; }
        public Field PenLight { get; set; }
        public Field AllWNL { get; set; }
        public Field LidLashLacrimal1 { get; set; }
        public Field LidLashLacrimal2 { get; set; }
        public Field LidLashLacrimal3 { get; set; }
        public Field LidLashLacrimal4 { get; set; }
        public Field LidLashLacrimal5 { get; set; }
        public Field Exophthalmometry { get; set; }
        public Field ExophthalmometryOD { get; set; }
        public Field ExophthalmometryOS { get; set; }
        public Field ODPF { get; set; }
        public Field ODMRD1 { get; set; }
        public Field ODMRD2 { get; set; }
        public Field ODLevatorFxn1 { get; set; }
        public Field ODLevatorFxn2 { get; set; }
        public Field OSPF { get; set; }
        public Field OSMRD1 { get; set; }
        public Field OSMRD2 { get; set; }
        public Field OSLevatorFxn1 { get; set; }
        public Field OSLevatorFxn2 { get; set; }
        public Field ConjSclera1 { get; set; }
        public Field ConjSclera2 { get; set; }
        public Field ConjSclera3 { get; set; }
        public Field ConjSclera4 { get; set; }
        public Field ConjSclera5 { get; set; }
        public Field Cornea1 { get; set; }
        public Field Cornea2 { get; set; }
        public Field Cornea3 { get; set; }
        public Field Cornea4 { get; set; }
        public Field Cornea5 { get; set; }
        public Field AntChamber1 { get; set; }
        public Field AntChamber2 { get; set; }
        public Field AntChamber3 { get; set; }
        public Field AntChamber4 { get; set; }
        public Field AntChamber5 { get; set; }
        public Field Iris1 { get; set; }
        public Field Iris2 { get; set; }
        public Field Iris3 { get; set; }
        public Field Iris4 { get; set; }
        public Field Iris5 { get; set; }
        public Field Lens1 { get; set; }
        public Field Lens2 { get; set; }
        public Field Lens3 { get; set; }
        public Field Lens4 { get; set; }
        public Field Lens5 { get; set; }
        public Field Tono1 { get; set; }
        public Field Tono2 { get; set; }
        public Field Tono3 { get; set; }
        public Field Tono4 { get; set; }
        public Field Dilate1 { get; set; }
        public Field Dilate2 { get; set; }
        public Field Dilate3 { get; set; }
        public Field Fundus1 { get; set; }
        public Field Fundus2 { get; set; }
        public Field RetinaOU { get; set; }
        public Field RetOD { get; set; }
        public Field RetOS { get; set; }
        public Field MaculaOD { get; set; }
        public Field MaculaOS { get; set; }

        //Summary
        public Field Summary { get; set; }
        public Field Discussed { get; set; }
        public Field Advised { get; set; }
        public Field FollowUp1 { get; set; }
        public Field FollowUp2 { get; set; }
        public Field FollowUp3 { get; set; }
        public Field FollowUp4 { get; set; }
        public Field CopyTo { get; set; }
        public Field cbPrintQueue { get; set; }
        public Field ExamNoteTo { get; set; }
        public Field Notes { get; set; }
        public Field patientID { get; set; }
        public Field hdnPageTitle { get; set; }
        public Field hdnTimeOut { get; set; }

        public Field ExamDate { get; set; }
        public Field ExamCorrectDate { get; set; }
        public Field ExamSaveDate { get; set; }
        public PosConstants.NotesType NotesType { get; set; }
        public Field PatientName { get; set; }

        //dropdowns
        public List<SelectListItem> Doctors { get; set; }
    }

    public class Field
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int ColourType { get; set; }
        public string LookUpFieldName { get; set; }
        public int FieldType { get; set; }
    }
}

