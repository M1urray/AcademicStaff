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
//
// namespace Latest_Staff_Portal.Controllers
// {
//     [CustomAuthorization(Role = "ALLUSERS")]
//     [CustomeAuthentication]
//     public class StandingImprestController : Controller
//     {
//         public JsonResult CancelAppApprovalRequest(string DocNo)
//         {
//             JsonResult jsonResult;
//             try
//             {
//                 Credentials.ObjNav.HRCancelStandingImprest(DocNo);
//                 jsonResult = base.Json(new { message = "Standing Imprest Requisition approval cancelled Successfully", success = true }, JsonRequestBehavior.AllowGet);
//             }
//             catch (Exception exception)
//             {
//                 Exception ex = exception;
//                 jsonResult = base.Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
//             }
//             return jsonResult;
//         }
//
//         public PartialViewResult FileUploadForm()
//         {
//             return base.PartialView("~/Views/StandingImprest/DocFileAttachmentForm.cshtml");
//         }
//
//         public ActionResult NewStandingImprestRequest()
//         {
//             ActionResult action;
//             try
//             {
//                 if (base.Session["Username"] != null)
//                 {
//                     string StaffNo = base.Session["Username"].ToString();
//                     NewStandingImprest NewImprest = new NewStandingImprest();
//                     string Dim1 = "";
//                     string Dim2 = "";
//                     string pageData = string.Concat("EmployeeList?$filter=No eq '", StaffNo, "'&$format=json");
//                     using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData(pageData).GetResponseStream()))
//                     {
//                         JObject details = JObject.Parse(streamReader.ReadToEnd());
//                         if (details["value"].Count<JToken>() > 0)
//                         {
//                             foreach (JObject config in (IEnumerable<JToken>)details["value"])
//                             {
//                                 Dim1 = (string)config["_x003C_GlobSal_Dimension_1_Code_x003E_"];
//                                 Dim2 = (string)config["GlobalDimension2Code"];
//                             }
//                         }
//                     }
//                     if (Dim1 == "")
//                     {
//                         Error erroMsg = new Error()
//                         {
//                             Message = "Your Station not set. Contact HR"
//                         };
//                         action = this.PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
//                     }
//                     else if (Dim2 != "")
//                     {
//                         List<DimensionValues> Dim1List = new List<DimensionValues>();
//                         using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData("DimensionValues?$filter=Global_Dimension_No eq 1&$format=json").GetResponseStream()))
//                         {
//                             foreach (JObject config in (IEnumerable<JToken>)JObject.Parse(streamReader.ReadToEnd())["value"])
//                             {
//                                 DimensionValues Department = new DimensionValues()
//                                 {
//                                     Code = (string)config["Code"],
//                                     Name = (string)config["Name"]
//                                 };
//                                 Dim1List.Add(Department);
//                             }
//                         }
//                         List<DimensionValues> Dim2List = new List<DimensionValues>();
//                         using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData("DimensionValues?$filter=Global_Dimension_No eq 2&$format=json").GetResponseStream()))
//                         {
//                             foreach (JObject config in (IEnumerable<JToken>)JObject.Parse(streamReader.ReadToEnd())["value"])
//                             {
//                                 DimensionValues DList = new DimensionValues()
//                                 {
//                                     Code = (string)config["Code"],
//                                     Name = (string)config["Name"]
//                                 };
//                                 Dim2List.Add(DList);
//                             }
//                         }
//                         List<DimensionValues> BankList = new List<DimensionValues>();
//                         using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData("Bank_Accounts?$filter=Bank_Type eq 'Cash'&$format=json").GetResponseStream()))
//                         {
//                             foreach (JObject config in (IEnumerable<JToken>)JObject.Parse(streamReader.ReadToEnd())["value"])
//                             {
//                                 DimensionValues BnkAcc = new DimensionValues()
//                                 {
//                                     Code = (string)config["No"],
//                                     Name = (string)config["Name"]
//                                 };
//                                 BankList.Add(BnkAcc);
//                             }
//                         }
//                         List<DimensionValues> BankList1 = new List<DimensionValues>();
//                         using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData("Bank_Accounts?$filter=Bank_Type eq 'Normal'&$format=json").GetResponseStream()))
//                         {
//                             foreach (JObject config in (IEnumerable<JToken>)JObject.Parse(streamReader.ReadToEnd())["value"])
//                             {
//                                 DimensionValues BnkAcc = new DimensionValues()
//                                 {
//                                     Code = (string)config["No"],
//                                     Name = (string)config["Name"]
//                                 };
//                                 BankList1.Add(BnkAcc);
//                             }
//                         }
//                         List<RespCenter> RespCList = new List<RespCenter>();
//                         using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData("ResponsibilityCenters?$format=json").GetResponseStream()))
//                         {
//                             foreach (JObject config in (IEnumerable<JToken>)JObject.Parse(streamReader.ReadToEnd())["value"])
//                             {
//                                 RespCenter RCList = new RespCenter()
//                                 {
//                                     Code = (string)config["Code"],
//                                     Name = (string)config["Name"]
//                                 };
//                                 RespCList.Add(RCList);
//                             }
//                         }
//                         action = base.View("~/Views/StandingImprest/NewRequest.cshtml", new NewStandingImprest()
//                         {
//                             Dim1 = "",
//                             Dim2 = "",
//                             Receiving_BnkAccount = "",
//                             Receiving_Amount = "",
//                             Dim11 = "",
//                             Dim21 = "",
//                             Remarks = "",
//                             RespC = "",
//                             RespC1 = "",
//                             Sending_Amount = "",
//                             Sending_BnkAccount = "",
//                             ListOfDim1 = (
//                                 from x in Dim1List
//                                 select new SelectListItem()
//                                 {
//                                     Text = x.Name,
//                                     Value = x.Code
//                                 }).ToList<SelectListItem>(),
//                             ListOfDim2 = (
//                                 from x in Dim2List
//                                 select new SelectListItem()
//                                 {
//                                     Text = x.Name,
//                                     Value = x.Code
//                                 }).ToList<SelectListItem>(),
//                             ListOfBankAccounts = (
//                                 from x in BankList
//                                 select new SelectListItem()
//                                 {
//                                     Text = x.Name,
//                                     Value = x.Code
//                                 }).ToList<SelectListItem>(),
//                             ListOfBankAccounts1 = (
//                                 from x in BankList1
//                                 select new SelectListItem()
//                                 {
//                                     Text = x.Name,
//                                     Value = x.Code
//                                 }).ToList<SelectListItem>(),
//                             ListOfResponsibility = (
//                                 from x in RespCList
//                                 select new SelectListItem()
//                                 {
//                                     Text = x.Name,
//                                     Value = x.Code
//                                 }).ToList<SelectListItem>()
//                         });
//                     }
//                     else
//                     {
//                         Error erroMsg = new Error()
//                         {
//                             Message = "Your Department not set. Contact HR"
//                         };
//                         action = this.PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
//                     }
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
//         public JsonResult SendForApproval(string DocNo, string Redirect)
//         {
//             JsonResult jsonResult;
//             try
//             {
//                 Credentials.ObjNav.StandingImprestsSendForApproval(DocNo);
//                 if (Redirect == "Y")
//                 {
//                     base.Session["SuccessMsg"] = string.Concat("Standing Imprest Requisition, Document No ", DocNo, " send for approval Successfully");
//                 }
//                 jsonResult = base.Json(new { message = string.Concat("Standing Imprest Requisition, Document No ", DocNo, " send for approval Successfully"), success = true }, JsonRequestBehavior.AllowGet);
//             }
//             catch (Exception exception)
//             {
//                 Exception ex = exception;
//                 jsonResult = base.Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
//             }
//             return jsonResult;
//         }
//
//         public ActionResult StandingImprestDocView(string DocNo)
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
//                     string page = string.Concat("InterBankTransfer?$filter=No eq '", DocNo, "'&$format=json");
//                     using (StreamReader streamReader = new StreamReader(Credentials.GetOdataData(page).GetResponseStream()))
//                     {
//                         foreach (JObject config in (IEnumerable<JToken>)JObject.Parse(streamReader.ReadToEnd())["value"])
//                         {
//                             ImpDoc.DocNo = (string)config["No"];
//                             DateTime dateTime = Convert.ToDateTime((string)config["Date"]);
//                             ImpDoc.Date = dateTime.ToString("dd/MM/yyyy");
//                             ImpDoc.Remarks = (string)config["Remarks"];
//                             ImpDoc.Dim1 = (string)config["Receiving_Region_Code"];
//                             ImpDoc.Dim2 = (string)config["Receiving_Department_Code"];
//                             ImpDoc.Dim11 = (string)config["Source_Depot_Code"];
//                             ImpDoc.Dim21 = (string)config["Source_Department_Code"];
//                             ImpDoc.RespC = (string)config["RecieptResponsibilityCenter"];
//                             ImpDoc.RespC1 = (string)config["SendingResponsibilityCenter"];
//                             ImpDoc.Receiving_BnkAccount = (string)config["ReceivingAccount"];
//                             ImpDoc.Sending_BnkAccount = (string)config["PayingAccount"];
//                             decimal num = Convert.ToDecimal((string)config["Amount"]);
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
//         public PartialViewResult StandingImprestListPartialView()
//         {
//             PartialViewResult partialViewResult;
//             try
//             {
//                 string StaffNo = base.Session["Username"].ToString();
//                 List<StandingImprest> STImpList = new List<StandingImprest>();
//                 string page = string.Concat("InterBankTransfer?$filter=Employee_No eq '", StaffNo, "' and Type eq 'Standing Imprest'&$format=json");
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
//                         StImpDoc.Receiving_Transfer_Type = (string)config["ReceivingTransferType"];
//                         StImpDoc.RespC = (string)config["RecieptResponsibilityCenter"];
//                         StImpDoc.Paying_Account = (string)config["ReceivingAccount"];
//                         StImpDoc.Paying_Bank_Account_Name = (string)config["PayingBankAccountName"];
//                         decimal num = Convert.ToDecimal((string)config["Amount"]);
//                         StImpDoc.Amount = num.ToString("#,##0.00");
//                         StImpDoc.Receiving_Negotiable_Rate = (string)config["ReceivingNegotiableRate"];
//                         StImpDoc.Exchange_Rate_Destination = (string)config["ExchRateDestination"];
//                         num = Convert.ToDecimal((string)config["RequestAmtLCY"]);
//                         StImpDoc.Request_Amount_LCY = num.ToString("#,##0.00");
//                         StImpDoc.Remarks = (string)config["Remarks"];
//                         StImpDoc.Source_Transfer_Type = (string)config["SourceTransferType"];
//                         StImpDoc.Sending_RespC = (string)config["SendingResponsibilityCenter"];
//                         StImpDoc.Receiving_Account = (string)config["PayingAccount"];
//                         StImpDoc.Receiving_Bank_Account_Name = (string)config["ReceivingBankAccountName"];
//                         StImpDoc.Currency_Code_Destination = (string)config["CurrencyCodeDestination"];
//                         StImpDoc.Currency_Code_Source = (string)config["CurrencyCodeSource"];
//                         StImpDoc.Payable_Negotiable_Rate = (string)config["PayableNegotiableRate"];
//                         num = Convert.ToDecimal((string)config["Control1102758027"]);
//                         StImpDoc.Pay_Amount = num.ToString("#,##0.00");
//                         num = Convert.ToDecimal((string)config["PayAmtLCY"]);
//                         StImpDoc.Pay_Amount_LCY = num.ToString("#,##0.00");
//                         StImpDoc.External_DocNo = (string)config["ExternalDocNo"];
//                         StImpDoc.Status = (string)config["Status"];
//                         STImpList.Add(StImpDoc);
//                     }
//                 }
//                 partialViewResult = this.PartialView("~/Views/StandingImprest/StandingImprestReqListView.cshtml",
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
//         public ActionResult StandingImprestRequisitionList()
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
//         [AcceptVerbs(HttpVerbs.Post)]
//         public ActionResult SubmitStadingImprestRequisition(NewStandingImprest DocHeader)
//         {
//             ActionResult action;
//             bool successVal = false;
//             try
//             {
//                 string Dim1 = "";
//                 string Dim2 = "";
//                 string Dim11 = "";
//                 string Dim21 = "";
//                 string RespC = "";
//                 string RespC1 = "";
//                 string ReceivingAccount = "";
//                 string SendingAccount = "";
//                 string Remarks = "";
//                 decimal ReceivingAmount = new decimal();
//                 decimal SendingAmount = new decimal();
//                 if (DocHeader.Dim1 != null)
//                 {
//                     Dim1 = DocHeader.Dim1;
//                 }
//                 if (DocHeader.Dim2 != null)
//                 {
//                     Dim2 = DocHeader.Dim2;
//                 }
//                 if (DocHeader.Dim11 != null)
//                 {
//                     Dim11 = DocHeader.Dim11;
//                 }
//                 if (DocHeader.Dim21 != null)
//                 {
//                     Dim21 = DocHeader.Dim21;
//                 }
//                 if (DocHeader.RespC != null)
//                 {
//                     RespC = DocHeader.RespC;
//                 }
//                 if (DocHeader.RespC1 != null)
//                 {
//                     RespC1 = DocHeader.RespC1;
//                 }
//                 if (DocHeader.Receiving_BnkAccount != null)
//                 {
//                     ReceivingAccount = DocHeader.Receiving_BnkAccount;
//                 }
//                 if (DocHeader.Receiving_Amount != null && DocHeader.Receiving_Amount != "")
//                 {
//                     ReceivingAmount = Convert.ToDecimal(DocHeader.Receiving_Amount);
//                 }
//                 if (DocHeader.Sending_BnkAccount != null)
//                 {
//                     SendingAccount = DocHeader.Sending_BnkAccount;
//                 }
//                 if (DocHeader.Remarks != null)
//                 {
//                     Remarks = DocHeader.Remarks;
//                 }
//                 if (base.Session["UserID"] == null || base.Session["Username"] == null)
//                 {
//                     action = base.RedirectToAction("Login", "Login");
//                 }
//                 else
//                 {
//                     string StaffNo = base.Session["Username"].ToString();
//                     string UserID = base.Session["UserID"].ToString();
//                     string DocNo = Credentials.ObjNav.InsertStandingImprest(RespC, ReceivingAccount, ReceivingAmount, Dim1, Dim2, Dim11, Dim21, RespC1, SendingAccount, DocHeader.Remarks, StaffNo, UserID);
//                     if (DocNo == "")
//                     {
//                         action = base.Json(new { message = "Document not created. Please try again later...", success = false }, JsonRequestBehavior.AllowGet);
//                     }
//                     else
//                     {
//                         string Redirect = string.Concat("/StandingImprest/StandingImprestDocView?DocNo=", DocNo);
//                         base.Session["SuccessMsg"] = string.Concat("Standing Imprest Requisition, Document No: ", DocNo, ", created Successfully and Send for Approval");
//                         action = base.Json(new { message = Redirect, success = true }, JsonRequestBehavior.AllowGet);
//                     }
//                 }
//             }
//             catch (Exception exception)
//             {
//                 Exception ex = exception;
//                 if (successVal)
//                 {
//                     base.Session["ErrorMsg"] = ex.Message.Replace("'", "");
//                 }
//                 action = base.Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
//             }
//             return action;
//         }
//     }
// }