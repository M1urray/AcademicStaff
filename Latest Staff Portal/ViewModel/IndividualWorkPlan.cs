using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Latest_Staff_Portal.ViewModel
{
    public class IndividualWorkPlan
    {
        public string DocNo { get; set; }
        public string StaffNo { get; set; }
        public string StaffName { get; set; }
        public string ApprisalPeriod { get; set; }
        public string Dim1 { get; set; }
        public string Dim2 { get; set; }
        public string Status { get; set; }
        public string Open_To { get; set; }
    }
    public class IndividualObjectives
    {
        public string Obj { get; set; }
        public string Remarks { get; set; }
        public string Code { get; set; }
        public string EntryNo { get; set; }
    }
    public class ObjTarget
    {
        public string Target { get; set; }
        public string ObjEntryNo { get; set; }
        public string EntryNo { get; set; }
        public List<TargetActivities> ListOfActivities { get; set; }
    }
    public class DocTarget
    {
        public string DocNo { get; set; }
        public string ObjEntryNo { get; set; }
        public string Objective { get; set; }
        public string Remarks { get; set; }
    }
    public class TargetActivities
    {
        public string Code { get; set; }
        public string Activity { get; set; }
        public string Resources_Required { get; set; }
        public string Expected_Results { get; set; }
        public string Time_Frame { get; set; }
        public string Performance_Indicator { get; set; }
        public string Objective_Entry_No { get; set; }
        public string Target_Entry_No { get; set; }
        public string Entry_No { get; set; }
    }
}