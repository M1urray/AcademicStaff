using Latest_Staff_Portal.CustomSecurity;
using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.Controllers
{
    [CustomeAuthentication]
    [CustomAuthorization(Role = "FULLTIME")]
    public class StaffClearanceController : Controller
    {
        // GET: StaffClearance
        public ActionResult ExitInterview()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                ExitInterview ExtInterview = new ExitInterview();
                ExtInterview.No = "";
                ExtInterview.Q1 = "";
                ExtInterview.Q2 = "";
                ExtInterview.Q3 = "";
                ExtInterview.Q4 = "";
                ExtInterview.Q5 = "";
                ExtInterview.Q6 = "";
                ExtInterview.Q7 = "";
                ExtInterview.Q8 = "";
                ExtInterview.Q9 = "";
                ExtInterview.Q10 = "";
                ExtInterview.Q11 = "";
                ExtInterview.Q12 = "";
                ExtInterview.Q13 = "";
                ExtInterview.Q14 = "";
                return View(ExtInterview);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }

        public ActionResult StaffClearanceRequestList()
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

        public PartialViewResult StaffClearanceListPartialView()
        {
            string StaffNo = Session["Username"].ToString();
            List<StaffClearanceList> staffClearanceLists = new List<StaffClearanceList>();

            string page = "StaffClearanceCard?$filter=Staff_No eq '" + StaffNo + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    StaffClearanceList staffClearanceList = new StaffClearanceList();
                    staffClearanceList.No = config["Code"].ToString();
                    staffClearanceList.AppliedDate = config["Date"].ToString();
                    staffClearanceList.StaffName = config["Staff_Names"].ToString();
                    staffClearanceList.EffectiveDate = config["Effective_Date"].ToString();
                    staffClearanceList.Status = config["Status"].ToString();
                    staffClearanceLists.Add(staffClearanceList);
                }
            }

            return PartialView("~/Views/StaffClearance/StaffClearanceListPartialView.cshtml",
                staffClearanceLists.OrderByDescending(x => x.No));
        }

        public PartialViewResult NewStaffClearanceRequest()
        {
            string StaffNo = Session["Username"].ToString();
            StaffClearance clearanceList = new StaffClearance();
            string page = "EmployeeList?$filter=No eq '" + StaffNo + "'&format=json";
            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    clearanceList.StaffNo = StaffNo;
                    clearanceList.Department = config["Department_Code"].ToString();
                    clearanceList.DepartmentName = config["Department_Name"].ToString();
                    clearanceList.DateOfAppointment = config["Date_Of_Join"].ToString();
                    clearanceList.StaffName = (config["First_Name"] + " " + config["Middle_Name"] + " " +
                                               config["Last_Name"]);
                    clearanceList.IDNo = config["ID_Number"].ToString();
                    clearanceList.Email = config["E_Mail"].ToString();
                    clearanceList.PhoneNumber = config["Cellular_Phone_Number"].ToString();
                    clearanceList.Address = config["Post_Office_No"].ToString();
                }
            }

            return PartialView("~/Views/StaffClearance/NewStaffClearanceRequest.cshtml", clearanceList);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitClearanceRequest(StaffClearance staffClearance)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();

                DateTime LastDayOfService = DateTime.ParseExact(staffClearance.LastDateOfService.Replace("-", "/"),
                    "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string DocNo = Credentials.ObjNav.StaffClearanceRequest(staffClearance.StaffNo,
                    staffClearance.PhoneNumber, staffClearance.Email, staffClearance.Address,
                    staffClearance.ReasonForClearing, LastDayOfService);
                if (DocNo != "")
                {
                    return Json(
                        new
                        {
                            message = "Clearance Requisition, Document No: " + DocNo + ", created Successfully.",
                            success = true
                        }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { message = "Document not created. Please try again later...", success = false },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public PartialViewResult StaffClearanceDocumentView(string DocNo)
        {
            string StaffNo = Session["Username"].ToString();
            StaffClearanceList staffClearanceLists = new StaffClearanceList();

            string page = "StaffClearanceCard?$filter=Code eq '" + DocNo + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    staffClearanceLists.No = config["Code"].ToString();
                    staffClearanceLists.AppliedDate = config["Date"].ToString();
                    staffClearanceLists.StaffName = config["Staff_Names"].ToString();
                    staffClearanceLists.EffectiveDate = config["Effective_Date"].ToString();
                    staffClearanceLists.Status = config["Status"].ToString();
                }
            }

            return PartialView("~/Views/StaffClearance/StaffClearanceDocumentView.cshtml", staffClearanceLists);
        }
    }
}