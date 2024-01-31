using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class PartTimeLecturerRegList
    {
        public IEnumerable<PartTimeLecturerDetails> ListOfPartTimeRequisition { get; set; }
    }
    public class PartTimeLecturerDetails
    {
        public string IDNo { get; set; }
        public string Date { get; set; }
        public string Firstname { get; set; }
        public string MiddelName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string School { get; set; }
        public string Campus { get; set; }
        public string Department { get; set; }
        public string Gender { get; set; }
        public string Remarks { get; set; }
    }
    public class UnitDetails
    {
        public string Prog { get; set; }
        public string Stage { get; set; }
        public string Unit { get; set; }
        public string UnitName { get; set; }
    }
    public class ParttimeView
    {
        public string ID_Number { get; set; }
        public string firstName { get; set; }
        public string Middle_Name { get; set; }
        public string Last_Name { get; set; }
        public string Department_Code { get; set; }
        public string School_Code { get; set; }
        public string Campus_Code { get; set; }
        public string Title { get; set; }
        public string Remarks { get; set; }
        public string Gender { get; set; }
        public List<SelectListItem> ListOfHREmpInitials { get; set; }
        public List<SelectListItem> ListOfCampus { get; set; }
        public List<SelectListItem> ListOfDepartment { get; set; }
        public List<SelectListItem> ListOfSchool { get; set; }
    }
}