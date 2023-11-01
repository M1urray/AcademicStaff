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
        public string Semester { get; set; }
        public string LnNo { get; set; }
    }
    public class StudentReqDoc
    {
        public StudentRequisition Doc { get; set; }
    }
    public class StudentReqLinesDoc
    {
        public string RegT { get; set; }
        public List<StudentReqLines> DocLines { get; set; }
    }
}