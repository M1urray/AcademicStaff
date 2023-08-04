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
    public class CafeteriaController : Controller
    {
        // GET: Cafeteria
        public ActionResult CafeteriaRequest()
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
        public PartialViewResult CafRequisitionListPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<Cafeteria> CafList = new List<Cafeteria>();

                string page = "CafeteriaReq?$filter=Employee_No eq '" + StaffNo + "'&$format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        Cafeteria Caf = new Cafeteria();
                        Caf.No = (string)config["Document_No"];
                        Caf.DateNeeded = ((DateTime)config["Date_Needed"]).ToString("dd/MM/yyyy");
                        Caf.TimeNeeded = ((DateTime)config["Booking_Time"]).ToString("h:mm tt");
                        Caf.Event_Name = (string)config["Meeting_Name"];
                        Caf.Venue = (string)config["Venue"];
                        Caf.TotalCost = (string)config["Total_Cost"];
                        Caf.Status = (string)config["Status"];
                        CafList.Add(Caf);
                    }
                }
                return PartialView("~/Views/Cafeteria/CafeteriaReqListView.cshtml", CafList.OrderByDescending(x => x.No));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public ActionResult NewCafRequest()
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
                    NewCafRequisition NewCaf = new NewCafRequisition();
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
                    if (Dim2 == "")
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

                        #region Event Type List
                        List<DimensionValues> EventTList = new List<DimensionValues>();
                        string pageEventT = "EventTypes?$format=json";

                        HttpWebResponse httpResponseEventT = Credentials.GetOdataData(pageEventT);
                        using (var streamReader = new StreamReader(httpResponseEventT.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);

                            foreach (JObject config in details["value"])
                            {
                                DimensionValues Ev = new DimensionValues();
                                Ev.Code = (string)config["Code"];
                                Ev.Name = (string)config["Description"];
                                EventTList.Add(Ev);
                            }
                        }
                        #endregion

                        NewCaf = new NewCafRequisition
                        {
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
                            ListOfEventTList = EventTList.Select(x =>
                                              new SelectListItem()
                                              {
                                                  Text = x.Name,
                                                  Value = x.Code
                                              }).ToList()
                        };
                        return View(NewCaf);
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
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetCafeteriaList(string Campus)
        {
            try
            {
                #region Caf List
                List<DropdownList> CafList = new List<DropdownList>();
                string page = "CafeteriaList?$filter=Campus eq '" + Campus + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList c = new DropdownList();
                        c.Value = (string)config["Code"];
                        c.Text = (string)config["Name"];
                        CafList.Add(c);
                    }
                }
                #endregion
                DropdownListData CafL = new DropdownListData
                {
                    ListOfddlData = CafList.Select(x =>
                                    new SelectListItem()
                                    {
                                        Text = x.Text,
                                        Value = x.Value
                                    }).ToList()
                };
                return Json(CafL, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitCafRequisition(Cafeteria cafHeader)
        {
            bool successVal = false;
            try
            {
                string School = "";
                if (cafHeader.School != null)
                {
                    School = cafHeader.School;
                }

                string StaffNo = Session["Username"].ToString();

                DateTime DateRequired = DateTime.ParseExact(cafHeader.DateNeeded.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string DocNo = "";// Credentials.ObjNav.SubmitFoodRequest(StaffNo, cafHeader.Campus, cafHeader.Department, DateRequired, cafHeader.RespC,
                    //cafHeader.Event_Name, cafHeader.Venue, "", Convert.ToDateTime(cafHeader.TimeNeeded), cafHeader.Caf, cafHeader.EventType);
                if (DocNo != "")
                {
                    string Redirect = "/Cafeteria/CafDocumentView?DocNo=" + DocNo;

                    Session["SuccessMsg"] = "Cafeteria Food Request, Document No: " + DocNo + ", created Successfully. Add line(s) and attachment(s) then send for approval";
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
        public ActionResult CafDocumentView(string DocNo)
        {
            try
            {
                if (Session["Username"] == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                else
                {
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
                    #region Event Type List
                    List<DimensionValues> EventTList = new List<DimensionValues>();
                    string pageEventT = "EventTypes?$format=json";

                    HttpWebResponse httpResponseEventT = Credentials.GetOdataData(pageEventT);
                    using (var streamReader = new StreamReader(httpResponseEventT.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);

                        foreach (JObject config in details["value"])
                        {
                            DimensionValues Ev = new DimensionValues();
                            Ev.Code = (string)config["Code"];
                            Ev.Name = (string)config["Description"];
                            EventTList.Add(Ev);
                        }
                    }
                    #endregion
                    #region Caf Header
                    Cafeteria CafDoc = new Cafeteria();

                    string page = "CafeteriaReq?$filter=Document_No eq '" + DocNo + "'&$format=json";
                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        foreach (JObject config in details["value"])
                        {
                            CafDoc.No = (string)config["Document_No"];
                            CafDoc.DateRaised = ((DateTime)config["Date_Raised"]).ToString("dd/MM/yyyy");
                            CafDoc.DateNeeded = ((DateTime)config["Date_Needed"]).ToString("dd/MM/yyyy");
                            CafDoc.TimeNeeded = ((DateTime)config["Booking_Time"]).ToString("h:mm tt");
                            CafDoc.Event_Name = (string)config["Meeting_Name"];
                            CafDoc.Venue = (string)config["Venue"];
                            CafDoc.TotalCost = (string)config["Total_Cost"];
                            CafDoc.Campus = (string)config["Campus"];
                            CafDoc.Department = (string)config["Department"];
                            CafDoc.RespC = (string)config["Responsibility_Center"];
                            CafDoc.Caf = (string)config["Cafeteria"];
                            CafDoc.CafName = (string)config["Cafeteria_Name"];
                            CafDoc.TotalCost = (string)config["Total_Cost"];
                            CafDoc.Status = (string)config["Status"];
                            CafDoc.EventType = (string)config["Event_Type"];
                        }
                    }
                    CafDoc.ListOfCampus = Campuses.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Name,
                                              Value = x.Code
                                          }).ToList();
                    CafDoc.ListOfDepartment = Department.Select(x =>
                                  new SelectListItem()
                                  {
                                      Text = x.Name,
                                      Value = x.Code
                                  }).ToList();
                    CafDoc.ListOfResponsibility = RespCList.Select(x =>
                                  new SelectListItem()
                                  {
                                      Text = x.Name,
                                      Value = x.Code
                                  }).ToList();
                    CafDoc.ListOfEventTList = EventTList.Select(x =>
                                 new SelectListItem()
                                 {
                                     Text = x.Name,
                                     Value = x.Code
                                 }).ToList();
                    #endregion

                    return View(CafDoc);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateCafRequisition(Cafeteria cafHeader)
        {
            bool successVal = false;
            try
            {
                string School = "";
                if (cafHeader.School != null)
                {
                    School = cafHeader.School;
                }

                string StaffNo = Session["Username"].ToString();

                //DateTime DateRequired = DateTime.ParseExact(cafHeader.DateNeeded.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //string DocNo = Credentials.ObjNav.foodre(StaffNo, cafHeader.Campus, cafHeader.Department, DateRequired, cafHeader.RespC,
                //    cafHeader.Event_Name, cafHeader.Venue, "", Convert.ToDateTime(cafHeader.TimeNeeded), cafHeader.Caf);
                //if (DocNo != "")
                //{
                //    string Redirect = "/Cafeteria/CafDocumentView?DocNo=" + DocNo;

                //    Session["SuccessMsg"] = "Cafeteria Food Request, Document No: " + DocNo + ", created Successfully. Add line(s) and attachment(s) then send for approval";
                //    return Json(new { message = Redirect, success = true }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    return Json(new { message = "Document not created. Please try again later...", success = false }, JsonRequestBehavior.AllowGet);
                //}

                return Json(new { message = "Problem Encountered", success = false }, JsonRequestBehavior.AllowGet);
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
        public PartialViewResult CafDocumentLines(string DocNo, string Status)
        {
            try
            {
                #region Caf Lines
                List<CafeteriaLines> CafLines = new List<CafeteriaLines>();
                string pageLine = "CafeteriaReqLines?$filter=Document_No eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        CafeteriaLines cafLn = new CafeteriaLines();
                        cafLn.No = (string)config["Document_No"];
                        cafLn.Item = (string)config["Item"];
                        cafLn.ItemName = (string)config["Description"];
                        cafLn.Quantity = (string)config["Quantity"];
                        cafLn.LnNo = (string)config["Line_No"];
                        cafLn.UnitCost = Convert.ToDecimal((string)config["Unit_Cost"]).ToString("#,##0.00");
                        cafLn.Amount = Convert.ToDecimal((string)config["Amount"]).ToString("#,##0.00");
                        CafLines.Add(cafLn);
                    }
                }
                #endregion
                CafLinesList Lines = new CafLinesList
                {
                    Status = Status,
                    ListOfCafLines = CafLines
                };
                return PartialView("~/Views/Cafeteria/CafDocumentLineView.cshtml", Lines);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewCafLine(string Caf)
        {
            try
            {
                MenuList menuList = new MenuList();

                #region Menu List
                List<DropdownList> MList = new List<DropdownList>();
                string page = "CafeteriaMenus?$select=Menu_Item,Description&$filter=Cafeteria eq '" + Caf + "' and Description ne ''&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        DropdownList m = new DropdownList();
                        m.Value = (string)config["Menu_Item"];
                        m.Text = (string)config["Description"];
                        MList.Add(m);
                    }
                }
                #endregion

                menuList = new MenuList
                {
                    ListOfMenuItems = MList.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Text,
                                              Value = x.Value
                                          }).OrderBy(x => x.Text).ToList()
                };

                return PartialView("~/Views/Cafeteria/CafItemForm.cshtml", menuList);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitCafLine(string DocNo, CafeteriaLines cafLine)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                string item = cafLine.Item.Trim();
                string Qnty = cafLine.Quantity.Trim();
                Credentials.ObjNav.SubmitFoodRequestLines(DocNo, item, Convert.ToDecimal(Qnty));
                string DocNetAmount = GetCafDocNetAmount(DocNo);
                return Json(new { NetAmout = DocNetAmount, message = "Line Added successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult EditCafLine(string LnNo, string DocNo, string Caf)
        {
            try
            {
                int ln = Convert.ToInt32(LnNo);
                #region Caf Lines
                CafeteriaLines CafItem = new CafeteriaLines();
                string pageLine = "CafeteriaReqLines?$filter=Document_No eq '" + DocNo + "' and Line_No eq " + ln + "&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        CafItem.No = (string)config["Document_No"];
                        CafItem.Item = (string)config["Item"];
                        CafItem.Quantity = (string)config["Quantity"];
                        CafItem.LnNo = (string)config["Line_No"];
                    }
                }
                #endregion
                #region Menu List
                List<DropdownList> MList = new List<DropdownList>();
                string page = "CafeteriaMenus?$select=Menu_Item,Description&$filter=Cafeteria eq '" + Caf + "' and Description ne ''&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        DropdownList m = new DropdownList();
                        m.Value = (string)config["Menu_Item"];
                        m.Text = (string)config["Description"];
                        MList.Add(m);
                    }
                }
                #endregion
                CafItemDetails itemDetails = new CafItemDetails
                {
                    ItemDetails = CafItem,
                    ListOfMenuItems = MList.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Text,
                                              Value = x.Value
                                          }).ToList()
                };
                return PartialView("~/Views/Cafeteria/CafEditItemForm.cshtml", itemDetails);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateCafLine(string DocNo, string Item, string LnNo, string Quantity)
        {
            try
            {
                Credentials.ObjNav.UpdateFoodRequestLines(DocNo, Item, Convert.ToDecimal(Quantity), Convert.ToInt32(LnNo));
                string DocNetAmount = GetCafDocNetAmount(DocNo);
                return Json(new { NetAmout = DocNetAmount, message = "Line Updated successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        protected string GetCafDocNetAmount(string DocNo)
        {
            string amount = "";
            string page = "CafeteriaReq?$select=Total_Cost&$filter=Document_No eq '" + DocNo + "'&$format=json";
            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        amount = Convert.ToDecimal((string)config["Total_Cost"]).ToString("#,##0.00");
                    }
                }
            }
            return amount;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RemoveCafLine(string DocNo, string Item, string LnNo)
        {
            try
            {
                Credentials.ObjNav.RemoveFoodRequestLines(DocNo, Item, Convert.ToInt32(LnNo));
                string DocNetAmount = GetCafDocNetAmount(DocNo);
                return Json(new { NetAmout = DocNetAmount, message = "Line removed successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult FileUploadForm()
        {
            return PartialView("~/Views/Cafeteria/FileAttachmentForm.cshtml");
        }
        [HttpPost]
        public JsonResult SendCafAppForApproval(string DocNo, string Redirect)
        {
            try
            {
                Credentials.ObjNav.FoodReqApprovalRequest(DocNo);
                if (Redirect == "Y")
                {
                    Session["SuccessMsg"] = "Cafeteria Food Request, Document No " + DocNo + " send for approval Successfully";
                }
                return Json(new { message = "Cafeteria Food Request,Document No " + DocNo + " send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult CancelCafAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.FoodReqcancelRequest(DocNo);
                return Json(new { message = "Cafeteria Food Request approval cancelled Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}