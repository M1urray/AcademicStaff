using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class StaffClearance
    {
        public string StaffNo { get; set; }
        public string StaffName { get; set; }
        public string IDNo { get; set; }
        public string Campus { get; set; }
        public string School { get; set; }
        public string Department { get; set; }
        public string RespC { get; set; }
        public string DateOfAppointment { get; set; }
        public string LastDateOfService { get; set; }
        public string ReasonForClearing { get; set; }
    }
    public class NewClearanceRequisition
    {
        public StaffClearance StaffClearanceDoc { get; set; }
        public List<SelectListItem> ListOfSchool { get; set; }
        public List<SelectListItem> ListOfCampus { get; set; }
        public List<SelectListItem> ListOfDepartment { get; set; }
        public List<SelectListItem> ListOfResponsibility { get; set; }
    }
}