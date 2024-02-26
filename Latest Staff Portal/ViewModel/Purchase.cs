using System.Collections.Generic;
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
        public string VendorName { get; set; }
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

    public class PurchaseQuote
    {
        public string DocumentType { get; set; }
        public string No { get; set; }
        public string BuyFromVendorNo { get; set; }
        public string BuyFromVendorName { get; set; }
        public string BuyFromAddress { get; set; }
        public string BuyFromAddress2 { get; set; }
        public string BuyFromCounty { get; set; }
        public string BuyFromPostCode { get; set; }
        public string BuyFromCity { get; set; }
        public string BuyFromCountryRegionCode { get; set; }
        public string BuyFromContactNo { get; set; }
        public string BuyFromContact { get; set; }
        public string DocumentDate { get; set; }
        public string DueDate { get; set; }
        public string OrderDate { get; set; }
        public int NoOfArchivedVersions { get; set; }
        public string RequestedReceiptDate { get; set; }
        public string VendorOrderNo { get; set; }
        public string VendorShipmentNo { get; set; }
        public string PurchaserCode { get; set; }
        public string DocumentType2 { get; set; }
        public string ShortcutDimension1Code2 { get; set; }
        public string ShortcutDimension2Code2 { get; set; }
        public string ShortcutDimension3Code { get; set; }
        public string PostingDescription { get; set; }
        public string EmployeeNo { get; set; }
        public string RFQNo { get; set; }
        public string ExpectedClosingDate { get; set; }
        public string ProcurementMethodCode { get; set; }
        public string CampaignNo { get; set; }
        public string OrderAddressCode { get; set; }
        public string ResponsibilityCenter { get; set; }
        public string AssignedUserID { get; set; }
        public string Status { get; set; }
        public string CurrencyCode { get; set; }
        public string ExpectedReceiptDate { get; set; }
        public bool PricesIncludingVAT { get; set; }
        public string VATBusPostingGroup { get; set; }
        public string PaymentTermsCode { get; set; }
        public string PaymentMethodCode { get; set; }
        public string TransactionType { get; set; }
        public int PaymentDiscountPercent { get; set; }
        public string PmtDiscountDate { get; set; }
        public string ShipmentMethodCode { get; set; }
        public string PaymentReference { get; set; }
        public string CreditorNo { get; set; }
        public string OnHold { get; set; }
        public bool TaxLiable { get; set; }
        public string TaxAreaCode { get; set; }
        public string ShippingOptionWithLocation { get; set; }
        public string LocationCode { get; set; }
        public string ShipToName { get; set; }
        public string ShipToAddress { get; set; }
        public string ShipToAddress2 { get; set; }
        public string ShipToCounty { get; set; }
        public string ShipToPostCode { get; set; }
        public string ShipToCity { get; set; }
        public string ShipToCountryRegionCode { get; set; }
        public string ShipToContact { get; set; }
        public string PayToOptions { get; set; }
        public string PayToName { get; set; }
        public string PayToAddress { get; set; }
        public string PayToAddress2 { get; set; }
        public string PayToCounty { get; set; }
        public string PayToPostCode { get; set; }
        public string PayToCity { get; set; }
        public string PayToCountryRegionCode { get; set; }
        public string PayToContactNo { get; set; }
        public string PayToContact { get; set; }
        public string TransactionSpecification { get; set; }
        public string TransportMethod { get; set; }
        public string EntryPoint { get; set; }
        public string Area { get; set; }
    }

}