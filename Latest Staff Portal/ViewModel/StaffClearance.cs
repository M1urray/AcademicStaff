using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Latest_Staff_Portal.ViewModel
{
    public class StaffClearance
    {
        public string StaffNo { get; set; }
        public string StaffName { get; set; }
        public string IDNo { get; set; }
        public string Department { get; set; }
        public string DepartmentName { get; set; }
        public string DateOfAppointment { get; set; }
        public string LastDateOfService { get; set; }
        public string ReasonForClearing { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int MemberBenefit { get; set; }
        public int EmployerBenefit { get; set; }
        public int EmployerBalance { get; set; }
    }

    public class StaffClearanceList
    {
        public string No { get; set; }
        public string StaffName { get; set; }
        public string AppliedDate { get; set; }
        public string EffectiveDate { get; set; }
        public string Status { get; set; }

    }
}