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
    [CustomAuthorization(Role = "FULLTIME,PARTTIME")]
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Dashboard()
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
                    EmployeeView EmpView = new EmployeeView();
                    string page = "EmployeeList?$filter=No eq '" + StaffNo + "'&$format=json";
                    decimal[] s = CommonClass.GetLeaveBal(StaffNo, "ANNUAL");
                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);

                        foreach (JObject config in details["value"])
                        {
                            EmpView = new EmployeeView
                            {
                                No = (string)config["No"],
                                Name = (string)config["First_Name"] + " " + (string)config["Middle_Name"] + " " + (string)config["Last_Name"],
                                IDNo = (string)config["ID_Number"],
                                Gender = (string)config["Gender"],
                                MaritalStatus = (string)config["Marital_Status"],
                                Nationality = (string)config["Country_Name"],
                                County = (string)config["County"],
                                DoB = (string)config["DateOfBirth"],
                                Address1 = config["Postal_Address"].ToString(),
                                Address2 = config["Postal_Address2"].ToString(),
                                City = config["City"].ToString(),
                                PostalCode = config["Post_Code"].ToString(),
                                HomeTelNo = config["Home_Phone_Number"].ToString(),
                                PhoneNo = config["Cellular_Phone_Number"].ToString(),
                                WorkTel = config["Work_Phone_Number"].ToString(),
                                CompanyEmail = config["Company_E_Mail"].ToString(),
                                PersonalEmail = config["E_Mail"].ToString(),
                                DateOfJoin = config["Date_Of_Join"].ToString(),
                                ProbationDate = config["Probation_Start_Date"].ToString(),
                                PenSchemeJoinDate = config["Pension_Scheme_Join"].ToString(),
                                JobTitle = config["Job_Title"].ToString(),
                                EmpStatus = config["Status"].ToString(),
                                JobCat = "",
                                Department = config["Department_Name"].ToString(),
                                Campus = config["Campus"].ToString(),
                                Bank = config["Main_Bank"].ToString(),
                                Branch = config["Branch_Bank"].ToString(),
                                AccountNo = config["Bank_Account_Number"].ToString(),
                                PinNo = config["PIN_Number"].ToString(),
                                NSSFNo = config["NSSF_No"].ToString(),
                                NHIFNo = config["NHIF_No"].ToString(),
                                AllocatedDays = s[0].ToString(),
                                CarryForawrd = s[1].ToString(),
                                ReimbDays = s[2].ToString(),
                                LeaveTaken = s[3].ToString(),
                                LeaveBal = s[4].ToString()
                            };
                        }
                    }
                    return View(EmpView);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult ProfilePicture(string gender)
        {
            try
            {
                EmployeeView EmpView = new EmployeeView();
                EmpView.Gender = gender;
                return PartialView("~/Views/Dashboard/ProfilePic.cshtml", EmpView);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public JsonResult SubmitProfilePicAttachment(string base64Upload, string fileName, string Extn)
        {
            bool successVal = false;
            string msg = "";
            if (base64Upload != "")
            {
                try
                {
                    string StaffNo = Session["Username"].ToString();
                    string filePath = Server.MapPath("~/Uploads/" + fileName);

                    Credentials.UploadProfilePic(StaffNo, base64Upload, filePath, fileName);
                    Session["ImgProfile"] = null;
                    msg = "Uploaded successfully";
                    successVal = true;
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    successVal = false;
                }
            }
            return Json(new { message = msg, success = successVal }, JsonRequestBehavior.AllowGet);
        }
    }
}