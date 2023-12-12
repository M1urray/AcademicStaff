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
    public class ItemCashController : Controller
    {
        // GET: ItemCash
        public ActionResult ItemCashRequisitionList()
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

        public PartialViewResult ItemCashRequisitionListPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<ItemCashList> ImpList = new List<ItemCashList>();

                string page = "ItemCashReq?$filter=Account_No eq '" + StaffNo + "'&format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        ItemCashList ImList = new ItemCashList();
                        ImList.No = (string)config["No"];
                        ImList.ReqDate = Convert.ToDateTime((string)config["Date"]).ToString("dd/MM/yyyy");
                        ImList.Purpose = (string)config["Purpose"];
                        ImList.Payee = (string)config["Payee"];
                        //ImList.Function = (string)config["FunctionName"];
                        //ImList.BudgetCeter = (string)config["Department_Name"];
                        ImList.Status = (string)config["Status"];
                        ImpList.Add(ImList);
                    }
                }
                return PartialView("~/Views/ItemCash/ItemCashReqListView.cshtml", ImpList.OrderByDescending(x => x.No));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public ActionResult NewItemCashRequest()
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
                    NewItemCashRequisition NewItemCash = new NewItemCashRequisition();
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

                  

                    NewItemCash = new NewItemCashRequisition
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
                       
                    };
                    return View(NewItemCash);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewItemCashLine()
        {
            try
            {
                ItemCashTypesList ItemCashTypes = new ItemCashTypesList();

                #region ItemCash Type List
                List<ItemCashTypes> ItemCashTList = new List<ItemCashTypes>();
                string page = "ItemCashTypes?$filter=Description ne ''&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        ItemCashTypes impList = new ItemCashTypes();
                        impList.Code = (string)config["No"];
                        impList.Description = (string)config["Description"];
                        ItemCashTList.Add(impList);
                    }
                }
                #endregion
                #region Unit Of Measure
                List<UnitOfMeasure> unitOfMeasures = new List<UnitOfMeasure>();
                string pageUOM = "UnitsOfMeasure?$filter=Description ne ''&format=json";

                HttpWebResponse httpResponseUOM = Credentials.GetOdataData(pageUOM);
                using (var streamReaderUOM = new StreamReader(httpResponseUOM.GetResponseStream()))
                {
                    var resultUOM = streamReaderUOM.ReadToEnd();

                    var detailsUOM = JObject.Parse(resultUOM);

                    foreach (JObject configUOM in detailsUOM["value"])
                    {
                        UnitOfMeasure uomList = new UnitOfMeasure();
                        uomList.Code = (string)configUOM["Code"];
                        uomList.Description = (string)configUOM["Description"];
                        unitOfMeasures.Add(uomList);
                    }
                }
                #endregion


                #region Location List
                List<Locations> Locations = new List<Locations>();
                string pageLocation = "Locations?$filter=Name ne ''&format=json";

                HttpWebResponse httpResponseLocation = Credentials.GetOdataData(pageLocation);
                using (var streamReader = new StreamReader(httpResponseLocation.GetResponseStream()))
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

                ItemCashTypes = new ItemCashTypesList
                {
                    ListOfItemCashTypes = ItemCashTList.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Description,
                                              Value = x.Code
                                          }).OrderBy(x => x.Text).ToList(),
                    ListOfLocations = Locations.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Name,
                                              Value = x.Code
                                          }).ToList(),
                    ListOfMeasures = unitOfMeasures.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Description,
                                              Value = x.Code
                                          }).ToList()
                };

                return PartialView("~/Views/ItemCash/ItemCashItemForm.cshtml", ItemCashTypes);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitItemCashRequisition(ItemCashHeader ItemCashHeader)
        {
            bool successVal = false;
            try
            { 
                string StaffNo = Session["Username"].ToString();
                string UserID = Session["Username"].ToString();

                DateTime DateRequired = DateTime.ParseExact(ItemCashHeader.DateNeeded.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string DocNo =  Credentials.ObjNav.ItemCashRequisitionCreate(StaffNo, ItemCashHeader.Campus, DateRequired, ItemCashHeader.Department, "", ItemCashHeader.Remarks, ItemCashHeader.RespC, UserID);
                if (DocNo != "")
                {
                    string Redirect = "/ItemCash/ItemCashDocumentView?DocNo=" + DocNo;

                    Session["SuccessMsg"] = "ItemCash Requisition, Document No: " + DocNo + ", created Successfully. Add line(s) and attachment(s) then send for approval";
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
        public ActionResult ItemCashDocumentView(string DocNo)
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
                    #region ItemCash Header
                    ItemCashHeader ImpDoc = new ItemCashHeader();

                    string page = "ItemCashReq?$filter=No eq '" + DocNo + "'&format=json";
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
                            ImpDoc.CampusName = (string)config["Function_Name"];
                            ImpDoc.Campus = (string)config["Global_Dimension_1_Code"];
                            ImpDoc.DepartmentName = (string)config["Department_Name"];
                            ImpDoc.Department = (string)config["Shortcut_Dimension_2_Code"];
                            ImpDoc.RespC = (string)config["Responsibility_Center"];
                            ImpDoc.TotalAmount = Convert.ToDecimal((string)config["Total_Net_Amount"]).ToString("#,##0.00");
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
        public PartialViewResult ItemCashDocumentLines(string DocNo, string Status)
        {
            try
            {
                #region ItemCash Lines
                List<ItemCashLines> ImpLines = new List<ItemCashLines>();
                string pageLine = "ItemCashLines?$filter=No eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        ItemCashLines ImLine = new ItemCashLines();
                        ImLine.DocNo = (string)config["No"];
                        ImLine.AdvanceType = (string)config["Quantity"];
                        ImLine.Item = (string)config["Account_No"];
                        ImLine.ItemDesc = (string)config["Account_Name"];
                        ImLine.UnitOfMeasure = (string)config["Unit_of_Measure"];
                        ImLine.LnNo = (string)config["Line_No"];
                        ImLine.Location = (string)config["Location"];
                        ImLine.UnitCost = (string)config["Unit_Cost_LCY"];
                        ImLine.Amount = Convert.ToDecimal((string)config["Amount"]).ToString("#,##0.00");
                        ImpLines.Add(ImLine);
                    }
                }
                #endregion
                ItemCashLinesList Lines = new ItemCashLinesList
                {
                    Status = Status,
                    ListOfItemCashLines = ImpLines
                };
                return PartialView("~/Views/ItemCash/ItemCashDocumentLineView.cshtml", Lines);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public JsonResult SendItemCashAppForApproval(string DocNo, string Redirect)
        {
            try
            {
                Credentials.ObjNav.ItemcashApprovalRequest(DocNo);
                if (Redirect == "Y")
                {
                    Session["SuccessMsg"] = "ItemCash Requisition, Document No " + DocNo + " send for approval Successfully";
                }
                return Json(new { message = "ItemCash Requisition,Document No " + DocNo + " send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CancelItemCashAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.CancelItemCashRequisition(DocNo);
                return Json(new { message = "ItemCash Requisition approval cancelled Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateItemCashHeader(string DocNo, ItemCashHeader ItemCashHeader)
        {
            try
            {
                string School = "", Campus = "", Department = "", RespC = "", Remarks = "";
                if (ItemCashHeader.Campus != null)
                {
                    Campus = ItemCashHeader.Campus;
                }
                if (ItemCashHeader.Department != null)
                {
                    Department = ItemCashHeader.Department;
                }
                if (ItemCashHeader.RespC != null)
                {
                    RespC = ItemCashHeader.RespC;
                }
                if (ItemCashHeader.Remarks != null)
                {
                    Remarks = ItemCashHeader.Remarks;
                }
                var DateRequired = DateTime.ParseExact(ItemCashHeader.DateNeeded.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                Credentials.ObjNav.UpdateItemCashHeader(DocNo.Trim(),"", DateRequired, Campus, Department,
                    "", RespC, Remarks);
                return Json(new { message = "ItemCash header Updated successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitItemCashLine(string DocNo, ItemCashHeader ItemCashHeader, ItemCashLines ItemCashLine)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                string item = ItemCashLine.Item.Trim();
                //string itemDesc = ItemCashLine.ItemDesc.Trim();
                //string amnt = ItemCashLine.Amount.Trim();
                Credentials.ObjNav.ItemCashLinesCreate(DocNo,item, Convert.ToInt32(ItemCashLine.Quantity), 
                    ItemCashHeader.Campus, ItemCashHeader.Department, "",
                    ItemCashLine.Location,Convert.ToDecimal(ItemCashLine.UnitCost));
                string DocNetAmount = GetImpDocNetAmount(DocNo);
                return Json(new { NetAmout = DocNetAmount, message = "ItemCash Line Added successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateItemCashLine(string DocNo, string LnNo, ItemCashLines ItemCashLine)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                string UnitCost = ItemCashLine.UnitCost.Trim();
                string qnty = ItemCashLine.Quantity.Trim();
                string Location = ItemCashLine.Location;
                Credentials.ObjNav.ItemCashLineUpdate(DocNo, Convert.ToInt32(LnNo), 
                    Convert.ToInt32(qnty),"",Convert.ToDecimal(UnitCost),Location);
                string DocNetAmount = GetImpDocNetAmount(DocNo);
                return Json(new { NetAmout = DocNetAmount, message = "Item Cash Line updated successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult EditItemCashLine(string LnNo, string DocNo)
        {
            try
            {
                int ln = Convert.ToInt32(LnNo);
                #region ItemCash Lines
                ItemCashLines ImLine = new ItemCashLines();
                string pageLine = "ItemCashLines?$filter=No eq '" + DocNo + "' and Line_No eq " + ln + "&$format=json";
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
                        ImLine.Quantity = (string)config["Quantity"];
                        ImLine.UnitCost= (string)config["Unit_Cost_LCY"];
                        ImLine.Location = (string)config["Location"];
                        ImLine.Amount = Convert.ToDecimal((string)config["Amount"]).ToString("#,##0.00");
                    }
                }
                #endregion
                #region ItemCash Type List
                List<ItemCashTypes> ItemCashTList = new List<ItemCashTypes>();
                string pageType = "ItemCashTypes?$filter=Description ne ''&format=json";

                HttpWebResponse httpResponseType = Credentials.GetOdataData(pageType);
                using (var streamReaderType = new StreamReader(httpResponseType.GetResponseStream()))
                {
                    var resultType = streamReaderType.ReadToEnd();

                    var detailsType = JObject.Parse(resultType);

                    foreach (JObject configType in detailsType["value"])
                    {
                        ItemCashTypes impList = new ItemCashTypes();
                        impList.Code = (string)configType["Code"];
                        impList.Description = (string)configType["Description"];
                        ItemCashTList.Add(impList);
                    }
                }
                #endregion
                #region Location List
                List<Locations> Locations = new List<Locations>();
                string pageLocation = "Locations?$filter=Name ne ''&format=json";

                HttpWebResponse httpResponseLocation = Credentials.GetOdataData(pageLocation);
                using (var streamReaderLocation = new StreamReader(httpResponseLocation.GetResponseStream()))
                {
                    var resultLocation = streamReaderLocation.ReadToEnd();

                    var detailsLocation = JObject.Parse(resultLocation);

                    foreach (JObject configLocation in detailsLocation["value"])
                    {
                        Locations LocList = new Locations();
                        LocList.Code = (string)configLocation["Code"];
                        LocList.Name = (string)configLocation["Name"];
                        Locations.Add(LocList);
                    }
                }
                #endregion
                #region Unit Of Measure
                List<UnitOfMeasure> unitOfMeasures = new List<UnitOfMeasure>();
                string pageUOM = "UnitsOfMeasure?$filter=Description ne ''&format=json";

                HttpWebResponse httpResponseUOM = Credentials.GetOdataData(pageUOM);
                using (var streamReaderUOM = new StreamReader(httpResponseUOM.GetResponseStream()))
                {
                    var resultUOM = streamReaderUOM.ReadToEnd();

                    var detailsUOM = JObject.Parse(resultUOM);

                    foreach (JObject configUOM in detailsUOM["value"])
                    {
                        UnitOfMeasure uomList = new UnitOfMeasure();
                        uomList.Code = (string)configUOM["Code"];
                        uomList.Description = (string)configUOM["Description"];
                        unitOfMeasures.Add(uomList);
                    }
                }
                #endregion
                ItemCashItemDetails itemDetails = new ItemCashItemDetails
                {
                    ItemDetails = ImLine,
                    ListOfItemCashTypes = ItemCashTList.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Description,
                                              Value = x.Code
                                          }).ToList(),
                    ListOfLocations = Locations.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Name,
                                              Value = x.Code
                                          }).ToList(),
                    ListOfMeasures = unitOfMeasures.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Description,
                                              Value = x.Code
                                          }).ToList()
                };
                return PartialView("~/Views/ItemCash/ItemCashEditItemForm.cshtml", itemDetails);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RemoveItemCashLine(string DocNo, string LnNo)
        {
            try
            {
                Credentials.ObjNav.ItemCashRemoveLine(Convert.ToInt32(LnNo), DocNo);
                string DocNetAmount = GetImpDocNetAmount(DocNo);
                return Json(new { NetAmout = DocNetAmount, message = "ItemCash Line removed successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        protected string GetImpDocNetAmount(string DocNo)
        {
            string amount = "";
            string page = "ItemCashReq?$select=Total_Net_Amount&$filter=No eq '" + DocNo + "'&format=json";
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
            return PartialView("~/Views/ItemCash/FileAttachmentForm.cshtml");
        }
    }
}