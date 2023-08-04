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
    public class LeaveController : Controller
    {
        // GET: Leave
        public ActionResult LeaveRequisitionList()
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
        public PartialViewResult LeaveRequisitionListPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<LeaveReqList> LeaveList = new List<LeaveReqList>();

                string page = "HRLeaveRequisition?$filter=EmployeeNo eq '" + StaffNo + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        LeaveReqList LvList = new LeaveReqList();
                        LvList.No = (string)config["ApplicationNo"];
                        LvList.Leave_Type = (string)config["LeaveType"];
                        LvList.Applied_Days = (string)config["DaysApplied"];
                        LvList.Date = Convert.ToDateTime((string)config["ApplicationDate"]).ToString("dd/MM/yyyy");
                        LvList.Starting_Date = Convert.ToDateTime((string)config["StartDate"]).ToString("dd/MM/yyyy");
                        LvList.End_Date = Convert.ToDateTime((string)config["EndDate"]).ToString("dd/MM/yyyy");
                        LvList.Return_Date = Convert.ToDateTime((string)config["ReturnDate"]).ToString("dd/MM/yyyy");
                        LvList.Reliever = (string)config["Reliever_Name"];
                        LvList.Status = (string)config["Status"];
                        LeaveList.Add(LvList);
                    }

                }
                return PartialView("~/Views/Leave/LeaveReqListPartialView.cshtml", LeaveList.OrderByDescending(x => x.No));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewLeaveApplication()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                NewLeaveApplication NewAppl = new NewLeaveApplication();
                #region LeaveTypes
                List<LvTypes> leaveTps = new List<LvTypes>();

                string gender = CommonClass.GetEmployeeGender(StaffNo);
                if (string.IsNullOrEmpty(gender.Trim()))
                {
                    Error erroMsg = new Error();
                    erroMsg.Message = "Your gender has not been set. Contact HR";
                    return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
                }
                else
                {
                    string page = "LeaveTypes?$filter=(Gender eq 'Both' or Gender eq '" + gender + "')&format=json";

                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        foreach (JObject config in details["value"])
                        {
                            LvTypes LTpe = new LvTypes();
                            LTpe.Code = (string)config["Code"];
                            LTpe.Description = (string)config["Description"];
                            leaveTps.Add(LTpe);
                        }
                    }
                    #endregion
                    #region ReliverList
                    List<RelieverList> relieverList = new List<RelieverList>();
                    string Department = CommonClass.EmployeeDepartment(StaffNo);
                    //string pageReliever = "EmployeeList?$filter=No ne '" + StaffNo + "' and GlobalDimension2Code eq '" + Department + "' and Status eq 'Active'&$format=json";
                    string pageReliever = "EmployeeList?$filter=No ne '" + StaffNo + "' and Status eq 'Active'&$format=json";

                    HttpWebResponse httpResponseReliever = Credentials.GetOdataData(pageReliever);
                    using (var streamReader = new StreamReader(httpResponseReliever.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        foreach (JObject config in details["value"])
                        {
                            if ((string)config["FirstName"] != "")
                            {
                                RelieverList Rlist = new RelieverList();
                                Rlist.No = (string)config["No"];
                                Rlist.Name = (string)config["FirstName"] + " " + (string)config["MiddleName"] + " " + (string)config["LastName"];
                                relieverList.Add(Rlist);
                            }
                        }

                    }
                    #endregion
                    NewAppl = new NewLeaveApplication
                    {
                        LeaveBal = "0",
                        AllocatedDays = "0",
                        ReimbDays = "0",
                        LeaveTaken = "0",
                        EarnedLeaveDays = "0",
                        CarryForawrd = "0",
                        ListOfLeaveTypes = leaveTps.Select(x =>
                                             new SelectListItem()
                                             {
                                                 Text = x.Description,
                                                 Value = x.Code
                                             }).ToList(),
                        ListOfRelievers = relieverList.Select(x =>
                                             new SelectListItem()
                                             {
                                                 Text = x.Name,
                                                 Value = x.No
                                             }).ToList()
                    };
                    return PartialView("~/Views/Leave/NewLeaveApplication.cshtml", NewAppl);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetLeaveBalance(string LvType)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                LeaveBalance newBal = new LeaveBalance();

                List<DropDownBalance> Lvbal = new List<DropDownBalance>();
                decimal availableDays = 0;
                decimal[] s = new decimal[5];
                if (LvType.Contains("ANNUAL"))
                {
                    s = CommonClass.GetLeaveBal(StaffNo, LvType);
                    if (s[0] > 1)
                    {
                        availableDays = (s[1] + s[4]) + s[0] - Math.Abs(s[2]);
                    }
                }
                else
                {
                    string page = "HRLeaveLedger?$select=No_Of_days&$filter=No eq '" + StaffNo + "' and Leave_Type eq '" + LvType + "'&$format=json";

                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);


                        foreach (JObject config in details["value"])
                        {
                            availableDays = availableDays + (int)config["No_Of_days"];
                        }
                    }
                    s[0] = 0;
                    s[1] = 0;
                    s[2] = 0;
                    s[4] = 0;
                }

                if (availableDays > 0)
                {
                    for (int i = 1; i <= availableDays; i++)
                    {
                        DropDownBalance NewV = new DropDownBalance();
                        NewV.Code = i.ToString();
                        Lvbal.Add(NewV);
                    }
                    newBal = new LeaveBalance
                    {
                        AllocatedDays = s[0].ToString(),
                        CarryForawrd = s[1].ToString(),
                        ReimbDays = s[4].ToString(),
                        LeaveTaken = s[2].ToString(),
                        // EarnedLeaveDays = s[3].ToString(),
                        Balance = (availableDays).ToString(),
                        ListOfDays = Lvbal.Select(x =>
                                         new SelectListItem()
                                         {
                                             Text = x.Code,
                                             Value = x.Code
                                         }).ToList()
                    };
                    return Json(new { newBal, success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "No Allocations for the specified leave type has been done. Contact HR", success = false }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult End_ReturnDates(string startDate, string days, string LvType)
        {
            try
            {
                EndReturnDates ERDate = new EndReturnDates();

                DateTime startD = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                DateTime[] s = Credentials.ObjNav.GetEndReturnDate(startD, Convert.ToInt32(days), LvType);
                DateTime endDate = s[0];
                DateTime returndate = s[1];
                ERDate = new EndReturnDates
                {
                    EndDate = endDate.ToString("dd/MM/yyyy"),
                    ReturnDate = returndate.ToString("dd/MM/yyyy")
                };
                return Json(new { ERDate, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitLeaveApplication(LeaveReqList NewApp, string base64Upload, string fileName, string Extn)
        {
            try
            {
                string msg = "";
                bool successVal = false, saveData = false; ;

                if (base64Upload != "")
                {
                    if (fileName != "")
                    {
                        string ext = Path.GetExtension(fileName);

                        if (ext.ToLower() == ".pdf" || ext.ToLower() == ".docx" || ext.ToLower() == ".doc" || ext.ToLower() == ".xlsx" ||
                            ext.ToLower() == ".jpeg" || ext.ToLower() == ".jpg" || ext.ToLower() == ".png")
                        {
                            saveData = true;
                        }
                        else
                        {
                            msg = "Only files with extensions(.pdf, .docx, .doc, .xlsx, .jpeg, .jpg, .png) can be uploaded";
                            successVal = false;
                            saveData = false;
                        }
                    }
                    else
                    {
                        msg = "Incorrect uploaded file!!";
                        successVal = false;
                        saveData = false;
                    }
                }
                else
                {
                    saveData = true;
                }
                if (saveData)
                {
                    DateTime startDate = DateTime.ParseExact(NewApp.Starting_Date.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime endDate = DateTime.ParseExact(NewApp.End_Date.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime returndate = DateTime.ParseExact(NewApp.Return_Date.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    string Remarks = "", Reliever = "";
                    if (NewApp.Remarks != null)
                    {
                        Remarks = NewApp.Remarks;
                    }
                    if (NewApp.Reliever != null)
                    {
                        Reliever = NewApp.Reliever;
                    }
                    string DocNo = Credentials.ObjNav.HRLeaveApplication(Session["username"].ToString(), NewApp.Leave_Type,
                         Convert.ToDecimal(NewApp.Applied_Days), startDate, endDate, returndate, Remarks, Reliever,
                         "", "");

                    if (base64Upload != "" && fileName != "")
                    {
                        string filePath = Server.MapPath("~/Uploads/" + fileName);
                        string s = Credentials.UploadDocumentAttachment(DocNo, base64Upload, filePath, 70134864);
                    }

                    msg = "A leave Application, Document Number " + DocNo + " has been Submitted successfully and send for approval";
                    successVal = true;
                }
                return Json(new { message = msg, success = successVal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateLeaveApplication(LeaveReqList NewApp)
        {
            try
            {
                string Remarks = "";
                if (NewApp.Remarks != null)
                {
                    Remarks = NewApp.Remarks;
                }
                DateTime startDate = DateTime.ParseExact(NewApp.Starting_Date.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.ParseExact(NewApp.End_Date.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime returndate = DateTime.ParseExact(NewApp.Return_Date.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                Credentials.ObjNav.HRUpdateLeaveApplication(NewApp.No, NewApp.Reliever, startDate, endDate, returndate, Remarks, Convert.ToDecimal(NewApp.Applied_Days));

                return Json(new { message = "Leave Application Updated and send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult LeaveDocumentViewView(string DocNo)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();

                #region ReliverList
                List<RelieverList> relieverList = new List<RelieverList>();

                string pageReliever = "EmployeeList?&format=json";

                HttpWebResponse httpResponseReliever = Credentials.GetOdataData(pageReliever);
                using (var streamReader = new StreamReader(httpResponseReliever.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        if ((string)config["FirstName"] != "")
                        {
                            RelieverList Rlist = new RelieverList();
                            Rlist.No = (string)config["No"];
                            Rlist.Name = (string)config["FirstName"] + " " + (string)config["MiddleName"] + " " + (string)config["LastName"];
                            relieverList.Add(Rlist);
                        }
                    }

                }
                #endregion
                LeaveReqList LeaveDoc = new LeaveReqList();

                string page = "HRLeaveRequisition?$filter=ApplicationNo eq '" + DocNo + "'&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        LeaveDoc.No = (string)config["ApplicationNo"];
                        LeaveDoc.EmpNo = (string)config["EmployeeNo"];
                        LeaveDoc.EmpName = (string)config["EmployeeName"];
                        LeaveDoc.Leave_Type = (string)config["LeaveType"];
                        LeaveDoc.Applied_Days = (string)config["DaysApplied"];
                        LeaveDoc.Date = Convert.ToDateTime((string)config["ApplicationDate"]).ToString("dd/MM/yyyy");
                        LeaveDoc.Starting_Date = Convert.ToDateTime((string)config["StartDate"]).ToString("dd/MM/yyyy");
                        LeaveDoc.End_Date = Convert.ToDateTime((string)config["EndDate"]).ToString("dd/MM/yyyy");
                        LeaveDoc.Return_Date = Convert.ToDateTime((string)config["ReturnDate"]).ToString("dd/MM/yyyy");
                        LeaveDoc.Reliever = (string)config["Reliever"];
                        LeaveDoc.Department = (string)config["Department_Code"];
                        LeaveDoc.Remarks = (string)config["Reason_for_leave"];
                        LeaveDoc.Status = (string)config["Status"];
                    }

                }
                decimal availableDays = 0;
                List<DropDownBalance> Lvbal = new List<DropDownBalance>();
                #region LeaveBal
                decimal[] s = CommonClass.GetLeaveBal(StaffNo, LeaveDoc.Leave_Type);
                #endregion

                if (LeaveDoc.Leave_Type.Contains("ANNUAL"))
                {
                    availableDays = (s[1] + s[0] + s[4]) - Math.Abs(s[2]);
                }
                else
                {
                    availableDays = s[0];
                }
                if (availableDays < Convert.ToDecimal(LeaveDoc.Applied_Days))
                {
                    availableDays = Convert.ToDecimal(LeaveDoc.Applied_Days);
                }
                for (int i = 1; i <= availableDays; i++)
                {
                    DropDownBalance NewV = new DropDownBalance();
                    NewV.Code = i.ToString();
                    Lvbal.Add(NewV);
                }

                LeaveDocumentDetails lvDoc = new LeaveDocumentDetails
                {
                    DocumentDetails = LeaveDoc,
                    ListOfRelievers = relieverList.Select(x =>
                                             new SelectListItem()
                                             {
                                                 Text = x.Name,
                                                 Value = x.No
                                             }).ToList(),
                    ListOfDays = Lvbal.Select(x =>
                                         new SelectListItem()
                                         {
                                             Text = x.Code,
                                             Value = x.Code
                                         }).ToList()
                };
                return PartialView("~/Views/Leave/LeaveDocumentView.cshtml", lvDoc);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public JsonResult SendLeaveAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.HRLeaveApprovalRequest(DocNo);

                return Json(new { message = "Leave Application send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult RecallLeaveAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.HRCancelLeaveApplication(DocNo, true, false);

                return Json(new { message = "Leave Application approval Recalled Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CancelLeaveAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.HRCancelLeaveApplication(DocNo, false, true);

                return Json(new { message = "Leave Application approval cancelled Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult LeavePlannerList()
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
        public PartialViewResult LeavePlannerListPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<LeavePlanner> LeaveList = new List<LeavePlanner>();

                string page = "HRLeavePlanner?$filter=Employee_No eq '" + StaffNo + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        LeavePlanner LvP = new LeavePlanner();
                        LvP.DocNo = (string)config["Application_Code"];
                        LvP.EmpNo = (string)config["Employee_No"];
                        LvP.EmpName = (string)config["Employee_No"];
                        LvP.JobTitle = (string)config["Job_Tittle"];
                        LvP.Dim1 = (string)config["Global_Dimension_1_Code"];
                        LvP.Dim2 = (string)config["Shortcut_Dimension_2_Code"];
                        LvP.HRCalender = (string)config["Calendar_Code"];
                        LvP.Status = (string)config["Status"];
                        LvP.LineNo = (string)config["Status"];
                        LeaveList.Add(LvP);
                    }

                }
                return PartialView("~/Views/Leave/LeavePlannerListPartialView.cshtml", LeaveList.OrderByDescending(x => x.DocNo));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult NewLeavePlanner()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                string DocNo = Credentials.ObjNav.CreateLeavePlannerHeader(StaffNo);
                return Json(new { message = "/Leave/LeavePlannerDoument?DocNo=" + DocNo, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult LeavePlannerDoument(string DocNo)
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
                    LeavePlanner LvP = new LeavePlanner();

                    #region Dim Captions
                    Dimension_Caption Dimcap = new Dimension_Caption();
                    string pageDimC= "GeneralLedgerSetup?$format=json";

                    HttpWebResponse httpResponseDinC = Credentials.GetOdataData(pageDimC);
                    using (var streamReader = new StreamReader(httpResponseDinC.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);

                        foreach (JObject config in details["value"])
                        {
                            Dimcap.Dim1_Caption = (string)config["Global_Dimension_1_Code"];
                            Dimcap.Dim2_Caption = (string)config["Global_Dimension_2_Code"];
                            Dimcap.Dim3_Caption = (string)config["Shortcut_Dimension_3_Code"];
                            Dimcap.Dim4_Caption = (string)config["Shortcut_Dimension_4_Code"];
                            Dimcap.Dim5_Caption = (string)config["Shortcut_Dimension_5_Code"];
                            Dimcap.Dim6_Caption = (string)config["Shortcut_Dimension_6_Code"];
                        }
                    }
                    #endregion
                    #region Doc Header
                    string page = "HRLeavePlanner?$filter=Application_Code eq '" + DocNo + "'&$format=json";
                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        foreach (JObject config in details["value"])
                        {
                            LvP.DocNo = (string)config["Application_Code"];
                            LvP.EmpNo = (string)config["Employee_No"];
                            LvP.EmpName = (string)config["Employee_No"];
                            LvP.JobTitle = (string)config["Job_Tittle"];
                            LvP.Dim1 = (string)config["Global_Dimension_1_Code"];
                            LvP.Dim2 = (string)config["Shortcut_Dimension_2_Code"];
                            LvP.HRCalender = (string)config["Calendar_Code"];
                            LvP.Status = (string)config["Status"];
                        }
                    }
                    #endregion
                    #region Directorate List
                    List<DimensionValues> DirectorateList = new List<DimensionValues>();
                    string pageDir = "DimensionValues?$filter=Dimension_Code eq 'DIRECTORATES'&$format=json";

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
                    LvP.ListOfDirectorate = DirectorateList.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Name,
                                              Value = x.Code
                                          }).ToList();
                    LvP.ListOfDepartment = DepartmentList.Select(x =>
                                  new SelectListItem()
                                  {
                                      Text = x.Name,
                                      Value = x.Code
                                  }).ToList();

                    LvP.ListOfDimcaption = Dimcap;

                    return View(LvP);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public ActionResult LoadLeavePlannerLinesList(string DocNo)
        {
            try
            {
                List<LeavePlannerLines> HRLvPLines = new List<LeavePlannerLines>();
                string page = "HRLeavePlannerLines?$filter=Application_Code eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    if (details["value"].Count() > 0)
                    {
                        foreach (var config in details["value"])
                        {
                            LeavePlannerLines PLine = new LeavePlannerLines();
                            PLine.Line_No = (string)config["Line_No"];
                            PLine.LeaveType = (string)config["Leave_Type"];
                            PLine.DaysApplied = (string)config["Days_Applied"];
                            PLine.StartDate = ((DateTime)config["Start_Date"]).ToString("dd/MM/yyyy");
                            PLine.EndDate = ((DateTime)config["End_Date"]).ToString("dd/MM/yyyy");
                            PLine.ReturnDate = ((DateTime)config["Return_Date"]).ToString("dd/MM/yyyy");
                            PLine.Comments = (string)config["Applicant_Comments"];
                            HRLvPLines.Add(PLine);
                        }
                    }
                }
                return PartialView("~/Views/Leave/LeavePlannerLines.cshtml", HRLvPLines);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewLeavePlanLine()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                NewLeavePlanForm NewLvPForm = new NewLeavePlanForm();

                #region LeaveTypes
                List<LvTypes> leaveTps = new List<LvTypes>();

                string gender = CommonClass.GetEmployeeGender(StaffNo);
                if (string.IsNullOrEmpty(gender.Trim()))
                {
                    Error erroMsg = new Error();
                    erroMsg.Message = "Your gender has not been set. Contact HR";
                    return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
                }
                else
                {
                    string page = "LeaveTypes?$filter=(Gender eq 'Both' or Gender eq '" + gender + "')&format=json";

                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        foreach (JObject config in details["value"])
                        {
                            LvTypes LTpe = new LvTypes();
                            LTpe.Code = (string)config["Code"];
                            LTpe.Description = (string)config["Description"];
                            leaveTps.Add(LTpe);
                        }
                    }
                }
                #endregion
                NewLvPForm = new NewLeavePlanForm
                {
                    Code = "",
                    ListOfLeaveTypes = leaveTps.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Description,
                                              Value = x.Code
                                          }).OrderBy(x => x.Text).ToList()
                };

                return PartialView("~/Views/Leave/NewLeavePlanForm.cshtml", NewLvPForm);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitLeavePlannerApplication(string DocNo, LeavePlannerLines NewApp)
        {
            try
            {
                DateTime startDate = DateTime.ParseExact(NewApp.StartDate.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.ParseExact(NewApp.EndDate.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime returndate = DateTime.ParseExact(NewApp.ReturnDate.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                string Remarks = "";
                if (NewApp.Comments != null)
                {
                    Remarks = NewApp.Comments;
                }
                Credentials.ObjNav.InsertLeavePlannerLines(DocNo, NewApp.LeaveType, Convert.ToDecimal(NewApp.DaysApplied), startDate, endDate, returndate, Remarks);
                return Json(new { message = "Record added successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}