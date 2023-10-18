using Latest_Staff_Portal.CustomSecurity;
using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
using Newtonsoft.Json;
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
    public class StoreController : Controller
    {
        // GET: Store
        public ActionResult StoreRequisitionList()
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
        public PartialViewResult StoreRequisitionListPartialView()
        {
            string StaffNo = Session["Username"].ToString();
            List<StoreReqList> StoreList = new List<StoreReqList>();

            string page = "StoreReqList?$filter=Employee_No eq '" + StaffNo + "'&format=json";
            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    StoreReqList storeList = new StoreReqList();
                    storeList.No = (string)config["No"];
                    storeList.ReqDate = Convert.ToDateTime((string)config["Request_date"]).ToString("dd/MM/yyyy");
                    storeList.DateRequired = Convert.ToDateTime((string)config["Required_Date"]).ToString("dd/MM/yyyy");
                    storeList.Description = (string)config["Request_Description"];
                    storeList.Function = (string)config["Function_Name"];
                    storeList.BudgetCeter = (string)config["Budget_Center_Name"];
                    storeList.Status = (string)config["Status"];
                    StoreList.Add(storeList);
                }
            }
            return PartialView("~/Views/Store/StoreReqListView.cshtml", StoreList.OrderByDescending(x => x.No));
        }
        public ActionResult NewStoreRequest()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                string StaffNo = Session["Username"].ToString();
                Session["Servicedetails"] = null;
                Session["Itemdetails"] = null;
                NewStoreRequisition NewIRV = new NewStoreRequisition();
                Session["httpResponse"] = null;
                #region Institute List
                List<DimensionValues> Campuses = new List<DimensionValues>();
                string pageCampus = "DimValues?$select=Code,Name&$filter=Dimension_Code eq 'CAMPUS' and Blocked eq false&$format=json";

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
                string pageSchool = "DimValues?$select=Code,Name&$filter=Dimension_Code eq 'SCHOOL' and Blocked eq false&$format=json";

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
                string pageDepartment = "DimValues?$select=Code,Name&$filter=Dimension_Code eq 'DEPARTMENTS' and Blocked eq false&$format=json";

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

                NewIRV = new NewStoreRequisition
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
                                       }).ToList()
                };
                return View(NewIRV);
            }
        }
        public PartialViewResult NewStoreLine()
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

            return PartialView("~/Views/Store/StoreItemForm.cshtml", locationList);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult CheckstockLevel(StoreLines storeLine)
        {
            bool successVal = false;
            string msg = "";
            try
            {
                string item = storeLine.Item.Trim();
                string qnty = storeLine.Qnty.Trim();
                string location = storeLine.Location.Trim();
                decimal s = Credentials.ObjNav.StockLevel(item, Convert.ToInt32(qnty), location);
                if (s < Convert.ToDecimal(qnty))
                {
                    successVal = false;
                    msg = storeLine.ItemDesc.Trim() + " will lead to negative stock." + s + " Remaining items";

                }
                else
                {
                    successVal = true;
                }
                return Json(new { message = msg, success = successVal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = successVal }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitStoreRequisition(StoretHeader storeHeader, List<StoreLines> storeLines)
        {
            bool successVal = false;
            try
            {
                string StaffNo = Session["Username"].ToString();

                DateTime DateRequired = DateTime.ParseExact(storeHeader.DateNeeded.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string DocNo = Credentials.ObjNav.StoreRequisitionCreate(StaffNo, 0, DateRequired, storeHeader.Campus,
                    storeHeader.Department, storeHeader.Remarks, storeHeader.RespC, "");
                foreach (var c in storeLines)
                {
                    string item = c.Item.Trim();
                    string itemDesc = c.ItemDesc.Trim();
                    string Descr = c.Description2.Trim();
                    string qnty = c.Qnty.Trim();
                    string location = c.Location.Trim();
                    Credentials.ObjNav.StoreRequisitionLines(DocNo, item, Convert.ToDecimal(qnty), Descr, location, "");
                }
                successVal = true;
                Credentials.ObjNav.StoreRequisitionApprovalRequest(DocNo);
                Session["SuccessMsg"] = "Store Requisition, Document No: " + DocNo + ", Submitted Successfully";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (successVal)
                {
                    Session["ErrorMsg"] = ex.Message.Replace("'", "");
                }
                return Json(new { message = ex.Message.Replace("'", ""), success = successVal }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult StoreDocumentView(string DocNo)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                string StaffNo = Session["Username"].ToString();
                #region Store Header
                StoretHeader StoreDoc = new StoretHeader();

                string page = "StoreReqList?$filter=No eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        StoreDoc.No = (string)config["No"];
                        StoreDoc.DateRequested = Convert.ToDateTime((string)config["Request_date"]).ToString("dd/MM/yyyy");
                        StoreDoc.DateNeeded = Convert.ToDateTime((string)config["Required_Date"]).ToString("dd/MM/yyyy");
                        StoreDoc.Remarks = (string)config["Request_Description"];
                        StoreDoc.Campus = (string)config["Global_Dimension_1_Code"];
                        StoreDoc.CampusName = (string)config["Function_Name"];
                        StoreDoc.Department = (string)config["Shortcut_Dimension_2_Code"];
                        StoreDoc.DepartmentName = (string)config["Budget_Center_Name"];
                        StoreDoc.RespC = (string)config["Responsibility_Center"];
                        StoreDoc.IssuingStore = (string)config["Issuing_Store"];
                        StoreDoc.Status = (string)config["Status"];
                    }
                }
                #endregion

                return View(StoreDoc);
            }
        }
        public PartialViewResult StoreDocumentLines(string DocNo, string Status)
        {
            try
            {
                #region Store Lines
                List<StoreLines> StoreLines = new List<StoreLines>();
                string pageLine = "StoreReqLines?$filter=Requistion_No eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        StoreLines StoreLine = new StoreLines();
                        StoreLine.DocNo = (string)config["Requistion_No"];
                        StoreLine.Item = (string)config["No"];
                        StoreLine.ItemDesc = (string)config["Description"];
                        StoreLine.Description2 = (string)config["Description_2"];
                        StoreLine.Qnty = (string)config["Quantity_Requested"];
                        StoreLine.Location = (string)config["Issuing_Store"];
                        StoreLine.LnNo = (string)config["Line_No"];
                        StoreLines.Add(StoreLine);
                    }
                }
                #endregion
                StoreLinesList Lines = new StoreLinesList
                {
                    Status = Status,
                    ListOfStoreLines = StoreLines
                };
                return PartialView("~/Views/Store/StoreDocumentLineView.cshtml", Lines);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return PartialView();
            }
        }
        public JsonResult SendStoreAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.StoreRequisitionApprovalRequest(DocNo);
                return Json(new { message = "Store Requisition send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CancelStoreAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.HRCancelStoreRequisition(DocNo);
                return Json(new { message = "Store Requisition approval cancelled Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateStoreHeader(string DocNo, StoretHeader storeHeader)
        {
            try
            {
                string School = "", Campus = "", Department = "", RespC = "", Remarks = "";

                if (storeHeader.Campus != null)
                {
                    Campus = storeHeader.Campus;
                }
                if (storeHeader.Department != null)
                {
                    Department = storeHeader.Department;
                }
                if (storeHeader.RespC != null)
                {
                    RespC = storeHeader.RespC;
                }
                if (storeHeader.Remarks != null)
                {
                    Remarks = storeHeader.Remarks;
                }
                var DateRequired = DateTime.ParseExact(storeHeader.DateNeeded.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                Credentials.ObjNav.UpdateStoreRequisition(DocNo.Trim(), DateRequired, Campus, Department,
                    School, RespC, Remarks);
                return Json(new { message = "Store header Updated successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult EditStoreLine(string LnNo, string DocNo, string ItemNo)
        {
            int ln = Convert.ToInt32(LnNo);
            #region Store Lines
            StoreLines StoreLine = new StoreLines();
            string pageLine = "StoreReqLines?$filter=Requistion_No eq '" + DocNo + "' and No eq '" + ItemNo + "' and Line_No eq " + ln + "&format=json";
            HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
            using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    StoreLine.DocNo = (string)config["Requistion_No"];
                    StoreLine.Item = (string)config["No"];
                    StoreLine.ItemDesc = (string)config["Description"];
                    StoreLine.Description2 = (string)config["Description_2"];
                    StoreLine.Qnty = (string)config["Quantity_Requested"];
                    StoreLine.Location = (string)config["Issuing_Store"];
                    StoreLine.LnNo = (string)config["Line_No"];
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

            StoreItemDetails itemDetails = new StoreItemDetails
            {
                ItemDetails = StoreLine,
                ListOfLocations = Locations.Select(x =>
                                      new SelectListItem()
                                      {
                                          Text = x.Name,
                                          Value = x.Code
                                      }).ToList()
            };
            return PartialView("~/Views/Store/StoreItemEditForm.cshtml", itemDetails);
        }
        public JsonResult SubmitStoreLine(string DocNo, StoretHeader storeHeader, StoreLines storeLine)
        {
            try
            {
                string item = storeLine.Item.Trim();
                string Descr = storeLine.Description2.Trim();
                string qnty = storeLine.Qnty.Trim();
                string location = storeLine.Location.Trim();
                Credentials.ObjNav.StoreRequisitionLines(DocNo, item, Convert.ToDecimal(qnty), Descr, location, "");

                return Json(new { message = "Store Line Added successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateStoreLine(StoreLines storeLine)
        {
            try
            {
                Credentials.ObjNav.StoreRequistionLineUpdate(Convert.ToInt32(storeLine.LnNo), Convert.ToInt32(storeLine.Qnty), storeLine.DocNo, storeLine.Description2, storeLine.Item);
                return Json(new { message = "Store Line Updated successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RemoveStoreLine(string DocNo, string LnNo, string ItemNo)
        {
            try
            {
                Credentials.ObjNav.StoreRequsitionRemoveLine(Convert.ToInt32(LnNo), DocNo, ItemNo);
                return Json(new { message = "Store Line Deleted successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}