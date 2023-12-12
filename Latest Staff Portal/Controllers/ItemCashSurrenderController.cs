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
    public class ItemCashSurrenderController : Controller
    {
        // GET: ItemCashSurrender
        public ActionResult ItemCashSurrenderRequisitionList()
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
        public PartialViewResult ItemCashSurrenderListPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<ItemCashSurrenderList> SurrenderList = new List<ItemCashSurrenderList>();

                string page = "ItemCashSurrenderList?$filter=Account_No eq '" + StaffNo + "'&format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        ItemCashSurrenderList ImList = new ItemCashSurrenderList();
                        ImList.No = (string)config["No"];
                        ImList.ReqDate = Convert.ToDateTime((string)config["Surrender_Date"]).ToString("dd/MM/yyyy");
                        ImList.Purpose = (string)config["Purpose"];
                        ImList.Function = (string)config["Function_Name"];
                        ImList.Amount = Convert.ToDecimal((string)config["Amount"]).ToString("#,##0.00");
                        ImList.BudgetCeter = (string)config["Budget_Center_Name"];
                        ImList.ImpDocNo = (string)config["Imprest_Issue_Doc_No"];
                        ImList.Status = (string)config["Status"];
                        SurrenderList.Add(ImList);

                    }
                }
                return PartialView("~/Views/ItemCashSurrender/ItemCashSurrenderListView.cshtml", SurrenderList.OrderByDescending(x => x.No));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult NewItemCashSurrender()
        {
            try
            {
                if (Session["Username"] == null)
                {
                    RedirectToAction("Login", "Login");
                }
                string StaffNo = Session["Username"].ToString();
                NewImpSurrender ImpSurrender = new NewImpSurrender();
                #region ItemCashList
                List<ItemCashList> ImpList = new List<ItemCashList>();
                ItemCashList Imp = null;
                string page = "PostedImprest?$filter=AccountNo eq '" + StaffNo + "' and SurrenderStatus ne 'Full' and imprest_TYpe eq 'Item Cash'&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        if (!isItemCashSurrendered((string)config["No"], (decimal)config["TotalPaymentAmount"]))
                        {
                            Imp = new ItemCashList();
                            Imp.No = (string)config["No"];
                            ImpList.Add(Imp);
                        }
                    }
                }
                #endregion
                ImpSurrender = new NewImpSurrender
                {
                    ListOfImprests = ImpList.Select(x =>
                                         new SelectListItem()
                                         {
                                             Text = x.No,
                                             Value = x.No
                                         }).ToList()
                };
                return Json(new { ImpSurrender, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        protected bool isItemCashSurrendered(string DocNo, decimal Amount)
        {
            bool ext = false;
            string page = "ItemCashSurrenderList?$select=Amount&$filter=Imprest_Issue_Doc_No eq '" + DocNo + "'&format=json";
            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                if (details["value"].Count() > 0)
                {
                    decimal amt = 0;
                    foreach (JObject config in details["value"])
                    {
                        amt = amt + (decimal)config["Amount"];
                    }
                    if (amt >= Amount)
                    {
                        ext = true;
                    }
                }
            }
            return ext;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitItemCashSurrender(string DocNo)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                string userID = Session["UserID"].ToString();
                string SurrDocNo = Credentials.ObjNav.fnItemCashSurrender(DocNo, StaffNo, userID);

                Session["SuccessMsg"] = "ItemCash surrender Document, Document No: " + SurrDocNo + ", created Successfully";

                return Json(new { DocNo = SurrDocNo, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ViewSurrenderDocument(string DocNo)
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
                    #region Imp Surrender Header
                    ItemCashSurrenderHeader ImpDoc = new ItemCashSurrenderHeader();

                    string page = "ItemCashAccountingCard?$filter=No eq '" + DocNo + "'&format=json";
                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        foreach (JObject config in details["value"])
                        {
                            ImpDoc.No = (string)config["No"];
                            ImpDoc.SurrenderDate = Convert.ToDateTime((string)config["Surrender_Date"]).ToString("dd/MM/yyyy");
                            ImpDoc.AccountNo = (string)config["Account_No"];
                            ImpDoc.AccountName = (string)config["AccountName"];
                            ImpDoc.ItemCashNo = (string)config["Imprest_Issue_Doc_No"];
                            ImpDoc.ImpIssueDate = (string)config["Imprest_Issue_Date"];
                            ImpDoc.CampusName = (string)config["Global_Dimension_1_Code"];
                            ImpDoc.DepartmentName = (string)config["Shortcut_Dimension_2_Code"];
                            ImpDoc.RespC = (string)config["Responsibility_Center"];
                            ImpDoc.TotalAmount = Convert.ToDecimal((string)config["Amount"]).ToString("#,##0.00");
                            ImpDoc.Status = (string)config["Status"];
                            ImpDoc.ImpPurpose = (string)config["Imp_Purpose"];
                        }
                    }
                    #endregion
                    return View(ImpDoc);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult ItemCashSurrenderLines(string DocNo, string Status)
        {
            try
            {
                #region Item Cash Surrender Lines
                List<ItemCashSurrenderLines> ImpLines = new List<ItemCashSurrenderLines>();
                string pageLine = "ItemCashSurrenderLines?$filter=SurrenderDocNo eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        ItemCashSurrenderLines ImSLine = new ItemCashSurrenderLines();
                        ImSLine.SurrenderDocNo = (string)config["SurrenderDocNo"];
                        ImSLine.AccountNo = (string)config["AccountNo"];
                        ImSLine.AccountName = (string)config["AccountName"];
                        ImSLine.LnNo = (string)config["Line_No"];
                        ImSLine.Amount = Convert.ToDecimal((string)config["Amount"]).ToString("#,##0.00");
                        ImSLine.ActaulSpend = Convert.ToDecimal((string)config["ActualSpent"]).ToString("#,##0.00");
                        ImSLine.ReceiptNo = (string)config["CashReceiptNo"];
                        ImSLine.ReceiptAmount = Convert.ToDecimal((string)config["CashReceiptAmount"]).ToString("#,##0.00");
                        ImpLines.Add(ImSLine);
                    }
                }
                #endregion
                ItemCashSurrenderLinesList Lines = new ItemCashSurrenderLinesList
                {
                    Status = Status,
                    ListOfItemCashSurrenderLines = ImpLines
                };
                return PartialView("~/Views/ItemCashSurrender/ItemCashSurrenderLineView.cshtml", Lines);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult EditImpSurrenderLine(string DocNo, string AccountNo, string Amount, string LnNo)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                #region Actaul Amount
                String ActAmount = "";
                string page = "ImpSurrenderLines?$select=ActualSpent&$filter=SurrenderDocNo eq '" + DocNo + "' and AccountNo eq '" + AccountNo + "'&$format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        ActAmount = Convert.ToDecimal((string)config["ActualSpent"]).ToString("#,##0.00");
                    }
                }
                #endregion
                #region Posted Receipts
                List<DropdownList> postedReciept = new List<DropdownList>();
                string pageLine = "PostedReceipts?$select=No&$filter=Customer_No eq '" + StaffNo + "' and Surrender_No eq ''&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        DropdownList ddl = new DropdownList();
                        ddl.Text = (string)config["No"];
                        ddl.Value = (string)config["No"];
                        postedReciept.Add(ddl);
                    }
                }
                #endregion
                ItmSurrenderLineDetails itemDetails = new ItmSurrenderLineDetails
                {
                    ActaulAmount = ActAmount,
                    DocNo = DocNo,
                    AccountNo = AccountNo,
                    Amount = Amount,
                    LnNo = LnNo,
                    ListOfPostedReceipts = postedReciept.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Text,
                                              Value = x.Value
                                          }).ToList()
                };
                return PartialView("~/Views/ItemCashSurrender/SurrenderItemEditForm.cshtml", itemDetails);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateSurrenderLine(string DocNo, string AccNo, string ActualSpend, string ReceiptNo, string LnNo)
        {
            try
            {
                string SDocNo = "", AccountNo = "", ActAmount = "", RptNo = "", LinNo = "";
                if (DocNo != null)
                {
                    SDocNo = DocNo;
                }
                if (AccNo != null)
                {
                    AccountNo = AccNo;
                }
                if (ActualSpend != null)
                {
                    ActAmount = ActualSpend;
                }
                if (ReceiptNo != null)
                {
                    RptNo = ReceiptNo;
                }
                if (LnNo != null)
                {
                    LinNo = LnNo;
                }
                Credentials.ObjNav.fnItemCashSurrenderLineUpdate(SDocNo, AccountNo, Convert.ToDecimal(ActAmount), RptNo, Convert.ToInt32(LinNo));

                string msg = "ItemCash surrender line updated Successfully";

                return Json(new { message = msg, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SendItemCashSurrenderForApproval(string DocNo, string Redirect)
        {
            try
            {
                Credentials.ObjNav.SendItemCashSurrenderForApproval(DocNo);
                if (Redirect == "Y")
                {
                    Session["SuccessMsg"] = "ItemCash Surrender, Document No " + DocNo + " send for approval Successfully";
                }
                return Json(new { message = "ItemCash Surrender,Document No " + DocNo + " send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult CancelItemCashSurrenderForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.HRCanceItemCashSurrenderRequisition(DocNo);

                return Json(new { message = "ItemCash Surrender approval cancelled Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public PartialViewResult FileUploadForm()
        {
            return PartialView("~/Views/ItemCashSurrender/FileAttachmentForm.cshtml");
        }
    }
}