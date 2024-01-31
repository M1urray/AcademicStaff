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
    public class DocumentApprovalController : Controller
    {
        [CustomeAuthentication]
        [CustomAuthorization(Role = "FULLTIME")]
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
                        DocCount.SRNCount = 0;
                        DocCount.ImpCount = 0;
                        DocCount.SurrCount = 0;
                        DocCount.ClaimCount = 0;
                        DocCount.Clearance = 0;
                        DocCount.TransferOrder = 0;

                        string page = "ApprovalEntries?$filter=Approver_ID eq '" + userID + "' and Status eq '" + rn + "'&format=json";
                        HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);
                            foreach (JObject config in details["value"])
                            {
                                if ((string)config["Table_ID"] == "70135114")
                                {
                                    DocCount.LeaveCount = DocCount.LeaveCount + 1;
                                }
                                if ((string)config["Table_ID"] == "38")
                                {
                                    DocCount.PRNCount = DocCount.PRNCount + 1;
                                }
                                if ((string)config["Table_ID"] == "70135010")
                                {
                                    DocCount.SRNCount = DocCount.SRNCount + 1;
                                }
                                if ((string)config["Table_ID"] == "70135469")
                                {
                                    DocCount.ImpCount = DocCount.ImpCount + 1;
                                }
                                if ((string)config["Table_ID"] == "70135450")
                                {
                                    DocCount.SurrCount = DocCount.SurrCount + 1;
                                }
                                if ((string)config["Table_ID"] == "70135454")
                                {
                                    DocCount.ClaimCount = DocCount.ClaimCount + 1;
                                }
                                if ((string)config["Table_ID"] == "70134894")
                                {
                                    DocCount.Clearance = DocCount.Clearance + 1;
                                }
                                if ((string)config["Table_ID"] == "5740")
                                {
                                    DocCount.TransferOrder = DocCount.TransferOrder + 1;
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
        public ActionResult GetDocumentForApprovalList(string TbID, string Title, string Status)
        {
            try
            {
                if (TbID != "" && Title != "" && Status != "")
                {
                    DocumentsForApprovalList newList = new DocumentsForApprovalList
                    {
                        TableID = TbID,
                        Title = Title,
                        Status = Status
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
        public ActionResult LoadDocumentForApprovalList(string TbID, string Status)
        {
            try
            {
                if (TbID != "")
                {
                    string userID = Session["UserID"].ToString();
                    List<DocumentsForApproval> approvalDocList = new List<DocumentsForApproval>();
                    string page = "ApprovalEntries?$select=Entry_No,Table_ID,Document_No,Sender_ID,Date_Time_Sent_for_Approval,Status,Sequence_No,SenderNames,Comment&$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" + Status + "'&format=json";
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
                                    string s = (string)config["RecordIDText"];
                                    string[] t = s.Split(':');
                                    DocList.Document_No = t[1].Trim();
                                }
                                else
                                {
                                    DocList.Document_No = (string)config["Document_No"];
                                }
                                DocList.Sender_Name = (string)config["SenderNames"];
                                DocList.DateSend = Convert.ToDateTime((string)config["Date_Time_Sent_for_Approval"]).ToString("dd/MM/yyyy");
                                DocList.Status = (string)config["Status"];
                                DocList.Sequence = (string)config["Sequence_No"];
                                if ((string)config["Table_ID"] == "70134894")
                                {
                                    string[] s = StudentDetails(DocList.Document_No);
                                    DocList.Sender_Name = s[0];
                                    DocList.Status = s[1];

                                    string comment = CommonClass.GetDocRejectionComment(DocList.Document_No, (int)config["Sequence_No"]);
                                    if (comment != "")
                                    {
                                        DocList.CommentFound = true;
                                    }
                                }
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
        public PartialViewResult LeaveReqDocApprovalDetails(string DocNo, string Sequence)
        {
            try
            {
                LeaveReqList LeaveDoc = new LeaveReqList();
                string page = "HRLeaveRequisition?$filter=No eq '" + DocNo + "'&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        LeaveDoc.No = (string)config["No"];
                        LeaveDoc.EmpNo = (string)config["Employee_No"];
                        LeaveDoc.EmpName = (string)config["Employee_Name"];
                        LeaveDoc.Leave_Type = (string)config["Leave_Type"];
                        LeaveDoc.Applied_Days = (string)config["Applied_Days"];
                        LeaveDoc.Date = ((DateTime)config["Date"]).ToString("dd/MM/yyyy");
                        LeaveDoc.Starting_Date = ((DateTime)config["Starting_Date"]).ToString("dd/MM/yyyy");
                        LeaveDoc.End_Date = ((DateTime)config["End_Date"]).ToString("dd/MM/yyyy");
                        LeaveDoc.Return_Date = ((DateTime)config["Return_Date"]).ToString("dd/MM/yyyy");
                        LeaveDoc.Reliever = (string)config["Reliever_Name"];
                        LeaveDoc.Responsibility = (string)config["Responsibility_Center"];
                        LeaveDoc.Remarks = (string)config["Purpose"];
                        LeaveDoc.Status = (string)config["Status"];
                    }
                }
                return PartialView("~/Views/DocumentApproval/Document Approval Views/LeaveApprovalDocDetails.cshtml", LeaveDoc);
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

                string page = "PurchaseRegDocument?$filter=No eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        PurchaseDoc.No = (string)config["No"];
                        PurchaseDoc.Date = Convert.ToDateTime((string)config["Order_Date"]).ToString("dd/MM/yyyy");
                        PurchaseDoc.Remarks = (string)config["Posting_Description"];
                        PurchaseDoc.Campus = CommonClass.GetDimensionValue((string)config["Shortcut_Dimension_1_Code"]);
                        PurchaseDoc.Department = (string)config["Department_Name"];
                        PurchaseDoc.RespC = (string)config["Responsibility_Center"];
                        PurchaseDoc.Status = (string)config["Status"];
                        PurchaseDoc.RequestorNo = (string)config["Employee_No"];
                        PurchaseDoc.RequestorName = CommonClass.GetEmployeeName((string)config["Employee_No"]);
                    }
                }
                #endregion
                #region Purchase Lines
                List<PRVLines> PurchaseLines = new List<PRVLines>();
                string pageLine = "PurchaseLines?$filter=Document_No eq '" + DocNo + "'&$format=json";
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
                        PurchaseLine.Item = (string)config["No"];
                        PurchaseLine.ItemDesc = (string)config["Description"];
                        PurchaseLine.Qnty = (string)config["Quantity"];
                        PurchaseLine.UnitM = (string)config["Unit_of_Measure"];
                        PurchaseLine.Amount = Convert.ToDecimal((string)config["Direct_Unit_Cost"]).ToString("#,##0.00");
                        PurchaseLine.LineAmount = Convert.ToDecimal((string)config["Line_Amount"]).ToString("#,##0.00");
                        PurchaseLine.Location = (string)config["Location_Code"];
                        PurchaseLines.Add(PurchaseLine);
                        TotalAmount = TotalAmount + (decimal)config["Direct_Unit_Cost"];
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
                        StoreDoc.Campus = (string)config["Function_Name"];
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

                string page = "ImprestReq?$filter=No eq '" + DocNo + "'&format=json";
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
                        ImpDoc.school = (string)config["Function_Name"];
                        ImpDoc.Campus = (string)config["FunctionName"];
                        ImpDoc.Department = (string)config["Department_Name"];
                        ImpDoc.RespC = (string)config["ResponsibilityCenter"];
                        ImpDoc.TotalAmount = Convert.ToDecimal((string)config["TotalNetAmount"]).ToString("#,##0.00");
                        ImpDoc.Status = (string)config["Status"];
                        ImpDoc.RequestorNo = (string)config["Employee_No"];
                        ImpDoc.RequestorName = CommonClass.GetEmployeeName((string)config["Employee_No"]);
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
                        ImLine.Amount = Convert.ToDecimal((string)config["Amount"]).ToString("#,##0.00");
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
                        ImpDoc.CampusName = (string)config["Global_Dimension_1_Code"];
                        ImpDoc.DepartmentName = (string)config["Shortcut_Dimension_2_Code"];
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

                string page = "StaffClaimCard?$filter=No eq '" + DocNo + "'&format=json";
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
                        ClaimDoc.school = (string)config["Dim3"];
                        ClaimDoc.Campus = (string)config["Function_Name"];
                        ClaimDoc.Department = (string)config["Budget_Center_Name"];
                        ClaimDoc.RespC = (string)config["Responsibility_Center"];
                        ClaimDoc.TotalAmount = Convert.ToDecimal((string)config["Total_Net_Amount"]).ToString("#,##0.00");
                        ClaimDoc.Status = (string)config["Status"];
                        ClaimDoc.RequestorNo = (string)config["Account_No"];
                        ClaimDoc.RequestorName = CommonClass.GetEmployeeName((string)config["Account_No"]);
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
        public PartialViewResult StudentClearanceDocDetails(string DocNo, string Sequence)
        {
            try
            {
                #region ClearanceDoc
                StudentClearance ClearanceDoc = new StudentClearance();

                string page = "StudentRequisitionCard?$filter=Code eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        ClearanceDoc.Code = (string)config["Code"];
                        ClearanceDoc.StudentNo = (string)config["StudentNo"];
                        ClearanceDoc.StudentName = (string)config["Name"];
                        ClearanceDoc.RegType = (string)config["RequisitionType"];
                        ClearanceDoc.DateApplied = Convert.ToDateTime((string)config["Date"]).ToString("dd/MM/yyyy");
                        ClearanceDoc.Status = (string)config["Status"];
                        ClearanceDoc.Programme = (string)config["CurrentProgramme"] + "(" + (string)config["CurrntProgDescr"] + ")";
                        ClearanceDoc.Semester = (string)config["Semester"];
                        ClearanceDoc.StudentStatus = CommonClass.StudentStatus((string)config["StudentNo"]);
                        if (Sequence != "")
                        {
                            string comment = CommonClass.GetDocRejectionComment(DocNo, Convert.ToInt32(Sequence));
                            if (comment != "")
                            {
                                ClearanceDoc.CommentFound = true;
                                ClearanceDoc.Comment = comment;
                            }
                        }
                    }
                }
                #endregion
                Session["Location"] = "6";
                return PartialView("~/Views/DocumentApproval/Document Approval Views/StudentClearanceDoc.cshtml", ClearanceDoc);
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
                #region Purchase Header
                TransferOrderHeader TOrderDoc = new TransferOrderHeader();

                string page = "Transfer_Orders?$filter=No eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        TOrderDoc.No = (string)config["No"];
                        TOrderDoc.TransferFrom = (string)config["Transfer_from_Name"];
                        TOrderDoc.TransferTo = (string)config["Transfer_to_Name"];
                        TOrderDoc.Posting_Date = Convert.ToDateTime((string)config["Posting_Date"]).ToString("dd/MM/yyyy");
                        TOrderDoc.Shipment_Date = Convert.ToDateTime((string)config["Shipment_Date"]).ToString("dd/MM/yyyy");
                        TOrderDoc.Receipt_Date = Convert.ToDateTime((string)config["Receipt_Date"]).ToString("dd/MM/yyyy");
                        TOrderDoc.CampusName = CommonClass.GetDimensionValue((string)config["Shortcut_Dimension_1_Code"]);
                        TOrderDoc.Department = CommonClass.GetDimensionValue((string)config["Shortcut_Dimension_2_Code"]);
                        TOrderDoc.Status = (string)config["Status"];
                        TOrderDoc.AssignedUser = (string)config["Assigned_User_ID"];
                        TOrderDoc.AssignedUserName = CommonClass.GetEmployeeName((string)config["Assigned_User_ID"]);
                    }
                }
                #endregion
                #region Purchase Lines
                List<TransferOrderLines> TransferLines = new List<TransferOrderLines>();
                string pageLine = "Transfer_Line?$filter=Document_No eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        TransferOrderLines TLine = new TransferOrderLines();

                        TLine.Item = (string)config["Item_No"];
                        TLine.ItemDesc = (string)config["Description"];
                        TLine.Qnty = (string)config["Quantity"];
                        TLine.UoM = (string)config["Unit_of_Measure"];
                        TLine.Qty_to_Ship = (string)config["Qty_to_Ship"];
                        TransferLines.Add(TLine);
                    }
                }
                #endregion
                TransferOrderDocument docDetails = new TransferOrderDocument
                {
                    DocHeader = TOrderDoc,
                    ListOfTransferOrderLines = TransferLines
                };
                return PartialView("~/Views/DocumentApproval/Document Approval Views/TransferOrder.cshtml", docDetails);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public JsonResult ApproveDocument(string DocNo,string EntryNo)
        {
            try
            {
                string userID = Session["UserID"].ToString();
                Credentials.ObjNav.DocumentApprovals(Convert.ToInt32(EntryNo), DocNo, userID);
                Session["SuccessMsg"] = "Request approved Successfully";
                return Json(new { message = "Request approved Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult RejectDocument(string TBID, string DocNo, string Comments, string SeqNo, string EntryNo)
        {
            try
            {
                string userID = Session["UserID"].ToString();
                //Credentials.ObjNav.DocumentRejectionCommentLine(
                //documentNo: DocNo,
                //commentLineText: Comments,
                //webUser: userID,
                //documentType: 0,
                //table_ID: Convert.ToInt32(TBID),
                //seqenceNo: Convert.ToInt32(SeqNo));
                string msg = "";
                if (TBID == "70134894")
                {
                    msg = "Clearance Approval Request Rejected";
                }
                else
                {
                    Credentials.ObjNav.DocumentRejections(Convert.ToInt32(EntryNo),DocNo, userID, Comments,
                        Convert.ToInt32(TBID), Convert.ToInt32(SeqNo));
                    msg = "Approval Request Rejected";
                }
                Session["SuccessMsg"] = msg;
                return Json(new { message = msg, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult PreviousApprovalTrail(string TableID, string DocNo)
        {
            try
            {
                List<DocumentsForApproval> approvalDocList = new List<DocumentsForApproval>();

                string page = "ApprovalEntries?$select=Sequence_No,Approver_ID&$filter=Document_No eq '" + DocNo + "' and Table_ID eq " + TableID + " and Status eq 'Approved'&format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        DocumentsForApproval DocList = new DocumentsForApproval();
                        DocList.Sequence = (string)config["Sequence_No"];
                        string[] s = config["Approver_ID"].ToString().Split('\\');
                        string EmplName = CommonClass.GetEmployeeName(s[1]);
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
        internal string[] StudentDetails(string DocNo)
        {
            string[] s = new string[3];
            try
            {
                string pageCl = "StudentRequisitionCard?$select=StudentNo&$filter=Code eq '" + DocNo + "'&format=json";
                HttpWebResponse httpResponseCl = Credentials.GetOdataData(pageCl);
                using (var streamReaderCl = new StreamReader(httpResponseCl.GetResponseStream()))
                {
                    var resultCl = streamReaderCl.ReadToEnd();

                    var detailsCl = JObject.Parse(resultCl);
                    foreach (JObject configCl in detailsCl["value"])
                    {
                        string page = "StudentCard?$select=Status&$filter=No eq '" + (string)configCl["StudentNo"] + "'&$format=json";

                        HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);

                            foreach (JObject config in details["value"])
                            {
                                s[0] = (string)configCl["StudentNo"];
                                s[1] = (string)config["Status"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return s;
        }
    }
}