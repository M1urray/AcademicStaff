using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Latest_Staff_Portal.ViewModel
{
    public class TransferOrderHeader
    {
        public string No { get; set; }
        public string TransferFrom { get; set; }
        public string TransferTo { get; set; }
        public string Posting_Date { get; set; }
        public string Shipment_Date { get; set; }
        public string Receipt_Date { get; set; }
        public string CampusName { get; set; }
        public string Department { get; set; }       
        public string Status { get; set; }
        public string AssignedUser { get; set; }
        public string AssignedUserName { get; set; }
    }
    public class TransferOrderLines
    {
        public string DocNo { get; set; }
        public string Item { get; set; }
        public string ItemDesc { get; set; }
        public string Qnty { get; set; }
        public string UoM { get; set; }
        public string Qty_to_Ship { get; set; }
    }
    public class TransferOrderDocument
    {
        public TransferOrderHeader DocHeader { get; set; }
        public List<TransferOrderLines> ListOfTransferOrderLines { get; set; }
    }
}