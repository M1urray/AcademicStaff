using iTextSharp.text.pdf;
using Latest_Staff_Portal.CustomSecurity;
using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
using Microsoft.Ajax.Utilities;
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
    public class ViewDocumentController : Controller
    {
        // GET: ViewDocument
        public ActionResult DocumentViewPayslip()
        {
            PayslipDetails ListYears = new PayslipDetails();
            #region Years
            List<YearCodes> yearCodes = new List<YearCodes>();

            string page = "PrPayrollPeriods?$select=PeriodYear&format=json";
            //string page = "PrPayrollPeriods?$select=PeriodYear&$filter=Closed eq true&format=json";
            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    YearCodes Years = new YearCodes();
                    Years.YList = (string)config["PeriodYear"];
                    yearCodes.Add(Years);
                }
            }
            #endregion            
            ListYears = new PayslipDetails
            {
                ListOfYears = yearCodes.Select(x =>
                                     new SelectListItem()
                                     {
                                         Text = x.YList,
                                         Value = x.YList
                                     }).OrderBy(x => Convert.ToInt32(x.Value)).DistinctBy(x => x.Value).ToList()
            };
            return View(ListYears);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetMonths(int Year)
        {
            try
            {
                MonthList ListMonths = new MonthList();
                #region Months
                List<MonthCodes> Months = new List<MonthCodes>();

                string page = "PrPayrollPeriods?$select=PeriodMonth&$filter=PeriodYear eq " + Year + "&format=json";// and Closed eq true&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        MonthCodes months = new MonthCodes();
                        months.MCode = (string)config["PeriodMonth"];
                        if (months.MCode == "1")
                        {
                            months.MDesc = "January";
                        }
                        else if (months.MCode == "2")
                        {
                            months.MDesc = "February";
                        }
                        else if (months.MCode == "3")
                        {
                            months.MDesc = "March";
                        }
                        else if (months.MCode == "4")
                        {
                            months.MDesc = "April";
                        }
                        else if (months.MCode == "5")
                        {
                            months.MDesc = "May";
                        }
                        else if (months.MCode == "6")
                        {
                            months.MDesc = "June";
                        }
                        else if (months.MCode == "7")
                        {
                            months.MDesc = "July";
                        }
                        else if (months.MCode == "8")
                        {
                            months.MDesc = "August";
                        }
                        else if (months.MCode == "9")
                        {
                            months.MDesc = "September";
                        }
                        else if (months.MCode == "10")
                        {
                            months.MDesc = "October";
                        }
                        else if (months.MCode == "11")
                        {
                            months.MDesc = "November";
                        }
                        else
                        {
                            months.MDesc = "December";
                        }
                        Months.Add(months);
                    }
                }
                #endregion
                ListMonths = new MonthList
                {
                    ListOfMonths = Months.Select(x =>
                                         new SelectListItem()
                                         {
                                             Text = x.MDesc,
                                             Value = x.MCode
                                         }).OrderBy(x => Convert.ToInt32(x.Value)).DistinctBy(x => x.Value).ToList()
                };

                return Json(ListMonths, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DocumentViewp9()
        {
            P9Details ListYears = new P9Details();
            #region YearList
            List<YearCodes> yearCodes = new List<YearCodes>();

            string page = "prTransactionList?$select=Period_Year&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    YearCodes Years = new YearCodes();
                    Years.YList = (string)config["Period_Year"];
                    yearCodes.Add(Years);
                }
            }
            #endregion
            ListYears = new P9Details
            {
                ListOfYears = yearCodes.Select(x =>
                                     new SelectListItem()
                                     {
                                         Text = x.YList,
                                         Value = x.YList
                                     }).DistinctBy(x => x.Value).ToList()
            };
            return View(ListYears);
        }
        public JsonResult GetPayslipReport(string Year, string Month)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();

                string message = "";
                string filename = "";
                bool success = false, view = false;

                string StaffIDNo = CommonClass.GetEmployeeIDNo(StaffNo);
                if (StaffIDNo == "")
                {
                    success = false;
                    message = "Employee ID Number is not set. Contact HR";
                }
                else
                {
                    string _filename = (StaffNo).Replace(@"/", @"");

                    int month = 0, year = 0;
                    if (Month != null)
                    {
                        month = Convert.ToInt32(Month);
                    }
                    if (Year != null)
                    {
                        year = Convert.ToInt32(Year);
                    }

                    // var period = month + "/01/" + Year;
                    //var period = "01/" + month + "/" + Year;
                    //DateTime Periodfilter = DateTime.ParseExact(period, "d/MM/yyyy", CultureInfo.InvariantCulture);
                    Credentials.ObjNav.GeneratePaySlipReport(StaffNo, month, year, "OLDPAYSLIP-" + _filename + ".pdf");
                    string OldPayslip = "OLDPAYSLIP-" + _filename + ".pdf";
                    filename = "PAYSLIP-" + _filename + ".pdf";
                    string FromPath = Credentials.fileSourcePath + OldPayslip;
                    string TPath = Credentials.fileSourcePath + filename;
                    addPassword(FromPath, TPath, StaffIDNo);

                    string DestPath = Server.MapPath("~/Downloads/");
                    CommonClass.MoveFile(filename, DestPath);
                    string DestinationPath = Server.MapPath("~/Downloads/" + filename);
                    CommonClass.MoveFile(filename, DestinationPath);

                    System.IO.FileInfo file = new System.IO.FileInfo(DestinationPath);
                    if (file.Exists)
                    {
                        success = true;
                        message = Credentials.fileDownLoads + filename;
                    }
                    else
                    {
                        success = false;
                        message = "File Not Found";
                    }
                }
                return Json(new { message = message, success, view }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        #region add password to pdf document
        internal static void addPassword(string TfileName, string NewFileName, string password)
        {
            try
            {
                using (Stream input = new FileStream(TfileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (Stream output = new FileStream(NewFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    PdfReader reader = new PdfReader(input);
                    PdfEncryptor.Encrypt(reader, output, true, password, password, PdfWriter.ALLOW_PRINTING);
                }
                if (System.IO.File.Exists(TfileName) == true)
                {
                    System.IO.File.Delete(TfileName);
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }
        #endregion
        public JsonResult GetP9Report(string Year)
        {
            try
            {
                //bool s = Request.Browser.mob;
                string StaffNo = Session["Username"].ToString();
                string _filename = (StaffNo).Replace(@"/", @"");
                string message = "";
                string filename = "";
                bool success = false, view = false;

                int period = Convert.ToInt32(Year);
                Credentials.ObjNav.GeneratePNineReport(StaffNo, period, "P9-" + _filename + ".pdf");

                filename = "P9-" + _filename + ".pdf";

                string DestPath = Server.MapPath("~/Downloads/");
                CommonClass.MoveFile(filename, DestPath);
                string DestinationPath = Server.MapPath("~/Downloads/" + filename);
                CommonClass.MoveFile(filename, DestinationPath);

                System.IO.FileInfo file = new System.IO.FileInfo(DestinationPath);
                if (file.Exists)
                {
                    success = true;
                    message = Credentials.fileDownLoads + filename;
                }
                else
                {
                    success = false;
                    message = "File Not Found";
                }
                return Json(new { message = message, success, view }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult GenerateAppraisalReport(string DocNo)
        {
            try
            {
                bool success = false;
                string message = "";
                Credentials.ObjNav.GenerateAppraisalPrintout(DocNo, "APPRAISAL-" + DocNo.Replace("/", "") + ".pdf");

                string filename = "APPRAISAL-" + DocNo.Replace(" / ", "") + ".pdf";
                CommonClass.MoveFileDev(filename);
                string DestinationPath = Server.MapPath("~/Downloads/" + filename);
                CommonClass.MoveFile(filename, DestinationPath);

                System.IO.FileInfo file = new System.IO.FileInfo(DestinationPath);
                if (file.Exists)
                {
                    success = true;
                    message = Credentials.fileDownLoads + filename;
                }
                else
                {
                    success = false;
                    message = "File Not Found";
                }
                return Json(new { message = message, success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}