using System.Collections.Generic;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class BackToOfficeList
    {    
        public string Application_No { get; set; }
        public string Application_Date { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string User_ID { get; set; }
        public string Supervisor { get; set; }
        public string BackToOffice_Category { get; set; }
        public string Course_Title { get; set; }
        public string Course_Desc { get; set; }
        public string Directorate { get; set; }
        public string Department { get; set; }
        public string RespC { get; set; }
        public string Trainer { get; set; }
        public string Sponsor { get; set; }
        public string Cost { get; set; }
        public string Purpose { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public List<SelectListItem> ListOfDirectorate { get; set; }
        public List<SelectListItem> ListOfDepartment { get; set; }
        public List<SelectListItem> ListOfResponsibility { get; set; }
        public List<SelectListItem> ListOfCourses { get; set; }
        public List<SelectListItem> ListOfTrainers { get; set; }
    }
    public class NewBackToOfficeDocument
    {
        public string Department { get; set; }
        public string Directorate { get; set; }
        public string RespC { get; set; }
        public string Course { get; set; }
        public string Trainer { get; set; }
        public List<SelectListItem> ListOfDepartment { get; set; }
        public List<SelectListItem> ListOfDirectorate { get; set; }
        public List<SelectListItem> ListOfResponsibility { get; set; }
        public List<SelectListItem> ListOfCourses { get; set; }
        public List<SelectListItem> ListOfTrainers { get; set; }
    }
    
    public class BackToOfficeDocument
    {
        public BackToOfficeList DocHeader { get; set; }
        public List<Trainees> ListOfTrainees { get; set; }
        public List<BackToOfficeCost> ListOfTraininingCost { get; set; }        
    }
    public class BackToOfficeCost
    {
        public string No { get; set; }
        public string Item { get; set; }
        public string Cost { get; set; }
    }
    
}