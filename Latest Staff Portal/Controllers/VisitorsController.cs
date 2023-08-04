using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.Controllers
{
    public class VisitorsController : Controller
    {
        // GET: Visitors
        public ActionResult NewVisitorsList()
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
        public ActionResult ActiveVisitorsList()
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
        public ActionResult ClearedVisitorsList()
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
        public PartialViewResult VisitorsListPartialView(string Status)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<Visitors> VisitList = new List<Visitors>();

                string page = "";
                if (Status == "Arrived")
                {
                    page = "NewVisitors?$filter=Status eq '" + Status + "'&$format=json";
                }
                if (Status == "Entered")
                {
                    page = "ActiveVisitors?$filter=Status eq '" + Status + "'&$format=json";
                }
                if (Status == "Cleared")
                {
                    page = "ClearedVisitor?$filter=Status eq '" + Status + "'&$format=json";
                }
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        Visitors visit = new Visitors();
                        visit.No = (string)config["No"];
                        visit.VisitorName = (string)config["VisitorName"];
                        visit.PurposeofVisit = (string)config["PurposeofVisit"];
                        visit.Department = (string)config["Department"];
                        visit.IDNumber = (string)config["IDNumber"];
                        visit.PhoneNumber = (string)config["PhoneNumber"];
                        visit.PersonToSee = (string)config["Visitor_Name"];
                        visit.VisitorPassNo = (string)config["VisitorPassNo"];
                        visit.CarRegNumber = (string)config["VisitorCarRegNumber"];
                        visit.Status = (string)config["Status"];
                        VisitList.Add(visit);
                    }
                }

                VisitDetailList VisitD = new VisitDetailList
                {
                    Status = Status,
                    VisitDetails = VisitList.OrderByDescending(x => x.No).ToList()
                };
                return PartialView("~/Views/Visitors/Partial Views/VisitorsListView.cshtml", VisitD);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewVisitorRequest()
        {
            try
            {
                NewVisiorForm NewVisit = new NewVisiorForm();

                #region Employee List
                List<DropdownList> EmployeeList = new List<DropdownList>();
                string page = "EmployeeList?$&format=json";

                HttpWebResponse httpResponseCampus = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponseCampus.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList ddl = new DropdownList();
                        ddl.Value = (string)config["No"];
                        ddl.Text = (string)config["FirstName"] + " " + (string)config["MiddleName"] + " " + (string)config["LastName"];
                        EmployeeList.Add(ddl);
                    }
                }
                #endregion

                #region Department List
                List<DimensionValues> Department = new List<DimensionValues>();
                string pageDepartment = "DimensionValues?$filter=Dimension_Code eq 'DEPARTMENT'&format=json";

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
                Visitors newVisit = new Visitors();
                LoadvsitorTypes();
                NewVisit = new NewVisiorForm
                {
                    VisitDoc = newVisit,
                    ListOfEmployee = EmployeeList.Select(x =>
                                         new SelectListItem()
                                         {
                                             Text = x.Text,
                                             Value = x.Value
                                         }).ToList(),
                    ListOfDepartment = Department.Select(x =>
                                        new SelectListItem()
                                        {
                                            Text = x.Name,
                                            Value = x.Code
                                        }).ToList()
                };
                return PartialView("~/Views/Visitors/Partial Views/NewVisitorForm.cshtml", NewVisit);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewVisitDocumentView(string DocNo)
        {
            try
            {
                #region Visit Doc
                #region Employee List
                List<DropdownList> EmployeeList = new List<DropdownList>();
                string page = "EmployeeList?$&format=json";

                HttpWebResponse httpResponseCampus = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponseCampus.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList ddl = new DropdownList();
                        ddl.Value = (string)config["No"];
                        ddl.Text = (string)config["FirstName"] + " " + (string)config["MiddleName"] + " " + (string)config["LastName"];
                        EmployeeList.Add(ddl);
                    }
                }
                #endregion

                #region Department List
                List<DimensionValues> Department = new List<DimensionValues>();
                string pageDepartment = "DimensionValues?$filter=Dimension_Code eq 'DEPARTMENT'&format=json";

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
                Visitors VisitDoc = new Visitors();
                LoadvsitorTypes();
                string pageVisit = "NewVisitors?$filter=No eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(pageVisit);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        VisitDoc.No = (string)config["No"];
                        if ((string)config["VisitorCategory"] == "Student")
                        {
                            VisitDoc.visitorCat = "1";
                        }
                        else if ((string)config["VisitorCategory"] == "Employee")
                        {
                            VisitDoc.visitorCat = "2";
                        }
                        else if ((string)config["VisitorCategory"] == "Supplier")
                        {
                            VisitDoc.visitorCat = "3";
                        }
                        else if ((string)config["VisitorCategory"] == "Customer")
                        {
                            VisitDoc.visitorCat = "4";
                        }
                        else if ((string)config["VisitorCategory"] == "Other")
                        {
                            VisitDoc.visitorCat = "5";
                        }
                        else
                        {
                            VisitDoc.visitorCat = "";
                        }
                        VisitDoc.VisitorName = (string)config["Visitor_Name"];
                        VisitDoc.IDNumber = (string)config["IDNumber"];
                        VisitDoc.PhoneNumber = (string)config["PhoneNumber"];
                        VisitDoc.CarRegNumber = (string)config["VisitorCarRegNumber"];
                        VisitDoc.PersonToSee = (string)config["Person_To_See"];
                        VisitDoc.Department = (string)config["Department"];
                        VisitDoc.VisitorPassNo = (string)config["VisitorPassNo"];
                        VisitDoc.PurposeofVisit = (string)config["PurposeofVisit"];
                        VisitDoc.Status = (string)config["Status"];
                    }
                }
                #endregion
                NewVisiorForm NewVisit = new NewVisiorForm
                {
                    VisitDoc = VisitDoc,
                    ListOfEmployee = EmployeeList.Select(x =>
                                         new SelectListItem()
                                         {
                                             Text = x.Text,
                                             Value = x.Value
                                         }).ToList(),
                    ListOfDepartment = Department.Select(x =>
                                        new SelectListItem()
                                        {
                                            Text = x.Name,
                                            Value = x.Code
                                        }).ToList()
                };
                return PartialView("~/Views/Visitors/Partial Views/NewVisitorForm.cshtml", NewVisit);

            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        protected void LoadvsitorTypes()
        {
            try
            {
                List<DropdownList> dropdownList = new List<DropdownList>();

                for (int i = 1; i < 6; i++)
                {
                    DropdownList ddl = new DropdownList();
                    ddl.Value = i.ToString();
                    if (i == 1)
                    {
                        ddl.Text = "Student";
                    }
                    else if (i == 2)
                    {
                        ddl.Text = "Employee";
                    }
                    else if (i == 3)
                    {
                        ddl.Text = "Supplier";
                    }
                    else if (i == 4)
                    {
                        ddl.Text = "Customer";
                    }
                    else
                    {
                        ddl.Text = "Other";
                    }
                    dropdownList.Add(ddl);
                }
                ViewBag.VisitorTypeList = dropdownList;
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }
        public ActionResult GatePassList()
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
        public PartialViewResult GatePassListPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<PurchaseReqList> PurchaseList = new List<PurchaseReqList>();

                string page = "PurchaseRequisition?$filter=Employee_No eq '" + StaffNo + "'&format=json";
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
        public PartialViewResult NewVisitorItemLine()
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
        public JsonResult SubmitVisitorData(Visitors VData)
        {
            try
            {
                string carNo = "";
                if (VData.CarRegNumber != null)
                {
                    carNo = VData.CarRegNumber;
                }
                if (VData.No != null && VData.No != "")
                {
                    //Credentials.ObjNav.UpdateNewVisitorInf(VData.No, Convert.ToInt32(VData.visitorCat), VData.PurposeofVisit, VData.IDNumber, VData.PhoneNumber, carNo,
                    //    "", VData.VisitorName, VData.PersonToSee, VData.Department, VData.VisitorPassNo);
                }
                else
                {
                    //Credentials.ObjNav.InsertNewVisitor(Convert.ToInt32(VData.visitorCat), VData.PurposeofVisit, VData.IDNumber, VData.PhoneNumber, carNo,
                    //    "", VData.VisitorName, VData.PersonToSee, VData.Department, VData.VisitorPassNo);
                }

                return Json(new { message = "Visit created successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AdmiVisitor(string DocNo)
        {
            try
            {
                Credentials.ObjNav.ChangeVisitorStatus(DocNo, 1);

                return Json(new { message = "Visitor admitted successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult PurchaseDocumentLines(string DocNo, string Status)
        {
            try
            {
                #region Purchase Lines
                List<PRVLines> PurchaseLines = new List<PRVLines>();
                string pageLine = "PurchaseLines?$filter=Document_No eq '" + DocNo + "'&format=json";
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
                        PurchaseLine.Amount = (string)config["Direct_Unit_Cost"];
                        PurchaseLine.Location = (string)config["Location_Code"];
                        PurchaseLine.LnNo = (string)config["Line_No"];
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
    }
}