using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class ItemCashList
    {
        public string No { get; set; }
        public string Payee { get; set; }
        public string Purpose { get; set; }
       
        public string Status { get; set; }
        public string ReqDate { get; set; }
    }
    public class NewItemCashRequisition
    {
        public string school { get; set; }
        public string Campus { get; set; }
        public string Department { get; set; }
        public string RespC { get; set; }
        public string PurchaseRequisition { get; set; }
        public List<SelectListItem> ListOfSchool { get; set; }
        public List<SelectListItem> ListOfCampus { get; set; }
        public List<SelectListItem> ListOfDepartment { get; set; }
        public List<SelectListItem> ListOfResponsibility { get; set; }
        public List<SelectListItem> ListofPurchaseRequisitions { get; set; }
    }
    public class ItemCashTypes
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
    public class UnitOfMeasure
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
    public class ItemCashTypesList
    {
        public string Code { get; set; }
        public List<SelectListItem> ListOfItemCashTypes { get; set; }
        public List<SelectListItem> ListOfLocations { get; set; }
        public List<SelectListItem> ListOfMeasures { get; set; }
    }
    public class ItemCashHeader
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
        public string PurchReq { get; set; }
        public string Status { get; set; }
        public string TotalAmount { get; set; }
        public string RequestorNo { get; set; }
        public string RequestorName { get; set; }
    }
    public class ItemCashLines
    {
        public string DocNo { get; set; }
        public string AdvanceType { get; set; }
        public string Item { get; set; }
        public string ItemDesc { get; set; }
        public string ItemDesc2 { get; set; }
        public string Amount { get; set; }
        public string LnNo { get; set; }
        public string Location { get; set; }
        public string Quantity { get; set; }
        public string UnitCost { get; set; }
        public string  UnitOfMeasure { get; set; }
    }
    public class ItemCashLinesList
    {
        public string Status { get; set; }
        public List<ItemCashLines> ListOfItemCashLines { get; set; }
    }
    public class ItemCashItemDetails

    {
        public string Code { get; set; }
        public string UnitOfMeasure { get; set; }
        public List<SelectListItem> ListOfItemCashTypes { get; set; }
        public ItemCashLines ItemDetails { get; set; }
        public List<SelectListItem> ListOfLocations { get; set; }
        public List<SelectListItem> ListOfMeasures { get; set; }
    }
    public class ItemCashDocument
    {
        public ItemCashHeader DocHeader { get; set; }
        public List<ItemCashLines> ListOfItemCashLines { get; set; }
    }
}