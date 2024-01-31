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
                //string page = "PurchaseRequisition?$filter=Employee_No eq '" + StaffNo + "'&$format=json";
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

            NewClearanceRequisition NewCLR = new NewClearanceRequisition();
            Session["httpResponse"] = null;
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

            StaffClearance clearance = new StaffClearance();
            clearance.StaffNo = "";
            clearance.StaffName = "";
            NewCLR = new NewClearanceRequisition
            {
                StaffClearanceDoc = clearance,
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
                                   }).ToList()
            };
            return View(NewCLR);
        }
    }
}