using Latest_Staff_Portal.CustomSecurity;
using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.Controllers
{
    [CustomeAuthentication]
    [CustomAuthorization(Role = "ALLUSERS")]
    public class StaffClaimController : Controller
    {
        // GET: StaffClaim
        public ActionResult StaffClaimRequisitionList()
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
                erroMsg.Message = ex.Message.Replace("'", "");
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult StaffClaimRequisitionListPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<StaffClaimList> ClaimList = new List<StaffClaimList>();

                string page = "StaffClaimList?$filter=Employee_No eq '" + StaffNo + "'&format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        StaffClaimList CList = new StaffClaimList();
                        CList.No = (string)config["No"];
                        CList.ReqDate = Convert.ToDateTime((string)config["Date"]).ToString("dd/MM/yyyy");
                        CList.Purpose = (string)config["Purpose"];
                        CList.Function = (string)config["Function_Name"];
                        CList.BudgetCeter = (string)config["Budget_Center_Name"];
                        if ((string)config["Status"] == "Released" || (string)config["Status"] == "Posted")
                        {
                            CList.Status = "Approved";
                        }
                        else if ((string)config["Status"] == "Pending")
                        {
                            CList.Status = "Open";
                        }
                        else
                        {
                            CList.Status = (string)config["Status"];
                        }
                        ClaimList.Add(CList);
                    }
                }
                return PartialView("~/Views/StaffClaim/StaffClaimReqListView.cshtml", ClaimList.OrderByDescending(x => x.No));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public ActionResult NewStaffClaimRequest()
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
                    NewStaffClaimRequisition NewStaffClaim = new NewStaffClaimRequisition();
                    string Dir = "", Dep = "";
                    bool ResRegardDirectorate = false;
                    #region Employee Data
                    string pageData = "EmployeeList?$filter=No eq '" + StaffNo + "'&$format=json";

                    HttpWebResponse httpResponse = Credentials.GetOdataData(pageData);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);

                        if (details["value"].Count() > 0)
                        {
                            foreach (JObject config in details["value"])
                            {
                                Dir = (string)config["_x003C_GlobSal_Dimension_1_Code_x003E_"];
                                Dep = (string)config["GlobalDimension2Code"];
                                ResRegardDirectorate = (bool)config["Disregard_Directorate"];
                            }
                        }
                    }
                    #endregion
                    if (Dir == "" && ResRegardDirectorate == false)
                    {
                        Error erroMsg = new Error();
                        erroMsg.Message = "Your directorate has not been set. Contact HR";
                        return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
                    }
                    else if (Dep == "")
                    {
                        Error erroMsg = new Error();
                        erroMsg.Message = "Your department has not been set. Contact HR";
                        return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
                    }
                    else
                    {
                        #region Directorate List
                        List<DimensionValues> DirectorateList = new List<DimensionValues>();
                        string pageDir = "DimensionValues?$filter=Dimension_Code eq 'BRANCH'&$format=json";

                        HttpWebResponse httpResponseDepartment = Credentials.GetOdataData(pageDir);
                        using (var streamReader = new StreamReader(httpResponseDepartment.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);


                            foreach (JObject config in details["value"])
                            {
                                DimensionValues Directorate = new DimensionValues();
                                Directorate.Code = (string)config["Code"];
                                Directorate.Name = (string)config["Name"];
                                DirectorateList.Add(Directorate);
                            }
                        }
                        #endregion

                        #region Department
                        List<DimensionValues> DepartmentList = new List<DimensionValues>();
                        string pageDepartment = "DimensionValues?$filter=Dimension_Code eq 'DEPARTMENT'&$format=json";

                        HttpWebResponse httpResponseDivision = Credentials.GetOdataData(pageDepartment);
                        using (var streamReader = new StreamReader(httpResponseDivision.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);


                            foreach (JObject config in details["value"])
                            {
                                DimensionValues Department = new DimensionValues();
                                Department.Code = (string)config["Code"];
                                Department.Name = (string)config["Name"];
                                DepartmentList.Add(Department);
                            }
                        }
                        #endregion

                        #region Section List
                        List<DimensionValues> SectionList = new List<DimensionValues>();
                        string pageSection = "DimensionValues?$filter=Dimension_Code eq 'SECTION'&format=json";

                        HttpWebResponse httpResponseSection = Credentials.GetOdataData(pageSection);
                        using (var streamReader = new StreamReader(httpResponseSection.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);


                            foreach (JObject config in details["value"])
                            {
                                DimensionValues Section = new DimensionValues();
                                Section.Code = (string)config["Code"];
                                Section.Name = (string)config["Name"];
                                SectionList.Add(Section);
                            }
                        }
                        #endregion

                        NewStaffClaim = new NewStaffClaimRequisition
                        {
                            Directorate = Dir,
                            Department = Dep,
                            DisDir = ResRegardDirectorate,
                            ListOfDirectorate = DirectorateList.Select(x =>
                                               new SelectListItem()
                                               {
                                                   Text = x.Name,
                                                   Value = x.Code
                                               }).ToList(),
                            ListOfDepartment = DepartmentList.Select(x =>
                                                 new SelectListItem()
                                                 {
                                                     Text = x.Name,
                                                     Value = x.Code
                                                 }).ToList()
                        };
                        return View("~/Views/StaffClaim/NewStaffClaimRequest.cshtml", NewStaffClaim);
                    }
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewStaffClaimLine()
        {
            try
            {
                StaffClaimTypesList StaffClaimTypes = new StaffClaimTypesList();

                #region Imprest Type List
                List<StaffClaimTypes> ClaimTList = new List<StaffClaimTypes>();
                string page = "StaffClaimTypes?$filter=Type eq 'Claim' and Description ne ''&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        StaffClaimTypes ClaimList = new StaffClaimTypes();
                        ClaimList.Code = (string)config["Code"];
                        ClaimList.Description = (string)config["Description"];
                        ClaimTList.Add(ClaimList);
                    }
                }
                #endregion

                StaffClaimTypes = new StaffClaimTypesList
                {
                    ListOfStaffClaimTypes = ClaimTList.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Description,
                                              Value = x.Code
                                          }).OrderBy(x => x.Text).ToList()
                };

                return PartialView("~/Views/StaffClaim/StaffClaimItemForm.cshtml", StaffClaimTypes);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitStaffClaimRequisition(StaffClaimHeader staffClaimHeader)
        {
            bool successVal = false;
            try
            {
                string Directorate = "", Section = "";
                string StaffNo = Session["Username"].ToString();
                if (staffClaimHeader.Directorate != null)
                {
                    Directorate = staffClaimHeader.Directorate;
                }
                string DocNo = Credentials.ObjNav.InsertStaffClaims(StaffNo, Directorate, staffClaimHeader.Department, Section, "", ""
                                  , staffClaimHeader.Remarks, false, "");
                if (DocNo != "")
                {
                    string Redirect = "/StaffClaim/StaffClaimDocumentView?DocNo=" + DocNo;

                    Session["SuccessMsg"] = "Staff Claim Requisition, Document No: " + DocNo + ", created Successfully. Add line(s) and attachment(s) then send for approval";
                    return Json(new { message = Redirect, success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "Document not created. Please try again later...", success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                if (successVal)
                {
                    Session["ErrorMsg"] = ex.Message.Replace("'", "");
                }
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult StaffClaimDocumentView(string DocNo)
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
                    #region Staff Claim Header

                    #region Directorate List
                    List<DimensionValues> DirectorateList = new List<DimensionValues>();
                    string pageDir = "DimensionValues?$filter=Dimension_Code eq 'DIRECTORATES'&$format=json";

                    HttpWebResponse httpResponseDepartment = Credentials.GetOdataData(pageDir);
                    using (var streamReader = new StreamReader(httpResponseDepartment.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);


                        foreach (JObject config in details["value"])
                        {
                            DimensionValues Directorate = new DimensionValues();
                            Directorate.Code = (string)config["Code"];
                            Directorate.Name = (string)config["Name"];
                            DirectorateList.Add(Directorate);
                        }
                    }
                    #endregion

                    #region Department
                    List<DimensionValues> DepartmentList = new List<DimensionValues>();
                    string pageDepartment = "DimensionValues?$filter=Dimension_Code eq 'DEPARTMENT'&$format=json";

                    HttpWebResponse httpResponseDivision = Credentials.GetOdataData(pageDepartment);
                    using (var streamReader = new StreamReader(httpResponseDivision.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);


                        foreach (JObject config in details["value"])
                        {
                            DimensionValues Department = new DimensionValues();
                            Department.Code = (string)config["Code"];
                            Department.Name = (string)config["Name"];
                            DepartmentList.Add(Department);
                        }
                    }
                    #endregion

                    #region Section List
                    List<DimensionValues> SectionList = new List<DimensionValues>();
                    string pageSection = "DimensionValues?$filter=Dimension_Code eq 'SECTION'&format=json";

                    HttpWebResponse httpResponseSection = Credentials.GetOdataData(pageSection);
                    using (var streamReader = new StreamReader(httpResponseSection.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);


                        foreach (JObject config in details["value"])
                        {
                            DimensionValues Section = new DimensionValues();
                            Section.Code = (string)config["Code"];
                            Section.Name = (string)config["Name"];
                            SectionList.Add(Section);
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
                    StaffClaimHeader ClaimDoc = new StaffClaimHeader();

                    string page = "StaffClaimCard?$filter=No eq '" + DocNo + "'&format=json";
                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        foreach (JObject config in details["value"])
                        {
                            ClaimDoc.No = (string)config["No"];
                            ClaimDoc.DateRequested = Convert.ToDateTime((string)config["Date"]).ToString("dd/MM/yyyy");
                            ClaimDoc.Remarks = (string)config["Purpose"];
                            ClaimDoc.Directorate = (string)config["GlobalDimension1Code"];
                            ClaimDoc.Department = (string)config["ShortcutDimension2Code"];
                            ClaimDoc.TotalAmount = Convert.ToDecimal((string)config["TotalNetAmount"]).ToString("#,##0.00");
                            ClaimDoc.DisDir = CommonClass.DisregardDirectorate(StaffNo);
                            if ((string)config["Status"] == "Released" || (string)config["Status"] == "Posted")
                            {
                                ClaimDoc.Status = "Approved";
                            }
                            else if ((string)config["Status"] == "Pending")
                            {
                                ClaimDoc.Status = "Open";
                            }
                            else
                            {
                                ClaimDoc.Status = (string)config["Status"];
                            }
                        }
                    }
                    ClaimDoc.ListOfDirectorate = DirectorateList.Select(x =>
                                           new SelectListItem()
                                           {
                                               Text = x.Name,
                                               Value = x.Code
                                           }).ToList();
                    ClaimDoc.ListOfDepartment = DepartmentList.Select(x =>
                                  new SelectListItem()
                                  {
                                      Text = x.Name,
                                      Value = x.Code
                                  }).ToList();
                    ClaimDoc.ListOfResponsibility = RespCList.Select(x =>
                                  new SelectListItem()
                                  {
                                      Text = x.Name,
                                      Value = x.Code
                                  }).ToList();
                    #endregion
                    return View(ClaimDoc);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult ClaimDocumentLines(string DocNo, string Status)
        {
            try
            {
                #region Staff Claim Lines
                List<StaffClaimLines> ClaimLines = new List<StaffClaimLines>();
                string pageLine = "StaffCaimLines?$filter=No eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        StaffClaimLines claimLine = new StaffClaimLines();
                        claimLine.DocNo = (string)config["No"];
                        claimLine.AdvanceType = (string)config["Advance_Type"];
                        claimLine.Item = (string)config["Account_No"];
                        claimLine.ItemDesc = (string)config["Account_Name"];
                        claimLine.ItemDesc2 = (string)config["Purpose"];
                        claimLine.LnNo = (string)config["Line_No"];
                        claimLine.Amount = Convert.ToDecimal((string)config["Amount"]).ToString("#,##0.00");
                        ClaimLines.Add(claimLine);
                    }
                }
                #endregion
                StaffClaimLinesList Lines = new StaffClaimLinesList
                {
                    Status = Status,
                    ListOfStaffClaimLines = ClaimLines
                };
                return PartialView("~/Views/StaffClaim/StaffClaimDocumentLineView.cshtml", Lines);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public JsonResult SendStaffClaimAppForApproval(string DocNo, string Redirect)
        {
            try
            {
                Credentials.ObjNav.StaffClaimRequisitionApprovalRequest(DocNo);
                if (Redirect == "Y")
                {
                    Session["SuccessMsg"] = "Staff Claim Requisition, Document No " + DocNo + " send for approval Successfully";
                }
                return Json(new { message = "Staff Claim Requisition send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CancelStaffClaimAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.HRCanceStaffClaimRequisition(DocNo);
                return Json(new { message = "Staff Claim Requisition approval cancelled Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateStaffClaimHeader(string DocNo, StaffClaimHeader staffClaimHeader)
        {
            try
            {
                string section = "", Remarks = "";
                if (staffClaimHeader.Remarks != null)
                {
                    Remarks = staffClaimHeader.Remarks;
                }

                Credentials.ObjNav.UpdateStaffClaims(DocNo.Trim(), DateTime.Today, staffClaimHeader.Directorate, staffClaimHeader.Department,
                    section, "", "", Remarks);
                return Json(new { message = "Staff Claim header Updated successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitClaimLine(string DocNo, StaffClaimHeader staffClaimHeader, StaffClaimLines staffClaimLine)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                string item = staffClaimLine.Item.Trim();
                string itemDesc = staffClaimLine.ItemDesc.Trim();
                string amnt = staffClaimLine.Amount.Trim();

                string Dir = "", Dep = "";
                if (staffClaimHeader.Directorate != null)
                {
                    Dir = staffClaimHeader.Directorate;
                }
                if (staffClaimHeader.Department != null)
                {
                    Dep = staffClaimHeader.Department;
                }
                Credentials.ObjNav.StaffClaimRequisitionLinesInsert(DocNo, item, Convert.ToDecimal(amnt), StaffNo, itemDesc, false);
                string DocNetAmount = GetClaimDocNetAmount(DocNo);
                return Json(new { NetAmout = DocNetAmount, message = "Claim Line Added successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult EditStaffClaimLine(string LnNo, string DocNo)
        {
            try
            {
                int ln = Convert.ToInt32(LnNo);

                #region Staff Claim Lines
                StaffClaimLines claimLine = new StaffClaimLines();
                string pageLine = "StaffCaimLines?$filter=No eq '" + DocNo + "' and Line_No eq " + ln + "&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        claimLine.DocNo = (string)config["No"];
                        claimLine.AdvanceType = (string)config["Advance_Type"];
                        claimLine.Item = (string)config["Account_No"];
                        claimLine.ItemDesc = (string)config["Account_Name"];
                        claimLine.ItemDesc2 = (string)config["Purpose"];
                        claimLine.LnNo = (string)config["Line_No"];
                        claimLine.Amount = Convert.ToDecimal((string)config["Amount"]).ToString("#,##0.00");
                    }
                }
                #endregion
                #region Imprest Type List
                List<StaffClaimTypes> ClaimTList = new List<StaffClaimTypes>();
                string page = "StaffClaimTypes?$filter=Type eq 'Claim' and Description ne ''&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        StaffClaimTypes ClaimList = new StaffClaimTypes();
                        ClaimList.Code = (string)config["Code"];
                        ClaimList.Description = (string)config["Description"];
                        ClaimTList.Add(ClaimList);
                    }
                }
                #endregion

                StaffClaimItemDetails itemDetails = new StaffClaimItemDetails
                {
                    ItemDetails = claimLine,
                    ListOfStaffClaimTypes = ClaimTList.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Description,
                                              Value = x.Code
                                          }).ToList()
                };
                return PartialView("~/Views/StaffClaim/StaffClaimEditItemForm.cshtml", itemDetails);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RemoveClaimLine(string DocNo, string LnNo, string ItemNo)
        {
            try
            {
                Credentials.ObjNav.StaffClaimRemoveLine(Convert.ToInt32(LnNo), DocNo, ItemNo);
                string DocNetAmount = GetClaimDocNetAmount(DocNo);
                return Json(new { NetAmout = DocNetAmount, message = "Claim Line Deleted successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        protected string GetClaimDocNetAmount(string DocNo)
        {
            string amount = "";
            string page = "StaffClaimCard?$select=TotalNetAmount&$filter=No eq '" + DocNo + "'&format=json";
            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        amount = Convert.ToDecimal((string)config["TotalNetAmount"]).ToString("#,##0.00");
                    }
                }
            }
            return amount;
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public PartialViewResult FileUploadForm()
        {
            return PartialView("~/Views/StaffClaim/FileAttachmentForm.cshtml");
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetImprestList()
        {
            try
            {
                //#region Imprest List
                //string StaffNo = Session["Username"].ToString();
                //List<ImprestList> ImprestList = new List<ImprestList>();

                //string page = "ImprestReq?$filter=AccountNo eq '" + StaffNo + "'&$format=json";
                //HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                //{
                //    var result = streamReader.ReadToEnd();

                //    var details = JObject.Parse(result);
                //    foreach (JObject config in details["value"])
                //    {
                //        ImprestList Imp = new ImprestList();
                //        Imp.No = (string)config["No"];
                //        ImprestList.Add(Imp);
                //    }
                //}
                //#endregion
                string StaffNo = Session["Username"].ToString();
                #region ImprestList
                List<ImprestList> ImprestList = new List<ImprestList>();
                ImprestList Imp = null;
                string page = "PostedImprest?$filter=AccountNo eq '" + StaffNo + "' and SurrenderStatus ne 'Full' &format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        if (!isImprestSurrendered((string)config["No"], (decimal)config["TotalPaymentAmount"]))
                        {
                            Imp = new ImprestList();
                            Imp.No = (string)config["No"];
                            ImprestList.Add(Imp);
                        }
                    }
                }

                #endregion
                StaffClaimImpList ImpList = new StaffClaimImpList
                {
                    ListOfImprest = ImprestList.Select(x =>
                                    new SelectListItem()
                                    {
                                        Text = x.No,
                                        Value = x.No
                                    }).ToList()
                };
                return Json(ImpList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        protected bool isImprestSurrendered(string DocNo, decimal Amount)
        {
            bool ext = false;
            string page = "ImprestSurrenderList?$select=Amount&$filter=Imprest_Issue_Doc_No eq '" + DocNo + "'&$format=json";
            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                if (details["value"].Count() > 0)
                {
                    decimal amt = 0;
                    foreach (JObject config in details["value"])
                    {
                        amt = amt + (decimal)config["Amount"];
                    }
                    if (amt >= Amount)
                    {
                        ext = true;
                    }
                }
            }
            return ext;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitStaffClaimFromImprest(string ImprestNo, string Remarks)
        {
            bool successVal = false;
            try
            {
                string StaffNo = Session["Username"].ToString();
                string DocNo = Credentials.ObjNav.RaiseStaffClaimFromImprest(ImprestNo, Remarks);
                if (DocNo != "")
                {
                    string Redirect = "/StaffClaim/StaffClaimDocumentView?DocNo=" + DocNo;

                    Session["SuccessMsg"] = "Staff Claim Requisition, Document No: " + DocNo + ", created Successfully.";
                    return Json(new { message = Redirect, success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "Document not created. Please try again later...", success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                if (successVal)
                {
                    Session["ErrorMsg"] = ex.Message.Replace("'", "");
                }
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}