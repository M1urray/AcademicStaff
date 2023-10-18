using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class PurchaseReqList
    {
        public string No { get; set; }
        public string OrderDate { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
    public class NewPurchaseRequisition
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
    public class PRVHeader
    {
        public string No { get; set; }
        public string Remarks { get; set; }
        public string Campus { get; set; }
        public string CampusName { get; set; }
        public string Department { get; set; }
        public string DepartmentName { get; set; }
        public string RespC { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string RequestorNo { get; set; }
        public string RequestorName { get; set; }
        public bool CommentFound { get; set; }
        public string Comment { get; set; }
    }
    public class PRVLines
    {
        public string DocNo { get; set; }
        public string Item { get; set; }
        public string ItemDesc { get; set; }
        public string Description2 { get; set; }
        public string Qnty { get; set; }
        public string LineAmount { get; set; }
        public string Amount { get; set; }
        public string Location { get; set; }
        public string UnitM { get; set; }
        public string LineType { get; set; }
        public string LnNo { get; set; }
    }
    public class PurchaseLinesList
    {
        public string Status { get; set; }
        public List<PRVLines> ListOfPurchaseLines { get; set; }
        public string TotalAmount { get; set; }
    }
    public class PurchaseDocument
    {
        public PRVHeader DocHeader { get; set; }
        public List<PRVLines> ListOfPurchaseLines { get; set; }
        public string TotalAmount { get; set; }
        public string AmountInWords { get; set; }
    }
    public class PurchaseItemDetails
    {
        public List<SelectListItem> ListOfLocations { get; set; }
        public PRVLines ItemDetails { get; set; }
    }
}