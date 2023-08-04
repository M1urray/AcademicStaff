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
using iTextSharp.text;

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
                erroMsg.Message = ex.Message.Replace("'", "");
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult ImprestRequisitionListPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<ImprestList> ImpList = new List<ImprestList>();

                string page = "ImprestReq?$filter=AccountNo eq '" + StaffNo + "'&$format=json";
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
                        if ((string)config["Status"] == "Pending")
                        {
                            ImList.Status = "Open";
                        }
                        else
                        {
                            ImList.Status = (string)config["Status"];
                        }
                        ImpList.Add(ImList);
                    }
                }
                return PartialView("~/Views/Imprest/ImprestReqListView.cshtml", ImpList.OrderByDescending(x => x.No));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
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
                                                 }).ToList(),
                            ListOfResponsibility = RespCList.Select(x =>
                                               new SelectListItem()
                                               {
                                                   Text = x.Name,
                                                   Value = x.Code
                                               }).ToList()
                        };
                        return View("~/Views/Imprest/NewImprestRequest.cshtml", NewImprest);
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
                #region UoM
                List<DropdownList> UoMList = new List<DropdownList>();
                string pageUoM = "UnitOfMeasure?$format=json";

                HttpWebResponse httpResponseUoM = Credentials.GetOdataData(pageUoM);
                using (var streamReader = new StreamReader(httpResponseUoM.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList UoM = new DropdownList();
                        UoM.Value = (string)config["Code"];
                        UoM.Text = (string)config["Description"];
                        UoMList.Add(UoM);
                    }
                }
                #endregion
                #region Travel Destinations

                List<TravelDestination> travelDestinations = new List<TravelDestination>();
                string pageTravel = "DestinationList?$format=json";
                HttpWebResponse httpResponseTravel = Credentials.GetOdataData(pageTravel);
                using (var streamReaderT = new StreamReader(httpResponseTravel.GetResponseStream()))
                {
                    var resultTravel = streamReaderT.ReadToEnd();

                    var detailsTravel = JObject.Parse(resultTravel);

                    foreach (JObject config in detailsTravel["value"])
                    {
                        TravelDestination travel = new TravelDestination();
                        travel.Code = (string)config["DestinationCode"];
                        travel.Description = (string)config["DestinationName"];
                        travelDestinations.Add(travel);
                    }
                }
                #endregion
                #region PerDiem
                List<PerDiem> imprestTList2 = new List<PerDiem>();
                string page2 = "ImprestTypes?$filter=Code eq 'PERDIEM' &$format=json";

                HttpWebResponse httpResponse2 = Credentials.GetOdataData(page2);
                using (var streamReader = new StreamReader(httpResponse2.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        PerDiem impList2 = new PerDiem();
                        impList2.Code = (string)config["Code"];
                        impList2.Description = (string)config["Description"];
                        imprestTList2.Add(impList2);
                    }
                }
                #endregion
                string staffNo = Session["Username"].ToString();
                string dir = "";
                #region Employee Data
                string pageData = "EmployeeList?$filter=No eq '" + staffNo + "'&$format=json";

                HttpWebResponse httpResponseEmp = Credentials.GetOdataData(pageData);
                using (var streamReader = new StreamReader(httpResponseEmp.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    if (details["value"].Count() > 0)
                    {
                        foreach (JObject config in details["value"])
                        {
                            dir = (string)config["Grade"];
                        }
                    }
                }
                #endregion
                imprestTypes = new ImprestTypesList
                {
                    ListOfPerDiem=imprestTList2.Select(x =>
                        new SelectListItem()
                        {
                            Text = x.Description,
                            Value = x.Code
                        }).OrderBy(x => x.Text).ToList(),
                    ListOfImprestTypes = ImprestTList.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Description,
                                              Value = x.Code
                                          }).OrderBy(x => x.Text).ToList(),
                    ListOfUnitMeasure = UoMList.Select(x =>
                                               new SelectListItem()
                                               {
                                                   Text = x.Text,
                                                   Value = x.Value
                                               }).ToList(),
                    ListOfDestination = travelDestinations.Select(x =>
                        new SelectListItem()
                        {
                            Text = x.Description,
                            Value = x.Code
                        }).ToList(),
                };

                return PartialView("~/Views/Imprest/ImprestItemForm.cshtml", imprestTypes);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubmitImprestRequisition(ImprestHeader imprestHeader)
        {
            bool successVal = false;
            try
            {
                string Directorate = "", section = "";

                if (Session["UserID"] == null || Session["Username"] == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    if (imprestHeader.Directorate != null)
                    {
                        Directorate = imprestHeader.Directorate;
                    }
                    string StaffNo = Session["Username"].ToString();
                    string UserID = Session["UserID"].ToString();
                    DateTime DateRequired = DateTime.ParseExact(imprestHeader.DateNeeded.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    string DocNo = Credentials.ObjNav.ImprestRequisitionCreate(StaffNo, DateRequired, Directorate, imprestHeader.Department, section, "", "", "", imprestHeader.Remarks, UserID,"", DateRequired, DateRequired);
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
                    #region Destinations
                    List<DropdownList> DestList = new List<DropdownList>();
                    string pageDest = "DestinationList?$format=json";

                    HttpWebResponse httpResponseDest = Credentials.GetOdataData(pageDest);
                    using (var streamReader = new StreamReader(httpResponseDest.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);


                        foreach (JObject config in details["value"])
                        {
                            DropdownList Dest = new DropdownList();
                            Dest.Value = (string)config["DestinationCode"];
                            Dest.Text = (string)config["DestinationName"];
                            DestList.Add(Dest);
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
                            ImpDoc.Directorate = (string)config["GlobalDimension1Code"];
                            ImpDoc.Department = (string)config["ShortcutDimension2Code"];
                            ImpDoc.DisDir = CommonClass.DisregardDirectorate(StaffNo);
                            ImpDoc.TotalAmount = Convert.ToDecimal((string)config["TotalNetAmount"]).ToString("#,##0.00");
                            if ((string)config["Status"] == "Pending")
                            {
                                ImpDoc.Status = "Open";
                            }
                            else
                            {
                                ImpDoc.Status = (string)config["Status"];
                            }
                        }
                    }
                    ImpDoc.ListOfDirectorate = DirectorateList.Select(x =>
                                           new SelectListItem()
                                           {
                                               Text = x.Name,
                                               Value = x.Code
                                           }).ToList();
                    ImpDoc.ListOfDepartment = DepartmentList.Select(x =>
                                  new SelectListItem()
                                  {
                                      Text = x.Name,
                                      Value = x.Code
                                  }).ToList();
                    ImpDoc.ListOfResponsibility = RespCList.Select(x =>
                                  new SelectListItem()
                                  {
                                      Text = x.Name,
                                      Value = x.Code
                                  }).ToList();
                    #endregion
                    return View(ImpDoc);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
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
                        ImLine.UoN = (string)config["Unit_of_Measure"];
                        ImLine.NoDays = (string)config["No_of_Days"];
                        ImLine.Quantity = (string)config["Quantity"];
                        ImLine.UnitAmount = Convert.ToDecimal((string)config["Daily_Rate_Amount"]).ToString("#,##0.00");
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
                erroMsg.Message = ex.Message.Replace("'", "");
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
                return Json(new { message = "Imprest Requisition, Document No " + DocNo + " send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
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
                string Dir = "", Dep = "", Section = "", Remarks = "";
                if (imprestHeader.Directorate != null)
                {
                    Dir = imprestHeader.Directorate;
                }
                if (imprestHeader.Department != null)
                {
                    Dep = imprestHeader.Department;
                }
                if (imprestHeader.Remarks != null)
                {
                    Remarks = imprestHeader.Remarks;
                }
                var DateRequired = DateTime.ParseExact(imprestHeader.DateNeeded.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                Credentials.ObjNav.UpdateImprestHeader(DocNo.Trim(), DateRequired, Dir, Dep, Section, "", "", "", Remarks);
                return Json(new { message = "Imprest header Updated successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitImprestLine(string DocNo, ImprestLines imprestLine)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                string item = imprestLine.Item.Trim();
                string itemDesc = imprestLine.ItemDesc.Trim();
                string amnt = imprestLine.Amount.Trim();
                //   string noofdays = imprestLine.Quantity.Trim();

                string UoN = "", Destination = "";
                if (!string.IsNullOrEmpty(imprestLine.UoN))
                {
                    UoN = imprestLine.UoN;
                }
                if (!string.IsNullOrEmpty(imprestLine.Destination))
                {
                    Destination = imprestLine.Destination;
                }
                if (Destination == "")
                {
                    Credentials.ObjNav.ImprestRequisitionLinesCreate(DocNo, item, Convert.ToDecimal(amnt), StaffNo,
                        Convert.ToDecimal(imprestLine.Quantity), UoN, itemDesc, Destination, Convert.ToInt32(imprestLine.Quantity), "");
                }
                else
                {
                    Credentials.ObjNav.ImprestRequisitionLinesCreate(DocNo, item,0, StaffNo,
                        1, UoN, itemDesc, Destination, Convert.ToInt32(imprestLine.Quantity), "");

                }
                string DocNetAmount = GetImpDocNetAmount(DocNo);
                return Json(new { NetAmout = DocNetAmount, message = "Imprest Line Added successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateImprestLine(string DocNo, ImprestLines imprestLine)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                string item = imprestLine.Item.Trim();
                string itemDesc = imprestLine.ItemDesc.Trim();
                string amnt = imprestLine.Amount.Trim();
                int LnNo = Convert.ToInt32(imprestLine.LnNo.Trim());
                Credentials.ObjNav.ImprestRequisitionLinesUpdate(DocNo, LnNo, Convert.ToDecimal(amnt), 1, itemDesc, Convert.ToInt32(imprestLine.Quantity));
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
                        ImLine.UoN = (string)config["Unit_of_Measure"];
                        ImLine.Quantity = (string)config["Quantity"];
                        ImLine.UnitAmount = Convert.ToDecimal((string)config["Daily_Rate_Amount"]).ToString("#,##0.00");
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
                #region UoM
                List<DropdownList> UoMList = new List<DropdownList>();
                string pageUoM = "UnitOfMeasure?$format=json";

                HttpWebResponse httpResponseUoM = Credentials.GetOdataData(pageUoM);
                using (var streamReader = new StreamReader(httpResponseUoM.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList UoM = new DropdownList();
                        UoM.Value = (string)config["Code"];
                        UoM.Text = (string)config["Description"];
                        UoMList.Add(UoM);
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
                                          }).ToList(),
                    ListOfUoM = UoMList.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Text,
                                              Value = x.Value
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