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

            //string page = "PrPayrollPeriods?$select=PeriodYear&$filter=Closed eq true&format=json";
            string page = "PrPayrollPeriods?$select=PeriodYear&$format=json";

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

                //string page = "PrPayrollPeriods?$select=PeriodMonth&$filter=PeriodYear eq " + Year + " and Closed eq true&format=json";
                string page = "PrPayrollPeriods?$select=PeriodMonth&$filter=PeriodYear eq " + Year + "&$format=json";
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

            //string page = "prTransactionList?$select=Period_Year&format=json";
            //string page = "PrPayrollPeriods?$select=PeriodYear&$filter=Closed eq true&format=json";
            string page = "PrPayrollPeriods?$select=PeriodYear&$format=json";

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
                bool success = false, view = false;

                string staffIdNo = CommonClass.GetEmployeeIDNo(StaffNo);
                if (staffIdNo == "")
                {
                    success = false;
                    message = "Employee ID Number is not set. Contact HR";
                }
                else
                {
                    string filename = (StaffNo).Replace(@"/", @"");

                    int Pmonth = 0, PYear = 0;
                    if (!string.IsNullOrEmpty(Month))
                    {
                        Pmonth = Convert.ToInt32(Month);
                    }
                    if (!string.IsNullOrEmpty(Year))
                    {
                        PYear = Convert.ToInt32(Year);
                    }
                    message = Credentials.ObjNav.GeneratePaySlipReport(StaffNo, Pmonth, PYear);
                    string filePath = Server.MapPath("~/Downloads/");
                    string OldPayslip = "OLDPAYSLIP-" + filename + ".pdf";

                    Credentials.SaveBase64DocumentAttachment(message, filePath + OldPayslip);

                    filename = "PAYSLIP-" + filename + ".pdf";

                    string FromPath = filePath + OldPayslip;
                    string path = filePath + filename;
                    addPassword(FromPath, path, staffIdNo);
                    string DestinationPath = filePath + filename;
                    FileInfo file = new FileInfo(DestinationPath);
                    if (file.Exists)
                    {
                        success = true;
                    }
                    else
                    {
                        message = "File Not Found";
                    }
                    if (success)
                    {
                        message = @"/Downloads/" + filename;
                    }
                }
                return Json(new { message, success, view }, JsonRequestBehavior.AllowGet);
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
                string StaffNo = Session["Username"].ToString();
                string message = "";
                bool success = false, view = false;

                int period = Convert.ToInt32(Year);
                message = Credentials.ObjNav.GeneratePNineReport(StaffNo, period);
                if (message == "")
                {
                    message = "File Not Found";
                }
                else
                {
                    success = true;
                }
                return Json(new { message, success, view }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}