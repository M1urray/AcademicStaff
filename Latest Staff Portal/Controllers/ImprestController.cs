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
    public class ImprestController : Controller
    {
        // GET: Imprest
        public ActionResult ImprestRequisitionList()
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
        public PartialViewResult ImprestRequisitionListPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<ImprestList> ImpList = new List<ImprestList>();

                string page = "ImprestReq?$filter=Employee_No eq '" + StaffNo + "'&format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        ImprestList ImList = new ImprestList();
                        ImList.No = (string)config["No"];
                        ImList.ReqDate = Convert.ToDateTime((string)config["Date"]).ToString("dd/MM/yyyy");
                        ImList.Purpose = (string)config["Purpose"];
                        ImList.Function = (string)config["FunctionName"];
                        ImList.BudgetCeter = (string)config["Department_Name"];
                        ImList.Status = (string)config["Status"];
                        ImpList.Add(ImList);
                    }
                }
                return PartialView("~/Views/Imprest/ImprestReqListView.cshtml", ImpList.OrderByDescending(x => x.No));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public ActionResult NewImprestRequest()
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
                    NewImprestRequisition NewImprest = new NewImprestRequisition();
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

                        NewImprest = new NewImprestRequisition
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
                        return View(NewImprest);
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
        public PartialViewResult NewImprestLine()
        {
            try
            {
                ImprestTypesList imprestTypes = new ImprestTypesList();

                #region Imprest Type List
                List<ImprestTypes> ImprestTList = new List<ImprestTypes>();
                string page = "ImprestTypes?$filter=Description ne ''&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        ImprestTypes impList = new ImprestTypes();
                        impList.Code = (string)config["Code"];
                        impList.Description = (string)config["Description"];
                        ImprestTList.Add(impList);
                    }
                }
                #endregion

                imprestTypes = new ImprestTypesList
                {
                    ListOfImprestTypes = ImprestTList.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Description,
                                              Value = x.Code
                                          }).OrderBy(x => x.Text).ToList()
                };

                return PartialView("~/Views/Imprest/ImprestItemForm.cshtml", imprestTypes);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitImprestRequisition(ImprestHeader imprestHeader)
        {
            bool successVal = false;
            try
            {
                string School = "";
                if (imprestHeader.school != null)
                {
                    School = imprestHeader.school;
                }

                string StaffNo = Session["Username"].ToString();
                string UserID = Session["Username"].ToString();

                DateTime DateRequired = DateTime.ParseExact(imprestHeader.DateNeeded.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string DocNo = Credentials.ObjNav.ImprestRequisitionCreate(StaffNo, imprestHeader.Campus, DateRequired, imprestHeader.Department, School,
                    imprestHeader.Remarks, imprestHeader.RespC, UserID, "", "");
                if (DocNo != "")
                {
                    string Redirect = "/Imprest/ImprestDocumentView?DocNo=" + DocNo;

                    Session["SuccessMsg"] = "Imprest Requisition, Document No: " + DocNo + ", created Successfully. Add line(s) and attachment(s) then send for approval";
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
        public ActionResult ImprestDocumentView(string DocNo)
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
                    #region Imp Header
                    ImprestHeader ImpDoc = new ImprestHeader();

                    string page = "ImprestReq?$filter=No eq '" + DocNo + "'&format=json";
                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        foreach (JObject config in details["value"])
                        {
                            ImpDoc.No = (string)config["No"];
                            ImpDoc.DateNeeded = Convert.ToDateTime((string)config["Date"]).ToString("dd/MM/yyyy");
                            ImpDoc.Remarks = (string)config["Purpose"];
                            //ImpDoc.school = (string)config["Function_Name"];
                            //ImpDoc.schoolName = (string)config["Function_Name"];
                            ImpDoc.CampusName = (string)config["FunctionName"];
                            ImpDoc.Campus = (string)config["GlobalDimension1Code"];
                            ImpDoc.DepartmentName = (string)config["Department_Name"];
                            ImpDoc.Department = (string)config["ShortcutDimension2Code"];
                            ImpDoc.RespC = (string)config["ResponsibilityCenter"];
                            ImpDoc.TotalAmount = Convert.ToDecimal((string)config["TotalNetAmount"]).ToString("#,##0.00");
                            ImpDoc.Status = (string)config["Status"];
                        }
                    }
                    #endregion
                    return View(ImpDoc);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult ImprestDocumentLines(string DocNo, string Status)
        {
            try
            {
                #region Imp Lines
                List<ImprestLines> ImpLines = new List<ImprestLines>();
                string pageLine = "ImprestLines?$filter=No eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        ImprestLines ImLine = new ImprestLines();
                        ImLine.DocNo = (string)config["No"];
                        ImLine.AdvanceType = (string)config["Advance_Type"];
                        ImLine.Item = (string)config["Account_No"];
                        ImLine.ItemDesc = (string)config["Account_Name"];
                        ImLine.ItemDesc2 = (string)config["Purpose"];
                        ImLine.LnNo = (string)config["Line_No"];
                        ImLine.Amount = Convert.ToDecimal((string)config["Amount"]).ToString("#,##0.00");
                        ImpLines.Add(ImLine);
                    }
                }
                #endregion
                ImprestLinesList Lines = new ImprestLinesList
                {
                    Status = Status,
                    ListOfImprestLines = ImpLines
                };
                return PartialView("~/Views/Imprest/ImprestDocumentLineView.cshtml", Lines);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public JsonResult SendImprestAppForApproval(string DocNo, string Redirect)
        {
            try
            {
                Credentials.ObjNav.ImprestRequisitionApprovalRequest(DocNo);
                if (Redirect == "Y")
                {
                    Session["SuccessMsg"] = "Imprest Requisition, Document No " + DocNo + " send for approval Successfully";
                }
                return Json(new { message = "Imprest Requisition,Document No " + DocNo + " send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CancelImprestAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.HRCanceImprestRequisition(DocNo);
                return Json(new { message = "Imprest Requisition approval cancelled Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateImprestHeader(string DocNo, ImprestHeader imprestHeader)
        {
            try
            {
                string School = "", Campus = "", Department = "", RespC = "", Remarks = "";
                if (imprestHeader.Campus != null)
                {
                    Campus = imprestHeader.Campus;
                }
                if (imprestHeader.school != null)
                {
                    School = imprestHeader.school;
                }
                if (imprestHeader.Department != null)
                {
                    Department = imprestHeader.Department;
                }
                if (imprestHeader.RespC != null)
                {
                    RespC = imprestHeader.RespC;
                }
                if (imprestHeader.Remarks != null)
                {
                    Remarks = imprestHeader.Remarks;
                }
                var DateRequired = DateTime.ParseExact(imprestHeader.DateNeeded.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                Credentials.ObjNav.UpdateImprestHeader(DocNo.Trim(), DateRequired, Campus, Department,
                    School, RespC, Remarks);
                return Json(new { message = "Imprest header Updated successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitImprestLine(string DocNo, ImprestHeader imprestHeader, ImprestLines imprestLine)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                string item = imprestLine.Item.Trim();
                string itemDesc = imprestLine.ItemDesc.Trim();
                string amnt = imprestLine.Amount.Trim();
                Credentials.ObjNav.ImprestRequisitionLinesCreate(DocNo, item, Convert.ToDecimal(amnt), StaffNo, imprestHeader.Campus, imprestHeader.Department, itemDesc);
                string DocNetAmount = GetImpDocNetAmount(DocNo);
                return Json(new { NetAmout = DocNetAmount, message = "Imprest Line Added successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateImprestLine(string DocNo, string LnNo, ImprestLines imprestLine)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                string itemDesc = imprestLine.ItemDesc.Trim();
                string amnt = imprestLine.Amount.Trim();
                Credentials.ObjNav.ImprestRequistionLineUpdate(DocNo, Convert.ToInt32(LnNo), Convert.ToDecimal(amnt), itemDesc);
                string DocNetAmount = GetImpDocNetAmount(DocNo);
                return Json(new { NetAmout = DocNetAmount, message = "Imprest Line updated successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult EditImprestLine(string LnNo, string DocNo)
        {
            try
            {
                int ln = Convert.ToInt32(LnNo);
                #region Imprest Lines
                ImprestLines ImLine = new ImprestLines();
                string pageLine = "ImprestLines?$filter=No eq '" + DocNo + "' and Line_No eq " + ln + "&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        ImLine.DocNo = (string)config["No"];
                        ImLine.AdvanceType = (string)config["Advance_Type"];
                        ImLine.Item = (string)config["Account_No"];
                        ImLine.ItemDesc = (string)config["Account_Name"];
                        ImLine.ItemDesc2 = (string)config["Purpose"];
                        ImLine.LnNo = (string)config["Line_No"];
                        ImLine.Amount = Convert.ToDecimal((string)config["Amount"]).ToString("#,##0.00");
                    }
                }
                #endregion
                #region Imprest Type List
                List<ImprestTypes> ImprestTList = new List<ImprestTypes>();
                string page = "ImprestTypes?$filter=Description ne ''&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        ImprestTypes impList = new ImprestTypes();
                        impList.Code = (string)config["Code"];
                        impList.Description = (string)config["Description"];
                        ImprestTList.Add(impList);
                    }
                }
                #endregion
                ImprestItemDetails itemDetails = new ImprestItemDetails
                {
                    ItemDetails = ImLine,
                    ListOfImprestTypes = ImprestTList.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Description,
                                              Value = x.Code
                                          }).ToList()
                };
                return PartialView("~/Views/Imprest/ImprestEditItemForm.cshtml", itemDetails);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RemoveImprestLine(string DocNo, string LnNo)
        {
            try
            {
                Credentials.ObjNav.ImprestRequsitionRemoveLine(Convert.ToInt32(LnNo), DocNo);
                string DocNetAmount = GetImpDocNetAmount(DocNo);
                return Json(new { NetAmout = DocNetAmount, message = "Imprest Line removed successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        protected string GetImpDocNetAmount(string DocNo)
        {
            string amount = "";
            string page = "ImprestReq?$select=TotalNetAmount&$filter=No eq '" + DocNo + "'&format=json";
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
            return PartialView("~/Views/Imprest/FileAttachmentForm.cshtml");
        }
    }
}