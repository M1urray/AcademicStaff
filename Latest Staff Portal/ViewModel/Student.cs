using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Latest_Staff_Portal.ViewModel
{
    public class StudentRequisition
    {
        public string Code { get; set; }
        public string StudentNo { get; set; }
        public string StudentName { get; set; }
        public string RegType { get; set; }
        public string DateApplied { get; set; }
        public string Status { get; set; }
        public string Programme { get; set; }
        public string ProgrammeTo { get; set; }
        public string ProgConcentration { get; set; }
        public string Semester { get; set; }
        public string StudentStatus { get; set; }
        public bool CommentFound { get; set; }
        public string Comment { get; set; }
        public int ReqCount { get; set; }
    }
    public class StudentReqLines
    {
        public string Unit { get; set; }
        public string UnitName { get; set; }
        public string Equivalent { get; set; }
        public string Lecture { get; set; }
        public string Section { get; set; }
        public bool Approved { get; set; }
        public bool Charge { get; set; }
        public string Semester { get; set; }
        public string Semester_Unit_Done { get; set; }
        public string LnNo { get; set; }
    }
    public class StudentReqDoc
    {
        public StudentRequisition Doc { get; set; }
        public string Sequence { get; set; }
    }
    public class StudentReqLinesDoc
    {
        public string RegT { get; set; }
        public List<StudentReqLines> DocLines { get; set; }
    }
    public class Online_App
    {
        public string Pic { get; set; }
        public Online_App_Header DocD { get; set; }
        public List<Appl_Qual> Qual_List { get; set; }
        public List<Appl_Subjects> Subj_List { get; set; }
    }
    public class Online_App_Header
    {       
        public string Code { get; set; }
        public string Names { get; set; }
        public string Application_Date { get; set; }
        public string Campus { get; set; }
        public string Intake_Code { get; set; }
        public string Academic_Year { get; set; }
        public string ModeOfStudy { get; set; }
        public string Index_No { get; set; }
        public string DoB { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string Programme { get; set; }
        public string Programme_Level { get; set; }
        public string Stage { get; set; }
        public string Phone_No { get; set; }
        public string Email { get; set; }       
    }
    public class Appl_Qual
    {
        public string Type { get; set; }
        public string Where_Obtained { get; set; }
        public string Award { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Graduation_Year { get; set; }
    }
    public class Appl_Subjects
    {
        public string Subject { get; set; }
        public string Grade { get; set; }
    }
}