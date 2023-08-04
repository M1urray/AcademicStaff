using Latest_Staff_Portal.CustomSecurity;
using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.Controllers
{
    [CustomeAuthentication]
    [CustomAuthorization(Role = "ALLUSERS")]
    public class PurchaseController : Controller
    {
        // GET: Purchase
        public ActionResult PurchaseRequisitionList()
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
        public PartialViewResult PurchaseRequisitionListPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<PurchaseReqList> PurchaseList = new List<PurchaseReqList>();

                string page = "PurchaseRequisition?$filter=Employee_No_ eq '" + StaffNo + "'&$format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        PurchaseReqList PRVList = new PurchaseReqList();
                        PRVList.No = (string)config["No_"];
                        PRVList.OrderDate = Convert.ToDateTime((string)config["Order_Date"]).ToString("dd/MM/yyyy");
                        PRVList.Description = (string)config["Posting_Description"];
                        if ((string)config["Status"] == "Released" || (string)config["Status"] == "Posted")
                        {
                            PRVList.Status = "Approved";
                        }
                        else
                        {
                            PRVList.Status = (string)config["Status"];
                        }
                        PurchaseList.Add(PRVList);
                    }
                }
                return PartialView("~/Views/Purchase/PRVReqListView.cshtml", PurchaseList.OrderByDescending(x => x.No));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public ActionResult NewPurchaseRequest()
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
                    string Dir = "", Dep = "";
                    bool ResRegardDirectorate = false;
                    NewPurchaseRequisition NewPRV = new NewPurchaseRequisition();
                    Session["httpResponse"] = null;

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

                        NewPRV = new NewPurchaseRequisition
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
                        return View("~/Views/Purchase/NewPurchaseRequest.cshtml", NewPRV);
                    }
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewPurchaseLine()
        {
            try
            {
                LocationList locationList = new LocationList();

                #region Location List
                List<Locations> Locations = new List<Locations>();
                string page = "Locations?$filter=Name ne ''&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        Locations LocList = new Locations();
                        LocList.Code = (string)config["Code"];
                        LocList.Name = (string)config["Name"];
                        Locations.Add(LocList);
                    }
                }
                #endregion

                locationList = new LocationList
                {
                    ListOfLocations = Locations.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Name,
                                              Value = x.Code
                                          }).ToList()
                };
                return PartialView("~/Views/Purchase/PRVItemForm.cshtml", locationList);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitPurchaseRequisition(PRVHeader prvHeader)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                string Directorate = "", section = "", respC = "", userID = "";
                if (prvHeader.Directorate != null)
                {
                    Directorate = prvHeader.Directorate;
                }
                if (prvHeader.RespC != null)
                {
                    respC = prvHeader.RespC;
                }
                if (Session["UserID"] != null)
                {
                    userID = Session["UserID"].ToString();
                }
                string DocNo = Credentials.ObjNav.PurchaseRequisitionCreate(StaffNo, Directorate, prvHeader.Department, section, prvHeader.Remarks, respC, userID);

                if (DocNo != "")
                {
                    string Redirect = "/Purchase/PurchaseDocumentView?DocNo=" + DocNo;

                    Session["SuccessMsg"] = "Purchase Requisition, Document No: " + DocNo + ", created Successfully. Add line(s) and attachment(s) then send for approval";
                    return Json(new { message = Redirect, success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "Document not created. Please try again later...", success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PurchaseDocumentView(string DocNo)
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
                    #region Purchase Header
                    PRVHeader PurchaseDoc = new PRVHeader();

                    #region Directorate List
                    List<DimensionValues> DirectorateList = new List<DimensionValues>();
                    string pageDepartment = "DimensionValues?$filter=Dimension_Code eq 'BRANCH'&format=json";

                    HttpWebResponse httpResponseDepartment = Credentials.GetOdataData(pageDepartment);
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
                    string pageDivision = "DimensionValues?$filter=Dimension_Code eq 'DEPARTMENT'&format=json";

                    HttpWebResponse httpResponseDivision = Credentials.GetOdataData(pageDivision);
                    using (var streamReader = new StreamReader(httpResponseDivision.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);


                        foreach (JObject config in details["value"])
                        {
                            DimensionValues DList = new DimensionValues();
                            DList.Code = (string)config["Code"];
                            DList.Name = (string)config["Name"];
                            DepartmentList.Add(DList);
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
                    string page = "PurchaseRequisition?$filter=No_ eq '" + DocNo + "'&$format=json";
                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        foreach (JObject config in details["value"])
                        {
                            PurchaseDoc.No = (string)config["No_"];
                            PurchaseDoc.Date = Convert.ToDateTime((string)config["Order_Date"]).ToString("dd/MM/yyyy");
                            PurchaseDoc.Remarks = (string)config["Posting_Description"];
                            PurchaseDoc.Directorate = (string)config["Shortcut_Dimension_1_Code"];
                            PurchaseDoc.Department = (string)config["Shortcut_Dimension_2_Code"];
                            //PurchaseDoc.Section = (string)config["Shortcut_Dimension_3_Code"];
                            PurchaseDoc.RespC = (string)config["Responsibility_Center"];
                            PurchaseDoc.DisDir = CommonClass.DisregardDirectorate(StaffNo);
                            if ((string)config["Status"] == "Released" || (string)config["Status"] == "Posted")
                            {
                                PurchaseDoc.Status = "Approved";
                            }
                            else
                            {
                                PurchaseDoc.Status = (string)config["Status"];
                            }
                        }
                    }
                    PurchaseDoc.ListOfDirectorate = DirectorateList.Select(x =>
                                           new SelectListItem()
                                           {
                                               Text = x.Name,
                                               Value = x.Code
                                           }).ToList();
                    PurchaseDoc.ListOfDepartment = DepartmentList.Select(x =>
                                  new SelectListItem()
                                  {
                                      Text = x.Name,
                                      Value = x.Code
                                  }).ToList();
                    PurchaseDoc.ListOfResponsibility = RespCList.Select(x =>
                                  new SelectListItem()
                                  {
                                      Text = x.Name,
                                      Value = x.Code
                                  }).ToList();
                    #endregion
                    return View(PurchaseDoc);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult PurchaseDocumentLines(string DocNo, string Status)
        {
            try
            {
                #region Purchase Lines
                List<PRVLines> PurchaseLines = new List<PRVLines>();
                string pageLine = "PurchaseLines?$filter=Document_No_ eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        PRVLines PurchaseLine = new PRVLines();
                        if ((string)config["Type"] == "G/L Account")
                        {
                            PurchaseLine.LineType = "Service";
                        }
                        else
                        {
                            PurchaseLine.LineType = (string)config["Type"];
                        }
                        PurchaseLine.DocNo = (string)config["Document_No_"];
                        PurchaseLine.Item = (string)config["No_"];
                        PurchaseLine.ItemDesc = (string)config["Description"];
                        PurchaseLine.Description2 = (string)config["Description_2"];
                        PurchaseLine.Qnty = (string)config["Quantity"];
                        PurchaseLine.UnitM = (string)config["Unit_of_Measure"];
                        PurchaseLine.Amount = (string)config["Direct_Unit_Cost"];
                        PurchaseLine.LineAmount = (string)config["Line_Amount"];
                        PurchaseLine.Location = (string)config["Location_Code"];
                        PurchaseLine.LnNo = (string)config["Line_No_"];
                        PurchaseLines.Add(PurchaseLine);
                    }
                }
                #endregion
                PurchaseLinesList Lines = new PurchaseLinesList
                {
                    Status = Status,
                    ListOfPurchaseLines = PurchaseLines
                };
                return PartialView("~/Views/Purchase/PurchaseDocumentLineView.cshtml", Lines);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public JsonResult SendPurchaseAppForApproval(string DocNo, string Redirect)
        {
            try
            {
                Credentials.ObjNav.PurchaseRequisitionApprovalRequest(DocNo);
                if (Redirect == "Y")
                {
                    Session["SuccessMsg"] = "Purchase Requisition, Document No " + DocNo + " send for approval Successfully";
                }
                return Json(new { message = "Purchase Requisition, Document No " + DocNo + " send for approval Successfully",  success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CancelPurchaseAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.HRCancelPurchaseRequisition(DocNo);
                return Json(new { message = "Purchase Requisition approval cancelled Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdatePurchaseHeader(string DocNo, PRVHeader prvHeader)
        {
            try
            {
                string Directorate = "", Department = "", Section = "", RespC = "", Remarks = "";

                if (prvHeader.Directorate != null)
                {
                    Directorate = prvHeader.Directorate;
                }
                if (prvHeader.Department != null)
                {
                    Department = prvHeader.Department;
                }
                //if (prvHeader.Section != null)
                //{
                //    Section = prvHeader.Section;
                //}
                if (prvHeader.RespC != null)
                {
                    RespC = prvHeader.RespC;
                }
                if (prvHeader.Remarks != null)
                {
                    Remarks = prvHeader.Remarks;
                }
                Credentials.ObjNav.UpdatePurchaseRequisition(DocNo.Trim(), Directorate, Department, Section,
                     RespC, Remarks);
                return Json(new { message = "Purchase header Updated successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult EditPurchaseLine(string LnNo, string DocNo)
        {
            try
            {
                int ln = Convert.ToInt32(LnNo);
                #region Purchase Line
                PRVLines PurchaseLine = new PRVLines();
                string pageLine = "PurchaseLines?$filter=Document_No_ eq '" + DocNo + "' and Line_No_ eq " + ln + "&format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        if ((string)config["Type"] == "G/L Account")
                        {
                            PurchaseLine.LineType = "Service";
                        }
                        else
                        {
                            PurchaseLine.LineType = (string)config["Type"];
                        }
                        PurchaseLine.DocNo = (string)config["Document_No_"];
                        PurchaseLine.Item = (string)config["No_"];
                        PurchaseLine.ItemDesc = (string)config["Description"];
                        PurchaseLine.Description2 = (string)config["Description_2"];
                        PurchaseLine.Qnty = (string)config["Quantity"];
                        PurchaseLine.UnitM = (string)config["Unit_of_Measure"];
                        PurchaseLine.Amount = (string)config["Direct_Unit_Cost"];
                        PurchaseLine.Location = (string)config["Location_Code"];
                        PurchaseLine.LnNo = (string)config["Line_No_"];
                    }
                }
                #endregion
                #region Location List
                List<Locations> Locations = new List<Locations>();
                string page = "Locations?$filter=Name ne ''&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        Locations LocList = new Locations();
                        LocList.Code = (string)config["Code"];
                        LocList.Name = (string)config["Name"];
                        Locations.Add(LocList);
                    }
                }
                #endregion

                PurchaseItemDetails itemDetails = new PurchaseItemDetails
                {
                    ItemDetails = PurchaseLine,
                    ListOfLocations = Locations.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Name,
                                              Value = x.Code
                                          }).ToList()
                };
                return PartialView("~/Views/Purchase/PRVItemEditForm.cshtml", itemDetails);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitPurchaseLine(string DocNo, PRVLines prvLine)
        {
            try
            {
                int Type = 0;
                if (prvLine.LineType.Trim() == "Service")
                {
                    Type = 1;
                }
                if (prvLine.LineType.Trim() == "Item")
                {
                    Type = 2;
                }
                if (prvLine.LineType.Trim() == "Asset")
                {
                    Type = 4;
                }
                string item = prvLine.Item.Trim();
                string itemDesc = "";
                if (prvLine.ItemDesc != null && prvLine.ItemDesc != "")
                {
                    itemDesc = prvLine.ItemDesc.Trim();
                }
                string qnty = prvLine.Qnty.Trim();
                string amnt = prvLine.Amount.Trim();
                string location = "";
                if (prvLine.Location != null && prvLine.Location != "")
                {
                    location = prvLine.Location.Trim();
                }
                Credentials.ObjNav.PurchaseRequisitionLines(DocNo, item, Convert.ToDecimal(qnty), Convert.ToDecimal(amnt), itemDesc, Type, location);

                return Json(new { message = "Purchase Line Added successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        protected int GetDocumentCount(string DocNo)
        {
            int count = 0;
            string pageLine = "PurchaseLines?$select=Line_No_&$orderby=Line_No_ desc&$top=1&$filter=Document_No_ eq '" + DocNo + "'&$format=json";
            HttpWebResponse httpResponse = Credentials.GetOdataData(pageLine);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        count = (int)config["Line_No_"];
                    }
                }
            }
            return count;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdatePurchaseLine(PRVLines prvLine)
        {
            try
            {
                string desc = "";
                if (prvLine.Description2 != null)
                {
                    desc = prvLine.Description2;
                }
                Credentials.ObjNav.PurchaseRequistionLineUpdate(Convert.ToInt32(prvLine.LnNo), Convert.ToInt32(prvLine.Qnty), Convert.ToInt32(prvLine.Amount), prvLine.DocNo, desc);
                return Json(new { message = "Purchase Line Updated successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RemovePurchaseLine(string DocNo, string LnNo)
        {
            try
            {
                Credentials.ObjNav.PurchaseRequsitionRemoveLine(Convert.ToInt32(LnNo), DocNo);
                return Json(new { message = "Purchase Line Deleted successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public PartialViewResult FileUploadForm()
        {
            return PartialView("~/Views/Purchase/FileAttachmentForm.cshtml");
        }
    }
}