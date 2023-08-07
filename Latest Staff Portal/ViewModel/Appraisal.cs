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
        public string RespCenter { get; set; }
        public string Department { get; set; }
        public string UserID { get; set; }
        public string Status { get; set; }
    }
    public class NewApprisalRequest
    {
        public string AppraisalPeriod { get; set; }
        public string AppraisalTypes { get; set; }
        public string RespC { get; set; }
        public List<SelectListItem> ListOfApprisalPeriods { get; set; }
        public List<SelectListItem> ListOfApprisalTypes { get; set; }
        public List<SelectListItem> ListOfResponsibility { get; set; }
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
    }
    public class ApprisalObjective
    {
        public string DocNo { get; set; }
        public string Objective { get; set; }
        public string KeyPerformance { get; set; }
        public string Achievement { get; set; }
        public string AppraisalPeriod { get; set; }
        public string Target { get; set; }
        public string TimeLines { get; set; }
        public string Section { get; set; }
    }
    public class ScoreCardObjectives
    {
        public string DocumentNo { get; set; }
        public string Objective { get; set; }
        public string KeyPerformanceIndicator { get; set; }
        public string Targets { get; set; }
        public string Achievements { get; set; }
        public string Ratings { get; set; }
        public string SupervisorRating { get; set; }
    }
}