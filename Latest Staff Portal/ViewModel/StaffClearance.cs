using System;
using System.Collections.Generic;
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
    }
}