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

            //string page = "TransportReqList?$filter=Employee_No eq '" + StaffNo + "'&format=json";
            string page = "TransportReqList?$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    TransportReqList TrList = new TransportReqList();
                    TrList.No = (string)config["TransportRequisitionNo"];
                    TrList.Commencement = (string)config["Commencement"];
                    TrList.Destination = (string)config["Destination"];
                    TrList.Vehicle = (string)config["VehicleAllocated"];
                    TrList.Driver = (string)config["DriverAllocated"];
                    TrList.DateRequested = Convert.ToDateTime((string)config["DateofRequest"]).ToString("dd/MM/yyyy");
                    TrList.NoOfDays = (string)config["NoofDaysRequested"];
                    TrList.Status = (string)config["Status"];
                    TransportList.Add(TrList);
                }
            }
            return PartialView("~/Views/Transport/TRListView.cshtml", TransportList);
        }
        public PartialViewResult NewTransportApplication()
        {
            string StaffNo = Session["Username"].ToString();
            NewTransportRequisition NewAppl = new NewTransportRequisition();
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

            NewAppl = new NewTransportRequisition
            {
                ListOfResponsibility = RespCList.Select(x =>
                                   new SelectListItem()
                                   {
                                       Text = x.Name,
                                       Value = x.Code
                                   }).ToList()
            };
            return PartialView("~/Views/Transport/NewTransportRequisition.cshtml", NewAppl);
        }
        public JsonResult SubmitTransportDocument(NewTransportDocument NewApp)
        {
            try
            {
                DateTime DateTrip = DateTime.ParseExact(NewApp.DateTrip.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                Credentials.ObjNav.TransportRequisitionCreate(Session["username"].ToString(), NewApp.Destination, NewApp.Commencement, DateTrip,
                    NewApp.Purpose, Convert.ToInt32(NewApp.NoOfDays), 0, 0, 0, NewApp.RespC);

                //string Redirect = "/Appraisal/ScoreCardAppraisal?AppDoc=" + DocNo;
                string Redirect = "";

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

                string page = "TransportReqList?$filter=TransportRequisitionNo eq '" + AppDoc + "'&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        TransDoc.No = (string)config["TransportRequisitionNo"];
                        TransDoc.Commencement = (string)config["Commencement"];
                        TransDoc.Destination = (string)config["Destination"];
                        TransDoc.Vehicle = (string)config["VehicleAllocated"];
                        TransDoc.Driver = (string)config["DriverAllocated"];
                        TransDoc.DateRequested = Convert.ToDateTime((string)config["DateofRequest"]).ToString("dd/MM/yyyy");
                        TransDoc.NoOfDays = (string)config["NoofDaysRequested"];
                        TransDoc.Status = (string)config["Status"];
                    }
                }
                return View(TransDoc);
            }
        }
        public JsonResult SendDocAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.TravelRequisitionApprovalRequest(DocNo);
                return Json(new { message = "Leave Application send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}