using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    // public class TrainingReqList
    // {
    //     public string No {get; set; }
    //     public string  Category { get; set; }
    //     public string CourseTitle { get; set; }
    //     public string FromDate { get; set; }
    //     public string ToDate { get; set; }
    //     public  string Institution { get; set; }
    //     public string Status { get; set; }
    //
    // }
    // public class NewTrainingRequisition
    // {
    //     public string No { get; set; }
    //     public string Campus { get; set; }
    //     public string Department { get; set; }
    //     public string Course { get; set; }
    //     public string  Trainers { get; set; }
    //     // public string Station { get; set; }
    //     public List<SelectListItem> ListOfCampus { get; set; }
    //     public List<SelectListItem> ListOfDepartments{ get; set; }
    //     // public List<SelectListItem> ListOfStations { get; set; }
    //     public List<SelectListItem> ListOfCourses { get; set; }
    //
    //     public List<SelectListItem> ListOfTrainers { get; set; }
    //
    // }
    //
    // public class TrainingRequistion
    // {
    //     public string No { get; set; }
    //     public string Campus { get; set; }
    //     public string  Department { get; set; }
    //     public string  Station { get; set; }
    //     public string Course { get; set; }
    //     public string CourseDescription { get; set; }
    //     public int  Category { get; set; }
    //     public int Sponsor { get; set; }
    //     public string SponsorName{get; set;}
    //     public string StartDate { get; set; }
    //     public string EndDate { get; set; }
    //     public string Trainer { get; set; }
    //     public string TrainerName { get; set; }
    //     public decimal  Cost { get; set; }
    //     public string Purpose { get; set; }
    //     public string Venue { get; set; }
    //
    // }
    //
    // public class TrainingCourses
    // {
    //     public string Code { get; set; }
    //     public string Title { get; set; }
    // }
    //
    // public class TrainingProviders
    // {
    //     public string  No { get; set; }
    //     public string ProviderName { get; set; }
    // }
    public class TrainingList
    {
        public string ApplicationNo { get; set; }
        public string TrainingCategory { get; set; }
        public string CourseTitle { get; set; }
        public string Status { get; set; }
        public string ApplicationDate {get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string CourseDesc { get; set; }
        public string Dim1 { get; set; }
        public string Dim2 { get; set; }
        public string Cost { get; set; }
        public string Sponsor { get; set; }
        public string Trainer { get; set; }
        public string Purpose { get; set; }
        public List<SelectListItem> ListOfDim1 { get; set; }

        public List<SelectListItem> ListOfDim2 { get; set; }
        public List<SelectListItem> ListOfCourses {get; set;}

        public List<SelectListItem> ListOfTrainers {get; set;}
    }

    public class NewTrainingDocument
    {
        public string Dim1 { get; set; }
        public string Dim2 { get; set; }
        public string Course { get; set; }

        public string Trainer { get; set; }

        public List<SelectListItem> ListOfDim1 { get; set; }

        public List<SelectListItem> ListOfDim2 { get; set; }
        public List<SelectListItem> ListOfCourses {get; set;}

        public List<SelectListItem> ListOfTrainers {get; set;}

    }

    public class TraineeList
    {
        public string Status { get; set; }
        public List<Trainees> ListOfTrainees {get; set;}
    }

    public class Trainees
    {
        public string No { get; set; }
        public string Name { get; set; }
        public List<SelectListItem> ListOfEmployee { get; set; }
    }
}