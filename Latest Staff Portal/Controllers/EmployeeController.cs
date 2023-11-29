using Latest_Staff_Portal.CustomSecurity;
using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Latest_Staff_Portal.Controllers
{
    [CustomeAuthentication]
    [CustomAuthorization(Role = "ALLUSERS")]
    public class EmployeeController : Controller
    {

        public ActionResult EmployeeRequestList()
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

        public PartialViewResult EmployeeListPartialView()
        {
            string StaffNo = Session["Username"].ToString();
            List<EmployeeRequisitionList> EmployeeLists = new List<EmployeeRequisitionList>();

            string page = "HREmp_Requisition?$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (var jToken in details["value"])
                {
                    var config = (JObject)jToken;
                    EmployeeRequisitionList EmployeeList = new EmployeeRequisitionList
                    {
                        RequisitionNo = config["RequisitionNo"]?.ToString(),
                        JobDescription = config["JobDescription"]?.ToString(),
                        Contract = config["Contract"]?.ToString(),
                        Status = config["Status"]?.ToString()
                    };
                    EmployeeLists.Add(EmployeeList);
                }
            }

            return PartialView("~/Views/Employee/EmployeeListPartialView.cshtml",
                EmployeeLists.OrderByDescending(x => x.RequisitionDate));
        }

        public ActionResult NewEmployeeRequest()
        {
            try
            {
                if (Session["Username"] == null)
                {
                    return RedirectToAction("Login", "Login");
                }

                string StaffNo = Session["Username"].ToString();
                NewEmployeeRequisition NewAppl = new NewEmployeeRequisition();

                #region TypeOfContracts

                List<DropdownList> contactsList = new List<DropdownList>();
                string pageContacts = "LookUp_Values?$filter=Type eq 'Contract Type'&format=json";

                HttpWebResponse httpResponseContract = Credentials.GetOdataData(pageContacts);
                using (var streamReaderContract = new StreamReader(httpResponseContract.GetResponseStream()))
                {
                    var resultContract = streamReaderContract.ReadToEnd();

                    var details = JObject.Parse(resultContract);


                    foreach (var jToken in details["value"])
                    {
                        var config = (JObject)jToken;
                        DropdownList CList = new DropdownList();
                        CList.Value = (string)config["Code"];
                        CList.Text = (string)config["Description"];
                        contactsList.Add(CList);
                    }
                }

                #endregion

                #region Jobs

                List<DropdownList> hrJobs = new List<DropdownList>();
                string pageJobs = "HrJobsList?$format=json";

                HttpWebResponse httpResponseJobs = Credentials.GetOdataData(pageJobs);
                using (var streamReader = new StreamReader(httpResponseJobs.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (var jToken in details["value"])
                    {
                        var config = (JObject)jToken;
                        DropdownList hrListJobs = new DropdownList
                        {
                            Value = (string)config["JobID"],
                            Text = (string)config["JobDescription"]
                        };
                        hrJobs.Add(hrListJobs);
                    }
                }

                #endregion

                NewAppl = new NewEmployeeRequisition
                {
                    ListOfJobs = hrJobs.Select(x => new SelectListItem()
                    {
                        Text = x.Text,
                        Value = x.Value
                    }).ToList(),
                    ListOfContracts = contactsList.Select(x =>
                        new SelectListItem()
                        {
                            Text = x.Text,
                            Value = x.Value
                        }).ToList()
                };

                return View(NewAppl);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }

        // [AcceptVerbs(HttpVerbs.Post)]
        //public JsonResult SubmitEmployeeRequest(NewEmployeeRequisition newEmployeeRequisition)
        // {
        //     try
        //     {
        //         string StaffNo = Session["Username"].ToString();
        //
        //         DateTime LastDayOfService = DateTime.ParseExact(Employee.LastDateOfService.Replace("-", "/"),
        //             "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //         string DocNo = Credentials.ObjNav.EmployeeRequest(Employee.StaffNo,
        //             Employee.PhoneNumber, Employee.Email, Employee.Address,
        //            Employee.ReasonForClearing, LastDayOfService);
        //         if (DocNo != "")
        //         {
        //             return Json(
        //                 new
        //                 {
        //                     //message = "Clearance Requisition, Document No: " + DocNo + ", created Successfully.",
        //                     success = true
        //                 }, JsonRequestBehavior.AllowGet);
        //         }
        //
        //         return Json(new { message = "Document not created. Please try again later...", success = false },
        //             JsonRequestBehavior.AllowGet);
        //     }
        //     catch (Exception ex)
        //     {
        //         return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
        //     }
        // }

        public PartialViewResult EmployeeDocumentView(string docNo)
        {
            _ = Session["Username"].ToString();
            EmployeeRequisitionDocumentView EmployeeLists = new EmployeeRequisitionDocumentView();

            string page = "HREmployeeRequisitionCard?$filter=Code eq '" + docNo + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (var jToken in details["value"])
                {
                    var config = (JObject)jToken;
                    EmployeeLists.RequisitionNo = config["Requisition_No"]?.ToString();
                    EmployeeLists.JobId = config["Job_Id"]?.ToString();
                    EmployeeLists.JobDescription = config["Job_Description"]?.ToString();
                    EmployeeLists.ReasonForRequest = config["Reason_For_Request"]?.ToString();
                    EmployeeLists.RequiredPositions = Convert.ToInt32(config["Required_Positions"]);
                    EmployeeLists.TypeOfContractRequired = config["Type_of_Contract_Required"]?.ToString();
                    EmployeeLists.Status = config["Status"]?.ToString();
                    EmployeeLists.Closed = Convert.ToBoolean(config["Closed"]);
                }
            }

            return PartialView("~/Views/Employee/EmployeeDocumentView.cshtml", EmployeeLists);
        }
    }
}