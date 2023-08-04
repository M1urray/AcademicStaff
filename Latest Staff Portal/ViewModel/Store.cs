using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class StoreReqList
    {
        public string No { get; set; }
        public string ReqDate { get; set; }
        public string DateRequired { get; set; }
        public string Description { get; set; }
        public string Function { get; set; }
        public string BudgetCeter { get; set; }
        public string Status { get; set; }
    }
    public class NewStoreRequisition
    {
        public string Department { get; set; }
        public string Directorate { get; set; }
        public string RespC { get; set; }
        public bool DisDir { get; set; }
        public List<SelectListItem> ListOfDepartment { get; set; }
        public List<SelectListItem> ListOfDirectorate { get; set; }
        public List<SelectListItem> ListOfSection { get; set; }
        public List<SelectListItem> ListOfResponsibility { get; set; }
    }

    public class StoretHeader
    {
        public string No { get; set; }
        public string DateRequested { get; set; }
        public string DateNeeded { get; set; }
        public string Remarks { get; set; }        
        public string Department { get; set; }
        public string Directorate { get; set; }
        public string RespC { get; set; }
        public string IssuingStore { get; set; }
        public string Status { get; set; }
        public string RequestorNo { get; set; }
        public string RequestorName { get; set; }
        public bool DisDir { get; set; }
        public List<SelectListItem> ListOfDepartment { get; set; }
        public List<SelectListItem> ListOfDirectorate { get; set; }
        public List<SelectListItem> ListOfSection { get; set; }
        public List<SelectListItem> ListOfResponsibility { get; set; }
    }
    public class StoreLines
    {
        public string DocNo { get; set; }
        public string Item { get; set; }
        public string ItemDesc { get; set; }
        public string Description2 { get; set; }
        public string Qnty { get; set; }
        public string Location { get; set; }
        public string LnNo { get; set; }
    }
    public class StoreLinesList
    {
        public string Status { get; set; }
        public List<StoreLines> ListOfStoreLines { get; set; }
    }
    public class StoreItemDetails
    {
        public List<SelectListItem> ListOfLocations { get; set; }
        public StoreLines ItemDetails { get; set; }
    }
    public class StoreDocument
    {
        public StoretHeader DocHeader { get; set; }
        public List<StoreLines> ListOfStoreLines { get; set; }
    }
}