using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class ExitQuestions
    {
        public string DocNo { get; set; }
        public string QuizNo { get; set; }
        public string Questionare { get; set; }
        public string Answer { get; set; }
        public string Category { get; set; }

        public string LnNo { get; set; }
    }
    public class QuestionAnswer
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public int QuizNo { get; set; }
    }
    public class NewExitHeader
    {
        public string No { get; set; }
        public string EmployeeNumber { get; set; }
        public string LeavingDate { get; set; }
        public string LengthOfService { get; set; }
        public string LengthInDepartment { get; set; }
        public string EmployeeCategory { get; set; }
        public string Designation { get; set; }
        public string ClearanceDate { get; set; }
        public string EmployeeName { get; set; }
        public List<ReasonForLeaving> Reasons { get; set; }
        public List<ExitQuestions> ExitQuestions { get; set; }
    }
    public class ReasonForLeaving
    {
        
        public string DocNo { get; set; }
        public string Reason { get; set; }
        public string LnNo { get; set; }
    }

}