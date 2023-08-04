using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class AppraisalCardList
    {
        public string ApprisalCode { get; set; }
        public string StaffNo { get; set; }
        public string StaffName { get; set; }
        public string ApprisalPeriod { get; set; }
        public string AppraisalType { get; set; }
        public string AppraisalDate { get; set; }
        public string AppraisalStartDate { get; set; }
        public string AppraisalEndDate { get; set; }
        public string AppointmentDate { get; set; }
        public string Designation { get; set; }
        public string AppraisalStage { get; set; }
        public string RespCenter { get; set; }
        public string Department { get; set; }
        public string UserID { get; set; }
        public string Status { get; set; }
        public string OpenTo { get; set; }
    }
    public class NewApprisalRequest
    {
        public string AppraisalPeriod { get; set; }
        public string AppraisalTypes { get; set; }
        public string RespC { get; set; }
        public string SupNo { get; set; }
        public List<SelectListItem> ListOfApprisalPeriods { get; set; }
        public List<SelectListItem> ListOfApprisalTypes { get; set; }
        public List<SelectListItem> ListOfResponsibility { get; set; }
        public List<SelectListItem> ListOfEmployees { get; set; }
    }
    public class AppraisalTypes
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
    public class AppraisalPeriods
    {
        public string Period { get; set; }
    }
    public class NewAppraisalDocument
    {
        public string ApprisalPeriod { get; set; }
        public string ApprisalType { get; set; }
        public string Responsibility { get; set; }
        public string SupNo { get; set; }
    }
    public class DepartmentalObjectives
    {
        public string Appraisal_No { get; set; }
        public string Objective_Code { get; set; }
        public string Perspective_Code { get; set; }
        public string Perspective_Description { get; set; }
        public string Objective_Description { get; set; }
        public string Ratings { get; set; }
        public string SupervisorRating { get; set; }
    }
    public class EvaluationLines
    {
        public string Appraisal_No { get; set; }
        public string Entry_No { get; set; }
        public string Objective { get; set; }
        public string Target { get; set; }
        public string Activity { get; set; }
        public string Resources_Required { get; set; }
        public string Expected_Results { get; set; }
        public string Time_Frame { get; set; }
        public string Performance_Indicator { get; set; }
        public string Appraisee_Score { get; set; }
        public string Supervisor_Score { get; set; }
        public string Agreed_Score { get; set; }
    }
    public class PerformanceIndicator
    {
        public string Appraisal_No { get; set; }
        public string Line_No { get; set; }
        public string Agreed_Performance_Targets { get; set; }
        public string Key_Performance_Indicator { get; set; }
        public string Key_Result_Areas_Output { get; set; }
        public string Self_Assesment { get; set; }
        public string Self_Score { get; set; }
        public string Supervisor_Assesment { get; set; }
        public string Supervisors_Score { get; set; }
        public string Agreed_Assesment_Results { get; set; }
        public string Agreed_Score { get; set; }
        public string Appraisee_Comments { get; set; }
        public string Supervisor_Comments { get; set; }
        public string Level { get; set; }
    }
    public class TrainingDev
    {
        public string Appraisal_No { get; set; }
        public string Line_No { get; set; }
        public string Name_of_the_Course { get; set; }
        public string Duration_of_Course { get; set; }
        public string Expected_Start_Date { get; set; }
        public string Expected_End_Date { get; set; }
        public string Reaction { get; set; }
        public string Learning_Obtained { get; set; }
        public string Behavior_Changes_Adopted { get; set; }
        public string Results_Obtained { get; set; }
        public string Remarks_Appraisee { get; set; }
        public string Remarks_Supervisor { get; set; }
        public string Level { get; set; }
    }
    public class CompetenceValues
    {
        public string Appraisal_No { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Appraisal_Assesment { get; set; }
        public string Score { get; set; }
        public string Score_Descriptors { get; set; }
        public string Line_No { get; set; }
        public string Level { get; set; }
    }
    public class SectionDetails
    {
        public List<PerformanceIndicator> KPIList { get; set; }
        public string Level { get; set; }
    }
}