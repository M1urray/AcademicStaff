using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class Cafeteria
    {
        public string No { get; set; }
        public string Employee { get; set; }
        public string EmployeeName { get; set; }
        public string Event_Name { get; set; }
        public string Venue { get; set; }
        public string Campus { get; set; }
        public string School { get; set; }
        public string Department { get; set; }
        public string RespC { get; set; }
        public string Caf { get; set; }
        public string CafName { get; set; }
        public string DateRaised { get; set; }
        public string DateNeeded { get; set; }
        public string TimeNeeded { get; set; }
        public string DateRecieved { get; set; }
        public string TotalCost { get; set; }
        public string Status { get; set; }
        public string EventType { get; set; }
        public List<SelectListItem> ListOfSchool { get; set; }
        public List<SelectListItem> ListOfCampus { get; set; }
        public List<SelectListItem> ListOfDepartment { get; set; }
        public List<SelectListItem> ListOfResponsibility { get; set; }
        public List<SelectListItem> ListOfEventTList { get; set; }
    }
    public class CafeteriaLines
    {
        public string No { get; set; }
        public string Item { get; set; }
        public string ItemName { get; set; }
        public string Quantity { get; set; }
        public string UnitCost { get; set; }
        public string Amount { get; set; }
        public string LnNo { get; set; }
    }
    public class NewCafRequisition
    {
        public string school { get; set; }
        public string Campus { get; set; }
        public string Department { get; set; }
        public string RespC { get; set; }
        public string EventType { get; set; }
        public List<SelectListItem> ListOfSchool { get; set; }
        public List<SelectListItem> ListOfCampus { get; set; }
        public List<SelectListItem> ListOfDepartment { get; set; }
        public List<SelectListItem> ListOfResponsibility { get; set; }
        public List<SelectListItem> ListOfEventTList { get; set; }
    }
    public class CafLinesList
    {
        public string Status { get; set; }
        public List<CafeteriaLines> ListOfCafLines { get; set; }
    }
    public class MenuList
    {
        public string Code { get; set; }
        public List<SelectListItem> ListOfMenuItems { get; set; }
    }
    public class CafItemDetails
    {
        public List<SelectListItem> ListOfMenuItems { get; set; }
        public CafeteriaLines ItemDetails { get; set; }
    }
    public class CafDocument
    {
        public Cafeteria DocHeader { get; set; }
        public List<CafeteriaLines> ListOfCafLines { get; set; }
    }
}