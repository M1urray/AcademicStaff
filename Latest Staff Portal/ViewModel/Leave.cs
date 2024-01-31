using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class LeaveReqList
    {
        public string No { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public string Leave_Type { get; set; }
        public string Applied_Days { get; set; }
        public string Date { get; set; }
        public string Starting_Date { get; set; }
        public string End_Date { get; set; }
        public string Return_Date { get; set; }
        public string Reliever { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string Department { get; set; }
        public string Responsibility { get; set; }
        public string Address { get; set; }
        public string phoneNo { get; set; }
        public List<string> Comments { get; set; }
    }
    public class LeaveDocumentDetails
    {
        public LeaveReqList DocumentDetails { get; set; }
        public string Reliever { get; set; }
        public string RespC { get; set; }
        public List<SelectListItem> ListOfLeaveTypes { get; set; }
        public List<SelectListItem> ListOfRelievers { get; set; }
        public List<SelectListItem> ListOfResponsibility { get; set; }
        public string Balance { get; set; }
        public List<SelectListItem> ListOfDays { get; set; }

    }
    public class NewLeaveApplication
    {
        public string Leave_Type { get; set; }
        public string Reliever { get; set; }
        public string LeaveBal { get; set; }
        public string AllocatedDays { get; set; }
        public string ReimbDays { get; set; }
        public string EarnedLeaveDays { get; set; }
        public string LeaveTaken { get; set; }
        public string AppliedDays { get; set; }
        public string RespC { get; set; }
        public string Address { get; set; }
        public string phoneNo { get; set; }
        public List<SelectListItem> ListOfLeaveTypes { get; set; }
        public List<SelectListItem> ListOfRelievers { get; set; }
        public List<SelectListItem> ListOfResponsibility { get; set; }
    }
    public class LvTypes
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
    public class RelieverList
    {
        public string No { get; set; }
        public string Name { get; set; }
    }
    public class LeaveBalance
    {
        public string AllocatedDays { get; set; }
        public string ReimbDays { get; set; }
        public string CarryForawrd { get; set; }
        public string EarnedLeaveDays { get; set; }
        public string LeaveTaken { get; set; }
        public string Balance { get; set; }
        public List<SelectListItem> ListOfDays { get; set; }
    }
    public class DropDownBalance
    {
        public string Code { get; set; }
    }
    public class EndReturnDates
    {
        public string EndDate { get; set; }
        public string ReturnDate { get; set; }
    }
}