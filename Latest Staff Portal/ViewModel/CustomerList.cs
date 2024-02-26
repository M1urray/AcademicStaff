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
        public string Attendance { get; set; }
        public string ApplicationNo { get; set; }
        public string Marks { get; set; }
        public string Unit { get; set; }
        public string Semester { get; set; }
        public string Grade { get; set; }
    }
    public class StudentRedUnit
    {
        public List<CustomerList> Cust { get; set; }
        public string Prog { get; set; }
        public string Unit { get; set; }
        public string Sem { get; set; }
    }
    public class ClassAttendanceEntries
    {
        public string DocNo { get; set; }
        public string Week { get; set; }
        public string Campus { get; set; }
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
        
    }
}