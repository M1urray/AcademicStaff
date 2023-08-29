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
    public class SeparationController : Controller
    {
        // GET: Separation
        public ActionResult SeparationRequestList()
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
        public PartialViewResult SeparationRequestListPartialView()
        {
            string StaffNo = Session["Username"].ToString();
            List<StaffClearanceList> staffClearanceLists = new List<StaffClearanceList>();

            string page = "";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    
                }
            }

            return PartialView("~/Views/Separation/SeparationRequestListPartialView.cshtml",
                staffClearanceLists.OrderByDescending(x => x.No));
        }
        public PartialViewResult NewSeparationRequest()
        {
            // string StaffNo = Session["Username"].ToString();
            // StaffClearance clearanceList = new StaffClearance();
            // string page = "EmployeeList?$filter=No eq '" + StaffNo + "'&format=json";
            // HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            // using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            // {
            //     var result = streamReader.ReadToEnd();
            //     var details = JObject.Parse(result);
            //     foreach (JObject config in details["value"])
            //     {
            //         clearanceList.StaffNo = StaffNo;
            //         clearanceList.Department = config["Department_Code"].ToString();
            //         clearanceList.DepartmentName = config["Department_Name"].ToString();
            //         clearanceList.DateOfAppointment = config["Date_Of_Join"].ToString();
            //         clearanceList.StaffName = (config["First_Name"] + " " + config["Middle_Name"] + " " +
            //                                    config["Last_Name"]);
            //         clearanceList.IDNo = config["ID_Number"].ToString();
            //         clearanceList.Email = config["E_Mail"].ToString();
            //         clearanceList.PhoneNumber = config["Cellular_Phone_Number"].ToString();
            //         clearanceList.Address = config["Post_Office_No"].ToString();
            //     }
            // }

            return PartialView("~/Views/Separation/NewSeparationRequest.cshtml");
        }
       
    }
}