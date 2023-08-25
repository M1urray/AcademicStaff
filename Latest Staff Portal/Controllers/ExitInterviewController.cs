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
    public class ExitInterviewController : Controller
    {
        // GET: ExitInterview
        public ActionResult ExitInterviewRequisitionList()
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
        public PartialViewResult ExitInterviewListPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<NewExitHeader> exitHeader = new List<NewExitHeader>();
                string page = "HREmployeeExitRequisition?$filter=EmployeeNo eq '" + StaffNo + "'&format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        NewExitHeader newExitHeader = new NewExitHeader();
                        newExitHeader.No = config["ExitClearanceNo"].ToString();
                        newExitHeader.EmployeeNumber = config["EmployeeNo"].ToString();
                        newExitHeader.ClearanceDate = config["DateOfClearance"].ToString();
                        newExitHeader.LengthInDepartment = config["Length_of_Service"].ToString();
                        newExitHeader.LeavingDate = config["DateOfLeaving"].ToString();

                        exitHeader.Add(newExitHeader);
                    }
                }
                return PartialView("~/Views/ExitInterview/ExitInterviewListPartialView.cshtml", exitHeader.OrderByDescending(x => x.No));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewExitInterviewRequest()
        {
            string StaffNo = Session["Username"].ToString();
                    NewExitHeader newExitHeader = new NewExitHeader();
                    string LengthOfService = "", Designation = "", EmpCat = "";
                    #region Employee Data
                    string pageData = "EmployeeList?$Length_Of_Service,Position,Category&$filter=No eq '" + StaffNo + "'&$format=json";

                    HttpWebResponse httpResponse = Credentials.GetOdataData(pageData);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);

                        if (details["value"].Count() > 0)
                        {
                            foreach (JObject config in details["value"])
                            {
                                LengthOfService = (string)config["Length_Of_Service"];
                                Designation = (string)config["Position"];
                                EmpCat = (string)config["Category"];
                            }
                        }
                    }
                    #endregion
                    newExitHeader = new NewExitHeader
                    {
                        EmployeeNumber = StaffNo,
                        LengthOfService = LengthOfService,
                        Designation = Designation,
                        EmployeeCategory = EmpCat
                };
            return PartialView("~/Views/ExitInterview/NewExitInterviewRequest.cshtml", newExitHeader);
        }
        [AcceptVerbs(HttpVerbs.Post)]

        public JsonResult SubmitExitInterview(NewExitHeader newExitInterview)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();

                DateTime LastDayOfService = DateTime.ParseExact(newExitInterview.LeavingDate.Replace("-", "/"),"dd/MM/yyyy", CultureInfo.InvariantCulture);
                string DocNo = Credentials.ObjNav.ExitInterviewHeader(newExitInterview.EmployeeNumber,0,LastDayOfService, "","",0, "","");
                if (DocNo != "")
                {
                    string Redirect = "/ExitInterview/ExitInterviewDocumentView?DocNo=" + DocNo;

                    Session["SuccessMsg"] = "Exit Interview Requisition, Document No: " + DocNo + ", created Successfully. Add line(s) and then send for approval";
                    return Json(new { message = Redirect, success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "Document not created. Please try again later...", success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ExitInterviewDocumentView(string DocNo)
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
                    #region Exit Header
                    NewExitHeader newExitHeader = new NewExitHeader();

                    string page = "HREmployeeExitRequisition?$filter=ExitClearanceNo eq '" + DocNo + "'&format=json";
                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        foreach (JObject config in details["value"])
                        {
                            newExitHeader.No = config["ExitClearanceNo"].ToString();
                            newExitHeader.EmployeeNumber = config["EmployeeNo"].ToString();
                            newExitHeader.EmployeeName = config["EmployeeName"].ToString();
                            newExitHeader.ClearanceDate = config["DateOfClearance"].ToString();
                            newExitHeader.LengthInDepartment = config["Length_of_Service"].ToString();
                            newExitHeader.LeavingDate = config["DateOfLeaving"].ToString();
                        }
                    }
                    #endregion
                    return View(newExitHeader);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }

        public PartialViewResult ReasonDocumentLines(string DocNo)
        {
            try
            {
                #region Reasons
                List<ReasonForLeaving> reasonForLeaving = new List<ReasonForLeaving>();
                string page = "HRReasonForLeaving?$filter=Code eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        ReasonForLeaving reasonForLeaving1 = new ReasonForLeaving();
                        reasonForLeaving1.DocNo=(string)config["Code"];
                        reasonForLeaving1.Reason = (string)config["Reason"];
                        reasonForLeaving1.LnNo = (string)config["Line_No"];
                        reasonForLeaving.Add(reasonForLeaving1);
                    }
                }
                #endregion
                NewExitHeader reasons = new NewExitHeader
                {
                    Reasons = reasonForLeaving
                };
                return PartialView("~/Views/ExitInterview/ExitInterviewDocumentLineView.cshtml",reasons);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult ExitQuestions(string DocNo)
        {
            try
            {
                #region Questions
                List<ExitQuestions> exitQue = new List<ExitQuestions>();
                string page = "HRExitInterviewQuiz?$filter=Code eq '" + DocNo + "'&$format=json";
                HttpWebResponse httpResponseLine = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        ExitQuestions exit = new ExitQuestions();
                        exit.DocNo= (string)config["Code"];
                        exit.Questionare= (string)config["Questionare"];
                        exit.Answer = (string)config["Answer"];
                        exitQue.Add(exit);
                    }
                }
                #endregion
                NewExitHeader questions = new NewExitHeader
                {
                    ExitQuestions = exitQue
                };
                return PartialView("~/Views/ExitInterview/ExitInterviewQuestionLineView.cshtml", questions);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewReasonLine()
        {
            string StaffNo = Session["Username"].ToString();
            #region Exit Header
            NewExitHeader newExitHeader = new NewExitHeader();

            string page = "HREmployeeExitRequisition?$filter=EmployeeNo eq '" + StaffNo + "'&format=json";
            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    newExitHeader.No = config["ExitClearanceNo"].ToString();
                }
            }
            #endregion
            return PartialView("~/Views/ExitInterview/ExitReasonsForm.cshtml",newExitHeader);
        }
        public PartialViewResult NewQuestionLine()
        {
            string StaffNo = Session["Username"].ToString();
            #region Exit Header
            NewExitHeader newExitHeader = new NewExitHeader();

            string page = "HREmployeeExitRequisition?$filter=EmployeeNo eq '" + StaffNo + "'&format=json";
            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    newExitHeader.No = config["ExitClearanceNo"].ToString();
                }
            }
            #endregion
            return PartialView("~/Views/ExitInterview/ExitInterviewForm.cshtml",newExitHeader);
        }
        public JsonResult SubmitExitReason(string DocNo,string Reason)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                Credentials.ObjNav.ExitInterviewReasonsForLeaving(DocNo, Reason);
                return Json(new { message = "Reason Added successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SumbitInterviewQuestionnare(string DocNo, List<QuestionAnswer> QuestionAnswers)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
        
                foreach (var questionAnswer in QuestionAnswers)
                {
                    Credentials.ObjNav.ExitInterviewQuestionair(DocNo, 3, questionAnswer.Question, questionAnswer.Answer, StaffNo, "");
                }
        
                return Json(new { message = "Questionnaire Submitted successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public PartialViewResult FileUploadForm()
        {
            return PartialView("~/Views/ExitInterview/FileAttachmentForm.cshtml");
        }
    }
}