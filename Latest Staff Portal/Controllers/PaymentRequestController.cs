using iText;
using Latest_Staff_Portal.CustomSecurity;
using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Latest_Staff_Portal.Controllers
{
    [CustomeAuthentication]
    [CustomAuthorization(Role = "ALLUSERS")]
    public class PaymentRequestController : Controller
    {
        // GET: PaymentRequest
        public ActionResult PaymentRequestRequisitionList()
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
        public PartialViewResult PaymentRequestRequisitionListPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<PaymentRequestList> ClaimList = new List<PaymentRequestList>();

                string page = "PaymentRequestList?$filter=Employee_No eq '" + StaffNo + "'&format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        PaymentRequestList CList = new PaymentRequestList();
                        CList.No = (string)config["No"];
                        CList.ReqDate = Convert.ToDateTime((string)config["Date"]).ToString("dd/MM/yyyy");
                        CList.Purpose = (string)config["Purpose"];
                        CList.RespC = (string)config["Responsibility_Center"];
                        CList.Status = (string)config["Status"];
                        ClaimList.Add(CList);
                    }
                }
                return PartialView("~/Views/PaymentRequest/PaymentRequestReqListView.cshtml", ClaimList.OrderByDescending(x => x.No));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public ActionResult NewPaymentRequest()
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
                    NewPaymentRequestRequisition NewPaymentRequest = new NewPaymentRequestRequisition();
                    string Dim1 = "", Dim2 = "", RespC = "";
                    #region Employee Data
                    string pageData = "EmployeeList?$Campus,Department_Code,Responsibility_Center&$filter=No eq '" + StaffNo + "'&$format=json";

                    HttpWebResponse httpResponse = Credentials.GetOdataData(pageData);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);

                        if (details["value"].Count() > 0)
                        {
                            foreach (JObject config in details["value"])
                            {
                                Dim1 = (string)config["Campus"];
                                Dim2 = (string)config["Department_Code"];
                                RespC = (string)config["Responsibility_Center"];
                            }
                        }
                    }
                    #endregion
                    if (Dim1 == "")
                    {
                        Error erroMsg = new Error();
                        erroMsg.Message = "Your Campus not set. Contact HR";
                        return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
                    }
                    else if (Dim2 == "")
                    {
                        Error erroMsg = new Error();
                        erroMsg.Message = "Your Department not set. Contact HR";
                        return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
                    }
                    else if (RespC == "")
                    {
                        Error erroMsg = new Error();
                        erroMsg.Message = "Your Responsibility Center not set. Contact HR";
                        return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
                    }
                    else
                    {
                        #region Campus List
                        List<DimensionValues> Campuses = new List<DimensionValues>();
                        string pageCampus = "DimensionValues?$filter=Global_Dimension_No_ eq 1 and Blocked eq false&$format=json";

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

                        #region School
                        List<DimensionValues> School = new List<DimensionValues>();
                        string pageSchool = "DimensionValues?$filter=Global_Dimension_No_ eq 3&$format=json";

                        HttpWebResponse httpResponseSchool = Credentials.GetOdataData(pageSchool);
                        using (var streamReader = new StreamReader(httpResponseSchool.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);


                            foreach (JObject config in details["value"])
                            {
                                DimensionValues SchoolList = new DimensionValues();
                                SchoolList.Code = (string)config["Code"];
                                SchoolList.Name = (string)config["Name"];
                                School.Add(SchoolList);
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

                        #region Vendors
                        List<DropdownList> VendorList = new List<DropdownList>();
                        string pageTrainer = "VendorList?$format=json";

                        HttpWebResponse httpResponseTrainer = Credentials.GetOdataData(pageTrainer);
                        using (var streamReader = new StreamReader(httpResponseTrainer.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);


                            foreach (JObject config in details["value"])
                            {
                                DropdownList CList = new DropdownList();
                                CList.Value = (string)config["No"];
                                CList.Text = (string)config["Name"];
                                VendorList.Add(CList);
                            }
                        }
                        #endregion

                        NewPaymentRequest = new NewPaymentRequestRequisition
                        {
                            Campus = Dim1,
                            Department = Dim2,
                            RespC = RespC,
                            ListOfCampus = Campuses.Select(x =>
                                                 new SelectListItem()
                                                 {
                                                     Text = x.Name,
                                                     Value = x.Code
                                                 }).ToList(),
                            ListOfSchool = School.Select(x =>
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
                            ListOfResponsibility = RespCList.Select(x =>
                                               new SelectListItem()
                                               {
                                                   Text = x.Name,
                                                   Value = x.Code
                                               }).ToList(),
                            ListOfVendors = VendorList.Select(x =>
                                               new SelectListItem()
                                               {
                                                   Text = x.Text,
                                                   Value = x.Value
                                               }).ToList()

                        };
                        return View(NewPaymentRequest);
                    }
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewPaymentRequestLine()
        {
            try
            {
                PaymentRequestTypesList PaymentRequestTypes = new PaymentRequestTypesList();

                #region Imprest Type List
                List<PaymentRequestTypes> ClaimTList = new List<PaymentRequestTypes>();
                string page = "StaffClaimTypes?$filter=Type eq 'Claim' and Description ne ''&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        PaymentRequestTypes ClaimList = new PaymentRequestTypes();
                        ClaimList.Code = (string)config["Code"];
                        ClaimList.Description = (string)config["Description"];
                        ClaimTList.Add(ClaimList);
                    }
                }
                #endregion
                PaymentRequestTypes = new PaymentRequestTypesList
                {
                    ListOfPaymentRequestTypes = ClaimTList.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Description,
                                              Value = x.Code
                                          }).OrderBy(x => x.Text).ToList()

                };

                return PartialView("~/Views/PaymentRequest/PaymentRequestItemForm.cshtml", PaymentRequestTypes);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitPaymentRequestRequisition(PaymentRequestHeader PaymentRequestHeader)
        {
            bool successVal = false;
            try
            {
                string School = "";
                if (PaymentRequestHeader.school != null)
                {
                    School = PaymentRequestHeader.school;
                }
                string StaffNo = Session["Username"].ToString();
                string DocNo = Credentials.ObjNav.InsertStaffClaims(StaffNo, PaymentRequestHeader.Campus, PaymentRequestHeader.Department
                                  , PaymentRequestHeader.RespC, PaymentRequestHeader.Remarks, "", "", 1);

                if (DocNo != "")
                {
                    string Redirect = "/PaymentRequest/PaymentRequestDocumentView?DocNo=" + DocNo;

                    Session["SuccessMsg"] = "Payment Request Requisition, Document No: " + DocNo + ", created Successfully. Add line(s) and attachment(s) then send for approval";
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
        public ActionResult PaymentRequestDocumentView(string DocNo)
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
                    #region Payment Request Header
                    PaymentRequestHeader ClaimDoc = new PaymentRequestHeader();

                    string page = "PaymentRequestCard?$filter=No eq '" + DocNo + "'&format=json";
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
                            ClaimDoc.school = (string)config["Shortcut_Dimension_3_Code"];
                            ClaimDoc.schoolName = (string)config["Dim3"];
                            ClaimDoc.Campus = (string)config["Global_Dimension_1_Code"];
                            ClaimDoc.CampusName = (string)config["Function_Name"];
                            ClaimDoc.Department = (string)config["Shortcut_Dimension_2_Code"];
                            ClaimDoc.DepartmentName = (string)config["Budget_Center_Name"];
                            ClaimDoc.RespC = (string)config["Responsibility_Center"];
                            ClaimDoc.TotalAmount = Convert.ToDecimal((string)config["Total_Net_Amount"]).ToString("#,##0.00");
                            ClaimDoc.Status = (string)config["Status"];
                        }
                    }
                    #endregion
                    return View(ClaimDoc);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult ClaimDocumentLines(string DocNo, string Status)
        {
            try
            {
                #region Payment Request Lines
                List<PaymentRequestLines> ClaimLines = new List<PaymentRequestLines>();
                string pageLine = "StaffCaimLines?$filter=No eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        PaymentRequestLines claimLine = new PaymentRequestLines();
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
                PaymentRequestLinesList Lines = new PaymentRequestLinesList
                {
                    Status = Status,
                    ListOfPaymentRequestLines = ClaimLines
                };
                return PartialView("~/Views/PaymentRequest/PaymentRequestDocumentLineView.cshtml", Lines);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public JsonResult SendPaymentRequestAppForApproval(string DocNo, string Redirect)
        {
            try
            {
                Credentials.ObjNav.StaffClaimRequisitionApprovalRequest(DocNo);
                if (Redirect == "Y")
                {
                    Session["SuccessMsg"] = "Payment Request Requisition, Document No " + DocNo + " send for approval Successfully";
                }
                return Json(new { message = "Payment Request Requisition,Document No " + DocNo + " send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CancelPaymentRequestAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.HRCanceStaffClaimRequisition(DocNo);
                return Json(new { message = "Payment Request Requisition approval cancelled Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdatePaymentRequestHeader(string DocNo, PaymentRequestHeader PaymentRequestHeader)
        {
            try
            {
                string School = "", Campus = "", Department = "", RespC = "", Remarks = "";
                if (PaymentRequestHeader.Campus != null)
                {
                    Campus = PaymentRequestHeader.Campus;
                }
                if (PaymentRequestHeader.school != null)
                {
                    School = PaymentRequestHeader.school;
                }
                if (PaymentRequestHeader.Department != null)
                {
                    Department = PaymentRequestHeader.Department;
                }
                if (PaymentRequestHeader.RespC != null)
                {
                    RespC = PaymentRequestHeader.RespC;
                }
                if (PaymentRequestHeader.Remarks != null)
                {
                    Remarks = PaymentRequestHeader.Remarks;
                }
                Credentials.ObjNav.UpdateStaffClaims(DocNo.Trim(), DateTime.Today, Campus, Department,
                    School, RespC, Remarks);
                return Json(new { message = "Payment Request header Updated successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitClaimLine(string DocNo, PaymentRequestHeader PaymentRequestHeader, PaymentRequestLines PaymentRequestLine)
        {
            try
            {
                string School = "";
                string StaffNo = Session["Username"].ToString();
                string item = PaymentRequestLine.Item.Trim();
                string itemDesc = PaymentRequestLine.ItemDesc.Trim();
                string amnt = PaymentRequestLine.Amount.Trim();

                Credentials.ObjNav.StaffClaimRequisitionLinesInsert(DocNo, item, Convert.ToDecimal(amnt), StaffNo, PaymentRequestHeader.Campus,
                    PaymentRequestHeader.Department, itemDesc, School);
                string DocNetAmount = GetDocNetAmount(DocNo);
                return Json(new { NetAmout = DocNetAmount, message = "Payment Request Line Added successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RemoveClaimLine(string DocNo, string LnNo, string ItemNo)
        {
            try
            {
                Credentials.ObjNav.StaffClaimRemoveLine(Convert.ToInt32(LnNo), DocNo, ItemNo);
                string DocNetAmount = GetDocNetAmount(DocNo);
                return Json(new { NetAmout = DocNetAmount, message = "Payment Request Line Deleted successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdatePaymentRequestLine(string DocNo, string LnNo, PaymentRequestLines paymentRequestLines)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                string itemDesc = paymentRequestLines.ItemDesc.Trim();
                string amnt = paymentRequestLines.Amount.Trim();
                Credentials.ObjNav.PaymentRequestLineUpdate(DocNo, Convert.ToInt32(LnNo), Convert.ToDecimal(amnt), itemDesc);
                string DocNetAmount = GetDocNetAmount(DocNo);
                return Json(new { NetAmount = DocNetAmount, message = "Payment Request Line updated successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
           protected string GetDocNetAmount(string DocNo)
        {
            string amount = "";
            string page = "StaffClaimCard?$select=Total_Net_Amount&$filter=No eq '" + DocNo + "'&format=json";
            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        amount = Convert.ToDecimal((string)config["Total_Net_Amount"]).ToString("#,##0.00");
                    }
                }
            }
            return amount;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult EditPaymentRequestLine(string LnNo, string DocNo)
        {
            try
            {
                int ln = Convert.ToInt32(LnNo);
                #region Payment Request Lines
                PaymentRequestLines claimLine = new PaymentRequestLines();
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
                List<PaymentRequestTypes> ClaimTList = new List<PaymentRequestTypes>();
                string page = "StaffClaimTypes?$filter=Type eq 'Claim' and Description ne ''&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        PaymentRequestTypes ClaimList = new PaymentRequestTypes();
                        ClaimList.Code = (string)config["Code"];
                        ClaimList.Description = (string)config["Description"];
                        ClaimTList.Add(ClaimList);
                    }
                }
                #endregion
                PaymentRequestItemDetails paymentRequest = new PaymentRequestItemDetails
                {
                    ItemDetails = claimLine,
                    ListOfPaymentRequestTypes = ClaimTList.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Description,
                                              Value = x.Code
                                          }).OrderBy(x => x.Text).ToList()
                };
                return PartialView("~/Views/PaymentRequest/PaymentReqEditItemForm.cshtml", paymentRequest);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public PartialViewResult FileUploadForm()
        {
            return PartialView("~/Views/PaymentRequest/FileAttachmentForm.cshtml");
        }
    }
}