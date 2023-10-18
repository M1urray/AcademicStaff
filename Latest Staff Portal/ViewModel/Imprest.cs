using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class ImprestList
    {
        public string No { get; set; }
        public string ReqDate { get; set; }
        public string Purpose { get; set; }
        public string Function { get; set; }
        public string BudgetCeter { get; set; }
        public string Status { get; set; }
    }
    public class NewImprestRequisition
    {
        public string school { get; set; }
        public string Campus { get; set; }
        public string Department { get; set; }
        public string RespC { get; set; }
        public List<SelectListItem> ListOfSchool { get; set; }
        public List<SelectListItem> ListOfCampus { get; set; }
        public List<SelectListItem> ListOfDepartment { get; set; }
        public List<SelectListItem> ListOfResponsibility { get; set; }
    }
    public class ImprestTypes
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
    public class ImprestTypesList
    {
        public string Code { get; set; }
        public List<SelectListItem> ListOfImprestTypes { get; set; }
    }
    public class ImprestHeader
    {
        public string No { get; set; }
        public string DateNeeded { get; set; }
        public string Remarks { get; set; }
        public string school { get; set; }
        public string schoolName { get; set; }
        public string Campus { get; set; }
        public string CampusName { get; set; }
        public string Department { get; set; }
        public string DepartmentName { get; set; }
        public string RespC { get; set; }
        public string Status { get; set; }
        public string TotalAmount { get; set; }
        public string RequestorNo { get; set; }
        public string RequestorName { get; set; }
    }
    public class ImprestLines
    {
        public string DocNo { get; set; }
        public string AdvanceType { get; set; }
        public string Item { get; set; }
        public string ItemDesc { get; set; }
        public string ItemDesc2 { get; set; }
        public string Amount { get; set; }
        public string LnNo { get; set; }
    }
    public class ImprestLinesList
    {
        public string Status { get; set; }
        public List<ImprestLines> ListOfImprestLines { get; set; }
    }
    public class ImprestDocument
    {
        public ImprestHeader DocHeader { get; set; }
        public List<ImprestLines> ListOfImprestLines { get; set; }
    }
}