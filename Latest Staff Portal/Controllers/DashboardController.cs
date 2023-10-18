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
                string page = "EmployeeList?$select=No,First_Name,Middle_Name,Last_Name&$filter=No eq '" + StaffNo + "'&$format=json";

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
                            NotfCount = CommonClass.GetDocumentCount()
                            //IDNo = (string)config["ID_Number"],
                            //Gender = (string)config["Gender"],
                            //MaritalStatus = (string)config["Marital_Status"],
                            //Nationality = (string)config["Citizenship"],
                            //County = (string)config["County_Name"],
                            //DoB = (string)config["Date_Of_Birth"],
                            //Address1 = config["Postal_Address"].ToString(),
                            //Address2 = config["Postal_Address2"].ToString(),
                            //City = config["City"].ToString(),
                            //PostalCode = config["Post_Code"].ToString(),
                            //HomeTelNo = config["Home_Phone_Number"].ToString(),
                            //PhoneNo = config["Cellular_Phone_Number"].ToString(),
                            //WorkTel = config["Work_Phone_Number"].ToString(),
                            //CompanyEmail = config["Company_E_Mail"].ToString(),
                            //PersonalEmail = config["E_Mail"].ToString(),
                            //DateOfJoin = config["Date_Of_Birth"].ToString(),
                            //ProbationEndDate = config["End_Of_Probation_Date"].ToString(),
                            //PenSchemeJoinDate = config["Pension_Scheme_Join"].ToString(),
                            //JobTitle = config["Job_Title"].ToString(),
                            //EmpStatus = config["Status"].ToString(),
                            //JobCat = "",
                            //Department = config["Department_Name"].ToString(),
                            //Campus = config["Campus"].ToString(),
                            //Bank = config["Main_Bank"].ToString(),
                            //Branch = config["Branch_Bank"].ToString(),
                            //AccountNo = config["Bank_Account_Number"].ToString(),
                            //PinNo = config["PIN_Number"].ToString(),
                            //NSSFNo = config["NSSF_No"].ToString(),
                            //NHIFNo = config["NHIF_No"].ToString()
                        };
                    }
                }
                return View(EmpView);
            }
        }
        public PartialViewResult LoadDashboardData()
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
                    EmpView = new EmployeeView
                    {
                        No = (string)config["No"],
                        Name = (string)config["First_Name"] + " " + (string)config["Middle_Name"] + " " + (string)config["Last_Name"],
                        IDNo = (string)config["ID_Number"],
                        Gender = (string)config["Gender"],
                        MaritalStatus = (string)config["Marital_Status"],
                        Nationality = (string)config["Citizenship"],
                        County = (string)config["County_Name"],
                        DoB = ((DateTime)config["Date_Of_Birth"]).ToString("dd/MM/yyyy"),
                        Address1 = config["Postal_Address"].ToString(),
                        Address2 = config["Postal_Address2"].ToString(),
                        City = config["City"].ToString(),
                        PostalCode = config["Post_Code"].ToString(),
                        HomeTelNo = config["Home_Phone_Number"].ToString(),
                        PhoneNo = config["Cellular_Phone_Number"].ToString(),
                        WorkTel = config["Work_Phone_Number"].ToString(),
                        CompanyEmail = config["Company_E_Mail"].ToString(),
                        PersonalEmail = config["E_Mail"].ToString(),
                        DateOfJoin = ((DateTime)config["Date_Of_Join"]).ToString("dd/MM/yyyy"),
                        ContractStartDate = ((DateTime)config["Contract_Start_Date"]).ToString("dd/MM/yyyy"),
                        ContractEndtDate = ((DateTime)config["Contract_End_Date"]).ToString("dd/MM/yyyy"),
                        ProbationDate = ((DateTime)config["Probation_Start_Date"]).ToString("dd/MM/yyyy"),
                        ProbationEndDate = ((DateTime)config["End_Of_Probation_Date"]).ToString("dd/MM/yyyy"),
                        PenSchemeJoinDate = ((DateTime)config["Pension_Scheme_Join"]).ToString("dd/MM/yyyy"),
                        JobTitle = config["Job_Title"].ToString(),
                        EmpStatus = config["Status"].ToString(),
                        JobCat = config["Category"].ToString(),
                        Department = config["Department_Name"].ToString(),
                        Campus = config["Campus"].ToString(),
                        School = config["Schools"].ToString(),
                        Bank = config["Main_Bank"].ToString(),
                        Branch = config["Branch_Bank"].ToString(),
                        AccountNo = config["Bank_Account_Number"].ToString(),
                        PinNo = config["PIN_Number"].ToString(),
                        NSSFNo = config["NSSF_No"].ToString(),
                        NHIFNo = config["NHIF_No"].ToString(),
                        ListInternalMemos = ImportantDocuments()
                    };
                }
            }
            return PartialView("~/Views/Dashboard/DashboardInformation.cshtml", EmpView);
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
        public ListOfInternalMemos ImportantDocuments()
        {
            List<DocumentAttachment> DocAttachment = new List<DocumentAttachment>();
            bool hasFile = false;
            try
            {
                string page = "InternalMemos?$filter=Category eq 'STAFF'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    if (details["value"].Count() > 0)
                    {
                        foreach (JObject config in details["value"])
                        {
                            string page1 = "DocumentAttachment?$filter=No eq '"+ (string)config["Code"] + "'&$format=json";

                            HttpWebResponse httpResponse1 = Credentials.GetOdataData(page1);
                            using (var streamReader1 = new StreamReader(httpResponse1.GetResponseStream()))
                            {
                                var result1 = streamReader1.ReadToEnd();

                                var details1 = JObject.Parse(result1);
                                if (details1["value"].Count() > 0)
                                {
                                    hasFile = true;
                                    foreach (JObject config1 in details1["value"])
                                    {
                                        DocumentAttachment docAttList = new DocumentAttachment();
                                        docAttList.TabelID = (int)config1["Table_ID"];
                                        docAttList.No = (string)config1["No"];
                                        docAttList.FileName = (string)config1["File_Name"];
                                        docAttList.Remarks = (string)config1["Document_Description"];
                                        docAttList.FileExt = (string)config1["File_Extension"];
                                        docAttList.ID = (int)config1["ID"];
                                        docAttList.LineNo = (string)config1["Line_No"];
                                        docAttList.DocType = (string)config1["Document_Type"];
                                        docAttList.Date = ((DateTime)config1["Attached_Date"]).ToString("dd/MM/yyyy");
                                        DocAttachment.Add(docAttList);
                                    }
                                }
                            }
                        }
                    }
                }              
            }
            catch (Exception ex)
            {
                DocumentAttachment docAttList = new DocumentAttachment();
                docAttList.FileName = ex.Message;
                docAttList.Date = DateTime.Now.ToString("dd/MM/yyyy");
                DocAttachment.Add(docAttList);
                hasFile = false;
            }
            ListOfInternalMemos fileList = new ListOfInternalMemos
            {
                ListOfIntMemos = DocAttachment.OrderByDescending(x => x.Date).ToList(),
                hasFiles = hasFile
            };
            return fileList;
        }
        [HttpGet]
        public virtual ActionResult Download(string fileName)
        {
            string fullPath = Credentials.ImportantDocParth + fileName;
            return File(fullPath, "application/octet-stream", fileName);
        }
    }
}