using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class TrainingList
    {    
        public string Application_No { get; set; }
        public string Application_Date { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string User_ID { get; set; }
        public string Supervisor { get; set; }
        public string Training_Category { get; set; }
        public string Course_Title { get; set; }
        public string Course_Desc { get; set; }
        public string Campus { get; set; }
        public string Department { get; set; }
        public string RespC { get; set; }
        public string Trainer { get; set; }
        public string TrainerName { get; set; }
        public string Sponsor { get; set; }
        public string Cost { get; set; }
        public string Purpose { get; set; }
        public string Status { get; set; }
        public List<SelectListItem> ListOfCampus { get; set; }
        public List<SelectListItem> ListOfDepartment { get; set; }
        public List<SelectListItem> ListOfResponsibility { get; set; }
        public List<SelectListItem> ListOfCourses { get; set; }
        public List<SelectListItem> ListOfTrainers { get; set; }
    }
    public class NewTrainingDocument
    {
        public string Campus { get; set; }
        public string Department { get; set; }        
        public string RespC { get; set; }
        public string Course { get; set; }
        public string Trainer { get; set; }
        public List<SelectListItem> ListOfDepartment { get; set; }
        public List<SelectListItem> ListOfCampus { get; set; }
        public List<SelectListItem> ListOfResponsibility { get; set; }
        public List<SelectListItem> ListOfCourses { get; set; }
        public List<SelectListItem> ListOfTrainers { get; set; }
    }
    public class Trainees
    {
        public string No { get; set; }
        public string Name { get; set; }
        public List<SelectListItem> ListOfEmployee { get; set; }
    }
    public class TraineeList
    {
        public string Status { get; set; }
        public List<Trainees> ListOfTrainees { get; set; }
    }
    public class TrainingDocument
    {
        public TrainingList DocHeader { get; set; }
        public List<Trainees> ListOfTrainees { get; set; }
        public List<TrainingCost> ListOfTraininingCost { get; set; }        
    }
    public class TrainingCost
    {
        public string No { get; set; }
        public string Item { get; set; }
        public string Cost { get; set; }
    }
    
}