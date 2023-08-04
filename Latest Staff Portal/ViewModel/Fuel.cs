using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Latest_Staff_Portal.ViewModel
{
    public class Fuel
    {
        public string No { get; set; }
        public string Date_Requested { get; set; }
        public string Driver { get; set; }
        public string Driver_Name { get; set; }
        public string Vehicle { get; set; }
        public string Max_Amount { get; set; }
        public string Amount_Consumed { get; set; }
        public string Amount_To_Topup { get; set; }
        public string Status { get; set; }
        
    }
}