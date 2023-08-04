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
    [CustomAuthorization(Role = "ALLUSERS")]
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Dashboard()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                try
                {
                    string StaffNo = Session["Username"].ToString();
                    EmployeeView EmpView = new EmployeeView();
                    string page = "EmployeeList?$filter=No eq '" + StaffNo + "'&$format=json";

                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);

                        foreach (JObject config in details["value"])
                        {
                            EmpView.No = (string)config["No"];
                            EmpView.Name = (string)config["FirstName"] + " " + (string)config["MiddleName"] + " " + (string)config["LastName"];
                            EmpView.IDNo = (string)config["IDNumber"];
                            EmpView.Gender = (string)config["Gender"];
                            EmpView.MaritalStatus = (string)config["MaritalStatus"];
                            EmpView.Nationality = (string)config["Citizenship"];
                            EmpView.County = (string)config["CountyName"];
                            EmpView.DoB = (string)config["DateOfBirth"];
                            EmpView.Address1 = config["PostalAddress"].ToString();
                            EmpView.City = config["City"].ToString();
                            EmpView.PostalCode = config["PostCode"].ToString();
                            EmpView.HomeTelNo = config["HomePhoneNumber"].ToString();
                            EmpView.PhoneNo = config["CellPhoneNumber"].ToString();
                            EmpView.CompanyEmail = config["CompanyEMail"].ToString();
                            EmpView.PersonalEmail = config["EMail"].ToString();
                            EmpView.JobTitle = config["JobTitle"].ToString();
                            EmpView.EmpStatus = config["Status"].ToString();
                            decimal[] s = CommonClass.GetLeaveBal(StaffNo, "ANNUAL");
                            EmpView.AllocatedDays = s[0].ToString();
                            EmpView.CarryForawrd = s[1].ToString();
                            EmpView.LeaveTaken = s[2].ToString();
                            // EmpView.EarnedLeaveDays = s[3].ToString();
                            EmpView.ReimbDays = s[4].ToString();
                            EmpView.LeaveBal = (s[1] + s[4] + s[0] - Math.Abs(s[2])).ToString();
                            EmpView.PinNo = config["PINNo"].ToString();
                            EmpView.NSSFNo = config["NSSFNo"].ToString();
                            EmpView.NHIFNo = config["NHIFNo"].ToString();
                            EmpView.Bank = config["Bank_Name"].ToString();
                            EmpView.Branch = config["Branch_Name"].ToString();
                            EmpView.AccountNo = config["Bank_Account_Number"].ToString();
                        }
                    }

                    return View(EmpView);
                }
                catch (Exception ex)
                {
                    Error erroMsg = new Error();
                    erroMsg.Message = ex.Message;
                    return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
                }
            }
        }
        public PartialViewResult ProfilePicture(string gender)
        {
            EmployeeView EmpView = new EmployeeView();
            EmpView.Gender = gender;
            return PartialView("~/Views/Dashboard/ProfilePic.cshtml", EmpView);
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
                    msg = ex.Message.Replace("'","");
                    successVal = false;
                }
            }
            return Json(new { message = msg, success = successVal }, JsonRequestBehavior.AllowGet);
        }
    }
}