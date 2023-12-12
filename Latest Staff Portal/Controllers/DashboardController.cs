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
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                string StaffNo = Session["Username"].ToString();
                EmployeeView EmpView = new EmployeeView();
                string page = "EmployeeList?$filter=No eq '" + StaffNo + "'&format=json";

                if (Session["CurrentSem"] == null || Session["CurrentSem"].ToString() == "")
                {
                    Session["CurrentSem"] = CommonClass.CurrentSemester();
                }

                string sem = Session["CurrentSem"].ToString();
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        EmpView.No = (string)config["No"];
                        EmpView.Name = (string)config["First_Name"] + " " + (string)config["Middle_Name"] + " " + (string)config["Last_Name"];
                        EmpView.IDNo = (string)config["ID_Number"];
                        EmpView.Gender = (string)config["Gender"];
                        EmpView.MaritalStatus = (string)config["Marital_Status"];
                        EmpView.Nationality = (string)config["Citizenship"];
                        EmpView.County = (string)config["County_Name"];
                        EmpView.DoB = Convert.ToDateTime((string)config["Date_Of_Birth"]).ToString("dd/MM/yyyy");
                        EmpView.DateOfJoin = Convert.ToDateTime((string)config["Date_Of_Join"]).ToString("dd/MM/yyyy");
                        EmpView.Address1 = config["Postal_Address"].ToString();
                        EmpView.City = config["City"].ToString();
                        EmpView.PostalCode = config["Post_Code"].ToString();
                        EmpView.HomeTelNo = config["Home_Phone_Number"].ToString();
                        EmpView.PhoneNo = config["Cellular_Phone_Number"].ToString();
                        EmpView.CompanyEmail = config["Company_E_Mail"].ToString();
                        EmpView.PersonalEmail = config["E_Mail"].ToString();
                        EmpView.JobTitle = config["Job_Title"].ToString();
                        EmpView.EmpStatus = config["Status"].ToString();
                        EmpView.Department = config["Department_Name"].ToString();
                        EmpView.JobCat = config["Category"].ToString();
                        EmpView.Campus = config["Campus"].ToString();
                        EmpView.PinNo = config["PIN_Number"].ToString();
                        EmpView.NSSFNo = config["NSSF_No"].ToString();
                        EmpView.NHIFNo = config["NHIF_No"].ToString();
                        EmpView.Bank = config["Main_Bank_Name"].ToString();
                        EmpView.Branch = config["Branch_Bank_Name"].ToString();
                        EmpView.AccountNo = config["Bank_Account_Number"].ToString();
                        EmpView.Semester = sem;
                        decimal[] s = CommonClass.GetLeaveBal(StaffNo, "ANNUAL");
                        EmpView.AllocatedDays = s[0].ToString();
                        EmpView.CarryForawrd = s[1].ToString();
                        EmpView.ReimbDays = s[2].ToString();
                        EmpView.LeaveTaken = s[3].ToString();                        
                        EmpView.LeaveBal = s[4].ToString();
                    }
                }               
                return View(EmpView);
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
                    msg = ex.Message;
                    successVal = false;
                }
            }
            return Json(new { message = msg, success = successVal }, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult GetStaffQualifications()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                #region Qual Lines
                List<Qualification> QualList = new List<Qualification>();
                string pageLine = "EmployeeQualifications?$filter=EmployeeNo eq '" + StaffNo + "' and Description ne ''&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        Qualification Qual = new Qualification();
                        Qual.Qual = (string)config["Qualification"];
                        Qual.Desc = (string)config["Description"];
                        Qual.Institution = (string)config["Institution"];
                        QualList.Add(Qual);
                    }
                }
                #endregion
                return PartialView("~/Views/Dashboard/StaffQualification.cshtml", QualList);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
    }
}