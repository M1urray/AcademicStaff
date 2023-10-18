using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Latest_Staff_Portal.ViewModel
{
    public class ADSetup
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Disabled { get; set; }        
    }
    public class UserDetails
    {
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string EmailAddress { get; set; }
        public string SamAccountName { get; set; }
        public string UserPrincipalName { get; set; }
        public string TelephoneNumber { get; set; }
        public string EmployeeId { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
    public class ChangePassword
    {
        public string DisplayName { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}