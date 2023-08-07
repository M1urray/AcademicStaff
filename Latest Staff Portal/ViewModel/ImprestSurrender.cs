using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class ImprestSurrenderList
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
    public class NewImpSurrender
    {
        public List<SelectListItem> ListOfImprests { get; set; }
    }
    public class ImprestSurrenderHeader
    {
        public string No { get; set; }
        public string SurrenderDate { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public string ImprestNo { get; set; }
        public string ImpIssueDate { get; set; }
        public string CampusName { get; set; }
        public string DepartmentName { get; set; }
        public string RespC { get; set; }
        public string Status { get; set; }
        public string TotalAmount { get; set; }
        public string ImpPurpose { get; set; }
    }
    public class ImprestSurrenderLines
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
    public class ImprestSurrenderLinesList
    {
        public string Status { get; set; }
        public List<ImprestSurrenderLines> ListOfImprestSurrenderLines { get; set; }
    }
    public class ImpSurrenderLineDetails
    {
        public string Receipt { get; set; }
        public List<SelectListItem> ListOfPostedReceipts { get; set; }
        public string DocNo { get; set; }
        public string AccountNo { get; set; }
        public string Amount { get; set; }
        public string ActaulAmount { get; set; }
        public string LnNo { get; set; }
    }
    public class ImprestSurrenderDocument
    {
        public ImprestSurrenderHeader DocHeader { get; set; }
        public List<ImprestSurrenderLines> ListOfImprestSurrenderLines { get; set; }
    }
}