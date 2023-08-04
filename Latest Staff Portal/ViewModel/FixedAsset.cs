using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class FixedAsset
    {
        public string AssetNo { get; set; }
        public string Description { get; set; }
    }
    public class AssetTransfer
    {
        public string No { get; set; }
        public string RaisedBy { get; set; }
        public string TransferType { get; set; }
        public string Type { get; set; }
        public string AssettoTransfer { get; set; }
        public string AssetDescription { get; set; }
        public string Status { get; set; }
        public string Transferred { get; set; }
        public string Comments { get; set; }
        public string FromLocation { get; set; }
        public string FromResponsibleEmployee { get; set; }
        public string FromDimension1Code { get; set; }
        public string FromDimension2Code { get; set; }
        public string ToLocation { get; set; }
        public string DestinationLocation { get; set; }
        public string ToResponsibleEmployee { get; set; }
        public string ToEmployeeName { get; set; }
        public string ToDimension1Code { get; set; }
        public string ToDimension2Code { get; set; }
    }
    public class NewTransferForm
    {
        public string AssetNo { get; set; }
        public AssetTransfer TransferDoc { get; set; }
        public List<SelectListItem> ListOfEmployee { get; set; }
        public List<SelectListItem> ListOfDepartment { get; set; }
        public List<SelectListItem> ListOfDivision { get; set; }
    }
}