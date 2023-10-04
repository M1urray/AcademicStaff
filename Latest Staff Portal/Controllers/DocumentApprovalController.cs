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
        [CustomAuthorization(Role = "ALLUSERS")]
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

                        #region Other Documents
                        DocumentCount DocCount = new DocumentCount();

                        DocCount.LeaveCount = 0;
                        DocCount.PRNCount = 0;
                        DocCount.LPOCount = 0;
                        DocCount.SRNCount = 0;
                        DocCount.ImpCount = 0;
                        DocCount.SurrCount = 0;
                        DocCount.ClaimCount = 0;
                        DocCount.TransportCount = 0;
                        DocCount.Clearance = 0;
                        DocCount.TransferOrder = 0;
                        DocCount.CafCount = 0;
                        DocCount.PVCount = 0;
                        DocCount.PurchaseClaimCount = 0;

                        string page = "ApprovalEntries?$filter=Approver_ID eq '" + userID + "' and Status eq '" + rn + "'&$format=json";
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
                                if ((string)config["Table_ID"] == "38" && (string)config["Document_Type"] == "Quote")
                                {
                                    DocCount.PRNCount = DocCount.PRNCount + 1;
                                }
                                if ((string)config["Table_ID"] == "38" && (string)config["Document_Type"] == "Order")
                                {
                                    DocCount.LPOCount = DocCount.LPOCount + 1;
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
                                if ((string)config["Table_ID"] == "70135454")
                                {
                                    DocCount.PurchaseClaimCount = DocCount.PurchaseClaimCount + 1;
                                }
                                if ((string)config["Table_ID"] == "70135362")
                                {
                                    DocCount.TransportCount = DocCount.TransportCount + 1;
                                }
                                if ((string)config["Table_ID"] == "70134894")
                                {
                                    DocCount.Clearance = DocCount.Clearance + 1;
                                }
                                if ((string)config["Table_ID"] == "5740")
                                {
                                    DocCount.TransferOrder = DocCount.TransferOrder + 1;
                                }
                                if ((string)config["Table_ID"] == "70135460")
                                {
                                    DocCount.PVCount = DocCount.PVCount + 1;
                                }
                                if ((string)config["Table_ID"] == "70134904")
                                {
                                    DocCount.CafCount = DocCount.CafCount + 1;
                                }
                            }
                            DocCount.Status = rn;
                        }
                        #endregion

                        #region Student requisitions
                        StdDocumentCount StdCount = new StdDocumentCount();

                        StdCount.CampusTraCount = 0;
                        StdCount.ClearanceCount = 0;
                        StdCount.DoubleConcCount = 0;
                        StdCount.DropCorsesCount = 0;
                        StdCount.MajorCount = 0;
                        StdCount.MinorCount = 0;
                        StdCount.ProgTransCount = 0;
                        StdCount.SExamCount = 0;
                        StdCount.ExamChlngConcCount = 0;
                        StdCount.ExemptionConcCount = 0;
                        StdCount.GraduationCount = 0;

                        string stdpage = "StudentReqApprovalList?$filter=Approver_ID eq '" + userID + "' and Status eq '" + rn + "'&$format=json";
                        HttpWebResponse httpResponsestd = Credentials.GetOdataData(stdpage);
                        using (var streamReader = new StreamReader(httpResponsestd.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);
                            foreach (JObject config in details["value"])
                            {
                                if ((string)config["Requisition_Type"] == "Campus Transfer")
                                {
                                    StdCount.CampusTraCount = StdCount.CampusTraCount + 1;
                                }
                                if ((string)config["Requisition_Type"] == "Clearance")
                                {
                                    StdCount.ClearanceCount = StdCount.ClearanceCount + 1;
                                }
                                if ((string)config["Requisition_Type"] == "Double Concentration")
                                {
                                    StdCount.DoubleConcCount = StdCount.DoubleConcCount + 1;
                                }
                                if ((string)config["Requisition_Type"] == "Drop Courses")
                                {
                                    StdCount.DropCorsesCount = StdCount.DropCorsesCount + 1;
                                }
                                if ((string)config["Requisition_Type"] == "Double Major")
                                {
                                    StdCount.MajorCount = StdCount.MajorCount + 1;
                                }
                                if ((string)config["Requisition_Type"] == "Programme Minor")
                                {
                                    StdCount.MinorCount = StdCount.MinorCount + 1;
                                }
                                if ((string)config["Requisition_Type"] == "Programme Transfer")
                                {
                                    StdCount.ProgTransCount = StdCount.ProgTransCount + 1;
                                }
                                if ((string)config["Requisition_Type"] == "Special Exams")
                                {
                                    StdCount.SExamCount = StdCount.SExamCount + 1;
                                }
                                if ((string)config["Requisition_Type"] == "Exam Challenge")
                                {
                                    StdCount.ExamChlngConcCount = StdCount.ExamChlngConcCount + 1;
                                }
                                if ((string)config["Requisition_Type"] == "Exemption")
                                {
                                    StdCount.ExemptionConcCount = StdCount.ExemptionConcCount + 1;
                                }
                                if ((string)config["Requisition_Type"] == "Graduation Request")
                                {
                                    StdCount.GraduationCount = StdCount.GraduationCount + 1;
                                }
                            }
                            StdCount.Status = rn;
                        }

                        #endregion

                        DocumentApprovalCount newC = new DocumentApprovalCount
                        {
                            DocCount = DocCount,
                            StdCount = StdCount
                        };
                        return View(newC);
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
                DocumentsForApprovalList newD = new DocumentsForApprovalList();
                newD.TableID = TbID;
                newD.Status = Status;
                newD.Status = Status;
                newD.DocType = DocType;
                if (TbID == "70134894" || TbID == "70134931")
                {
                    return PartialView("~/Views/DocumentApproval/Document Approval Views/StudentReqApprovalList.cshtml", newD);
                }
                else
                {
                    return PartialView("~/Views/DocumentApproval/Document Approval Views/DocumentForApprovalList.cshtml", newD);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [HttpPost]
        public ActionResult GetDataForApprovalList(string TbID, string Status, string DocType)
        {
            try
            {
                int start = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);

                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];
                int TotalRows = 0;
                List<DocumentsForApproval> approvalDocList = new List<DocumentsForApproval>();
                if (TbID != "")
                {
                    string userID = Session["UserID"].ToString();
                    string page = "";
                    TotalRows = GetTotalsApprovalEntries(TbID, Status, DocType);
                    if (DocType != "N")
                    {
                        page = "ApprovalEntries?$top=" + length + "&$skip=" + start + "&$select=Entry_No,Table_ID,Document_No,Document_Type,Sender_ID,Date_Time_Sent_for_Approval,Status,Sequence_No,Comment,Record_ID_to_Approve,SenderNames&$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" + Status + "' and Document_Type eq '" + DocType + "'&$format=json";
                    }
                    else
                    {
                        page = "ApprovalEntries?$top=" + length + "&$skip=" + start + "&$select=Entry_No,Table_ID,Document_No,Document_Type,Sender_ID,Date_Time_Sent_for_Approval,Status,Sequence_No,Comment,Record_ID_to_Approve,SenderNames&$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" + Status + "'&$format=json";
                    }
                    //page = "ApprovalEntries?$top=" + length + "&$skip=" + start + "&$select=Entry_No,Table_ID,Document_No,RecordIDText,Sender_ID,Date_Time_Sent_for_Approval,Status,Sequence_No,Comment&$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" + Status + "'&$format=json";

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
                    return Json(new { data = approvalDocList, draw = Request["draw"], recordsTotal = TotalRows, recordsFiltered = TotalRows }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = approvalDocList }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult GetStudentClearanceDataForApprovalList(string TbID, string Status, string Type, string RegType)
        {
            try
            {
                int start = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);

                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];
                int TotalRows = 0;
                List<DocumentsForApproval> approvalDocList = new List<DocumentsForApproval>();
                if (TbID != "")
                {
                    string userID = Session["UserID"].ToString();
                    TotalRows = GetTotalsApprovalEntries(TbID, Status, searchValue, Type, RegType);
                    string page = "";
                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        if (Status == "Rejected")
                        {
                            page = "StudentReqApprovalList?$top=" + length + "&$skip=" + start + "&$select=Entry_No,Table_ID,Document_No,Sender_ID,Date_Time_Sent_for_Approval,Status,Sequence_No,Sender_Code,SenderNames,Sender_Status,Commented_On,ReqType&$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "'" +
                                " and Requisition_Type eq '" + RegType + "' and Status eq '" + Status + "' and contains('" + searchValue.ToLower() + "',tolower(Sender_Code))&$format=json";
                        }
                        else
                        {
                            if (Type == "2")
                            {
                                //page = "StudentReqApprovalList?$top=" + length + "&$skip=" + start + "&$select=Entry_No,Table_ID,Document_No,Sender_ID,Date_Time_Sent_for_Approval,Status,Sequence_No,Sender_Code,SenderNames,Sender_Status,Commented_On,ReqType&$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "'" +
                                //    " and Requisition_Type eq '" + RegType + "' and Status eq '" + Status + "' and Commented_On eq true and contains('" + searchValue.ToLower() + "',tolower(Sender_Code))&$format=json";
                                page = "StudentReqApprovalList?$top=" + length + "&$skip=" + start + "&$select=Entry_No,Table_ID,Document_No,Sender_ID,Date_Time_Sent_for_Approval,Status,Sequence_No,Sender_Code,SenderNames,Sender_Status,Commented_On,ReqType&$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "'" +
                                   " and Requisition_Type eq '" + RegType + "' and Status eq '" + Status + "' and contains('" + searchValue.ToLower() + "',tolower(Sender_Code))&$format=json";
                            }
                            else
                            {
                                //page = "StudentReqApprovalList?$top=" + length + "&$skip=" + start + "&$select=Entry_No,Table_ID,Document_No,Sender_ID,Date_Time_Sent_for_Approval,Status,Sequence_No,Sender_Code,SenderNames,Sender_Status,Commented_On,ReqType&$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "'" +
                                //    " and Requisition_Type eq '" + RegType + "' and Status eq '" + Status + "' and Commented_On eq false and contains('" + searchValue.ToLower() + "',tolower(Sender_Code))&$format=json";

                                page = "StudentReqApprovalList?$top=" + length + "&$skip=" + start + "&$select=Entry_No,Table_ID,Document_No,Sender_ID,Date_Time_Sent_for_Approval,Status,Sequence_No,Sender_Code,SenderNames,Sender_Status,Commented_On,ReqType&$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "'" +
                                    " and Requisition_Type eq '" + RegType + "' and Status eq '" + Status + "' and contains('" + searchValue.ToLower() + "',tolower(Sender_Code))&$format=json";
                            }
                        }
                    }
                    else
                    {
                        if (Status == "Rejected")
                        {
                            page = "StudentReqApprovalList?$top=" + length + "&$skip=" + start + "&$select=Entry_No,Table_ID,Document_No,Sender_ID,Date_Time_Sent_for_Approval,Status,Sequence_No,Sender_Code,SenderNames,Sender_Status,Commented_On,ReqType&$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" + Status
                                     + "' and Requisition_Type eq '" + RegType + "'&$format=json";
                        }
                        else
                        {
                            if (Type == "2")
                            {
                                //page = "StudentReqApprovalList?$top=" + length + "&$skip=" + start + "&$select=Entry_No,Table_ID,Document_No,Sender_ID,Date_Time_Sent_for_Approval,Status,Sequence_No,Sender_Code,SenderNames,Sender_Status,Commented_On,ReqType&$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" + Status
                                //    + "' and Requisition_Type eq '" + RegType + "' and Commented_On eq true&$format=json";
                                page = "StudentReqApprovalList?$top=" + length + "&$skip=" + start + "&$select=Entry_No,Table_ID,Document_No,Sender_ID,Date_Time_Sent_for_Approval,Status,Sequence_No,Sender_Code,SenderNames,Sender_Status,Commented_On,ReqType&$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" + Status
                                    + "' and Requisition_Type eq '" + RegType + "'&$format=json";
                            }
                            else
                            {
                                //page = "StudentReqApprovalList?$top=" + length + "&$skip=" + start + "&$select=Entry_No,Table_ID,Document_No,Sender_ID,Date_Time_Sent_for_Approval,Status,Sequence_No,Sender_Code,SenderNames,Sender_Status,Commented_On,ReqType&$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" + Status
                                //    + "' and Requisition_Type eq '" + RegType + "' and Commented_On eq false&$format=json";

                                page = "StudentReqApprovalList?$top=" + length + "&$skip=" + start + "&$select=Entry_No,Table_ID,Document_No,Sender_ID,Date_Time_Sent_for_Approval,Status,Sequence_No,Sender_Code,SenderNames,Sender_Status,Commented_On,ReqType&$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" + Status
                                    + "' and Requisition_Type eq '" + RegType + "'&$format=json";
                            }
                        }
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
                                //if ((string)config["Requisition_Type"] == RegType)
                                //{
                                DocumentsForApproval DocList = new DocumentsForApproval();
                                DocList.TabelID = (string)config["Table_ID"];
                                DocList.Entry_No = (string)config["Entry_No"];
                                DocList.Document_No = (string)config["Document_No"];
                                DocList.Sender = (string)config["Sender_Code"];
                                DocList.Sender_Name = (string)config["SenderNames"];
                                DocList.DateSend = Convert.ToDateTime((string)config["Date_Time_Sent_for_Approval"]).ToString("dd/MM/yyyy");
                                DocList.Status = (string)config["Sender_Status"];
                                DocList.Sequence = (string)config["Sequence_No"];
                                DocList.CommentFound = (bool)config["Commented_On"];
                                DocList.ReqType = (string)config["ReqType"];
                                approvalDocList.Add(DocList);
                                //}
                            }
                        }
                    }
                    return Json(new { data = approvalDocList, draw = Request["draw"], recordsTotal = TotalRows, recordsFiltered = TotalRows }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = approvalDocList }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        protected int GetTotalsApprovalEntries(string TbID, string Status, string DocType)
        {
            int count = 0;
            try
            {
                string userID = Session["UserID"].ToString();
                string page = "";
                if (DocType != "N")
                {
                    page = "ApprovalEntries?$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" + Status + "' and Document_Type eq '" + DocType + "'&$format=json";
                }
                else
                {
                    page = "ApprovalEntries?$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" + Status + "'&$format=json";
                }
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    if (details["value"].Count() > 0)
                    {
                        count = details["value"].Count();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return count;
        }
        protected int GetTotalsApprovalEntries(string TbID, string Status, string searchValue, string Type, string RegType)
        {
            int count = 0;
            try
            {
                string userID = Session["UserID"].ToString();
                string page = "";
                if (!string.IsNullOrEmpty(searchValue))
                {
                    if (Type == "2")
                    {
                        //page = "StudentReqApprovalList?$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" + Status + "'" +
                        //    " and Requisition_Type eq '" + RegType + "' and Commented_On eq true and contains('" + searchValue.ToLower() + "',tolower(Sender_Code))&$format=json";

                        page = "StudentReqApprovalList?$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" + Status + "'" +
                            " and Requisition_Type eq '" + RegType + "' and contains('" + searchValue.ToLower() + "',tolower(Sender_Code))&$format=json";
                    }
                    else
                    {
                        //page = "StudentReqApprovalList?$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" + Status + "'" +
                        //    " and Requisition_Type eq '" + RegType + "' and Commented_On eq false and contains('" + searchValue.ToLower() + "',tolower(Sender_Code))&$format=json";
                        page = "StudentReqApprovalList?$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" + Status + "'" +
                           " and Requisition_Type eq '" + RegType + "' and contains('" + searchValue.ToLower() + "',tolower(Sender_Code))&$format=json";
                    }
                }
                else
                {
                    if (Type == "2")
                    {
                        //page = "StudentReqApprovalList?$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" +
                        //    Status + "' and Requisition_Type eq '" + RegType + "' and Commented_On eq true&$format=json";
                        page = "StudentReqApprovalList?$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" +
                            Status + "' and Requisition_Type eq '" + RegType + "'&$format=json";
                    }
                    else
                    {
                        //page = "StudentReqApprovalList?$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" +
                        //    Status + "' and Requisition_Type eq '" + RegType + "' and Commented_On eq false&$format=json";

                        page = "StudentReqApprovalList?$filter=Table_ID eq " + Convert.ToInt32(TbID) + " and Approver_ID eq '" + userID + "' and Status eq '" +
                            Status + "' and Requisition_Type eq '" + RegType + "'&$format=json";
                    }
                }
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    if (details["value"].Count() > 0)
                    {
                        count = details["value"].Count();
                        //foreach (JObject config in details["value"])
                        //{
                        //    if ((string)config["ReqType"] == RegType)
                        //    {
                        //        count = count + 1;
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return count;
        }
        public PartialViewResult LeaveReqDocApprovalDetails(string DocNo, string Sequence)
        {
            try
            {
                LeaveReqList LeaveDoc = new LeaveReqList();
                string page = "HRLeaveRequisition?$filter=No eq '" + DocNo + "'&$format=json";

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

                string page = "PurchaseRegDocument?$filter=No eq '" + DocNo + "'&$format=json";
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
                        PurchaseDoc.VendorName = (string)config["Buy_from_Vendor_Name"];
                        PurchaseDoc.Status = (string)config["Status"];
                        if ((string)config["Employee_No"] != "")
                        {
                            PurchaseDoc.RequestorNo = (string)config["Employee_No"];
                            PurchaseDoc.RequestorName = CommonClass.GetEmployeeName((string)config["Employee_No"]);
                        }
                        else
                        {
                            if ((string)config["User_ID"] != "")
                            {
                                string[] s = CommonClass.GetEmployeeByUserID((string)config["User_ID"]);
                                if (s[0] != null && s[1] != null)
                                {
                                    PurchaseDoc.RequestorNo = s[0];
                                    PurchaseDoc.RequestorName = s[1];
                                }
                            }
                        }
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
                        if ((string)config["Remarks"] != "")
                        {
                            PurchaseLine.Description2 = (string)config["Remarks"];
                        }
                        else
                        {
                            PurchaseLine.Description2 = (string)config["Description_2"];
                        }
                        PurchaseLine.Qnty = (string)config["Quantity"];
                        PurchaseLine.UnitM = (string)config["Unit_of_Measure"];
                        PurchaseLine.Amount = Convert.ToDecimal((string)config["Direct_Unit_Cost"]).ToString("#,##0.00");
                        PurchaseLine.LineAmount = Convert.ToDecimal((string)config["Line_Amount"]).ToString("#,##0.00");
                        PurchaseLine.Location = (string)config["Location_Code"];
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
        public PartialViewResult StoreReqDocApprovalDetails(string DocNo, string Sequence)
        {
            try
            {
                #region Store Header
                StoretHeader StoreDoc = new StoretHeader();

                string page = "StoreReqList?$filter=No eq '" + DocNo + "'&$format=json";
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
                string pageLine = "StoreReqLines?$filter=Requistion_No eq '" + DocNo + "'&$format=json";
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
                string pageLine = "ImprestLines?$filter=No eq '" + DocNo + "'&$format=json";
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

                string page = "ImprestSurrenderCard?$filter=No eq '" + DocNo + "'&$format=json";
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
                string pageLine = "ImpSurrenderLines?$filter=SurrenderDocNo eq '" + DocNo + "'&$format=json";
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
                string pageLine = "StaffCaimLines?$filter=No eq '" + DocNo + "'&$format=json";
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
        public PartialViewResult PurchaseClaimReqDocApprovalDetails(string DocNo, string Sequence)
        {
            try
            {
                #region Purchase Claim Header
                PurchaseClaimHeader ClaimDoc = new PurchaseClaimHeader();

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
                #region Purchase Claim Lines
                List<PurchaseClaimLines> ClaimLines = new List<PurchaseClaimLines>();
                string pageLine = "StaffCaimLines?$filter=No eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        PurchaseClaimLines claimLine = new PurchaseClaimLines();
                        claimLine.AdvanceType = (string)config["Advance_Type"];
                        claimLine.Item = (string)config["Account_No"];
                        claimLine.ItemDesc = (string)config["Account_Name"];
                        claimLine.ItemDesc2 = (string)config["Purpose"];
                        claimLine.Amount = Convert.ToDecimal((string)config["Amount"]).ToString("#,##0.00");
                        ClaimLines.Add(claimLine);
                    }
                }
                #endregion
                PurchaseClaimDocument docDetails = new PurchaseClaimDocument
                {
                    DocHeader = ClaimDoc,
                    ListOfPurchaseClaimLines = ClaimLines
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
                bool transMng = false;
                if (Session["TRMNG"] != null)
                {
                    transMng = (bool)Session["TRMNG"];
                }
                #region Header
                TransportReqList TransDoc = new TransportReqList();
                string page = "TransportReqList?$filter=TransportRequisitionNo eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        TransDoc.No = (string)config["TransportRequisitionNo"];
                        TransDoc.Commencement = (string)config["Commencement"];
                        TransDoc.Destination = (string)config["Destination"];
                        if ((bool)config["Vehicle_Hired_"])
                        {
                            TransDoc.VehicleHired = "Yes";
                            TransDoc.Vehicle = (string)config["Hired_Vehicle_Description"];
                        }
                        else
                        {
                            TransDoc.VehicleHired = "No";
                            TransDoc.Vehicle = (string)config["VehicleAllocated"];
                        }
                        if ((bool)config["Driver_Hired_"])
                        {
                            TransDoc.DriverHired = "Yes";
                            TransDoc.Driver = (string)config["Hired_Driver_Name"];
                        }
                        else
                        {
                            TransDoc.DriverHired = "No";
                            TransDoc.Driver = (string)config["DriverAllocated"];
                        }
                        TransDoc.DateRequested = Convert.ToDateTime((string)config["DateofRequest"]).ToString("dd/MM/yyyy");
                        TransDoc.DateOfTrip = Convert.ToDateTime((string)config["Date_of_Trip"]).ToString("dd/MM/yyyy");
                        TransDoc.NoOfDays = (string)config["NoofDaysRequested"];
                        TransDoc.NoOfPassngers = (string)config["No_of_Passengers"];
                        TransDoc.Cost = ((Decimal)config["Cost"]).ToString("#,##0.00");
                        TransDoc.Dep_Time = ((DateTime)config["Time_of_trip"]).ToString("h:mm tt");
                        TransDoc.Purpose_Of_Trip = (string)config["Purpose_of_Trip"];
                        TransDoc.Status = (string)config["Status"];
                        TransDoc.TRMgr = transMng;
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
                        v.Value = (string)config["Registration_No"];
                        v.Text = (string)config["Registration_No"] + "-" + (string)config["Description"];
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
        public PartialViewResult CafFoodReqDocApprovalDetails(string DocNo)
        {
            try
            {
                #region Caf Header
                Cafeteria CafDoc = new Cafeteria();

                string page = "CafeteriaReq?$filter=Document_No eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        CafDoc.No = (string)config["Document_No"];
                        CafDoc.DateRaised = ((DateTime)config["Date_Raised"]).ToString("dd/MM/yyyy");
                        CafDoc.DateNeeded = ((DateTime)config["Date_Needed"]).ToString("dd/MM/yyyy");
                        CafDoc.TimeNeeded = ((DateTime)config["Booking_Time"]).ToString("h:mm tt");
                        CafDoc.Event_Name = (string)config["Meeting_Name"];
                        CafDoc.Employee = (string)config["Employee_No"];
                        CafDoc.EmployeeName = (string)config["Employee_Name"];
                        CafDoc.Venue = (string)config["Venue"];
                        CafDoc.TotalCost = (string)config["Total_Cost"];
                        CafDoc.Campus = (string)config["Campus"];
                        CafDoc.Department = (string)config["Department"];
                        CafDoc.RespC = (string)config["Responsibility_Center"];
                        CafDoc.Caf = (string)config["Cafeteria"];
                        CafDoc.CafName = (string)config["Cafeteria_Name"];
                        CafDoc.TotalCost = (string)config["Total_Cost"];
                        CafDoc.Status = (string)config["Status"];
                    }
                }
                #endregion
                #region Caf Lines
                List<CafeteriaLines> CafLines = new List<CafeteriaLines>();
                string pageLine = "CafeteriaReqLines?$filter=Document_No eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        CafeteriaLines cafLn = new CafeteriaLines();
                        cafLn.No = (string)config["Document_No"];
                        cafLn.Item = (string)config["Item"];
                        cafLn.ItemName = (string)config["Description"];
                        cafLn.Quantity = (string)config["Quantity"];
                        cafLn.LnNo = (string)config["Line_No"];
                        cafLn.UnitCost = Convert.ToDecimal((string)config["Unit_Cost"]).ToString("#,##0.00");
                        cafLn.Amount = Convert.ToDecimal((string)config["Amount"]).ToString("#,##0.00");
                        CafLines.Add(cafLn);
                    }
                }
                #endregion
                CafDocument docDetails = new CafDocument
                {
                    DocHeader = CafDoc,
                    ListOfCafLines = CafLines
                };
                return PartialView("~/Views/DocumentApproval/Document Approval Views/CafApprovalDocDetails.cshtml", docDetails);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult StudentRequisitionDocDetails(string DocNo, string Sequence)
        {
            try
            {
                #region Requisition Doc
                StudentRequisition RDoc = new StudentRequisition();
                string page = "StudentRequisitionCard?$filter=Code eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        RDoc.Code = (string)config["Code"];
                        RDoc.StudentNo = (string)config["StudentNo"];
                        RDoc.StudentName = (string)config["Name"];
                        RDoc.RegType = (string)config["RequisitionType"];
                        RDoc.DateApplied = Convert.ToDateTime((string)config["Date"]).ToString("dd/MM/yyyy");
                        RDoc.Status = (string)config["Status"];
                        RDoc.Programme = (string)config["CurrentProgramme"] + "(" + (string)config["CurrntProgDescr"] + ")";
                        RDoc.Semester = (string)config["Semester"];
                        if ((string)config["Concentration"] != "")
                        {
                            RDoc.ProgrammeTo = (string)config["Concentration"] + "-" + (string)config["Concentration_Name"];
                        }
                        else
                        {
                            RDoc.ProgrammeTo = (string)config["Programme_To"] + "-" + (string)config["ProgToDescr"];
                        }

                        RDoc.StudentStatus = CommonClass.StudentStatus((string)config["StudentNo"]);
                        if (Sequence != "")
                        {
                            string comment = CommonClass.GetDocRejectionComment(DocNo, Convert.ToInt32(Sequence));
                            if (comment != "")
                            {
                                RDoc.CommentFound = true;
                                RDoc.Comment = comment;
                            }
                        }
                    }
                }
                #endregion

                #region Doc Lines
                List<StudentReqLines> DocLines = new List<StudentReqLines>();
                string pageLn = "StudentRequisitionLines?$filter=Application_No eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponseLn = Credentials.GetOdataData(pageLn);
                using (var streamReader = new StreamReader(httpResponseLn.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    if (details["value"].Count() > 0)
                    {
                        RDoc.ReqCount = details["value"].Count();
                    }
                    else
                    {
                        RDoc.ReqCount = 0;
                    }
                }
                #endregion

                StudentReqDoc newDoc = new StudentReqDoc
                {
                    Doc = RDoc,
                    Sequence = Sequence
                };
                return PartialView("~/Views/DocumentApproval/Document Approval Views/StudentRequisitionDoc.cshtml", newDoc);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult GraduationDocDetails(string DocNo)
        {
            try
            {
                #region Requisition Doc
                GraduationReq RDoc = new GraduationReq();
                string page = "GraduationRequest?$filter=Code eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        RDoc.Code = (string)config["Code"];
                        RDoc.Student_No = (string)config["StudentNo"];
                        RDoc.Names = (string)config["Names"];
                        RDoc.Date_Requested = Convert.ToDateTime((string)config["Date_Requested"]).ToString("dd/MM/yyyy");
                        RDoc.Address = (string)config["Address"];                       
                        RDoc.Status = (string)config["Status"];
                        RDoc.Programme = (string)config["Programme"];
                        RDoc.ProgrammeName = (string)config["Programme"] + "(" + CommonClass.GetProgrammeName((string)config["Programme"]) + ")";
                        RDoc.Telephone = (string)config["Telephone"];
                        RDoc.Email = (string)config["Email"];
                        RDoc.PersonalEmail = (string)config["PersonalEmail"];
                        RDoc.IDNumber = (string)config["IDNumber"];
                        RDoc.Currentprofession = (string)config["Currentprofession"];
                        RDoc.Current_Phone_No = (string)config["Current_Phone_No"];
                        RDoc.CurrentInstitustionCompany = (string)config["CurrentInstitustionCompany"];
                    }
                }
                #endregion
                return PartialView("~/Views/DocumentApproval/Document Approval Views/GraduationRequestDoc.cshtml", RDoc);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult StudentRequisitionDocLines(string DocNo, string RegT)
        {
            try
            {
                #region Doc Lines
                List<StudentReqLines> DocLines = new List<StudentReqLines>();
                string pageLn = "StudentRequisitionLines?$count=true&$filter=Application_No eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponseLn = Credentials.GetOdataData(pageLn);
                using (var streamReader = new StreamReader(httpResponseLn.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        StudentReqLines ln = new StudentReqLines();
                        ln.Unit = (string)config["Unit_Code"];
                        ln.UnitName = (string)config["Unit_Title"];
                        ln.Equivalent = (string)config["Equivalent_Unit"];
                        ln.Lecture = (string)config["Lecture_Name"];
                        ln.Section = (string)config["Section"];
                        ln.Approved = (bool)config["Approved"];
                        ln.Charge = (bool)config["Charge_Unit"];
                        ln.Semester = (string)config["Semester"];
                        ln.Semester_Unit_Done = (string)config["Semester_Unit_Done"];
                        ln.LnNo = (string)config["Line_No"];
                        DocLines.Add(ln);
                    }
                }
                #endregion
                StudentReqLinesDoc NewDoc = new StudentReqLinesDoc
                {
                    RegT = RegT,
                    DocLines = DocLines
                };
                return PartialView("~/Views/DocumentApproval/Document Approval Views/StudentReqUnits.cshtml", NewDoc);
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

                string page = "Transfer_Orders?$filter=No eq '" + DocNo + "'&$format=json";
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
                string pageLine = "Transfer_Line?$filter=Document_No eq '" + DocNo + "'&$format=json";
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
        [HttpPost]
        public JsonResult ApproveDocument(string DocNo, string EntryNo, string TBID, string ChargeApp)
        {
            try
            {
                string userID = Session["UserID"].ToString();

                if (TBID == "70134894")
                {
                    if (ChargeApp != null && ChargeApp != "")
                    {
                        Credentials.ObjNav.ChargeStudentRequest(DocNo, Convert.ToInt32(ChargeApp));
                    }
                }
                Credentials.ObjNav.DocumentApprovals(Convert.ToInt32(EntryNo), DocNo, userID);
                Session["SuccessMsg"] = "Request approved Successfully";
                return Json(new { message = "Request approved Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ApproveUnits(string DocNo, List<StudentReqLines> ReqUnits, string ChargeApp)
        {
            try
            {
                if (ChargeApp != null && ChargeApp != "")
                {
                    Credentials.ObjNav.ChargeStudentRequest(DocNo, Convert.ToInt32(ChargeApp));
                }

                foreach (var c in ReqUnits)
                {
                    string unit = c.Unit.Trim();
                    string Ln = c.LnNo.Trim();
                    bool App = c.Approved;
                    bool ChargeUnit = c.Charge;
                    Credentials.ObjNav.ApproveStudentRequisitionLines(DocNo, unit, Convert.ToInt32(Ln), App, ChargeUnit);
                }
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
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
                //if (TBID == "70134894")
                //{
                //    msg = "Clearance Approval Request Rejected";
                //}
                //else
                //{
                //    Credentials.ObjNav.DocumentRejections(Convert.ToInt32(EntryNo), DocNo, userID, Comments,
                //        Convert.ToInt32(TBID), Convert.ToInt32(SeqNo));
                //    msg = "Approval Request Rejected";
                //}

                Credentials.ObjNav.DocumentRejections(Convert.ToInt32(EntryNo), DocNo, userID, Comments,
                       Convert.ToInt32(TBID), Convert.ToInt32(SeqNo));
                msg = "Approval Request Rejected";
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

                string page = "ApprovalEntries?$select=Sequence_No,Approver_ID,ApproverNames&$filter=Document_No eq '" + DocNo + "' and Table_ID eq " + TableID + " and Status eq 'Approved'&$format=json";
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
        internal string[] StudentDetails(string DocNo)
        {
            string[] s = new string[3];
            try
            {
                string pageCl = "StudentRequisitionCard?$select=StudentNo&$filter=Code eq '" + DocNo + "'&$format=json";
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
        [HttpPost]
        public JsonResult AssignDriverVehicle(string DocNo, string HireVehicle, string HireDriver, string Driver, string Vehicle, string HiredVehicle, string HiredDriver, string Cost)
        {
            try
            {
                bool rtVal = false;
                string msg = "";

                if (Session["Username"] != null)
                {
                    bool HireV = false, HireD = false;
                    string VehicleHiredD = "", DriverHiredD = "";
                    if (HireVehicle != null)
                    {
                        if (HireVehicle == "1")
                        {
                            HireV = true;
                            VehicleHiredD = HiredVehicle;
                        }
                        else
                        {
                            HireV = false;
                            VehicleHiredD = "";
                        }
                    }
                    if (HireDriver != null)
                    {
                        if (HireDriver == "1")
                        {
                            HireD = true;
                            DriverHiredD = HiredDriver;
                        }
                        else
                        {
                            HireD = false;
                            HiredDriver = "";
                        }
                    }
                    if (Cost == null || Cost == "")
                    {
                        Cost = "0";
                    }
                    string userID = Session["Username"].ToString();
                    Credentials.ObjNav.AssignTransportRequisitionDriver(DocNo, Driver, Vehicle, userID, HireV, HireD,
                        VehicleHiredD, HiredDriver, Convert.ToDecimal(Cost));
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
        public ActionResult NewLecAssignForm(string DocNo, string Unit, string Sem, string Ln)
        {
            try
            {
                if (Session["Username"] == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    LecAssignUnit LecList = new LecAssignUnit();
                    #region Lec List
                    List<DimensionValues> ListLec = new List<DimensionValues>();
                    string pageCampus = "Timetable?$filter=Unit eq '" + Unit + "' and Semester eq '" + Sem + "'&$format=json";

                    HttpWebResponse httpResponseCampus = Credentials.GetOdataData(pageCampus);
                    using (var streamReader = new StreamReader(httpResponseCampus.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);


                        foreach (JObject config in details["value"])
                        {
                            DimensionValues l = new DimensionValues();
                            l.Code = (string)config["Lecturer"];
                            l.Name = (string)config["Lecturer_Name"];
                            ListLec.Add(l);
                        }
                    }
                    #endregion

                    LecList = new LecAssignUnit
                    {
                        Code = "",
                        ListOfLec = ListLec.Select(x =>
                                             new SelectListItem()
                                             {
                                                 Text = x.Name,
                                                 Value = x.Code
                                             }).ToList(),
                        DocNo = DocNo,
                        Unit = Unit,
                        Semester = Sem,
                        Ln = Ln
                    };
                    return PartialView("~/Views/DocumentApproval/Document Approval Views/AssignLecForm.cshtml", LecList);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetLecturerSections(string Lec, string Unit, string Sem)
        {
            try
            {
                #region Section
                List<DropdownList> DropDList = new List<DropdownList>();
                string page = "Timetable?$filter=Lecturer eq '" + Lec + "' and Unit eq '" + Unit + "' and Semester eq '" + Sem + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList d = new DropdownList();
                        d.Value = (string)config["Unit_Class"];
                        d.Text = (string)config["Unit_Class"];
                        DropDList.Add(d);
                    }
                }
                #endregion
                DropdownListData newList = new DropdownListData
                {
                    ListOfddlData = DropDList.Select(x =>
                                    new SelectListItem()
                                    {
                                        Text = x.Text,
                                        Value = x.Value
                                    }).ToList()
                };
                return Json(newList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SaveSpecialExamLec(string DocNo, string Unit, string Lec, string Sec, string Ln, string Sem)
        {
            try
            {
                Credentials.ObjNav.AssignLecStudentRequisitionLines(DocNo, Unit, Convert.ToInt32(Ln), true, Lec, Sec);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GenerateStudentAudit(string StudentNo, string Prog)
        {
            try
            {
                string message = "";
                bool success = false;
                if (Session["Username"] == null)
                {
                    Response.Redirect(Url.Action("Login", "Login"));
                }
                else
                {
                    string filename = StudentNo.Replace("/", "");
                    Credentials.ObjNav.GenerateStudentAudit(StudentNo, Prog, "STDAUDIT-" + filename + ".pdf");
                    filename = "STDAUDIT-" + filename + ".pdf";
                    string fileDestinationPath = Server.MapPath("~/Uploads/");
                    CommonClass.MoveFile(filename, fileDestinationPath);
                    string DestinationPath = fileDestinationPath + filename;
                    System.IO.FileInfo file = new System.IO.FileInfo(DestinationPath);
                    if (file.Exists)
                    {
                        success = true;
                        message = filename;
                    }
                    else
                    {
                        success = false;
                        message = "File Not Found";
                    }
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