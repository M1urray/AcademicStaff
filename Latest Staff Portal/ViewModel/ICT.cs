using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class ICTRequest
    {
        public string No { get; set; }
        public string Date { get; set; }
        public string ReqCat { get; set; }
        public string Directorate { get; set; }
        public string DirectorateName { get; set; }
        public string Department { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }
        public string Urgency { get; set; }
        public string RequiredDate { get; set; }
        public string Status { get; set; }
        public string Resoltion_Remarks { get; set; }
        public string Assignee { get; set; }
    }
    public class ICTRequestLines
    {
        public string Description { get; set; }
        public string Quantity { get; set; }
    }
    public class NewICTRequisition
    {
        public string Code { get; set; }
        public string Directorate { get; set; }
        public string Department { get; set; }
        public string Category { get; set; }
        public string Date { get; set; }
        public List<SelectListItem> ListOfDirectorate { get; set; }
        public List<SelectListItem> ListOfDepartment { get; set; }
        public List<SelectListItem> ListOfCategory { get; set; }
        public List<SelectListItem> ListOfICTAsset { get; set; }
    }
    public class ICTAssetRequest
    {
        public string DocNo { get; set; }
        public string Asset { get; set; }
        public string Description { get; set; }
        public string Requestor_No { get; set; }
        public string Requestor_Name { get; set; }
        public string Date_Requested { get; set; }
        public string Date_Moved { get; set; }
        public string Date_Returned { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
    public class ICTServiceRequest
    {
        public string DocNo { get; set; }
        public string Asset { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string ServiceDate { get; set; }
        public string LastServiceDate { get; set; }
        public string NextSeviceDate { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
    public class ICTCancel
    {
        public string DocNo { get; set; }
        public string Remarks { get; set; }
    }
}