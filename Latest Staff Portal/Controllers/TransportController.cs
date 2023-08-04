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
    public class TransportController : Controller
    {
        // GET: Transport
        public ActionResult TransportRequisitionList()
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
        public PartialViewResult TransportRequisitionListPartialView()
        {
            string StaffNo = Session["Username"].ToString();
            List<TransportReqList> TransportList = new List<TransportReqList>();

            string page = "TransportReqList?$filter=Empoyee_No eq '" + StaffNo + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    TransportReqList TrList = new TransportReqList();
                    TrList.No = (string)config["Transport_Requisition_No"];
                    TrList.Commencement = (string)config["From"];
                    TrList.Destination = (string)config["To"];
                    TrList.Vehicle = (string)config["Vehicle_Allocated"];
                    TrList.Driver = (string)config["Driver_Allocated"];
                    TrList.DateRequested = Convert.ToDateTime((string)config["Date_of_Request"]).ToString("dd/MM/yyyy");
                    TrList.DateOfTrip = Convert.ToDateTime((string)config["Date_of_Trip"]).ToString("dd/MM/yyyy");
                    TrList.NoOfDays = (string)config["No_of_Days_Requested"];
                    TrList.Status = (string)config["Status"];
                    TransportList.Add(TrList);
                }
            }
            return PartialView("~/Views/Transport/Partial Views/TRListView.cshtml", TransportList);
        }
        public PartialViewResult NewTransportApplication()
        {
            string StaffNo = Session["Username"].ToString();
            NewTransportRequisition NewAppl = new NewTransportRequisition();
            string Dir = "", Dep = "";
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
                    }
                }
            }
            #endregion
            if (Dir == "")
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

                NewAppl = new NewTransportRequisition
                {
                    ListOfResponsibility = RespCList.Select(x =>
                                       new SelectListItem()
                                       {
                                           Text = x.Name,
                                           Value = x.Code
                                       }).ToList()
                };
                return PartialView("~/Views/Transport/Partial Views/NewTransportRequisition.cshtml", NewAppl);
            }
        }
        [HttpPost]
        public JsonResult SubmitTransportDocument(NewTransportDocument NewApp)
        {
            try
            {
                DateTime DateTrip = DateTime.ParseExact(NewApp.DateTrip.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string DocNo = Credentials.ObjNav.TransportRequisitionCreate(Session["username"].ToString(), NewApp.Destination, NewApp.Commencement, DateTrip,
                    NewApp.Purpose, Convert.ToInt32(NewApp.NoOfDays), 0, 0, 0,0,new DateTime(0));

                string Redirect = "/Transport/TransportDocumentDetails?AppDoc=" + DocNo;

                return Json(new { message = Redirect, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult TransportDocumentDetails(string AppDoc)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                string StaffNo = Session["Username"].ToString();
                TransportReqList TransDoc = new TransportReqList();

                string page = "TransportReqList?$filter=Transport_Requisition_No eq '" + AppDoc + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        TransDoc.No = (string)config["Transport_Requisition_No"];
                        TransDoc.Commencement = (string)config["From"];
                        TransDoc.Destination = (string)config["To"];
                        TransDoc.Vehicle = (string)config["Vehicle_Allocated"];
                        TransDoc.Driver = (string)config["Driver_Allocated"] + "(" + CommonClass.GetEmployeeName((string)config["Driver_Allocated"]) + ")";
                        TransDoc.DateOfTrip = Convert.ToDateTime((string)config["Date_of_Trip"]).ToString("dd/MM/yyyy");
                        TransDoc.NoOfDays = (string)config["No_of_Days_Requested"];
                        TransDoc.Status = (string)config["Status"];
                    }
                }
                return View(TransDoc);
            }
        }
        public PartialViewResult TransportPassengers(string DocNo, string Status)
        {
            try
            {
                #region Passenger Lines
                List<Passengers> PassengerList = new List<Passengers>();
                string pageLine = "TransportPassengers?$filter=Req_No eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        Passengers passenger = new Passengers();
                        passenger.Type = (string)config["Passenger_Type"];
                        passenger.No = (string)config["No"];
                        passenger.Name = (string)config["Name"];
                        passenger.Position = (string)config["Position"];
                        PassengerList.Add(passenger);
                    }
                }
                #endregion
                PassengerList Lines = new PassengerList
                {
                    Status = Status,
                    ListOfPassengers = PassengerList
                };
                return PartialView("~/Views/Transport/Partial Views/PassengerList.cshtml", Lines);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewPassgerForm()
        {
            string StaffNo = Session["Username"].ToString();
            Passengers NewAppl = new Passengers();
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

            NewAppl.ListOfEmployee = EmployeeList.Select(x =>
                                           new SelectListItem()
                                           {
                                               Text = x.Text,
                                               Value = x.Value
                                           }).ToList();
            return PartialView("~/Views/Transport/Partial Views/PassengerForm.cshtml", NewAppl);
        }
        [HttpPost]
        public JsonResult SubmitPassengerLine(string DocNo, string PassengerType, string PassengerNo)
        {
            try
            {
                Credentials.ObjNav.InsertTransportReqPassenger(DocNo, Convert.ToInt32(PassengerType), PassengerNo);

                return Json(new { message = "Passenger added successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult RemovePassengerLine(string DocNo, string PassengerNo)
        {
            try
            {
               // Credentials.ObjNav.transp(DocNo, Convert.ToInt32(PassengerType), PassengerNo);

                return Json(new { message = "Passenger removed successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SendDocAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.TransportRequisitionApprovalRequest(DocNo);
                return Json(new { message = "Transport Requisition send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CancelTransportAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.CanceTransportRequisition(DocNo);
                return Json(new { message = "Transport Requisition approval cancelled Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}