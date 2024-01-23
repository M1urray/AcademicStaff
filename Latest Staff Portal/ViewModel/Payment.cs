using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Latest_Staff_Portal.ViewModel
{
    public class PaymentHeader
    {
        public string No { get; set; }
        public string Date { get; set; }
        public string Remarks { get; set; }
        public string Directorate { get; set; }
        public string Department { get; set; }
        public string RespC { get; set; }
        public string Pay_Mode { get; set; }
        public string ChequeNo { get; set; }
        public string PayingBank { get; set; }
        public string Paying_Bank_Account { get; set; }        
        public string PaymentTo { get; set; }
        public string OnBehalfOf { get; set; }        
        public string RaisedBy { get; set; }
        public string TotalAmount { get; set; }
        public string TotalVATAmount { get; set; }
        public string TotalWithHTAXAmount { get; set; }
        public string TotalRETAmount { get; set; }
        public string TotalVATWITHHAmount { get; set; }
        public string TotalPAYEAmount { get; set; }
        public string TotalNETAmount { get; set; }
        public string Status { get; set; }
    }
    public class PaymentLines
    {
        public string DocNo { get; set; }
        public string Type { get; set; }
        public string AccountType { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public string Amount { get; set; }
    }
    public class PaymentDocument
    {
        public PaymentHeader DocHeader { get; set; }
        public List<PaymentLines> ListOfPaymentLines { get; set; }
    }
}