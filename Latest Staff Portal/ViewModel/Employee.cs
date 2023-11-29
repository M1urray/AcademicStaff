using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class EmployeeRequisitionList
    {
        public string RequisitionNo { get; set; }
        public string RequisitionDate { get; set; }
        public string JobDescription { get; set; }
        public string Requestor { get; set; }
        public string ReasonForRequest { get; set; }
        public int RequiredPositions { get; set; }
        public string Contract { get; set; }
        public string RequisitionType { get; set; }
        public string OpeningDate { get; set; }
        public string ClosingDate { get; set; }
        public bool Closed { get; set; }
        public bool Advertised { get; set; }
        public string Status { get; set; }
        public string JobID { get; set; }
    }
    public class EmployeeRequisitionDocumentView
    {
        public string RequisitionNo { get; set; }
        public string RequisitionDate { get; set; }
        public string Requestor { get; set; }
        public string JobId { get; set; }
        public string JobDescription { get; set; }
        public string JobRefNo { get; set; }
        public string JobGrade { get; set; }
        public string ReasonForRequest { get; set; }
        public string TypeOfContractRequired { get; set; }
        public string Priority { get; set; }
        public int VacantPositions { get; set; }
        public int RequiredPositions { get; set; }
        public string OpeningDate { get; set; }
        public string ClosingDate { get; set; }
        public string RequisitionType { get; set; }
        public bool Advertised { get; set; }
        public bool Closed { get; set; }
        public string Status { get; set; }
        public string AnyAdditionalInformation { get; set; }
        public string ReasonForRequestOther { get; set; }
    }

    public class NewEmployeeRequisition
    {
        public string JobId { get; set; }
        public string Contract { get; set; }

        public List<SelectListItem> ListOfJobs { get; set; }
        public int  ReasonForRequest{ get; set; }
        public List<SelectListItem> ListOfContracts { get; set; }
        public string Priority { get; set; }
        public decimal RequiredPositions  { get; set; }
        public decimal VacantPositions { get; set; }
    }

}