using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class RespCenter
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class AcademicYearList
    {
        public string Code { get; set; }
    }
    public class ApprovalEntries
    {
        public string DocNo { get; set; }
        public string UserID { get; set; }
        public string DateSendForApproval { get; set; }
        public string DueDate { get; set; }
        public string Status { get; set; }
        public int Sequence { get; set; }
    }
    public class DimensionValues
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class SemesterList
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class Locations
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class LocationList
    {
        public string Code { get; set; }
        public List<SelectListItem> ListOfLocations { get; set; }
    }
    public class CommonDropDownList
    {
        public List<SelectListItem> ListOfSchools { get; set; }
        public List<SelectListItem> ListOfDepartments { get; set; }
        public List<SelectListItem> ListOfCampus { get; set; }
        public List<SelectListItem> ListOfRespC { get; set; }
    }
    public class DropdownList
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
    public class NoticeBoard
    {
        public string Description { get; set; }
        public string Campus { get; set; }
        public string DatePosted { get; set; }
    }
}