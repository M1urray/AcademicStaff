using Latest_Staff_Portal.CustomSecurity;
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
                //string page = "PurchaseRequisition?$filter=Employee_No eq '" + StaffNo + "'&format=json";
                //HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                //{
                //    var result = streamReader.ReadToEnd();

                //    var details = JObject.Parse(result);
                //    if (details["value"].Count() > 0)
                //    {
                //        foreach (JObject config in details["value"])
                //        {
                //            ExtInterview.No = "";
                //            ExtInterview.Q1 = "";
                //            ExtInterview.Q2 = "";
                //            ExtInterview.Q3 = "";
                //            ExtInterview.Q4 = "";
                //            ExtInterview.Q5 = "";
                //            ExtInterview.Q6 = "";
                //            ExtInterview.Q7 = "";
                //            ExtInterview.Q8 = "";
                //            ExtInterview.Q9 = "";
                //            ExtInterview.Q10 = "";
                //            ExtInterview.Q11 = "";
                //            ExtInterview.Q12 = "";
                //            ExtInterview.Q13 = "";
                //            ExtInterview.Q14 = "";
                //        }
                //    }
                //    else
                //    {
                //        ExtInterview.No = "";
                //        ExtInterview.Q1 = "";
                //        ExtInterview.Q2 = "";
                //        ExtInterview.Q3 = "";
                //        ExtInterview.Q4 = "";
                //        ExtInterview.Q5 = "";
                //        ExtInterview.Q6 = "";
                //        ExtInterview.Q7 = "";
                //        ExtInterview.Q8 = "";
                //        ExtInterview.Q9 = "";
                //        ExtInterview.Q10 = "";
                //        ExtInterview.Q11 = "";
                //        ExtInterview.Q12 = "";
                //        ExtInterview.Q13 = "";
                //        ExtInterview.Q14 = "";
                //    }
                //}
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
        public ActionResult StaffClearance()
        {
            string StaffNo = Session["Username"].ToString();
            StaffClearance clearance = new StaffClearance();
            clearance.StaffNo = StaffNo;
            clearance.StaffName = "";
            clearance.IDNo = StaffNo;
            clearance.Department = "";
            clearance.DepartmentName = StaffNo;
            return View();
        }
    }
}