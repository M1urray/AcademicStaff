using Latest_Staff_Portal.CustomSecurity;
using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.Controllers
{
    [CustomeAuthentication]
    [CustomAuthorization(Role = "ALLUSERS")]
    public class DocumentApprovalController : Controller
    {
        // GET: DocumentApproval
        public ActionResult DocumentForApprovalSummery(string rn)
        {
            try
            {
                if (Session["UserID"] == null || Session["Username"] == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    if (rn == "" || rn == null)
                    {
                        return RedirectToAction("Dashboard", "Dashboard");
                    }
                    else
                    {
                        string userID = Session["UserID"].ToString();
                        DocumentCount DocCount = new DocumentCount();

                        DocCount.LeaveCount = 0;
                        DocCount.PRNCount = 0;
                        DocCount.LPOCount = 0;
                        DocCount.SRNCount = 0;
                        DocCount.ImpCount = 0;
                        DocCount.SurrCount = 0;
                        DocCount.ClaimCount = 0;
                        DocCount.Transport = 0;
                        DocCount.Training = 0;
                        DocCount.PVCount = 0;
                        DocCount.PayrollCount = 0;
                        //DocCount.TransferOrder = 0;

                        string page = "ApprovalEntries?$filter=Approver_ID eq '" + userID + "' and Status eq '" + rn + "'&$format=json";
                        HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);
                            foreach (JObject config in details["value"])
                            {
                                if ((string)config["Table_ID"] == "70134864")
                                {
                                    DocCount.LeaveCount = DocCount.LeaveCount + 1;
                                }
                                if ((string)config["Table_ID"] == "38" && (string)config["Document_Type"] == "Quote")
                                {
                                    DocCount.PRNCount = DocCount.PRNCount + 1;
                                }
                                if ((string)config["Table_ID"] == "38" && (string)config["Document_Type"] == "Order")
                                {
                                    DocCount.LPOCount = DocCount.LPOCount + 1;
                                }
                                if ((string)config["Table_ID"] == "70134954")
                                {
                                    DocCount.SRNCount = DocCount.SRNCount + 1;
                                }
                                if ((string)config["Table_ID"] == "70135176")
                                {
                                    DocCount.ImpCount = DocCount.ImpCount + 1;
                                }
                                if ((string)config["Table_ID"] == "70135168")
                                {
                                    DocCount.SurrCount = DocCount.SurrCount + 1;
                                }
                                if ((string)config["Table_ID"] == "70135169")
                                {
                                    DocCount.ClaimCount = DocCount.ClaimCount + 1;
                                }
                                if ((string)config["Table_ID"] == "70135138")
                                {
                                    DocCount.Transport = DocCount.Transport + 1;
                                }
                                if ((string)config["Table_ID"] == "70135040")
                                {
                                    DocCount.Training = DocCount.Training + 1;
                                }
                                if ((string)config["Table_ID"] == "5740")
                                {
                                    DocCount.TransferOrder = DocCount.TransferOrder + 1;
                                }
                                if ((string)config["Table_ID"] == "70135171")
                                {
                                    DocCount.PVCount = DocCount.PVCount + 1;
                                }
                                if ((string)config["Table_ID"] == "232")
                                {
                                    DocCount.PayrollCount = DocCount.PayrollCount + 1;
                                }
                            }
                            DocCount.Status = rn;
                            return View(DocCount);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public ActionResult GetDocumentForApprovalList(string TbID, string Title, string Status, string DocType)
        {
            try
            {
                if (TbID != "" && Title != "" && Status != "")
                {
                    DocumentsForApprovalList newList = new DocumentsForApprovalList
                    {
                        TableID = TbID,
                        Title = Title,
                        Status = Status,
                        DocType = DocType
                    };
                    return View(newList);
                }
                else
                {
                    return RedirectToAction("Dashboard", "Dashboard");
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public ActionResult LoadDocumentForApprovalList(string TbID, string Status, string DocType)
        {
            try
            {
                if (TbID != "")
                {
                    string userID = Session["UserID"].ToString();
                    List<DocumentsForApproval> approvalDocList = new List<DocumentsForApproval>();
                    string page = "";
                    if (DocType != "N")
                    {
                        page = "ApprovalEntries?$select=Entry_No,Table_ID,Document_No,Document_Type,Sender_ID,Date_Time_Sent_for_Approval,Status,Sequence_No,Comment,Record_ID_to_Approve&$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" + Status + "' and Document_Type eq '" + DocType + "'&$format=json";
                    }
                    else
                    {
                        page = "ApprovalEntries?$select=Entry_No,Table_ID,Document_No,Document_Type,Sender_ID,Date_Time_Sent_for_Approval,Status,Sequence_No,Comment,Record_ID_to_Approve&$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" + Status + "'&$format=json";
                    }
                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        if (details["value"].Count() > 0)
                        {
                            foreach (JObject config in details["value"])
                            {
                                DocumentsForApproval DocList = new DocumentsForApproval();
                                DocList.TabelID = (string)config["Table_ID"];
                                DocList.Entry_No = (string)config["Entry_No"];
                                if ((string)config["Document_No"] == "")
                                {
                                    string s = (string)config["Record_ID_to_Approve"];
                                    string[] t = s.Split(':');
                                    DocList.Document_No = t[1].Trim();
                                }
                                else
                                {
                                    DocList.Document_No = (string)config["Document_No"];
                                }
                                if (TbID == "70134864")
                                {
                                    string pageLv = "HRLeaveRequisition?$select=EmpoyeeName&$filter=ApplicationNo eq '" + DocList.Document_No + "'&$format=json";

                                    HttpWebResponse httpResponseLV = Credentials.GetOdataData(pageLv);
                                    using (var streamReaderLV = new StreamReader(httpResponseLV.GetResponseStream()))
                                    {
                                        var resultLV = streamReaderLV.ReadToEnd();

                                        var detailsLV = JObject.Parse(resultLV);
                                        foreach (JObject configLV in detailsLV["value"])
                                        {
                                            DocList.Sender_Name = (string)configLV["EmpoyeeName"];
                                        }
                                    }
                                }
                                else if (TbID == "38" && DocType== "Quote")
                                {
                                    DocList.Document_Type = DocType;
                                    string pagePRV = "PurchaseRequisition?$select=Employee_No_&$filter=No_ eq '" + DocList.Document_No + "'&$format=json";

                                    HttpWebResponse httpResponseP = Credentials.GetOdataData(pagePRV);
                                    using (var streamReaderP = new StreamReader(httpResponseP.GetResponseStream()))
                                    {
                                        var resultP = streamReaderP.ReadToEnd();

                                        var detailsP = JObject.Parse(resultP);
                                        foreach (JObject configP in detailsP["value"])
                                        {
                                            DocList.Sender_Name = CommonClass.GetEmployeeName((string)configP["Employee_No_"]);
                                        }
                                    }
                                }
                                else if (TbID == "38" && DocType== "Order")
                                {
                                    DocList.Document_Type = DocType;
                                    DocList.Sender_Name = (string)config["Sender_ID"];
                                }
                                else if (TbID == "70134954")
                                {
                                    string pageStore = "StoreReqList?$select=Employee_No&$filter=No eq '" + DocList.Document_No + "'&$format=json";
                                    HttpWebResponse httpResponseStore = Credentials.GetOdataData(pageStore);
                                    using (var streamReaderStore = new StreamReader(httpResponseStore.GetResponseStream()))
                                    {
                                        var resultP = streamReaderStore.ReadToEnd();

                                        var detailsP = JObject.Parse(resultP);
                                        foreach (JObject configP in detailsP["value"])
                                        {
                                            DocList.Sender_Name = CommonClass.GetEmployeeName((string)configP["Employee_No"]);
                                        }
                                    }
                                }
                                else if (TbID == "70135176")
                                {
                                    string pageImp = "ImprestReq?$select=AccountNo&$filter=No eq '" + DocList.Document_No + "'&$format=json";
                                    HttpWebResponse httpResponseStore = Credentials.GetOdataData(pageImp);
                                    using (var streamReaderStore = new StreamReader(httpResponseStore.GetResponseStream()))
                                    {
                                        var resultP = streamReaderStore.ReadToEnd();

                                        var detailsP = JObject.Parse(resultP);
                                        foreach (JObject configP in detailsP["value"])
                                        {
                                            DocList.Sender_Name = CommonClass.GetEmployeeName((string)configP["AccountNo"]);
                                        }
                                    }
                                }
                                else if (TbID == "70135168")
                                {
                                    string pageImp = "ImprestSurrenderList?$select=Account_No&$filter=No eq '" + DocList.Document_No + "'&$format=json";
                                    HttpWebResponse httpResponseStore = Credentials.GetOdataData(pageImp);
                                    using (var streamReaderStore = new StreamReader(httpResponseStore.GetResponseStream()))
                                    {
                                        var resultP = streamReaderStore.ReadToEnd();

                                        var detailsP = JObject.Parse(resultP);
                                        foreach (JObject configP in detailsP["value"])
                                        {
                                            DocList.Sender_Name = CommonClass.GetEmployeeName((string)configP["Account_No"]);
                                        }
                                    }
                                }
                                else if (TbID == "70135138")
                                {
                                    string pageTransport = "TransportReqList?$select=Empoyee_No&$filter=Transport_Requisition_No eq '" + DocList.Document_No + "'&$format=json";
                                    HttpWebResponse httpResponseStore = Credentials.GetOdataData(pageTransport);
                                    using (var streamReaderStore = new StreamReader(httpResponseStore.GetResponseStream()))
                                    {
                                        var resultP = streamReaderStore.ReadToEnd();

                                        var detailsP = JObject.Parse(resultP);
                                        foreach (JObject configP in detailsP["value"])
                                        {
                                            DocList.Sender_Name = CommonClass.GetEmployeeName((string)configP["Employee_No"]);
                                        }
                                    }
                                }
                                else if (TbID == "70135169")
                                {
                                    string pageStaffC = "StaffClaimList?$select=Employee_No&$filter=No eq '" + DocList.Document_No + "'&$format=json";
                                    HttpWebResponse httpResponseStore = Credentials.GetOdataData(pageStaffC);
                                    using (var streamReaderStore = new StreamReader(httpResponseStore.GetResponseStream()))
                                    {
                                        var resultP = streamReaderStore.ReadToEnd();

                                        var detailsP = JObject.Parse(resultP);
                                        foreach (JObject configP in detailsP["value"])
                                        {
                                            DocList.Sender_Name = CommonClass.GetEmployeeName((string)configP["Employee_No"]);
                                        }
                                    }
                                }
                                else
                                {
                                    string Names = CommonClass.GetEmployeeNameByUserID((string)config["Sender_ID"]);
                                    if (Names != "")
                                    {
                                        DocList.Sender_Name = Names;
                                    }
                                    else
                                    {
                                        DocList.Sender_Name = (string)config["Sender_ID"];
                                    }
                                }
                                DocList.DateSend = ((DateTime)config["Date_Time_Sent_for_Approval"]).ToString("dd/MM/yyyy");
                                if (Status == "Open")
                                {
                                    DocList.Status = "Pending Approval";
                                }
                                else
                                {
                                    DocList.Status = Status;
                                }
                                DocList.Sequence = (string)config["Sequence_No"];
                                approvalDocList.Add(DocList);
                            }
                        }
                    }
                    return PartialView("~/Views/DocumentApproval/Document Approval Views/DocumentForApprovalList.cshtml", approvalDocList);
                }
                else
                {
                    return PartialView("~/Views/DocumentApproval/Document Approval Views/DocumentForApprovalList.cshtml");
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult LeaveReqDocApprovalDetails(string DocNo)
        {
            try
            {
                LeaveReqList LeaveDoc = new LeaveReqList();

                string page = "HRLeaveRequisition?$filter=ApplicationNo eq '" + DocNo + "'&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        LeaveDoc.No = (string)config["ApplicationNo"];
                        LeaveDoc.EmpNo = (string)config["EmployeeNo"];
                        LeaveDoc.EmpName = (string)config["EmpoyeeName"];
                        LeaveDoc.Leave_Type = (string)config["LeaveType"];
                        LeaveDoc.Applied_Days = (string)config["DaysApplied"];
                        LeaveDoc.Date = Convert.ToDateTime((string)config["ApplicationDate"]).ToString("dd/MM/yyyy");
                        LeaveDoc.Starting_Date = Convert.ToDateTime((string)config["StartDate"]).ToString("dd/MM/yyyy");
                        LeaveDoc.End_Date = Convert.ToDateTime((string)config["EndDate"]).ToString("dd/MM/yyyy");
                        LeaveDoc.Return_Date = Convert.ToDateTime((string)config["ReturnDate"]).ToString("dd/MM/yyyy");
                        LeaveDoc.Reliever = (string)config["Reliever_Name"];
                        LeaveDoc.Department = (string)config["Department_Name"];
                        LeaveDoc.Remarks = (string)config["Reason_for_leave"];
                        LeaveDoc.Status = (string)config["Status"];
                    }
                }
                return PartialView("~/Views/DocumentApproval/Document Approval Views/LeaveDocumentView.cshtml", LeaveDoc);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult PurchaseReqDocApprovalDetails(string DocNo, string Sequence)
        {
            try
            {
                decimal TotalAmount = 0;
                #region Purchase Header
                PRVHeader PurchaseDoc = new PRVHeader();

                string page = "PurchaseRequisition?$filter=No_ eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        PurchaseDoc.No = (string)config["No_"];
                        PurchaseDoc.Date = Convert.ToDateTime((string)config["Order_Date"]).ToString("dd/MM/yyyy");
                        PurchaseDoc.Remarks = (string)config["Posting_Description"];
                        PurchaseDoc.Directorate = CommonClass.GetDimensionValue((string)config["Shortcut_Dimension_1_Code"]);
                        PurchaseDoc.Department = CommonClass.GetDimensionValue((string)config["Shortcut_Dimension_2_Code"]);
                        //PurchaseDoc.Section = (string)config["Shortcut_Dimension_3_Code"];
                        PurchaseDoc.RespC = (string)config["Responsibility_Center"];
                        PurchaseDoc.Status = (string)config["Status"];
                        PurchaseDoc.RequestorNo = (string)config["Employee_No_"];
                        PurchaseDoc.RequestorName = CommonClass.GetEmployeeName((string)config["Employee_No_"]);
                    }
                }
                #endregion
                #region Purchase Lines
                List<PRVLines> PurchaseLines = new List<PRVLines>();
                string pageLine = "PurchaseLines?$filter=Document_No_ eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        PRVLines PurchaseLine = new PRVLines();
                        if ((string)config["Type"] == "G/L Account")
                        {
                            PurchaseLine.LineType = "Service";
                        }
                        else
                        {
                            PurchaseLine.LineType = (string)config["Type"];
                        }
                        PurchaseLine.Item = (string)config["No_"];
                        PurchaseLine.ItemDesc = (string)config["Description"];
                        PurchaseLine.Description2 = (string)config["Description_2"];
                        PurchaseLine.Qnty = (string)config["Quantity"];
                        PurchaseLine.UnitM = (string)config["Unit_of_Measure"];
                        PurchaseLine.Amount = ((decimal)config["Direct_Unit_Cost"]).ToString("#,##0.00");
                        PurchaseLine.LineAmount = ((decimal)config["Line_Amount"]).ToString("#,##0.00");
                        PurchaseLine.Location = (string)config["Location_Code"];
                        PurchaseLine.LnNo = (string)config["Line_No_"];
                        PurchaseLines.Add(PurchaseLine);
                        TotalAmount = TotalAmount + (decimal)config["Line_Amount"];
                    }
                }
                #endregion
                Session["Location"] = "2";
                string amountInWords = Credentials.ObjNav.ReturnAmountInWords(TotalAmount);
                PurchaseDocument docDetails = new PurchaseDocument
                {
                    DocHeader = PurchaseDoc,
                    ListOfPurchaseLines = PurchaseLines,
                    TotalAmount = TotalAmount.ToString("#,##0.00"),
                    AmountInWords = amountInWords
                };
                return PartialView("~/Views/DocumentApproval/Document Approval Views/PurchaseApprovalDocDetails.cshtml", docDetails);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult PurchaseOrderDocApprovalDetails(string DocNo, string SenderName)
        {
            try
            {
                decimal TotalAmount = 0;

                #region Purchase Req Header

                PRVHeader purchaseDoc1 = new PRVHeader();
                string purchaseReqPage = "PurchaseRequisition?$filter=No_ eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponse1 = Credentials.GetOdataData(purchaseReqPage);

                using (var streamReader = new StreamReader(httpResponse1.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        purchaseDoc1.No = (string)config["No_"];
                        purchaseDoc1.Date = Convert.ToDateTime((string)config["Order_Date"]).ToString("dd/MM/yyyy");
                        purchaseDoc1.Remarks = (string)config["Posting_Description"];
                        purchaseDoc1.Directorate = CommonClass.GetDimensionValue((string)config["Shortcut_Dimension_1_Code"]);
                        purchaseDoc1.Department = CommonClass.GetDimensionValue((string)config["Shortcut_Dimension_2_Code"]);
                        //PurchaseDoc.Section = (string)config["Shortcut_Dimension_3_Code"];
                        purchaseDoc1.RespC = (string)config["Responsibility_Center"];
                        purchaseDoc1.Status = (string)config["Status"];
                    }
                }
                #endregion

                #region Purchase Order
                
                string purchaseOrderPage = "PurchaseOrders?$filter=No eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponse2 = Credentials.GetOdataData(purchaseOrderPage);

                using (var streamReader = new StreamReader(httpResponse2.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        purchaseDoc1.SenderName = SenderName;
                        purchaseDoc1.VendorName = (string)config["Buy_from_Vendor_Name"];
                        purchaseDoc1.DocumentDate = Convert.ToDateTime((string)config["Document_Date"]).ToString("dd/MM/yyyy");
                    }
                }
                #endregion
                
                #region Purchase Lines
                List<PRVLines> PurchaseLines = new List<PRVLines>();
                string pageLine = "PurchaseLines?$filter=Document_No_ eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        PRVLines PurchaseLine = new PRVLines();
                        if ((string)config["Type"] == "G/L Account")
                        {
                            PurchaseLine.LineType = "Service";
                        }
                        else
                        {
                            PurchaseLine.LineType = (string)config["Type"];
                        }
                        PurchaseLine.Item = (string)config["No_"];
                        PurchaseLine.ItemDesc = (string)config["Description"];
                        PurchaseLine.Description2 = (string)config["Description_2"];
                        PurchaseLine.Qnty = (string)config["Quantity"];
                        PurchaseLine.UnitM = (string)config["Unit_of_Measure"];
                        PurchaseLine.Amount = ((decimal)config["Direct_Unit_Cost"]).ToString("#,##0.00");
                        PurchaseLine.LineAmount = ((decimal)config["Line_Amount"]).ToString("#,##0.00");
                        PurchaseLine.Location = (string)config["Location_Code"];
                        PurchaseLine.LnNo = (string)config["Line_No_"];
                        PurchaseLines.Add(PurchaseLine);
                        TotalAmount = TotalAmount + (decimal)config["Line_Amount"];
                    }
                }
                #endregion

                Session["Location"] = "2";
                string amountInWords = Credentials.ObjNav.ReturnAmountInWords(TotalAmount);

                PurchaseDocument docDetails = new PurchaseDocument
                {
                    DocHeader = purchaseDoc1,
                    ListOfPurchaseLines = PurchaseLines,
                    TotalAmount = TotalAmount.ToString("#,##0.00"),
                    AmountInWords = amountInWords
                };
                return PartialView("~/Views/DocumentApproval/Document Approval Views/PurchaseApprovalDocDetails.cshtml", docDetails);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult StoreReqDocApprovalDetails(string DocNo, string Sequence)
        {
            try
            {
                #region Store Header
                StoretHeader StoreDoc = new StoretHeader();

                string page = "StoreReqList?$filter=No eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        StoreDoc.No = (string)config["No"];
                        StoreDoc.DateRequested = Convert.ToDateTime((string)config["Request_date"]).ToString("dd/MM/yyyy");
                        StoreDoc.DateNeeded = Convert.ToDateTime((string)config["Required_Date"]).ToString("dd/MM/yyyy");
                        StoreDoc.Remarks = (string)config["Request_Description"];
                        StoreDoc.Directorate = (string)config["Function_Name"];
                        StoreDoc.Department = (string)config["Budget_Center_Name"];
                        StoreDoc.RespC = (string)config["Responsibility_Center"];
                        StoreDoc.IssuingStore = (string)config["Issuing_Store"];
                        StoreDoc.Status = (string)config["Status"];
                        StoreDoc.RequestorNo = (string)config["Employee_No"];
                        StoreDoc.RequestorName = CommonClass.GetEmployeeName((string)config["Employee_No"]);
                    }
                }
                #endregion
                #region Store Lines
                List<StoreLines> StoreLines = new List<StoreLines>();
                string pageLine = "StoreReqLines?$filter=Requistion_No eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        StoreLines StoreLine = new StoreLines();
                        StoreLine.Item = (string)config["No"];
                        StoreLine.ItemDesc = (string)config["Description"];
                        StoreLine.Qnty = (string)config["Quantity_Requested"];
                        StoreLine.Location = (string)config["Issuing_Store"];
                        StoreLines.Add(StoreLine);
                    }
                }
                #endregion
                Session["Location"] = "3";
                StoreDocument docDetails = new StoreDocument
                {
                    DocHeader = StoreDoc,
                    ListOfStoreLines = StoreLines
                };
                return PartialView("~/Views/DocumentApproval/Document Approval Views/StoreApprovalDocDetails.cshtml", docDetails);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult ImprestReqDocApprovalDetails(string DocNo)
        {
            try
            {
                #region Imp Header
                ImprestHeader ImpDoc = new ImprestHeader();

                string page = "ImprestReq?$filter=No eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        ImpDoc.No = (string)config["No"];
                        ImpDoc.DateNeeded = Convert.ToDateTime((string)config["Date"]).ToString("dd/MM/yyyy");
                        ImpDoc.Remarks = (string)config["Purpose"];
                        ImpDoc.RequestorNo = (string)config["AccountNo"];
                        ImpDoc.RequestorName = CommonClass.GetEmployeeName((string)config["AccountNo"]);
                        ImpDoc.Directorate = CommonClass.GetDimensionValue((string)config["GlobalDimension1Code"]);
                        ImpDoc.Department = CommonClass.GetDimensionValue((string)config["ShortcutDimension2Code"]);
                        //ImpDoc.Section = "";
                        ImpDoc.TotalAmount = Convert.ToDecimal((string)config["TotalNetAmount"]).ToString("#,##0.00");
                        ImpDoc.Status = (string)config["Status"];
                    }
                }
                #endregion
                #region Imp Lines
                List<ImprestLines> ImpLines = new List<ImprestLines>();
                string pageLine = "ImprestLines?$filter=No eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        ImprestLines ImLine = new ImprestLines();
                        ImLine.AdvanceType = (string)config["Advance_Type"];
                        ImLine.Item = (string)config["Account_No"];
                        ImLine.ItemDesc = (string)config["Account_Name"];
                        ImLine.ItemDesc2 = (string)config["Purpose"];
                        ImLine.Quantity = (string)config["Quantity"];
                        ImLine.UnitAmount = ((decimal)config["Daily_Rate_Amount"]).ToString("#,##0.00"); ;
                        ImLine.Amount = ((decimal)config["Amount"]).ToString("#,##0.00");
                        ImpLines.Add(ImLine);
                    }
                }
                #endregion
                Session["Location"] = "4";
                ImprestDocument docDetails = new ImprestDocument
                {
                    DocHeader = ImpDoc,
                    ListOfImprestLines = ImpLines
                };
                return PartialView("~/Views/DocumentApproval/Document Approval Views/ImprestApprovalDocDetails.cshtml", docDetails);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult ImprestSurrenderDocApprovalDetails(string DocNo, string Sequence)
        {
            try
            {
                #region Imp Surrender Header
                ImprestSurrenderHeader ImpDoc = new ImprestSurrenderHeader();

                string page = "ImprestSurrenderCard?$filter=No eq '" + DocNo + "'&format=json";
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
                        ImpDoc.AccountName = (string)config["Received_From"];
                        ImpDoc.ImprestNo = (string)config["Imprest_Issue_Doc_No"];
                        ImpDoc.ImpIssueDate = (string)config["Imprest_Issue_Date"];
                        ImpDoc.Directorate = CommonClass.GetDimensionValue((string)config["Global_Dimension_1_Code"]);
                        ImpDoc.Department = CommonClass.GetDimensionValue((string)config["Shortcut_Dimension_2_Code"]);
                        ImpDoc.RespC = (string)config["Responsibility_Center"];
                        ImpDoc.TotalAmount = Convert.ToDecimal((string)config["Amount"]).ToString("#,##0.00");
                        ImpDoc.Status = (string)config["Status"];
                        ImpDoc.ImpPurpose = (string)config["Imp_Purpose"];
                    }
                }
                #endregion
                #region Imp surrender Lines
                List<ImprestSurrenderLines> ImpLines = new List<ImprestSurrenderLines>();
                string pageLine = "ImpSurrenderLines?$filter=SurrenderDocNo eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        ImprestSurrenderLines ImSLine = new ImprestSurrenderLines();
                        ImSLine.SurrenderDocNo = (string)config["SurrenderDocNo"];
                        ImSLine.AccountNo = (string)config["AccountNo"];
                        ImSLine.AccountName = (string)config["AccountName"];
                        ImSLine.Amount = Convert.ToDecimal((string)config["Amount"]).ToString("#,##0.00");
                        ImSLine.ActaulSpend = (string)config["ActualSpent"];
                        ImSLine.ReceiptNo = (string)config["CashReceiptNo"];
                        ImSLine.ReceiptAmount = Convert.ToDecimal((string)config["CashReceiptAmount"]).ToString("#,##0.00");
                        ImpLines.Add(ImSLine);
                    }
                }
                #endregion
                Session["Location"] = "7";
                ImprestSurrenderDocument docDetails = new ImprestSurrenderDocument
                {
                    DocHeader = ImpDoc,
                    ListOfImprestSurrenderLines = ImpLines
                };
                return PartialView("~/Views/DocumentApproval/Document Approval Views/ImprestSurrenderApprovalDocDetails.cshtml", docDetails);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult StaffClaimReqDocApprovalDetails(string DocNo, string Sequence)
        {
            try
            {
                #region Staff Claim Header
                StaffClaimHeader ClaimDoc = new StaffClaimHeader();

                string page = "StaffClaimCard?$filter=No eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        ClaimDoc.No = (string)config["No"];
                        ClaimDoc.DateRequested = Convert.ToDateTime((string)config["Date"]).ToString("dd/MM/yyyy");
                        ClaimDoc.Remarks = (string)config["Purpose"];
                        ClaimDoc.Directorate = CommonClass.GetDimensionValue((string)config["GlobalDimension1Code"]);
                        ClaimDoc.Department = CommonClass.GetDimensionValue((string)config["ShortcutDimension2Code"]);
                        ClaimDoc.TotalAmount = Convert.ToDecimal((string)config["TotalNetAmount"]).ToString("#,##0.00");
                        ClaimDoc.Status = (string)config["Status"];
                        ClaimDoc.RequestorNo = (string)config["StaffNoName"];
                        ClaimDoc.RequestorName = CommonClass.GetEmployeeName((string)config["StaffNoName"]);
                    }
                }
                #endregion
                #region Staff Claim Lines
                List<StaffClaimLines> ClaimLines = new List<StaffClaimLines>();
                string pageLine = "StaffCaimLines?$filter=No eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        StaffClaimLines claimLine = new StaffClaimLines();
                        claimLine.AdvanceType = (string)config["Advance_Type"];
                        claimLine.Item = (string)config["Account_No"];
                        claimLine.ItemDesc = (string)config["Account_Name"];
                        claimLine.ItemDesc2 = (string)config["Purpose"];
                        claimLine.Amount = Convert.ToDecimal((string)config["Amount"]).ToString("#,##0.00");
                        ClaimLines.Add(claimLine);
                    }
                }
                #endregion
                Session["Location"] = "5";
                StaffClaimDocument docDetails = new StaffClaimDocument
                {
                    DocHeader = ClaimDoc,
                    ListOfStaffClaimLines = ClaimLines
                };
                return PartialView("~/Views/DocumentApproval/Document Approval Views/StaffclaimApprovalDocDetails.cshtml", docDetails);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult TransportReqDocApprovalDetails(string DocNo, string Sequence)
        {
            try
            {
                #region Header
                TransportReqList TransDoc = new TransportReqList();
                string page = "TransportReqList?$filter=Transport_Requisition_No eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        TransDoc.No = (string)config["Transport_Requisition_No"];
                        TransDoc.Commencement = (string)config["From"];
                        TransDoc.Destination = (string)config["To"];
                        TransDoc.Vehicle = (string)config["Vehicle_Allocated"];
                        TransDoc.Driver = (string)config["Driver_Allocated"];
                        TransDoc.DateRequested = Convert.ToDateTime((string)config["Date_of_Request"]).ToString("dd/MM/yyyy");
                        TransDoc.DateOfTrip = Convert.ToDateTime((string)config["Date_of_Trip"]).ToString("dd/MM/yyyy");
                        TransDoc.NoOfDays = (string)config["No_of_Days_Requested"];
                        TransDoc.Status = (string)config["Status"];
                    }
                }
                #endregion
                #region Passenger Lines
                List<Passengers> PassengerList = new List<Passengers>();
                string pageLine = "TransportPassengers?$filter=Req_No eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        Passengers passenger = new Passengers();
                        passenger.Type = (string)config["Passenger_Type"];
                        passenger.No = (string)config["No"];
                        passenger.Name = (string)config["Name"];
                        passenger.Position = (string)config["Position"];
                        PassengerList.Add(passenger);
                    }
                }
                #endregion
                #region Driver List
                List<DropdownList> driverList = new List<DropdownList>();
                string pageDriver = "FltDriverList?$format=json";
                HttpWebResponse httpResponseDriver = Credentials.GetOdataData(pageDriver);
                using (var streamReader = new StreamReader(httpResponseDriver.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        DropdownList d = new DropdownList();
                        d.Value = (string)config["Driver"];
                        d.Text = (string)config["Driver_Name"];
                        driverList.Add(d);
                    }
                }
                #endregion
                #region Vehicle List
                List<DropdownList> vehiclerList = new List<DropdownList>();
                string pageVehicle = "FltVehicleList?$format=json";
                HttpWebResponse httpResponseVehicle = Credentials.GetOdataData(pageVehicle);
                using (var streamReader = new StreamReader(httpResponseVehicle.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        DropdownList v = new DropdownList();
                        v.Value = (string)config["No"];
                        v.Text = (string)config["No"] + "-" + (string)config["Description"];
                        vehiclerList.Add(v);
                    }
                }
                #endregion

                TransDocument docDetails = new TransDocument
                {
                    DocHeader = TransDoc,
                    ListOfPassengers = PassengerList,
                    ListOfDrivers = driverList.Select(x =>
                                                 new SelectListItem()
                                                 {
                                                     Text = x.Text,
                                                     Value = x.Value
                                                 }).ToList(),
                    ListOfVehicles = vehiclerList.Select(x =>
                                                new SelectListItem()
                                                {
                                                    Text = x.Text,
                                                    Value = x.Value
                                                }).ToList()
                };
                return PartialView("~/Views/DocumentApproval/Document Approval Views/TransportApprovalDocDetails.cshtml", docDetails);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult TrainingReqDocApprovalDetails(string DocNo, string Sequence)
        {
            try
            {
                #region Header
                TrainingList TranDoc = new TrainingList();
                string page = "HRTrainingApplication?$filter=Application_No eq '" + DocNo + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        TranDoc.Application_No = (string)config["Application_No"];
                        TranDoc.Application_Date = Convert.ToDateTime((string)config["Application_Date"]).ToString("dd/MM/yyyy");
                        TranDoc.StartDate = Convert.ToDateTime((string)config["From_Date"]).ToString("dd/MM/yyyy");
                        TranDoc.EndDate = Convert.ToDateTime((string)config["To_Date"]).ToString("dd/MM/yyyy");
                        TranDoc.Training_Category = (string)config["Training_Category"];
                        TranDoc.Course_Title = (string)config["Course_Title"];
                        TranDoc.Course_Desc = (string)config["Description"];
                        TranDoc.Directorate = CommonClass.GetDimensionValue((string)config["Global_Dimension_1"]);
                        TranDoc.Department = CommonClass.GetDimensionValue((string)config["Global_Dimension_2"]);
                        TranDoc.Trainer = (string)config["Trainer"];
                        TranDoc.Purpose = (string)config["Purpose_of_Training"];
                        TranDoc.Status = (string)config["Status"];
                    }
                }
                #endregion
                #region Training Lines
                List<Trainees> participantList = new List<Trainees>();
                string pageLine = "HRTrainingPartcipants?$filter=TrainingCode eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        Trainees participants = new Trainees();
                        participants.No = (string)config["EmployeeCode"];
                        participants.Name = (string)config["Employeename"];
                        participantList.Add(participants);
                    }
                }
                #endregion
                #region Training Lines
                List<TrainingCost> TrainingCostList = new List<TrainingCost>();
                string pageTLine = "HRTrainingCost?$filter=TrainingId eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponseTLine = Credentials.GetOdataData(pageTLine);
                using (var streamReader = new StreamReader(httpResponseTLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        TrainingCost TrCost = new TrainingCost();
                        TrCost.No = (string)config["TrainingId"];
                        TrCost.Item = (string)config["TrainingCostItem"];
                        TrCost.Cost = ((decimal)config["Cost"]).ToString("#,##0.00");
                        TrainingCostList.Add(TrCost);
                    }
                }
                #endregion
                TrainingDocument docDetails = new TrainingDocument
                {
                    DocHeader = TranDoc,
                    ListOfTrainees = participantList,
                    ListOfTraininingCost = TrainingCostList
                };
                return PartialView("~/Views/DocumentApproval/Document Approval Views/TrainingApprovalDocDetails.cshtml", docDetails);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult TransferOrderDocApprovalDetails(string DocNo, string Sequence)
        {
            try
            {
                //#region Purchase Header
                //TransferOrderHeader TOrderDoc = new TransferOrderHeader();

                //string page = "Transfer_Orders?$filter=No eq '" + DocNo + "'&format=json";
                //HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                //{
                //    var result = streamReader.ReadToEnd();

                //    var details = JObject.Parse(result);
                //    foreach (JObject config in details["value"])
                //    {
                //        TOrderDoc.No = (string)config["No"];
                //        TOrderDoc.TransferFrom = (string)config["Transfer_from_Name"];
                //        TOrderDoc.TransferTo = (string)config["Transfer_to_Name"];
                //        TOrderDoc.Posting_Date = Convert.ToDateTime((string)config["Posting_Date"]).ToString("dd/MM/yyyy");
                //        TOrderDoc.Shipment_Date = Convert.ToDateTime((string)config["Shipment_Date"]).ToString("dd/MM/yyyy");
                //        TOrderDoc.Receipt_Date = Convert.ToDateTime((string)config["Receipt_Date"]).ToString("dd/MM/yyyy");
                //        TOrderDoc.CampusName = CommonClass.GetDimensionValue((string)config["Shortcut_Dimension_1_Code"]);
                //        TOrderDoc.Department = CommonClass.GetDimensionValue((string)config["Shortcut_Dimension_2_Code"]);
                //        TOrderDoc.Status = (string)config["Status"];
                //        TOrderDoc.AssignedUser = (string)config["Assigned_User_ID"];
                //        TOrderDoc.AssignedUserName = CommonClass.GetEmployeeName((string)config["Assigned_User_ID"]);
                //    }
                //}
                //#endregion
                //#region Purchase Lines
                //List<TransferOrderLines> TransferLines = new List<TransferOrderLines>();
                //string pageLine = "Transfer_Line?$filter=Document_No eq '" + DocNo + "'&format=json";
                //HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                //using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                //{
                //    var result = streamReader.ReadToEnd();

                //    var details = JObject.Parse(result);
                //    foreach (JObject config in details["value"])
                //    {
                //        TransferOrderLines TLine = new TransferOrderLines();

                //        TLine.Item = (string)config["Item_No"];
                //        TLine.ItemDesc = (string)config["Description"];
                //        TLine.Qnty = (string)config["Quantity"];
                //        TLine.UoM = (string)config["Unit_of_Measure"];
                //        TLine.Qty_to_Ship = (string)config["Qty_to_Ship"];
                //        TransferLines.Add(TLine);
                //    }
                //}
                //#endregion
                //TransferOrderDocument docDetails = new TransferOrderDocument
                //{
                //    DocHeader = TOrderDoc,
                //    ListOfTransferOrderLines = TransferLines
                //};
                return PartialView("~/Views/DocumentApproval/Document Approval Views/TransferOrder.cshtml");
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult PVReqDocApprovalDetails(string DocNo)
        {
            try
            {
                #region PV Header
                PaymentHeader PayDoc = new PaymentHeader();

                string page = "PVHeader?$filter=No eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        PayDoc.No = (string)config["No"];
                        PayDoc.Date = Convert.ToDateTime((string)config["Date"]).ToString("dd/MM/yyyy");
                        PayDoc.Remarks = (string)config["Payment_Narration"];
                        PayDoc.Directorate = (string)config["Function_Name"];
                        PayDoc.Department = (string)config["Budget_Center_Name"];
                        PayDoc.Pay_Mode = (string)config["Pay_Mode"];
                        PayDoc.ChequeNo = (string)config["Cheque_No"];
                        PayDoc.PayingBank = (string)config["Bank_Name"];
                        PayDoc.Paying_Bank_Account = (string)config["Paying_Bank_Account"];
                        PayDoc.PaymentTo = (string)config["Payee"];
                        PayDoc.OnBehalfOf = (string)config["On_Behalf_Of"];
                        PayDoc.RaisedBy = (string)config["Cashier"];
                        PayDoc.TotalAmount = Convert.ToDecimal((string)config["Total_Payment_Amount"]).ToString("#,##0.00");
                        PayDoc.TotalVATAmount = Convert.ToDecimal((string)config["Total_VAT_Amount"]).ToString("#,##0.00");
                        PayDoc.TotalWithHTAXAmount = Convert.ToDecimal((string)config["Total_Witholding_Tax_Amount"]).ToString("#,##0.00");
                        PayDoc.TotalRETAmount = Convert.ToDecimal((string)config["Total_Retention_Amount"]).ToString("#,##0.00");
                        PayDoc.TotalVATWITHHAmount = Convert.ToDecimal((string)config["Total_VAT_Withholding_Amount"]).ToString("#,##0.00");
                        PayDoc.TotalPAYEAmount = Convert.ToDecimal((string)config["Total_PAYE_Amount"]).ToString("#,##0.00");
                        PayDoc.TotalNETAmount = Convert.ToDecimal((string)config["Total_Net_Amount"]).ToString("#,##0.00");
                        PayDoc.Status = (string)config["Status"];
                    }
                }
                #endregion
                #region Payment Lines
                List<PaymentLines> PLines = new List<PaymentLines>();
                string pageLine = "PVLines?$filter=No eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        PaymentLines PLine = new PaymentLines();
                        PLine.DocNo = (string)config["No"];
                        PLine.Type = (string)config["Type"];
                        PLine.AccountType = (string)config["AccountType"];
                        PLine.AccountNo = (string)config["AccountNo"];
                        PLine.AccountName = (string)config["AccountName"];
                        PLine.Amount = ((decimal)config["Amount"]).ToString("#,##0.00");
                        PLines.Add(PLine);
                    }
                }
                #endregion
                PaymentDocument docDetails = new PaymentDocument
                {
                    DocHeader = PayDoc,
                    ListOfPaymentLines = PLines
                };
                return PartialView("~/Views/DocumentApproval/Document Approval Views/PVDocument.cshtml", docDetails);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult PayrollJouralLines()
        {
            try
            {
                #region Payroll Journal Lines
                List<PayrollJournal> PLines = new List<PayrollJournal>();
                string pageLine = "GenJournalLines?$filter=Journal_Batch_Name eq 'PAYROLL'&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        PayrollJournal ln = new PayrollJournal();
                        ln.Posting_Date = ((DateTime)config["Posting_Date"]).ToString("dd/MM/yyyy");
                        ln.DocNo = (string)config["Document_No"];
                        ln.AccountNo = (string)config["Account_No"];
                        ln.AccountName = (string)config["AccountName"];
                        ln.AccountName = (string)config["Description"];
                        ln.Amount = ((decimal)config["Amount"]).ToString("#,##0.00");
                        PLines.Add(ln);
                    }
                }
                #endregion                
                return PartialView("~/Views/DocumentApproval/Document Approval Views/PayrollJournal.cshtml", PLines);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [HttpPost]
        public JsonResult ApproveDocument(string DocNo, string EntryNo)
        {
            try
            {
                bool rtVal = false;
                string msg = "";

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    Credentials.ObjNav.DocumentApprovals(Convert.ToInt32(EntryNo), userID);
                    rtVal = true;
                    msg = "Request approved Successfully";
                }
                else
                {
                    rtVal = false;
                    msg = "/Login/Login";
                }
                return Json(new { message = msg, success = true, Redirect = rtVal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult RejectDocument(string TBID, string DocNo, string Comments, string SeqNo, string EntryNo)
        {
            try
            {
                string userID = Session["UserID"].ToString();

                Credentials.ObjNav.DocumentRejections(Convert.ToInt32(EntryNo), DocNo, userID, Comments,
                   Convert.ToInt32(TBID), Convert.ToInt32(SeqNo));
                string msg = "Approval Request Rejected";
                return Json(new { message = msg, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult PreviousApprovalTrail(string TableID, string RecordID)
        {
            try
            {
                List<DocumentsForApproval> approvalDocList = new List<DocumentsForApproval>();

                string page = "ApprovalEntries?$select=Sequence_No,Approver_ID&$filter=Record_ID_to_Approve eq '" + RecordID + "' and Table_ID eq " + TableID + " and Status eq 'Approved'&format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        DocumentsForApproval DocList = new DocumentsForApproval();
                        DocList.Sequence = (string)config["Sequence_No"];
                        string EmplName = (string)config["ApproverNames"];
                        if (EmplName != null)
                        {
                            DocList.Approver = EmplName;
                        }
                        else
                        {
                            DocList.Approver = (string)config["Approver_ID"];
                        }
                        approvalDocList.Add(DocList);
                    }
                }
                return PartialView("~/Views/DocumentApproval/Document Approval Views/DocApprovalTrail.cshtml", approvalDocList.OrderByDescending(x => x.Sequence));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [HttpPost]
        public JsonResult AssignDriverVehicle(string DocNo, string Driver, string Vehicle)
        {
            try
            {
                bool rtVal = false;
                string msg = "";

                if (Session["Username"] != null)
                {
                    string userID = Session["Username"].ToString();
                    Credentials.ObjNav.AssignTransportRequisitionDriver(DocNo, Driver, Vehicle, userID);
                    rtVal = true;
                    msg = "Request approved Successfully";
                }
                else
                {
                    rtVal = false;
                    msg = "/Login/Login";
                }
                return Json(new { message = msg, success = true, Redirect = rtVal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}