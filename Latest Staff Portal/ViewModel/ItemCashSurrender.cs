using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class ItemCashSurrenderList
    {
        public string No { get; set; }
        public string ReqDate { get; set; }
        public string Purpose { get; set; }
        public string Function { get; set; }
        public string Amount { get; set; }
        public string BudgetCeter { get; set; }
        public string ImpDocNo { get; set; }
        public string Status { get; set; }
    }
    public class NewItmSurrender
    {
        public List<SelectListItem> ListOfItemCashs { get; set; }
    }
    public class ItemCashSurrenderHeader
    {
        public string No { get; set; }
        public string SurrenderDate { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public string ItemCashNo { get; set; }
        public string ImpIssueDate { get; set; }
        public string CampusName { get; set; }
        public string DepartmentName { get; set; }
        public string RespC { get; set; }
        public string Status { get; set; }
        public string TotalAmount { get; set; }
        public string ImpPurpose { get; set; }
    }
    public class ItemCashSurrenderLines
    {
        public string SurrenderDocNo { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public string Amount { get; set; }
        public string ActaulSpend { get; set; }
        public string ReceiptNo { get; set; }
        public string ReceiptAmount { get; set; }
        public string LnNo { get; set; }
    }
    public class ItemCashSurrenderLinesList
    {
        public string Status { get; set; }
        public List<ItemCashSurrenderLines> ListOfItemCashSurrenderLines { get; set; }
    }
    public class ItmSurrenderLineDetails
    {
        public string Receipt { get; set; }
        public List<SelectListItem> ListOfPostedReceipts { get; set; }
        public string DocNo { get; set; }
        public string AccountNo { get; set; }
        public string Amount { get; set; }
        public string ActaulAmount { get; set; }
        public string LnNo { get; set; }
    }
    public class ItemCashSurrenderDocument
    {
        public ItemCashSurrenderHeader DocHeader { get; set; }
        public List<ItemCashSurrenderLines> ListOfItemCashSurrenderLines { get; set; }
    }
}