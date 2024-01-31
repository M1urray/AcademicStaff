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
    public class TransportController : Controller
    {
        // GET: Transport
        public ActionResult TransportRequisitionList()
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
        public PartialViewResult TransportRequisitionListPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<TransportReqList> TransportList = new List<TransportReqList>();

                //string page = "TransportReqList?$filter=Employee_No eq '" + StaffNo + "'&$format=json";
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
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewTransportApplication()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                NewTransportRequisition NewAppl = new NewTransportRequisition();
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
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public JsonResult SubmitTransportDocument(NewTransportDocument NewApp, string base64Upload, string fileName, string Extn)
        {
            try
            {
                DateTime DateTrip = DateTime.ParseExact(NewApp.DateTrip.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //Credentials.ObjNav.TransportRequisitionCreate(Session["username"].ToString(), NewApp.Destination, NewApp.Commencement, DateTrip,
                //    NewApp.Purpose, Convert.ToInt32(NewApp.NoOfDays), 0, 0, 0, NewApp.RespC);

                //string Redirect = "/Appraisal/ScoreCardAppraisal?AppDoc=" + DocNo;
                string Redirect = "";
                //if (base64Upload != "")
                //{
                //    string filePath = Server.MapPath("~/Uploads/" + fileName);
                //    CommonClass.MoveUploadedFile(filePath, fileName);
                //    string UploadFilePath = Credentials.fileUploadsPath + fileName;
                //    if (CommonClass.IfFileExists(UploadFilePath))
                //    {
                //        string s = Credentials.UploadDocumentAttachment(DocNo, base64Upload, UploadFilePath, 38);
                //        if (s == "SUCCESS")
                //        {
                //            Session["SuccessMsg"] = "Purchase Requisition, Document No: " + DocNo + ", Submitted Successfully and attachment File Uploaded Successfully";
                //        }
                //        else
                //        {
                //            Session["SuccessMsg"] = "Purchase Requisition, Document No: " + DocNo + ", Submitted Successfully but error encountered while uploading attachment" +
                //                "Error encountered :" + s;
                //        }
                //    }
                //}

                return Json(new { message = Redirect, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult TransportDocumentDetails(string DocNo)
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
                    TransportReqList TransDoc = new TransportReqList();

                    string page = "TransportReqList?$filter=TransportRequisitionNo eq '" + DocNo + "'&$format=json";

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
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public JsonResult SendDocAppForApproval(string DocNo)
        {
            try
            {
                //Credentials.ObjNav.TravelRequisitionApprovalRequest(DocNo);
                return Json(new { message = "Leave Application send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}