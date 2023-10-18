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
            string StaffNo = Session["Username"].ToString();
            List<LeaveReqList> LeaveList = new List<LeaveReqList>();

            string page = "HRLeaveRequisition?$filter=Employee_No eq '" + StaffNo + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    LeaveReqList LvList = new LeaveReqList();
                    LvList.No = (string)config["No"];
                    LvList.Leave_Type = (string)config["Leave_Type"];
                    LvList.Applied_Days = (string)config["Applied_Days"];
                    LvList.Date = Convert.ToDateTime((string)config["Date"]).ToString("dd/MM/yyyy");
                    LvList.Starting_Date = Convert.ToDateTime((string)config["Starting_Date"]).ToString("dd/MM/yyyy");
                    LvList.End_Date = Convert.ToDateTime((string)config["End_Date"]).ToString("dd/MM/yyyy");
                    LvList.Return_Date = Convert.ToDateTime((string)config["Return_Date"]).ToString("dd/MM/yyyy");
                    LvList.Reliever = (string)config["Reliever_Name"];
                    LvList.Status = (string)config["Status"];
                    LeaveList.Add(LvList);
                }

            }
            return PartialView("~/Views/Leave/LeaveReqListPartialView.cshtml", LeaveList.OrderByDescending(x => x.No));
        }
        
        public PartialViewResult NewLeaveApplication()
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

                string pageReliever = "EmployeeList?&format=json";

                HttpWebResponse httpResponseReliever = Credentials.GetOdataData(pageReliever);
                using (var streamReader = new StreamReader(httpResponseReliever.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        if ((string)config["First_Name"] != "")
                        {
                            RelieverList Rlist = new RelieverList();
                            Rlist.No = (string)config["No"];
                            Rlist.Name = (string)config["First_Name"] + " " + (string)config["Middle_Name"] + " " + (string)config["Last_Name"];
                            relieverList.Add(Rlist);
                        }
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
                #region Campus List
                List<Campus> Campuses = new List<Campus>();
                string pageCampus = "DimValues?$select=Code,Name&$filter=Dimension_Code eq 'CAMPUS' and Blocked eq false&$format=json";

                HttpWebResponse httpResponseCampus = Credentials.GetOdataData(pageCampus);
                using (var streamReader = new StreamReader(httpResponseCampus.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        Campus CmpList = new Campus();
                        CmpList.Code = (string)config["Code"];
                        CmpList.Description = (string)config["Name"];
                        Campuses.Add(CmpList);
                    }
                }
                #endregion
                NewAppl = new NewLeaveApplication
                {
                    LeaveBal = "0",
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
                                         }).ToList(),
                    ListofCampus = Campuses.Select(x =>
                                               new SelectListItem()
                                               {
                                                   Text = x.Description,
                                                   Value = x.Code
                                               }).ToList()
                };
                return PartialView("~/Views/Leave/NewLeaveApplication.cshtml", NewAppl);
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetLeaveBalance(string LvType)
        {
            try
            {
                LeaveBalance newBal = new LeaveBalance();

                #region LeaveBal
                List<DropDownBalance> Lvbal = new List<DropDownBalance>();
                string StaffNo = Session["Username"].ToString();
                string page = "HRLeaveLedger?$filter=EmployeeNo eq '" + StaffNo + "' and LeaveType eq '" + LvType + "'&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                int availableDays = 0;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        availableDays = availableDays + (int)config["NoofDays"];
                    }
                }
                #endregion

                for (int i = 1; i <= availableDays; i++)
                {
                    DropDownBalance NewV = new DropDownBalance();
                    NewV.Code = i.ToString();
                    Lvbal.Add(NewV);
                }

                newBal = new LeaveBalance
                {
                    Balance = availableDays.ToString(),
                    ListOfDays = Lvbal.Select(x =>
                                     new SelectListItem()
                                     {
                                         Text = x.Code,
                                         Value = x.Code
                                     }).ToList()
                };
                return Json(new { newBal, success = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult End_ReturnDates(string startDate, string days, string LvType)
        {
            try
            {
                EndReturnDates ERDate = new EndReturnDates();

                DateTime startD = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //bool isworkingDate = Credentials.ObjNav.DetermineIfIsNonWorking(ref startD, ref LvType);
                //if (isworkingDate)
                //{
                DateTime endDate = Credentials.ObjNav.CalcEndDate(startD, Convert.ToInt32(days), LvType);
                DateTime returndate = Credentials.ObjNav.CalcReturnDate(endDate, LvType);
                ERDate = new EndReturnDates
                {
                    EndDate = endDate.ToString("dd/MM/yyyy"),
                    ReturnDate = returndate.ToString("dd/MM/yyyy")
                };
                return Json(new { ERDate, success = true }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    return Json(new { message="Is not a working date", success = false }, JsonRequestBehavior.AllowGet);
                //}
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitLeaveApplication(LeaveReqList NewApp)
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

                string DocNo = Credentials.ObjNav.HRLeaveApplication(Session["username"].ToString(), NewApp.Leave_Type,
                     Convert.ToDecimal(NewApp.Applied_Days), startDate, endDate, returndate, Remarks, NewApp.Reliever,
                     NewApp.Responsibility, Session["UserID"].ToString());


                Credentials.ObjNav.HRLeaveApprovalRequest(DocNo);
                return Json(new { message = "Leave Application Submitted Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult LeaveDocumentViewView(string DocNo)
        {
            string StaffNo = Session["Username"].ToString();
            LeaveReqList LeaveDoc = new LeaveReqList();

            string page = "HRLeaveRequisition?$filter=No eq '" + DocNo + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    LeaveDoc.No = (string)config["No"];
                    LeaveDoc.EmpNo = (string)config["Employee_No"];
                    LeaveDoc.EmpName = (string)config["Employee_Name"];
                    LeaveDoc.Leave_Type = (string)config["Leave_Type"];
                    LeaveDoc.Applied_Days = (string)config["Applied_Days"];
                    LeaveDoc.Date = Convert.ToDateTime((string)config["Date"]).ToString("dd/MM/yyyy");
                    LeaveDoc.Starting_Date = Convert.ToDateTime((string)config["Starting_Date"]).ToString("dd/MM/yyyy");
                    LeaveDoc.End_Date = Convert.ToDateTime((string)config["End_Date"]).ToString("dd/MM/yyyy");
                    LeaveDoc.Return_Date = Convert.ToDateTime((string)config["Return_Date"]).ToString("dd/MM/yyyy");
                    LeaveDoc.Reliever = (string)config["Reliever_Name"];
                    LeaveDoc.Department = (string)config["Department_Code"];
                    LeaveDoc.Remarks = (string)config["Purpose"];
                    LeaveDoc.Status = (string)config["Status"];
                }

            }
            return PartialView("~/Views/Leave/LeaveDocumentView.cshtml", LeaveDoc);
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
        public JsonResult CancelLeaveAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.HRCancelLeaveApplication_735371815(DocNo);
                return Json(new { message = "Leave Application approval cancelled Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}