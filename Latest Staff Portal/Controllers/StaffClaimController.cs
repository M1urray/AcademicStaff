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
                erroMsg.Message = ex.Message;
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
                        CList.RespC = (string)config["Responsibility_Center"];
                        CList.Status = (string)config["Status"];
                        ClaimList.Add(CList);
                    }
                }
                return PartialView("~/Views/StaffClaim/StaffClaimReqListView.cshtml", ClaimList.OrderByDescending(x => x.No));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
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

                        NewStaffClaim = new NewStaffClaimRequisition
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
                                               }).ToList()
                        };
                        return View(NewStaffClaim);
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
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        // [AcceptVerbs(HttpVerbs.Post)]
        // public JsonResult SubmitStaffClaimRequisition(StaffClaimHeader staffClaimHeader)
        // {
        //     bool successVal = false;
        //     try
        //     {
        //         string School = "";
        //         if (staffClaimHeader.school != null)
        //         {
        //             School = staffClaimHeader.school;
        //         }
        //         string StaffNo = Session["Username"].ToString();
        //         string DocNo = Credentials.ObjNav.InsertStaffClaims(StaffNo, staffClaimHeader.Campus, staffClaimHeader.Department
        //                           , staffClaimHeader.RespC, staffClaimHeader.Remarks, School,"",0);
        //
        //         if (DocNo != "")
        //         {
        //             string Redirect = "/StaffClaim/StaffClaimDocumentView?DocNo=" + DocNo;
        //
        //             Session["SuccessMsg"] = "Staff Claim Requisition, Document No: " + DocNo + ", created Successfully. Add line(s) and attachment(s) then send for approval";
        //             return Json(new { message = Redirect, success = true }, JsonRequestBehavior.AllowGet);
        //         }
        //         else
        //         {
        //             return Json(new { message = "Document not created. Please try again later...", success = false }, JsonRequestBehavior.AllowGet);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         if (successVal)
        //         {
        //             Session["ErrorMsg"] = ex.Message.Replace("'", "");
        //         }
        //         return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
        //     }
        // }
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
                erroMsg.Message = ex.Message;
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
                return Json(new { message = "Staff Claim Requisition,Document No " + DocNo + " send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
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
                string School = "", Campus = "", Department = "", RespC = "", Remarks = "";
                if (staffClaimHeader.Campus != null)
                {
                    Campus = staffClaimHeader.Campus;
                }
                if (staffClaimHeader.school != null)
                {
                    School = staffClaimHeader.school;
                }
                if (staffClaimHeader.Department != null)
                {
                    Department = staffClaimHeader.Department;
                }
                if (staffClaimHeader.RespC != null)
                {
                    RespC = staffClaimHeader.RespC;
                }
                if (staffClaimHeader.Remarks != null)
                {
                    Remarks = staffClaimHeader.Remarks;
                }
                Credentials.ObjNav.UpdateStaffClaims(DocNo.Trim(), DateTime.Today, Campus, Department,
                    School, RespC, Remarks);
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
                string School = "";
                string StaffNo = Session["Username"].ToString();
                string item = staffClaimLine.Item.Trim();
                string itemDesc = staffClaimLine.ItemDesc.Trim();
                string amnt = staffClaimLine.Amount.Trim();

                Credentials.ObjNav.StaffClaimRequisitionLinesInsert(DocNo, item, Convert.ToDecimal(amnt), StaffNo, staffClaimHeader.Campus,
                    staffClaimHeader.Department, itemDesc, School);
                string DocNetAmount = GetDocNetAmount(DocNo);
                return Json(new { NetAmout = DocNetAmount, message = "Claim Line Added successfully", success = true }, JsonRequestBehavior.AllowGet);
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
                return Json(new { NetAmout = DocNetAmount, message = "Claim Line Deleted successfully", success = true }, JsonRequestBehavior.AllowGet);
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
        [AcceptVerbs(HttpVerbs.Get)]
        public PartialViewResult FileUploadForm()
        {
            return PartialView("~/Views/StaffClaim/FileAttachmentForm.cshtml");
        }
    }
}