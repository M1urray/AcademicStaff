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
    public class GatePassController : Controller
    {
        // GET: GatePass
        public ActionResult GatePassList()
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
        public ActionResult ApprovedGatePassList()
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
        public PartialViewResult GatePassListPartialView(string Status)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<GatePass> GatePassList = new List<GatePass>();

                string page = "";
                if (Status == "Open")
                {
                    page = "GatePassCard?$filter=Status eq '" + Status + "'&$format=json";
                }
                else
                {
                    page = "GatePassCard?$filter=Status ne '" + Status + "'&$format=json";
                }
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        GatePass GatePass = new GatePass();
                        GatePass.No = (string)config["No"];
                        GatePass.EmployeeNo = (string)config["EmployeeNo"];
                        GatePass.EmployeeName = (string)config["EmployeeName"];
                        GatePass.DateOut = (string)config["DateOut"];
                        GatePass.DateCreated = (string)config["DateCreated"];
                        GatePass.TimeOut = (string)config["TimeOut"];
                        GatePass.AssetTransferNo = (string)config["AssetTransferNo"];
                        GatePass.AssetDescription = (string)config["AssetDescription"];
                        GatePass.AssetFromLocation = (string)config["AssetFromLocation"];
                        GatePass.AssetToLocation = (string)config["AssetToLocation"];
                        GatePass.ResponsibilityCenter = (string)config["ResponsibilityCenter"];
                        GatePass.ToBeReturned = (string)config["ToBeReturned"];
                        GatePass.Comment = (string)config["Comment"];
                        GatePass.Status = (string)config["Status"];
                        GatePassList.Add(GatePass);
                    }
                }

                GateDetailList Gpass = new GateDetailList
                {
                    Status = Status,
                    GatePassDetails = GatePassList.OrderByDescending(x => x.No).ToList()
                };
                return PartialView("~/Views/GatePass/Partial Views/GatePassListView.cshtml", Gpass);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewGatePassRequest()
        {
            try
            {
                NewGatePassForm NewVisit = new NewGatePassForm();

                #region AssetTransfer List
                List<DropdownList> AssetTransList = new List<DropdownList>();
                string page = "AssetTransferList?$&filter=Status eq 'Approved'&$format=json";

                HttpWebResponse httpResponseCampus = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponseCampus.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList ddl = new DropdownList();
                        ddl.Value = (string)config["No"];
                        ddl.Text = (string)config["AssettoTransfer"] + " " + CommonClass.GetFixedAssetDescription((string)config["AssettoTransfer"]);
                        AssetTransList.Add(ddl);
                    }
                }
                #endregion

                GatePass newGatePass = new GatePass();
                NewVisit = new NewGatePassForm
                {
                    GatePassDoc = newGatePass,
                    ListOfAssetTransfer = AssetTransList.Select(x =>
                                         new SelectListItem()
                                         {
                                             Text = x.Text,
                                             Value = x.Value
                                         }).ToList()
                };
                return PartialView("~/Views/GatePass/Partial Views/GatePassForm.cshtml", NewVisit);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitGatePassData(string AsstTransferNo, string Return)
        {
            try
            {
                string EmpNo = Session["Username"].ToString();
                Credentials.ObjNav.InsertGatePass(EmpNo, AsstTransferNo, Convert.ToInt32(Return));

                return Json(new { message = "Gate Pass created successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}