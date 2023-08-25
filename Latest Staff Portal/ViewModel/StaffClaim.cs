using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class StaffClaimList
    {
        public string No { get; set; }
        public string ReqDate { get; set; }
        public string Purpose { get; set; }
        public string Function { get; set; }
        public string BudgetCeter { get; set; }
        public string Status { get; set; }
        public string RespC { get; set; }
    }
    public class NewStaffClaimRequisition
    {
        public string school { get; set; }
        public string Campus { get; set; }
        public string Department { get; set; }
        public string RespC { get; set; }
        public List<SelectListItem> ListOfSchool { get; set; }
        public List<SelectListItem> ListOfCampus { get; set; }
        public List<SelectListItem> ListOfDepartment { get; set; }
        public List<SelectListItem> ListOfResponsibility { get; set; }
    }
    public class StaffClaimTypes
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
    public class StaffClaimTypesList
    {
        public string Code { get; set; }
        public List<SelectListItem> ListOfStaffClaimTypes { get; set; }
    }
    public class StaffClaimHeader
    {
        public string No { get; set; }
        public string Remarks { get; set; }
        public string school { get; set; }
        public string schoolName { get; set; }
        public string Campus { get; set; }
        public string CampusName { get; set; }
        public string Department { get; set; }
        public string DepartmentName { get; set; }
        public string RespC { get; set; }
        public string Status { get; set; }
        public string DateRequested { get; set; }
        public string TotalAmount { get; set; }
        public string RequestorNo { get; set; }
        public string RequestorName { get; set; }
    }
    public class StaffClaimLines
    {
        public string DocNo { get; set; }
        public string AdvanceType { get; set; }
        public string Item { get; set; }
        public string ItemDesc { get; set; }
        public string ItemDesc2 { get; set; }
        public string Amount { get; set; }
        public string LnNo { get; set; }
    }
    public class StaffClaimLinesList
    {
        public string Status { get; set; }
        public List<StaffClaimLines> ListOfStaffClaimLines { get; set; }
    }
    public class StaffClaimDocument
    {
        public StaffClaimHeader DocHeader { get; set; }
        public List<StaffClaimLines> ListOfStaffClaimLines { get; set; }
    }
}