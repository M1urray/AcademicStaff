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
    public class FuelController : Controller
    {
        // GET: Fuel
        public ActionResult FuelChargeCardList()
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
                erroMsg.Message = ex.Message.Replace("'", "");
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult FuelRechardCardRequisitionListPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<Fuel> ReqList = new List<Fuel>();

                string page = "FuelRechargeCard?$filter=Driver eq '" + StaffNo + "'&$format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        Fuel f = new Fuel();
                        f.No = (string)config["Requisition_No"];
                        f.Date_Requested = Convert.ToDateTime((string)config["Date"]).ToString("dd/MM/yyyy");
                        f.Driver = (string)config["Driver"];
                        f.Driver_Name = (string)config["Driver_Name"];
                        f.Vehicle = (string)config["Vehicle_Reg_No"];
                        f.Max_Amount = (string)config["Max_Amount_Allocated"];
                        f.Amount_Consumed = (string)config["Amount_Consumed"];
                        f.Amount_To_Topup = (string)config["Amount_To_be_Toped_Up"];
                        f.Status = (string)config["Status"];
                        ReqList.Add(f);
                    }
                }


                return PartialView("~/Views/Fuel/FuelRechargeCardReqListView.cshtml", ReqList.OrderByDescending(x => x.No));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public ActionResult NewFuelRechardCardRequest()
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
                    Fuel f = new Fuel();
                    return View("~/Views/Fuel/NewCardRechargeRequest.cshtml", f);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubmitFuelRechardCardRequisition(ImprestHeader imprestHeader)
        {
            bool successVal = false;
            try
            {
                string Directorate = "", section = "";

                if (Session["UserID"] == null || Session["Username"] == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    if (imprestHeader.Directorate != null)
                    {
                        Directorate = imprestHeader.Directorate;
                    }
                    string StaffNo = Session["Username"].ToString();
                    string UserID = Session["UserID"].ToString();
                    DateTime DateRequired = DateTime.ParseExact(imprestHeader.DateNeeded.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    string DocNo = Credentials.ObjNav.ImprestRequisitionCreate(StaffNo, DateRequired, Directorate, imprestHeader.Department, section, "", "", "", imprestHeader.Remarks, UserID, "", DateRequired, DateRequired);
                    if (DocNo != "")
                    {
                        string Redirect = "/Imprest/ImprestDocumentView?DocNo=" + DocNo;

                        Session["SuccessMsg"] = "Imprest Requisition, Document No: " + DocNo + ", created Successfully. Add line(s) and attachment(s) then send for approval";
                        return Json(new { message = Redirect, success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { message = "Document not created. Please try again later...", success = false }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                if (successVal)
                {
                    Session["ErrorMsg"] = ex.Message.Replace("'", "");
                }
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}