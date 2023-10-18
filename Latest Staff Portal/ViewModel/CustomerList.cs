using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class CustomerList
    {
        public string No { get; set; }
        public string Name { get; set; }
        public string Campus { get; set; }
        public string GradePrefix { get; set; }
        public string Attendance { get; set; }
    }
    public class StudentUnitList
    {
        public List<CustomerList> StudList { get; set; }
        public List<SelectListItem> ListOfExamRules { get; set; }
    }
    public class ClassAttendanceEntries
    {
        public string DocNo { get; set; }
        public string Week { get; set; }
        public string SectionWK { get; set; }
        public string Campus { get; set; }
        public string SettmtT { get; set; }
        public string PresentCount { get; set; }
        public string AbsentCount { get; set; }
        public bool Posted { get; set; }
    }
    public class ClassAttendance
    {
        public IEnumerable<CustomerList> StudentList { get; set; }
        public string Week { get; set; }
        public List<SelectListItem> ListOfWeeks { get; set; }
        public string DocNo { get; set; }
        public List<SelectListItem> ListOfSections { get; set; }
        public string SectionWK { get; set; }
    }
    public class SettlementList
    {
        public string Code { get; set; }
        public List<SelectListItem> ListOfSettlements { get; set; }
    }
}