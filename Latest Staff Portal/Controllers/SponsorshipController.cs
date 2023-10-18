using Latest_Staff_Portal.CustomSecurity;
using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.Controllers
{
    [CustomeAuthentication]
    [CustomAuthorization(Role = "ALLUSERS")]
    public class SponsorshipController : Controller
    {
        // GET: Sponsorship
        public ActionResult ConceptApplicationList()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                return View();
            }
        }
        public ActionResult SponsorshipApplicationList()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                return View();
            }
        }
        public PartialViewResult SponsorshipRequisitionListPartialView()
        {
            if (Session["Username"] == null)
            {
                Response.Redirect(Url.Action("Login", "Login"));
            }
            string StaffNo = Session["Username"].ToString();
            List<SponsorshipList> ListOfSponsorship = new List<SponsorshipList>();

            string page = "ConceptCard?$filter=Employee_No eq '"+ StaffNo + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    SponsorshipList SpnList = new SponsorshipList();
                    SpnList.No = (string)config["No"];
                    SpnList.Description = (string)config["Description"];
                    SpnList.CreationDate = Convert.ToDateTime((string)config["Creation_Date"]).ToString("dd/MM/yyyy");
                    SpnList.ApprovalStatus = (string)config["Approval_Status"];
                    SpnList.Status = (string)config["Status"];
                    ListOfSponsorship.Add(SpnList);
                }
            }
            return PartialView("~/Views/Sponsorship/SponsorshipList.cshtml", ListOfSponsorship);
        }
        public PartialViewResult NewConceptRequisition()
        {
            if (Session["Username"] == null)
            {
                Response.Redirect(Url.Action("Login", "Login"));
            }
            string StaffNo = Session["Username"].ToString();
            NewSponsorship NewAppl = new NewSponsorship();
            #region Institute List
            List<DimensionValues> Campuses = new List<DimensionValues>();
            string pageCampus = "DimValues?$select=Code,Name&$filter=Dimension_Code eq 'INSTITUTE' and Blocked eq false&$format=json";

            HttpWebResponse httpResponseCampus = Credentials.GetOdataData(pageCampus);
            using (var streamReader = new StreamReader(httpResponseCampus.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);


                foreach (JObject config in details["value"])
                {
                    DimensionValues CmpList = new DimensionValues();
                    CmpList.Code = (string)config["Code"];
                    CmpList.Name = (string)config["Name"];
                    Campuses.Add(CmpList);
                }
            }
            #endregion
            #region Department List
            List<DimensionValues> Department = new List<DimensionValues>();
            string pageDepartment = "DimValues?$select=Code,Name&$filter=Dimension_Code eq 'DEPARTMENT' and Blocked eq false&$format=json";

            HttpWebResponse httpResponseDepartment = Credentials.GetOdataData(pageDepartment);
            using (var streamReader = new StreamReader(httpResponseDepartment.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);


                foreach (JObject config in details["value"])
                {
                    DimensionValues DepartmentList = new DimensionValues();
                    DepartmentList.Code = (string)config["Code"];
                    DepartmentList.Name = (string)config["Name"];
                    Department.Add(DepartmentList);
                }
            }
            #endregion
            #region Customer
            List<CustomerList> CustList = new List<CustomerList>();

            string pageResourse = "CustomerList?&select=Customer_Posting_Group&$filter=Customer_Posting_Group eq 'ACE II'&format=json";

            HttpWebResponse httpResponseResource = Credentials.GetOdataData(pageResourse);
            using (var streamReader = new StreamReader(httpResponseResource.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    CustomerList NewCustomer = new CustomerList();
                    NewCustomer.No = (string)config["No"];
                    NewCustomer.Name = (string)config["Name"];
                    CustList.Add(NewCustomer);
                }
            }
            #endregion
            #region Responsibility
            List<RespCenter> RespCList = new List<RespCenter>();
            string pageResC = "ResponsibilityCenters?$format=json";

            HttpWebResponse httpResponseResC = Credentials.GetOdataData(pageResC);
            using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);


                foreach (JObject config in details["value"])
                {
                    RespCenter RCList = new RespCenter();
                    RCList.Code = (string)config["Code"];
                    RCList.Name = (string)config["Name"];
                    RespCList.Add(RCList);
                }
            }
            #endregion
            NewAppl = new NewSponsorship
            {
                ListOfCampus = Campuses.Select(x =>
                                     new SelectListItem()
                                     {
                                         Text = x.Name,
                                         Value = x.Code
                                     }).ToList(),
                ListOfDepartment = Department.Select(x =>
                                     new SelectListItem()
                                     {
                                         Text = x.Name,
                                         Value = x.Code
                                     }).ToList(),
                ListOfCustomers = CustList.Select(x =>
                                     new SelectListItem()
                                     {
                                         Text = x.Name,
                                         Value = x.No
                                     }).ToList(),
                ListOfResponsibility = RespCList.Select(x =>
                                   new SelectListItem()
                                   {
                                       Text = x.Name,
                                       Value = x.Code
                                   }).ToList()
            };
            return PartialView("~/Views/Sponsorship/NewSponsorshipApplication.cshtml", NewAppl);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitSponsorshipDocumentDocument(NewSponsorshipDocument NewApp)
        {
            try
            {
                //string DocNo = Credentials.ObjNav.InsertScolorship(NewApp.Title, NewApp.PInvest, NewApp.Objective, NewApp.Email,
                   // NewApp.Address, 0, Session["username"].ToString(), NewApp.RespC, NewApp.Dim1, NewApp.Dim2, Session["username"].ToString());

                //Credentials.ObjNav.SendConceptForApproval(DocNo);

                return Json(new { message = "Concept Document created successfully and send for Approval", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult SponsorshipDocumentDetails(string DocNo)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect(Url.Action("Login", "Login"));
            }
            string StaffNo = Session["Username"].ToString();
            NewSponsorshipDocument SponshpDoc = new NewSponsorshipDocument();

            string page = "ConceptList?$filter=No eq '" + DocNo + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    SponshpDoc.No = (string)config["No"];
                    SponshpDoc.Title = (string)config["Description"];
                    SponshpDoc.Objective = (string)config["Objective"];
                    SponshpDoc.Address = (string)config["Bill_to_Address"];
                    SponshpDoc.Email = (string)config["Bill_to_Address_2"];
                    SponshpDoc.DateCreated = Convert.ToDateTime((string)config["Creation_Date"]).ToString("dd/MM/yyyy");
                    SponshpDoc.PInvest = (string)config["Principal_Investigator_name"];
                    SponshpDoc.RespC = (string)config["Responsibility_Center"];
                    SponshpDoc.Dim1 = (string)config["Responsibility_Center"];
                    SponshpDoc.Dim2 = (string)config["Responsibility_Center"];
                    SponshpDoc.Status = (string)config["Status"];
                    SponshpDoc.ApprovalStatus = (string)config["Approval_Status"];
                }
            }
            return PartialView("~/Views/Sponsorship/SponsorshipDocumentDetails.cshtml", SponshpDoc);
        }
        public JsonResult SendDocAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.SendConceptForApproval(DocNo);
                return Json(new { message = "Concept Application send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ProposalEthicalReview()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                return View();
            }
        }
        public PartialViewResult ProposalEthicalReviewList()
        {
            if (Session["Username"] == null)
            {
                Response.Redirect(Url.Action("Login", "Login"));
            }
            string StaffNo = Session["Username"].ToString();
            List<Proposal> ListOfProposalReview = new List<Proposal>();

            string page = "ProposalEthicalReview?$filter=Staff_No eq '" + StaffNo + "' and Reviewed eq false&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    Proposal propReview = new Proposal();
                    propReview.Line_No = (string)config["Line_No"];
                    propReview.Proposal_No = (string)config["Proposal_No"];
                    string pageProposal = "ProposalCard?$filter=No eq '" + (string)config["Proposal_No"] + "'&format=json";
                    HttpWebResponse httpResponseProposal = Credentials.GetOdataData(pageProposal);
                    using (var streamReaderProposal = new StreamReader(httpResponseProposal.GetResponseStream()))
                    {
                        var resultProposal = streamReaderProposal.ReadToEnd();
                        var detailsProposal = JObject.Parse(resultProposal);
                        foreach (JObject config1 in detailsProposal["value"])
                        {
                            propReview.Description = (string)config1["Description"];
                            propReview.Objective = (string)config1["Objective"];
                        }
                    }
                    ListOfProposalReview.Add(propReview);
                }
            }
            return PartialView("~/Views/Sponsorship/EthicalReviewList.cshtml", ListOfProposalReview);
        }
        public PartialViewResult EthicalReviewDocument(string DocNo)
        {
            string StaffNo = Session["Username"].ToString();
            NewSponsorshipDocument SponshpDoc = new NewSponsorshipDocument();

            string page = "ProposalCard?$filter=No eq '" + DocNo + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    SponshpDoc.No = (string)config["No"];
                    SponshpDoc.Title = (string)config["Description"];
                    SponshpDoc.Objective = (string)config["Objective"];
                    SponshpDoc.Address = (string)config["Bill_to_Address"];
                    SponshpDoc.Email = (string)config["Bill_to_Address_2"];
                    SponshpDoc.DateCreated = Convert.ToDateTime((string)config["Creation_Date"]).ToString("dd/MM/yyyy");
                    SponshpDoc.PInvest = (string)config["Principal_Investigator_name"];
                    SponshpDoc.RespC = (string)config["Responsibility_Center"];
                    SponshpDoc.Status = (string)config["Status"];
                    SponshpDoc.ApprovalStatus = (string)config["Proposal_Status"];
                }
            }
            return PartialView("~/Views/Sponsorship/EthicalReviewDocument.cshtml", SponshpDoc);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitProposalEthicalReview(string DocNo, string Remarks, string Recommendation)
        {
            try
            {
                //Credentials.ObjNav.UpdateConceptReview(DocNo, Session["Username"].ToString(), Remarks, Convert.ToInt32(Recommendation));
                return Json(new { message = "Concept Document Reviewed successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}