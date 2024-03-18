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

                string page = "PurchaseRegDocument?$filter=Employee_No eq '" + StaffNo + "'&format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        PurchaseReqList PRVList = new PurchaseReqList();
                        PRVList.No = (string)config["No"];
                        PRVList.OrderDate = Convert.ToDateTime((string)config["Order_Date"]).ToString("dd/MM/yyyy");
                        PRVList.Description = (string)config["Posting_Description"];
                        PRVList.Status = (string)config["Status"];
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

                        NewPurchaseRequisition NewPRV = new NewPurchaseRequisition();
                        Session["httpResponse"] = null;
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

                        NewPRV = new NewPurchaseRequisition
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
                        return View(NewPRV);
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
        //[AcceptVerbs(HttpVerbs.Post)]
        //public JsonResult SubmitPurchaseRequisition(PRVHeader prvHeader)
        //{
        //    try
        //    {
        //        string StaffNo = Session["Username"].ToString();
        //        string DocNo = Credentials.ObjNav.PurchaseRequisitionCreate(StaffNo, prvHeader.Campus, prvHeader.Department, prvHeader.Remarks, prvHeader.RespC, "");

        //        string Redirect = "/Purchase/PurchaseDocumentView?DocNo=" + DocNo;               

        //        Session["SuccessMsg"] = "Purchase Requisition, Document No: " + DocNo + ", created Successfully. Add line(s) and attachment(s) then send for approval";
        //        return Json(new { message = Redirect, success = true }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
        //    }
        //}
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

                    string page = "PurchaseRegDocument?$filter=No eq '" + DocNo + "'&format=json";
                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        foreach (JObject config in details["value"])
                        {
                            PurchaseDoc.No = (string)config["No"];
                            PurchaseDoc.Date = Convert.ToDateTime((string)config["Order_Date"]).ToString("dd/MM/yyyy");
                            PurchaseDoc.Remarks = (string)config["Posting_Description"];
                            PurchaseDoc.Campus = (string)config["Shortcut_Dimension_1_Code"];
                            PurchaseDoc.CampusName = CommonClass.GetDimensionValue((string)config["Shortcut_Dimension_1_Code"]);
                            PurchaseDoc.Department = (string)config["Shortcut_Dimension_2_Code"];
                            PurchaseDoc.DepartmentName = (string)config["Department_Name"];
                            PurchaseDoc.RespC = (string)config["Responsibility_Center_BR"];
                            PurchaseDoc.Status = (string)config["Status"];
                            //string comment = CommonClass.RequisitionApprovalLevel(DocNo);
                        }
                    }
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
                decimal TotalAmount = 0;
                #region Purchase Lines
                List<PRVLines> PurchaseLines = new List<PRVLines>();
                string pageLine = "PurchaseLines?$filter=Document_No eq '" + DocNo + "'&$format=json";
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
                        PurchaseLine.DocNo = (string)config["Document_No"];
                        PurchaseLine.Item = (string)config["No"];
                        PurchaseLine.ItemDesc = (string)config["Description"];
                        PurchaseLine.Description2 = (string)config["Description_2"];
                        PurchaseLine.Qnty = (string)config["Quantity"];
                        PurchaseLine.UnitM = (string)config["Unit_of_Measure"];                       
                        PurchaseLine.Amount = Convert.ToDecimal((string)config["Direct_Unit_Cost"]).ToString("#,##0.00"); 
                        PurchaseLine.LineAmount = Convert.ToDecimal((string)config["Line_Amount"]).ToString("#,##0.00"); 
                        PurchaseLine.Location = (string)config["Location_Code"];
                        PurchaseLine.LnNo = (string)config["Line_No"];
                        PurchaseLines.Add(PurchaseLine);
                        TotalAmount = TotalAmount + (decimal)config["Direct_Unit_Cost"];
                    }
                }
                #endregion
                PurchaseLinesList Lines = new PurchaseLinesList
                {
                    Status = Status,
                    ListOfPurchaseLines = PurchaseLines,
                    TotalAmount = TotalAmount.ToString("#,##0.00")
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
                Credentials.ObjNav.CustomPurchaseRequisitionApprovalRequest(DocNo);
                if (Redirect == "Y")
                {
                    Session["SuccessMsg"] = "Purchase Requisition, Document No " + DocNo + " send for approval Successfully";
                }
                return Json(new { message = "Purchase Requisition, Document No " + DocNo + " send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CancelPurchaseAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.CustomHRCancelPurchaseRequisition(DocNo);
                return Json(new { message = "Purchase Requisition approval cancelled Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdatePurchaseHeader(string DocNo, PRVHeader prvHeader)
        {
            try
            {
                string School = "", Campus = "", Department = "", RespC = "", Remarks = "";
                if (prvHeader.Campus != null)
                {
                    Campus = prvHeader.Campus;
                }
                if (prvHeader.Department != null)
                {
                    Department = prvHeader.Department;
                }
                if (prvHeader.RespC != null)
                {
                    RespC = prvHeader.RespC;
                }
                if (prvHeader.Remarks != null)
                {
                    Remarks = prvHeader.Remarks;
                }
                Credentials.ObjNav.UpdatePurchaseRequisition(DocNo.Trim(), Campus, Department,
                    School, RespC, Remarks);
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
                string pageLine = "PurchaseLines?$filter=Document_No eq '" + DocNo + "' and Line_No eq " + ln + "&$format=json";
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
                        PurchaseLine.DocNo = (string)config["Document_No"];
                        PurchaseLine.Item = (string)config["No"];
                        PurchaseLine.ItemDesc = (string)config["Description"];
                        PurchaseLine.Description2 = (string)config["Description_2"];
                        PurchaseLine.Qnty = (string)config["Quantity"];
                        PurchaseLine.UnitM = (string)config["Unit_of_Measure"];
                        PurchaseLine.Amount = (string)config["Direct_Unit_Cost"];
                        PurchaseLine.Location = (string)config["Location_Code"];
                        PurchaseLine.LnNo = (string)config["Line_No"];
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
                string itemDesc = prvLine.ItemDesc.Trim();
                string qnty = prvLine.Qnty.Trim();
                string amnt = prvLine.Amount.Trim();
                string location = prvLine.Location.Trim();

                Credentials.ObjNav.PurchaseRequisitionLines(DocNo, item, Convert.ToDecimal(qnty), itemDesc, Type, location,Convert.ToDecimal(amnt));

                return Json(new { message = "Purchase Line Added successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdatePurchaseLine(PRVLines prvLine)
        {
            try
            {
                Credentials.ObjNav.PurchaseRequistionLineUpdate(Convert.ToInt32(prvLine.LnNo), Convert.ToInt32(prvLine.Qnty), prvLine.DocNo, prvLine.Description2,Convert.ToDecimal(prvLine.Amount));
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