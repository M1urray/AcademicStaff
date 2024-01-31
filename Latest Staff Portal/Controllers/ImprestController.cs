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
    [CustomAuthorization(Role = "FULLTIME")]
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
                List<ImprestList> ImpList = new List<ImprestList>();
                if (Session["Username"] != null)
                {
                    string StaffNo = Session["Username"].ToString();
                    string page = "ImprestReq?$filter=Employee_No eq '" + StaffNo + "'&$format=json";
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
                else
                {
                    Error erroMsg = new Error();
                    erroMsg.Message = "Your session has expired. Log out and login again";
                    return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
                }
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
                    #region Project List
                    List<DimensionValues> ProjectList = new List<DimensionValues>();
                    string pageProj = "DimensionValues?$filter=Global_Dimension_No_ eq 4&$format=json";

                    HttpWebResponse httpResponseProj = Credentials.GetOdataData(pageProj);
                    using (var streamReader = new StreamReader(httpResponseProj.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);


                        foreach (JObject config in details["value"])
                        {
                            DimensionValues project = new DimensionValues();
                            project.Code = (string)config["Code"];
                            project.Name = (string)config["Name"];
                            ProjectList.Add(project);
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
                        ListOfProjects = ProjectList.Select(x =>
                                           new SelectListItem()
                                           {
                                               Text = x.Name,
                                               Value = x.Code
                                           }).ToList()
                    };
                    return View(NewImprest);
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
                if (Session["Username"] != null)
                {
                    ImprestTypesList imprestTypes = new ImprestTypesList();

                    #region Imprest Type List
                    List<ImprestTypes> ImprestTList = new List<ImprestTypes>();
                    string page = "ImprestTypes?$filter=Description ne ''&$format=json";

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
                else
                {
                    Error erroMsg = new Error();
                    erroMsg.Message = "Your session has expired. Log out and login again";
                    return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
                }
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
            try
            {
                string Redirect = "";
                if (Session["Username"] != null)
                {
                    string School = "", project = "";
                    if (imprestHeader.school != null)
                    {
                        School = imprestHeader.school;
                    }
                    if (imprestHeader.Project != null)
                    {
                        project = imprestHeader.Project;
                    }

                    string StaffNo = Session["Username"].ToString();
                    DateTime DateRequired = DateTime.ParseExact(imprestHeader.DateNeeded.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    string DocNo = "";// Credentials.ObjNav.ImprestRequisitionCreate(StaffNo, School, DateRequired,
                        //imprestHeader.Campus, imprestHeader.Department, imprestHeader.Remarks, imprestHeader.RespC, "", project);

                    Redirect = "/Imprest/ImprestDocumentView?DocNo=" + DocNo;
                    //foreach (var c in imprestLines)
                    //{
                    //    string item = c.Item.Trim();
                    //    string itemDesc = c.ItemDesc.Trim();
                    //    string amnt = c.Amount.Trim();
                    //    Credentials.ObjNav.ImprestRequisitionLinesCreate(DocNo, item, Convert.ToDecimal(amnt), StaffNo, imprestHeader.Campus, imprestHeader.Department, itemDesc);
                    //}
                    //successVal = true;
                    //Credentials.ObjNav.ImprestRequisitionApprovalRequest(DocNo);
                    //Session["SuccessMsg"] = "Imprest Requisition, Document No: " + DocNo + ", Submitted Successfully";
                    //if (base64Upload != "")
                    //{
                    //    string filePath = Server.MapPath("~/Uploads/" + fileName);
                    //    CommonClass.MoveUploadedFile(base64Upload, filePath, fileName);
                    //    string UploadFilePath = Credentials.fileUploadsPath + fileName;
                    //    if (CommonClass.IfFileExists(UploadFilePath))
                    //    {
                    //        string s = Credentials.UploadDocumentAttachment(DocNo, base64Upload, UploadFilePath, 70135469);
                    //        if (s == "SUCCESS")
                    //        {
                    //            Session["SuccessMsg"] = "Imprest Requisition, Document No: " + DocNo + ", Submitted Successfully and attachment File Uploaded Successfully";
                    //        }
                    //        else
                    //        {
                    //            Session["SuccessMsg"] = "Imprest Requisition, Document No: " + DocNo + ", Submitted Successfully but error encountered while uploading attachment" +
                    //                "Error encountered :" + s;
                    //        }
                    //    }
                    //}
                    Session["SuccessMsg"] = "Imprest Requisition, Document No: " + DocNo + ", created Successfully. Add line(s) and attachment(s) then send for approval";
                }
                else
                {
                    Redirect = "/Loin/Login";
                }
                return Json(new { message = Redirect, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
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

                    string page = "ImprestReq?$filter=No eq '" + DocNo + "'&$format=json";
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
                if (Session["Username"] != null)
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
                else
                {
                    Error erroMsg = new Error();
                    erroMsg.Message = "Your session has expired. Log out and login again";
                    return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public JsonResult SendImprestAppForApproval(string DocNo)
        {
            try
            {
                string Redirect = "";
                bool LogOut = false;
                if (Session["Username"] != null)
                {
                    Credentials.ObjNav.ImprestRequisitionApprovalRequest(DocNo);
                    Redirect = "Imprest Requisition send for approval Successfully";
                    LogOut = false;
                }
                else
                {
                    Redirect = "/Login/Login";
                    LogOut = true;
                }
                return Json(new { message = Redirect, success = true, LogOut }, JsonRequestBehavior.AllowGet);
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
                string Redirect = "";
                bool LogOut = false;
                if (Session["Username"] != null)
                {
                    Credentials.ObjNav.HRCanceImprestRequisition(DocNo);
                    Redirect = "Imprest Requisition approval cancelled Successfully";
                    LogOut = false;
                }
                else
                {
                    Redirect = "/Login/Login";
                    LogOut = true;
                }
                return Json(new { message = Redirect, success = true, LogOut }, JsonRequestBehavior.AllowGet);
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
            string page = "ImprestReq?$select=TotalNetAmount&$filter=No eq '" + DocNo + "'&$format=json";
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