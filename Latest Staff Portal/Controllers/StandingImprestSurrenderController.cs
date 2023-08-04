// using Latest_Staff_Portal.CustomSecurity;
// using Latest_Staff_Portal.Models;
// using Latest_Staff_Portal.ViewModel;
// using Newtonsoft.Json.Linq;
// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Web;
// using System.Web.Mvc;
// using System.Web.Services.Description;
//
// namespace Latest_Staff_Portal.Controllers
// {
//     [CustomAuthorization(Role = "ALLUSERS")]
//     [CustomeAuthentication]
//     public class StandingImprestSurrenderController : Controller
//     {
//         [AcceptVerbs(HttpVerbs.Post)]
//         public JsonResult CancelAppApprovalRequest(string DocNo)
//         {
//             JsonResult jsonResult;
//             try
//             {
//                 Credentials.ObjNav.HRCancelStandingImprestSurrender(DocNo);
//                 jsonResult = base.Json(new { message = "Standing Imprest Reimbursement Requisition approval cancelled Successfully", success = true }, JsonRequestBehavior.AllowGet);
//             }
//             catch (Exception exception)
//             {
//                 Exception ex = exception;
//                 jsonResult = base.Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
//             }
//             return jsonResult;
//         }
//
//         public PartialViewResult EditSISurrenderLine(string ItemNo, string LineNo)
//         {
//             PartialViewResult partialViewResult;
//             try
//             {
//                 ImprestTypesList imprestTypes = new ImprestTypesList();
//                 List<ImprestTypes> ImprestTList = new List<ImprestTypes>();
//                 using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData("Payment_Types?$filter=Description ne '' and AccountType eq 'G/L Account' and Blocked eq false and GLAccount ne ''&$format=json").GetResponseStream()))
//                 {
//                     foreach (JObject config in (IEnumerable<JToken>)JObject.Parse(streamReader.ReadToEnd())["value"])
//                     {
//                         ImprestTypes impList = new ImprestTypes()
//                         {
//                             Code = (string)config["Code"],
//                             Description = (string)config["Description"]
//                         };
//                         ImprestTList.Add(impList);
//                     }
//                 }
//                 string Amount = this.GetLineAmount(LineNo);
//                 partialViewResult = this.PartialView("~/Views/StandingImprestSurrender/SISurrenderItemForm.cshtml", new ImprestTypesList()
//                 {
//                     Code = ItemNo,
//                     ListOfImprestTypes = (
//                         from x in ImprestTList
//                         select new SelectListItem()
//                         {
//                             Text = x.Description,
//                             Value = x.Code
//                         } into x
//                         orderby x.Text
//                         select x).ToList<SelectListItem>(),
//                     Amount = Amount
//                 });
//             }
//             catch (Exception exception)
//             {
//                 Exception ex = exception;
//                 Error erroMsg = new Error()
//                 {
//                     Message = ex.Message.Replace("'", "")
//                 };
//                 partialViewResult = this.PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
//             }
//             return partialViewResult;
//         }
//
//         public PartialViewResult FileUploadForm()
//         {
//             return base.PartialView("~/Views/StandingImprestSurrender/FileAttachmentForm.cshtml");
//         }
//
//         protected string GetImpDocNetAmount(string DocNo)
//         {
//             string amount = "";
//             string page = string.Concat("PaymentHeader?$select=Total_Payment_Amount&$filter=No eq '", DocNo, "'&$format=json");
//             using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData(page).GetResponseStream()))
//             {
//                 JObject details = JObject.Parse(streamReader.ReadToEnd());
//                 if (details["value"].Count<JToken>() > 0)
//                 {
//                     foreach (JObject config in (IEnumerable<JToken>)details["value"])
//                     {
//                         decimal num = Convert.ToDecimal((string)config["Total_Payment_Amount"]);
//                         amount = num.ToString("#,##0.00");
//                     }
//                 }
//             }
//             return amount;
//         }
//
//         protected string GetLineAmount(string LineNo)
//         {
//             string amount = "";
//             int num = Convert.ToInt32(LineNo);
//             string page = string.Concat("PaymentLines?$select=Amount&$filter=Line_Nos eq ", num.ToString(), "&$format=json");
//             using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData(page).GetResponseStream()))
//             {
//                 JObject details = JObject.Parse(streamReader.ReadToEnd());
//                 if (details["value"].Count<JToken>() > 0)
//                 {
//                     foreach (JObject config in (IEnumerable<JToken>)details["value"])
//                     {
//                         decimal num1 = Convert.ToDecimal((string)config["Amount"]);
//                         amount = num1.ToString("#,##0.00");
//                     }
//                 }
//             }
//             return amount;
//         }
//
//         public PartialViewResult NewReimbLine()
//         {
//             PartialViewResult partialViewResult;
//             try
//             {
//                 ImprestTypesList imprestTypes = new ImprestTypesList();
//                 List<ImprestTypes> ImprestTList = new List<ImprestTypes>();
//                 using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData("Payment_Types?$filter=Description ne '' and AccountType eq 'G/L Account' and Blocked eq false and GLAccount ne ''&$format=json").GetResponseStream()))
//                 {
//                     foreach (JObject config in (IEnumerable<JToken>)JObject.Parse(streamReader.ReadToEnd())["value"])
//                     {
//                         ImprestTypes impList = new ImprestTypes()
//                         {
//                             Code = (string)config["Code"],
//                             Description = (string)config["Description"]
//                         };
//                         ImprestTList.Add(impList);
//                     }
//                 }
//                 partialViewResult = this.PartialView("~/Views/StandingImprestSurrender/SISurrenderItemForm.cshtml", new ImprestTypesList()
//                 {
//                     ListOfImprestTypes = (
//                         from x in ImprestTList
//                         select new SelectListItem()
//                         {
//                             Text = x.Description,
//                             Value = x.Code
//                         } into x
//                         orderby x.Text
//                         select x).ToList<SelectListItem>()
//                 });
//             }
//             catch (Exception exception)
//             {
//                 Exception ex = exception;
//                 Error erroMsg = new Error()
//                 {
//                     Message = ex.Message.Replace("'", "")
//                 };
//                 partialViewResult = this.PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
//             }
//             return partialViewResult;
//         }
//
//         public ActionResult NewRequest()
//         {
//             ActionResult action;
//             try
//             {
//                 if (base.Session["Username"] != null)
//                 {
//                     string StaffNo = base.Session["Username"].ToString();
//                     DropdownListData NewDoc = new DropdownListData();
//                     List<DimensionValues> Dim1List = new List<DimensionValues>();
//                     string page = string.Concat("InterBankTransfer?$filter=Employee_No eq '", StaffNo, "' and Type eq 'Standing Imprest' and Posted eq true&$format=json");
//                     using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData(page).GetResponseStream()))
//                     {
//                         foreach (JObject config in (IEnumerable<JToken>)JObject.Parse(streamReader.ReadToEnd())["value"])
//                         {
//                             DimensionValues Department = new DimensionValues()
//                             {
//                                 Code = (string)config["No"],
//                                 Name = (string)config["No"]
//                             };
//                             Dim1List.Add(Department);
//                         }
//                     }
//                     action = base.View("~/Views/StandingImprestSurrender/NewRequest.cshtml", new DropdownListData()
//                     {
//                         Code = "",
//                         ListOfddlData = (
//                             from x in Dim1List
//                             select new SelectListItem()
//                             {
//                                 Text = x.Name,
//                                 Value = x.Code
//                             }).ToList<SelectListItem>()
//                     });
//                 }
//                 else
//                 {
//                     action = base.RedirectToAction("Login", "Login");
//                 }
//             }
//             catch (Exception exception)
//             {
//                 Exception ex = exception;
//                 Error erroMsg = new Error()
//                 {
//                     Message = ex.Message.Replace("'", "")
//                 };
//                 action = base.View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
//             }
//             return action;
//         }
//
//         public PartialViewResult SImprestDocumentLines(string DocNo, string Status)
//         {
//             PartialViewResult partialViewResult;
//             try
//             {
//                 List<ImprestLines> ImpLines = new List<ImprestLines>();
//                 string pageLine = string.Concat("PaymentLines?$filter=No eq '", DocNo, "'&$format=json");
//                 using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData(pageLine).GetResponseStream()))
//                 {
//                     foreach (JObject config in (IEnumerable<JToken>)JObject.Parse(streamReader.ReadToEnd())["value"])
//                     {
//                         ImprestLines ImLine = new ImprestLines()
//                         {
//                             DocNo = (string)config["No"],
//                             AdvanceType = (string)config["Type"],
//                             Item = (string)config["AccountNo"],
//                             ItemDesc = (string)config["AccountName"],
//                             ItemDesc2 = (string)config["Transaction_Name"]
//                         };
//                         decimal num = Convert.ToDecimal((string)config["Amount"]);
//                         ImLine.UnitAmount = num.ToString("#,##0.00");
//                         ImLine.LnNo = (string)config["Line_Nos"];
//                         ImpLines.Add(ImLine);
//                     }
//                 }
//                 partialViewResult = this.PartialView("~/Views/StandingImprestSurrender/SImprestDocumentLines.cshtml", new ImprestLinesList()
//                 {
//                     Status = Status,
//                     ListOfImprestLines = ImpLines
//                 });
//             }
//             catch (Exception exception)
//             {
//                 Exception ex = exception;
//                 Error erroMsg = new Error()
//                 {
//                     Message = ex.Message.Replace("'", "")
//                 };
//                 partialViewResult = this.PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
//             }
//             return partialViewResult;
//         }
//
//         public ActionResult SISurrenderDocView(string DocNo)
//         {
//             ActionResult action;
//             try
//             {
//                 if (base.Session["Username"] != null)
//                 {
//                     base.Session["Username"].ToString();
//                     NewStandingImprest ImpDoc = new NewStandingImprest();
//                     List<DimensionValues> Dim1List = new List<DimensionValues>();
//                     using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData("DimensionValues?$filter=Global_Dimension_No eq 1&$format=json").GetResponseStream()))
//                     {
//                         foreach (JObject config in (IEnumerable<JToken>)JObject.Parse(streamReader.ReadToEnd())["value"])
//                         {
//                             DimensionValues Department = new DimensionValues()
//                             {
//                                 Code = (string)config["Code"],
//                                 Name = (string)config["Name"]
//                             };
//                             Dim1List.Add(Department);
//                         }
//                     }
//                     List<DimensionValues> Dim2List = new List<DimensionValues>();
//                     using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData("DimensionValues?$filter=Global_Dimension_No eq 2&$format=json").GetResponseStream()))
//                     {
//                         foreach (JObject config in (IEnumerable<JToken>)JObject.Parse(streamReader.ReadToEnd())["value"])
//                         {
//                             DimensionValues DList = new DimensionValues()
//                             {
//                                 Code = (string)config["Code"],
//                                 Name = (string)config["Name"]
//                             };
//                             Dim2List.Add(DList);
//                         }
//                     }
//                     List<DimensionValues> BankList = new List<DimensionValues>();
//                     using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData("Bank_Accounts?$filter=Bank_Type eq 'Cash'&$format=json").GetResponseStream()))
//                     {
//                         foreach (JObject config in (IEnumerable<JToken>)JObject.Parse(streamReader.ReadToEnd())["value"])
//                         {
//                             DimensionValues BnkAcc = new DimensionValues()
//                             {
//                                 Code = (string)config["No"],
//                                 Name = (string)config["Name"]
//                             };
//                             BankList.Add(BnkAcc);
//                         }
//                     }
//                     List<DimensionValues> BankList1 = new List<DimensionValues>();
//                     using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData("Bank_Accounts?$filter=Bank_Type eq 'Normal'&$format=json").GetResponseStream()))
//                     {
//                         foreach (JObject config in (IEnumerable<JToken>)JObject.Parse(streamReader.ReadToEnd())["value"])
//                         {
//                             DimensionValues BnkAcc = new DimensionValues()
//                             {
//                                 Code = (string)config["No"],
//                                 Name = (string)config["Name"]
//                             };
//                             BankList1.Add(BnkAcc);
//                         }
//                     }
//                     List<RespCenter> RespCList = new List<RespCenter>();
//                     using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData("ResponsibilityCenters?$format=json").GetResponseStream()))
//                     {
//                         foreach (JObject config in (IEnumerable<JToken>)JObject.Parse(streamReader.ReadToEnd())["value"])
//                         {
//                             RespCenter RCList = new RespCenter()
//                             {
//                                 Code = (string)config["Code"],
//                                 Name = (string)config["Name"]
//                             };
//                             RespCList.Add(RCList);
//                         }
//                     }
//                     string page = string.Concat("PaymentHeader?$filter=No eq '", DocNo, "'&$format=json");
//                     using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData(page).GetResponseStream()))
//                     {
//                         foreach (JObject config in (IEnumerable<JToken>)JObject.Parse(streamReader.ReadToEnd())["value"])
//                         {
//                             ImpDoc.DocNo = (string)config["No"];
//                             DateTime dateTime = Convert.ToDateTime((string)config["Date"]);
//                             ImpDoc.Date = dateTime.ToString("dd/MM/yyyy");
//                             ImpDoc.Remarks = (string)config["Payment_Narration"];
//                             ImpDoc.Dim1 = (string)config["Global_Dimension_1_Code"];
//                             ImpDoc.Dim2 = (string)config["Shortcut_Dimension_2_Code"];
//                             ImpDoc.RespC = (string)config["Responsibility_Center"];
//                             ImpDoc.Receiving_BnkAccount = (string)config["Paying_Bank_Account"];
//                             ImpDoc.Sending_BnkAccount = (string)config["Reimbursement_Bank_Account"];
//                             decimal num = Convert.ToDecimal((string)config["Total_Payment_Amount"]);
//                             ImpDoc.Receiving_Amount = num.ToString("#,##0.00");
//                             ImpDoc.Status = (string)config["Status"];
//                         }
//                     }
//                     ImpDoc.ListOfDim1 = (
//                         from x in Dim1List
//                         select new SelectListItem()
//                         {
//                             Text = x.Name,
//                             Value = x.Code
//                         }).ToList<SelectListItem>();
//                     ImpDoc.ListOfDim2 = (
//                         from x in Dim2List
//                         select new SelectListItem()
//                         {
//                             Text = x.Name,
//                             Value = x.Code
//                         }).ToList<SelectListItem>();
//                     ImpDoc.ListOfBankAccounts = (
//                         from x in BankList
//                         select new SelectListItem()
//                         {
//                             Text = x.Name,
//                             Value = x.Code
//                         }).ToList<SelectListItem>();
//                     ImpDoc.ListOfBankAccounts1 = (
//                         from x in BankList1
//                         select new SelectListItem()
//                         {
//                             Text = x.Name,
//                             Value = x.Code
//                         }).ToList<SelectListItem>();
//                     ImpDoc.ListOfResponsibility = (
//                         from x in RespCList
//                         select new SelectListItem()
//                         {
//                             Text = x.Name,
//                             Value = x.Code
//                         }).ToList<SelectListItem>();
//                     action = base.View(ImpDoc);
//                 }
//                 else
//                 {
//                     action = base.RedirectToAction("Login", "Login");
//                 }
//             }
//             catch (Exception exception)
//             {
//                 Exception ex = exception;
//                 Error erroMsg = new Error()
//                 {
//                     Message = ex.Message.Replace("'", "")
//                 };
//                 action = base.View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
//             }
//             return action;
//         }
//
//         public ActionResult SISurrenderList()
//         {
//             ActionResult action;
//             try
//             {
//                 if (base.Session["Username"] != null)
//                 {
//                     action = base.View();
//                 }
//                 else
//                 {
//                     action = base.RedirectToAction("Login", "Login");
//                 }
//             }
//             catch (Exception exception)
//             {
//                 Exception ex = exception;
//                 Error erroMsg = new Error()
//                 {
//                     Message = ex.Message.Replace("'", "")
//                 };
//                 action = base.View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
//             }
//             return action;
//         }
//
//         public PartialViewResult SISurrenderListPartialView()
//         {
//             PartialViewResult partialViewResult;
//             try
//             {
//                 string StaffNo = base.Session["Username"].ToString();
//                 List<StandingImprest> STImpList = new List<StandingImprest>();
//                 string page = string.Concat("PaymentHeader?$filter=PF_No eq '", StaffNo, "' and Request_Type eq 'Standing Imprest Surrender'&$format=json");
//                 using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData(page).GetResponseStream()))
//                 {
//                     foreach (JObject config in (IEnumerable<JToken>)JObject.Parse(streamReader.ReadToEnd())["value"])
//                     {
//                         StandingImprest StImpDoc = new StandingImprest()
//                         {
//                             No = (string)config["No"]
//                         };
//                         DateTime dateTime = Convert.ToDateTime((string)config["Date"]);
//                         StImpDoc.ReqDate = dateTime.ToString("dd/MM/yyyy");
//                         decimal item = (decimal)config["Total_Payment_Amount"];
//                         StImpDoc.Pay_Amount = item.ToString("#,##0.00");
//                         StImpDoc.Receiving_Bank_Account_Name = (string)config["Bank_Name"];
//                         StImpDoc.Paying_Bank_Account_Name = (string)config["Reimbursement_Bank_Name"];
//                         StImpDoc.Status = (string)config["Status"];
//                         STImpList.Add(StImpDoc);
//                     }
//                 }
//                 partialViewResult = this.PartialView("~/Views/StandingImprestSurrender/SISurrenderReqListView.cshtml",
//                     from x in STImpList
//                     orderby x.No descending
//                     select x);
//             }
//             catch (Exception exception)
//             {
//                 Exception ex = exception;
//                 Error erroMsg = new Error()
//                 {
//                     Message = ex.Message.Replace("'", "")
//                 };
//                 partialViewResult = this.PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
//             }
//             return partialViewResult;
//         }
//
//         [AcceptVerbs(HttpVerbs.Post)]
//         public JsonResult RemoveSISurrenderLine(string DocNo, string LnNo)
//         {
//             JsonResult jsonResult;
//             try
//             {
//                 Credentials.ObjNav.DeleteStandingImprestsurrenderLines(Convert.ToInt32(LnNo));
//                 string DocNetAmount = this.GetImpDocNetAmount(DocNo);
//                 jsonResult = base.Json(new { NetAmout = DocNetAmount, message = "Line removed successfully", success = true }, JsonRequestBehavior.AllowGet);
//             }
//             catch (Exception exception)
//             {
//                 Exception ex = exception;
//                 jsonResult = base.Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
//             }
//             return jsonResult;
//         }
//
//         [AcceptVerbs(HttpVerbs.Post)]
//         public JsonResult SendForApproval(string DocNo, string Redirect)
//         {
//             JsonResult jsonResult;
//             try
//             {
//                 Credentials.ObjNav.StandingImprestSurrenderSendForApproval(DocNo);
//                 if (Redirect == "Y")
//                 {
//                     base.Session["SuccessMsg"] = string.Concat("Standing Imprest Surrender Requisition, Document No ", DocNo, " send for approval Successfully");
//                 }
//                 jsonResult = base.Json(new { message = string.Concat("Standing Imprest Surrender Requisition, Document No ", DocNo, " send for approval Successfully"), success = true }, JsonRequestBehavior.AllowGet);
//             }
//             catch (Exception exception)
//             {
//                 Exception ex = exception;
//                 jsonResult = base.Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
//             }
//             return jsonResult;
//         }
//
//         [AcceptVerbs(HttpVerbs.Post)]
//         public JsonResult SubmitSISurrenderLine(string DocNo, string item, string Amount)
//         {
//             JsonResult jsonResult;
//             try
//             {
//                 base.Session["Username"].ToString();
//                 string ItemNo = "";
//                 string LineAmount = "0";
//                 if (item != "" && item != null)
//                 {
//                     ItemNo = item.Trim();
//                 }
//                 if (Amount != "" && Amount != null)
//                 {
//                     LineAmount = Amount.Trim();
//                 }
//                 Credentials.ObjNav.InsertStandingImprestsurrenderLines(DocNo, item, Convert.ToDecimal(LineAmount));
//                 string DocNetAmount = this.GetImpDocNetAmount(DocNo);
//                 jsonResult = base.Json(new { NetAmout = DocNetAmount, message = "Line Added successfully", success = true }, JsonRequestBehavior.AllowGet);
//             }
//             catch (Exception exception)
//             {
//                 Exception ex = exception;
//                 jsonResult = base.Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
//             }
//             return jsonResult;
//         }
//
//         [AcceptVerbs(HttpVerbs.Post)]
//         public ActionResult SubmitSISurrenderRequisition(string ReqNo, string RespC, string Remarks)
//         {
//             ActionResult actionResult;
//             bool successVal = false;
//             try
//             {
//                 string StaffNo = base.Session["Username"].ToString();
//                 string UserID = base.Session["UserID"].ToString();
//                 string DocNo = Credentials.ObjNav.InsertStandingImprestsurrender(ReqNo, RespC, Remarks, StaffNo, UserID, 3);
//                 if (DocNo == "")
//                 {
//                     actionResult = base.Json(new { message = "Document not created. Please try again later...", success = false }, JsonRequestBehavior.AllowGet);
//                 }
//                 else
//                 {
//                     base.Session["SuccessMsg"] = string.Concat("Standing Imprest Surrender Requisition, Document No: ", DocNo, ", created Successfully");
//                     actionResult = base.Json(new { message= DocNo, success = true }, JsonRequestBehavior.AllowGet);
//                 }
//             }
//             catch (Exception exception)
//             {
//                 Exception ex = exception;
//                 if (successVal)
//                 {
//                     base.Session["ErrorMsg"] = ex.Message.Replace("'", "");
//                 }
//                 actionResult = base.Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
//             }
//             return actionResult;
//         }
//     }
// }
