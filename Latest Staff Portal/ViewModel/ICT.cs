using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class ICTRequest
    {
        public string No { get; set; }
        public string Date { get; set; }
        public string ReqCat { get; set; }
        public string Campus { get; set; }
        public string CampusName { get; set; }
        public string Department { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }
        public string Urgency { get; set; }
        public string RequiredDate { get; set; }
        public string Status { get; set; }
        public string Resoltion_Remarks { get; set; }
    }
    public class ICTRequestLines
    {
        public string Description { get; set; }
        public string Quantity { get; set; }
    }
        public class NewICTRequisition
    {
        public string Campus { get; set; }
        public string Department { get; set; }
        public string Category { get; set; }
        public string Date { get; set; }
        public List<SelectListItem> ListOfCampus { get; set; }
        public List<SelectListItem> ListOfDepartment { get; set; }
        public List<SelectListItem> ListOfCategory { get; set; }
    }
}