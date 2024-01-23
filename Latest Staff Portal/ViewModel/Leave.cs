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
    }
    public class NewLeaveApplication
    {
        public string Leave_Type { get; set; }
        public string Reliever { get; set; }
        public string LeaveBal { get; set; }
        public string AllocatedDays { get; set; }
        public string ReimbDays { get; set; }
        public string CarryForawrd { get; set; }
        public string EarnedLeaveDays { get; set; }
        public string LeaveTaken { get; set; }
        public string AppliedDays { get; set; }
        public List<SelectListItem> ListOfLeaveTypes { get; set; }
        public List<SelectListItem> ListOfRelievers { get; set; }
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
    public class LeavePlanner
    {
        public string DocNo { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public string JobTitle { get; set; }
        public string Dim1 { get; set; }
        public string Dim2 { get; set; }
        public string HRCalender { get; set; }
        public string DateApplied { get; set; }
        public string Status { get; set; }
        public string LineNo { get; set; }
        public List<SelectListItem> ListOfDirectorate { get; set; }
        public List<SelectListItem> ListOfDepartment { get; set; }
    }
    public class LeavePlannerLines
    {
        public string DocNo { get; set; }
        public string Line_No { get; set; }
        public string LeaveType { get; set; }
        public string DaysApplied { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ReturnDate { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
    }
    public class NewLeavePlanForm
    {
        public string Code { get; set; }
        public List<SelectListItem> ListOfLeaveTypes { get; set; }
    }
    public class LeavePlannerLinesList
    {
        public string DocNo { get; set; }
        public string Status { get; set; }
        public List<LeavePlannerLines> ListOfLeavePlannerLines { get; set; }
        public string TotalAmount { get; set; }
    }
    public class LeavePlannerDoc
    {
        public LeavePlanner DocHeader { get; set; }
        public List<LeavePlannerLines> ListOfLeavePlannerLines { get; set; }
    }
}