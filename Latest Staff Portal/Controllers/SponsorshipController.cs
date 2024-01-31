using Latest_Staff_Portal.CustomSecurity;
using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.Controllers
{
    [CustomeAuthentication]
    [CustomAuthorization(Role = "FULLTIME")]
    public class SponsorshipController : Controller
    {
        // GET: Sponsorship
        public ActionResult ConceptApplicationList()
        {
            try
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
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public ActionResult ProtocolApplicationList()
        {
            try
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
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public ActionResult ApprovedConceptApplicationList()
        {
            try
            {
                if (Session["Username"] == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    string StaffNo = Session["Username"].ToString();
                    List<SponsorshipList> ListOfSponsorship = new List<SponsorshipList>();

                    string page = "ConceptList?$filter=Employee_No eq '" + StaffNo + "'&$format=json";

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
                            //SpnList.ApprovalStatus = CommonClass.GetConceptApprovalStatus((string)config["No"]);
                            if (SpnList.ApprovalStatus != "Approved")
                            {
                                continue;
                            }
                            SpnList.Status = (string)config["Status"];
                            ListOfSponsorship.Add(SpnList);
                        }
                    }
                    return View(ListOfSponsorship);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult SponsorshipRequisitionListPartialView()
        {
            try
            {
                if (Session["Username"] == null)
                {
                    Response.Redirect(Url.Action("Login", "Login"));
                }
                string StaffNo = Session["Username"].ToString();
                List<SponsorshipList> ListOfSponsorship = new List<SponsorshipList>();

                string page = "ProtocolApplication?$filter=Employee_No eq '" + StaffNo + "'&$format=json";

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
                        SpnList.Status = (string)config["Application_Level"];
                        SpnList.ApprovalStatus = (string)config["Review_Status"];
                        ListOfSponsorship.Add(SpnList);
                    }
                }
                return PartialView("~/Views/Sponsorship/SponsorshipList.cshtml", ListOfSponsorship);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewConceptRequisition()
        {
            try
            {
                if (Session["Username"] == null)
                {
                    Response.Redirect(Url.Action("Login", "Login"));
                }
                string StaffNo = Session["Username"].ToString();
                NewSponsorship NewAppl = new NewSponsorship();
                #region Campus List
                List<DimensionValues> Campuses = new List<DimensionValues>();
                string pageCampus = "DimensionValues?$filter=Global_Dimension_No_ eq 1&$format=json";

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
                string pageDepartment = "DimensionValues?$filter=Global_Dimension_No_ eq 2&$format=json";

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
                #region School List
                List<DimensionValues> schools = new List<DimensionValues>();
                string pageSchool = "DimensionValues?$filter=Global_Dimension_No_ eq 3&$format=json";

                HttpWebResponse httpResponseSchools = Credentials.GetOdataData(pageSchool);
                using (var streamReader = new StreamReader(httpResponseSchools.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues SchoolList = new DimensionValues();
                        SchoolList.Code = (string)config["Code"];
                        SchoolList.Name = (string)config["Name"];
                        schools.Add(SchoolList);
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
                    ListOfSchools = schools.Select(x =>
                                       new SelectListItem()
                                       {
                                           Text = x.Name,
                                           Value = x.Code
                                       }).ToList()
                };
                return PartialView("~/Views/Sponsorship/NewSponsorshipApplication.cshtml", NewAppl);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitSponsorshipDocumentDocument(NewSponsorshipDocument NewApp)
        {
            try
            {
                string Dim1 = "", Dim2 = "", Dim3 = "";
                if (NewApp.Dim1 != null)
                {
                    Dim1 = NewApp.Dim1;
                }
                if (NewApp.Dim2 != null)
                {
                    Dim2 = NewApp.Dim2;
                }
                if (NewApp.School != null)
                {
                    Dim3 = NewApp.School;
                }
                string DocNo = Credentials.ObjNav.InsertScolorship(NewApp.Title, Convert.ToInt32(NewApp.ApplicationLevel), NewApp.Email,
                   NewApp.Address, 0, Dim1, Dim2, Dim3, Session["username"].ToString(),
                    0);

                return Json(new { message = DocNo, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult SponsorshipDocumentDetails(string DocNo)
        {
            try
            {
                if (Session["Username"] == null)
                {
                    Response.Redirect(Url.Action("Login", "Login"));
                }
                string StaffNo = Session["Username"].ToString();
                NewSponsorshipDocument SponshpDoc = new NewSponsorshipDocument();

                string page = "ProtocolApplication?$filter=No eq '" + DocNo + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        SponshpDoc.No = (string)config["No"];
                        SponshpDoc.Title = (string)config["Description"];
                        SponshpDoc.ApplicationLevel = (string)config["Application_Level"];
                        SponshpDoc.Address = (string)config["Bill_to_Address"];
                        //SponshpDoc.Email = (string)config["Bill_to_Address_2"];
                        //SponshpDoc.DateCreated = Convert.ToDateTime((string)config["Creation_Date"]).ToString("dd/MM/yyyy");
                        //SponshpDoc.PInvest = (string)config["Principal_Investigator_name"];
                        SponshpDoc.School = (string)config["Schools"];
                        SponshpDoc.Dim1 = (string)config["Global_Dimension_1_Code"];
                        SponshpDoc.Dim2 = (string)config["Global_Dimension_2_Code"];

                    }
                }
                return PartialView("~/Views/Sponsorship/SponsorshipDocumentDetails.cshtml", SponshpDoc);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public JsonResult SendDocAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.SendConceptForApproval(DocNo);
                return Json(new { message = "Protocol Application send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ProtocolEthicalReview()
        {
            try
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
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult ProposalEthicalReviewList()
        {
            try
            {
                if (Session["Username"] == null)
                {
                    Response.Redirect(Url.Action("Login", "Login"));
                }
                string StaffNo = Session["Username"].ToString();
                List<Proposal> ListOfProposalReview = new List<Proposal>();

                string page = "ProposalEthicalReview?$filter=Staff_No eq '" + StaffNo + "' and Reviewed eq false&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        Proposal propReview = new Proposal();
                        propReview.Line_No = (string)config["Line_No"];
                        propReview.Proposal_No = (string)config["No"];
                        string pageCard = "ProtocolEthicolReview?$select=Concept_Number&$filter=No eq '" + (string)config["No"] + "'&$format=json";
                        HttpWebResponse httpResponseCard = Credentials.GetOdataData(pageCard);
                        using (var streamReaderCard = new StreamReader(httpResponseCard.GetResponseStream()))
                        {
                            var resultCard = streamReaderCard.ReadToEnd();

                            var detailsCard = JObject.Parse(resultCard);
                            foreach (JObject config1 in detailsCard["value"])
                            {
                                propReview.Concept_No = (string)config1["Concept_Number"];
                                string pageProposal = "ProtocolApplication?$filter=No eq '" + (string)config1["Concept_Number"] + "'&$format=json";
                                HttpWebResponse httpResponseProposal = Credentials.GetOdataData(pageProposal);
                                using (var streamReaderProposal = new StreamReader(httpResponseProposal.GetResponseStream()))
                                {
                                    var resultProposal = streamReaderProposal.ReadToEnd();
                                    var detailsProposal = JObject.Parse(resultProposal);
                                    foreach (JObject config2 in detailsProposal["value"])
                                    {
                                        propReview.Description = (string)config2["Description"];
                                        propReview.ApplicationLevel = (string)config2["Application_Level"];
                                    }
                                }
                                ListOfProposalReview.Add(propReview);
                            }
                        }
                    }
                    return PartialView("~/Views/Sponsorship/EthicalReviewList.cshtml", ListOfProposalReview.DistinctBy(x => x.Proposal_No).ToList());
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult EthicalReviewDocument(string DocNo, string PropNo)
        {
            try
            {
                if (Session["Username"] == null)
                {
                    Response.Redirect(Url.Action("Login", "Login"));
                }
                NewSponsorshipDocument SponshpDoc = new NewSponsorshipDocument();

                string pageProposal = "ProtocolApplication?$filter=No eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponseProposal = Credentials.GetOdataData(pageProposal);
                using (var streamReaderProposal = new StreamReader(httpResponseProposal.GetResponseStream()))
                {
                    var resultProposal = streamReaderProposal.ReadToEnd();
                    var detailsProposal = JObject.Parse(resultProposal);
                    foreach (JObject config in detailsProposal["value"])
                    {
                        SponshpDoc.No = (string)config["No"];
                        SponshpDoc.ProposalNo = PropNo;
                        SponshpDoc.Title = (string)config["Description"];
                        SponshpDoc.ApplicationLevel = (string)config["Application_Level"];
                        SponshpDoc.Address = (string)config["Bill_to_Address"];
                        //SponshpDoc.Email = (string)config["Bill_to_Address_2"];
                        //SponshpDoc.DateCreated = Convert.ToDateTime((string)config["Creation_Date"]).ToString("dd/MM/yyyy");
                        //SponshpDoc.PInvest = (string)config["Principal_Investigator_name"];
                        SponshpDoc.School = (string)config["Schools"];
                        SponshpDoc.Dim1 = (string)config["Global_Dimension_1_Code"];
                        SponshpDoc.Dim2 = (string)config["Global_Dimension_2_Code"];
                    }
                }
                return PartialView("~/Views/Sponsorship/EthicalReviewDocument.cshtml", SponshpDoc);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitProposalEthicalReview(string DocNo, string Cr1, string Cr2, string Cr3, string Cr4, string Cr5, string Cr6, string Cr7, string Cr8, string Recommendation)
        {
            try
            {
                Credentials.ObjNav.UpdateConceptReview(DocNo, Session["Username"].ToString(), Cr1, Cr2, Cr3, Cr4, Cr5, Cr6, Cr7, Cr8, Convert.ToInt32(Recommendation));
                return Json(new { message = "Protocol Document Reviewed successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}