using Latest_Staff_Portal.CustomSecurity;
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
    [CustomeAuthentication]
    [CustomAuthorization(Role = "ALLUSERS")]
    public class AppraisalController : Controller
    {
        // GET: Appraisal
        public ActionResult AppraisalPreamble()
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
        public ActionResult MyAppraisalList()
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
        public PartialViewResult AppraisalRequisitionListPartialView()
        {
            string StaffNo = Session["Username"].ToString();
            List<AppraisalCardList> ListOfAppraisal = new List<AppraisalCardList>();

            string page = "AppraisalCard?$filter=Employee_No eq '" + StaffNo + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    AppraisalCardList ApprList = new AppraisalCardList();
                    ApprList.ApprisalCode = (string)config["Appraisal_No"];
                    ApprList.AppraisalType = "";// (string)config["Appraisal_Type"];
                    ApprList.StaffName = (string)config["Employee_Name"];
                    ApprList.ApprisalPeriod = (string)config["Appraisal_Period"];
                    ApprList.RespCenter = "";// (string)config["Resp_Center"];
                    ApprList.Status = (string)config["Status"];
                    ListOfAppraisal.Add(ApprList);
                }
            }
            return PartialView("~/Views/Appraisal/Partial Views/AppraisalListView.cshtml", ListOfAppraisal);
        }
        public PartialViewResult NewApprisalRequisition()
        {
            string StaffNo = Session["Username"].ToString();
            NewApprisalRequest NewAppl = new NewApprisalRequest();

            #region ApprisalPeriod
            List<AppraisalPeriods> AppPeriod = new List<AppraisalPeriods>();

            string pageReliever = "ApprisalPeriods?&format=json";

            HttpWebResponse httpResponseReliever = Credentials.GetOdataData(pageReliever);
            using (var streamReader = new StreamReader(httpResponseReliever.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    AppraisalPeriods ApPeriod = new AppraisalPeriods();
                    ApPeriod.Period = (string)config["Code"];
                    AppPeriod.Add(ApPeriod);
                }
            }
            #endregion

            #region Sup List
            List<RelieverList> EmpList = new List<RelieverList>();
            string Department = CommonClass.EmployeeDepartment(StaffNo);
            string pageSup = "EmployeeList?$filter=No ne '" + StaffNo + "' and Status eq 'Active'&$format=json";

            HttpWebResponse httpResponseSup = Credentials.GetOdataData(pageSup);
            using (var streamReader = new StreamReader(httpResponseSup.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    if ((string)config["FirstName"] != "")
                    {
                        RelieverList Rlist = new RelieverList();
                        Rlist.No = (string)config["No"];
                        Rlist.Name = (string)config["FirstName"] + " " + (string)config["MiddleName"] + " " + (string)config["LastName"];
                        EmpList.Add(Rlist);
                    }
                }

            }
            #endregion

            NewAppl = new NewApprisalRequest
            {
                ListOfApprisalPeriods = AppPeriod.Select(x =>
                                    new SelectListItem()
                                    {
                                        Text = x.Period,
                                        Value = x.Period
                                    }).ToList(),
                ListOfEmployees = EmpList.Select(x =>
                                   new SelectListItem()
                                   {
                                       Text = x.Name,
                                       Value = x.No
                                   }).ToList()
            };
            return PartialView("~/Views/Appraisal/Partial Views/NewApprisalRequest.cshtml", NewAppl);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitAppraisalDocument(NewAppraisalDocument NewApp)
        {
            try
            {
                string StaffNo = Session["username"].ToString();
                string DocNo = Credentials.ObjNav.createAppraisalDocument(StaffNo, NewApp.ApprisalPeriod, NewApp.SupNo,0);
                string Redirect = "/Appraisal/AppraisalScoreCard?AppDoc=" + DocNo;

                return Json(new { message = Redirect, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AppraisalScoreCard(string AppDoc)
        {
            string StaffNo = Session["Username"].ToString();
            AppraisalCardList AppDocDetails = new AppraisalCardList();

            string page = "AppraisalCard?$filter=Appraisal_No eq '" + AppDoc + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    AppDocDetails.ApprisalCode = (string)config["Appraisal_No"];
                    AppDocDetails.AppraisalDate = ((DateTime)config["Appraisal_Date"]).ToString("dd/MM/yyyy");
                    AppDocDetails.StaffNo = StaffNo;
                    AppDocDetails.StaffName = (string)config["Employee_Name"];
                    AppDocDetails.AppraisalStartDate = ((DateTime)config["Evaluation_Period_Start_Date"]).ToString("dd/MM/yyyy");
                    AppDocDetails.AppraisalEndDate = ((DateTime)config["Evaluation_Period_End_Date"]).ToString("dd/MM/yyyy");
                    AppDocDetails.AppointmentDate = ((DateTime)config["Date_of_First_Appointment"]).ToString("dd/MM/yyyy");
                    AppDocDetails.Designation = (string)config["Designation"];
                    AppDocDetails.Department = (string)config["Department_Name"];
                    AppDocDetails.ApprisalPeriod = (string)config["Appraisal_Period"];
                    AppDocDetails.AppraisalStage = (string)config["Appraisal_Stage"];
                }
            }
            return View(AppDocDetails);
        }
        public PartialViewResult LoadEvaluationLines(string AppDoc, string Qs, string Level)
        {
            string StaffNo = Session["Username"].ToString();
            List<EvaluationLines> ListOfEvaluationLines = new List<EvaluationLines>();

            string page = "AppraisalEvaluationLines?$filter=Appraisal_Code eq '" + AppDoc + "' and Time_Frame eq '" + Qs + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    EvaluationLines Ln = new EvaluationLines();
                    Ln.Appraisal_No = (string)config["Appraisal_Code"];
                    Ln.Entry_No = (string)config["Entry_No"];
                    Ln.Objective = (string)config["Objective"];
                    Ln.Target = (string)config["Target"];
                    Ln.Activity = (string)config["Activity"];
                    Ln.Resources_Required = (string)config["Resources_Required"];
                    Ln.Expected_Results = (string)config["Expected_Results"];
                    Ln.Performance_Indicator = (string)config["Performance_Indicator"];
                    Ln.Time_Frame = (string)config["Time_Frame"];
                    Ln.Appraisee_Score = (string)config["Appraisee_Score"];
                    Ln.Supervisor_Score = (string)config["Supervisor_Score"];
                    Ln.Agreed_Score = (string)config["Supervisor_Score"];
                    ListOfEvaluationLines.Add(Ln);
                }
            }
            return PartialView("~/Views/Appraisal/Partial Views/ApprisalEvaluationLines.cshtml", ListOfEvaluationLines);
        }
        public PartialViewResult LoadPerformanceIndicator(string AppDoc, string Type, string Level)
        {
            string StaffNo = Session["Username"].ToString();
            List<PerformanceIndicator> ListOfPerfIndicator = new List<PerformanceIndicator>();

            string page = "PerformanceIndicator?$filter=Appraisal_No eq '" + AppDoc + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    PerformanceIndicator PerfInd = new PerformanceIndicator();
                    PerfInd.Appraisal_No = (string)config["Appraisal_No"];
                    PerfInd.Line_No = (string)config["Line_No"];
                    PerfInd.Agreed_Performance_Targets = (string)config["Agreed_Performance_Targets"];
                    PerfInd.Key_Performance_Indicator = (string)config["Key_Performance_Indicator"];
                    PerfInd.Key_Result_Areas_Output = (string)config["Key_Result_Areas_Output"];
                    PerfInd.Self_Assesment = (string)config["Self_Assesment"];
                    PerfInd.Self_Score = (string)config["Self_Score"];
                    PerfInd.Supervisor_Assesment = (string)config["Supervisor_Assesment"];
                    PerfInd.Supervisors_Score = (string)config["Supervisors_Score"];
                    PerfInd.Agreed_Assesment_Results = (string)config["Agreed_Assesment_Results"];
                    PerfInd.Agreed_Score = (string)config["Agreed_Score"];
                    PerfInd.Appraisee_Comments = (string)config["Appraisee_Comments"];
                    PerfInd.Supervisor_Comments = (string)config["Supervisor_Comments"];
                    ListOfPerfIndicator.Add(PerfInd);
                }
            }
            SectionDetails newsection = new SectionDetails
            {
                KPIList = ListOfPerfIndicator,
                Level = Level
            };
            if (Type == "S")
            {
                return PartialView("~/Views/Appraisal/Partial Views/PerformanceIndicator.cshtml", newsection);
            }
            else
            {
                return PartialView("~/Views/Appraisal/Partial Views/PerformanceIndicatorReview.cshtml", newsection);
            }
        }
        public PartialViewResult LoadTrainingDev(string AppDoc)
        {
            string StaffNo = Session["Username"].ToString();
            List<TrainingDev> ListOfTrainingDev = new List<TrainingDev>();

            string page = "AppraisalTrainingDev?$filter=Appraisal_No eq '" + AppDoc + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    TrainingDev TrainDev = new TrainingDev();
                    TrainDev.Appraisal_No = (string)config["Appraisal_No"];
                    TrainDev.Line_No = (string)config["Line_No"];
                    TrainDev.Name_of_the_Course = (string)config["Name_of_the_Course"];
                    TrainDev.Duration_of_Course = (string)config["Duration_of_Course"];
                    TrainDev.Expected_Start_Date = (string)config["Expected_Start_Date"];
                    TrainDev.Expected_End_Date = (string)config["Expected_End_Date"];
                    TrainDev.Reaction = (string)config["Reaction"];
                    TrainDev.Learning_Obtained = (string)config["Learning_Obtained"];
                    TrainDev.Behavior_Changes_Adopted = (string)config["Behavior_Changes_Adopted"];
                    TrainDev.Results_Obtained = (string)config["Results_Obtained"];
                    TrainDev.Remarks_Appraisee = (string)config["Remarks_Appraisee"];
                    TrainDev.Remarks_Supervisor = (string)config["Remarks_Supervisor"];
                    ListOfTrainingDev.Add(TrainDev);
                }
            }
            return PartialView("~/Views/Appraisal/Partial Views/TrainingDevelopment.cshtml", ListOfTrainingDev);
        }
        public PartialViewResult LoadCoreCompetenceValues(string AppDoc)
        {
            string StaffNo = Session["Username"].ToString();
            List<CompetenceValues> ListOfCompetenceValues = new List<CompetenceValues>();

            string page = "CompetenceValues?$filter=Appraisal_No eq '" + AppDoc + "' and Category eq 'Staff Values %26 Core Competence'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    CompetenceValues CompV = new CompetenceValues();
                    CompV.Appraisal_No = (string)config["Appraisal_No"];
                    CompV.Code = (string)config["Code"];
                    CompV.Description = (string)config["Description"];
                    CompV.Category = (string)config["Category"];
                    CompV.Appraisal_Assesment = (string)config["Appraisal_Assesment"];
                    CompV.Score = (string)config["Score"];
                    CompV.Score_Descriptors = (string)config["Score_Descriptors"];
                    CompV.Line_No = (string)config["Line_No"];
                    ListOfCompetenceValues.Add(CompV);
                }
            }
            return PartialView("~/Views/Appraisal/Partial Views/CompetenceValues.cshtml", ListOfCompetenceValues);
        }
        public PartialViewResult LoadMGRCoreCompetenceValues(string AppDoc)
        {
            string StaffNo = Session["Username"].ToString();
            List<CompetenceValues> ListOfCompetenceValues = new List<CompetenceValues>();

            string page = "CompetenceValues?$filter=Appraisal_No eq '" + AppDoc + "' and Category eq 'Managerial and Supervisory Competence'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    CompetenceValues CompV = new CompetenceValues();
                    CompV.Appraisal_No = (string)config["Appraisal_No"];
                    CompV.Code = (string)config["Code"];
                    CompV.Description = (string)config["Description"];
                    CompV.Category = (string)config["Category"];
                    CompV.Appraisal_Assesment = (string)config["Appraisal_Assesment"];
                    CompV.Score = (string)config["Score"];
                    CompV.Score_Descriptors = (string)config["Score_Descriptors"];
                    CompV.Line_No = (string)config["Line_No"];
                    ListOfCompetenceValues.Add(CompV);
                }
            }
            return PartialView("~/Views/Appraisal/Partial Views/MGRCompetenceValues.cshtml", ListOfCompetenceValues);
        }
        public PartialViewResult KPIForm()
        {
            return PartialView("~/Views/Appraisal/Partial Views/KPIForm.cshtml");
        }
        public PartialViewResult KPIEvaluationForm(string DocNo, string LnNo, string Level)
        {
            PerformanceIndicator PerfInd = new PerformanceIndicator();

            string page = "PerformanceIndicator?$filter=Appraisal_No eq '" + DocNo + "' and Line_No eq " + Convert.ToInt32(LnNo) + "&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    PerfInd.Appraisal_No = (string)config["Appraisal_No"];
                    PerfInd.Line_No = (string)config["Line_No"];
                    PerfInd.Agreed_Performance_Targets = (string)config["Agreed_Performance_Targets"];
                    PerfInd.Key_Performance_Indicator = (string)config["Key_Performance_Indicator"];
                    PerfInd.Key_Result_Areas_Output = (string)config["Key_Result_Areas_Output"];
                    PerfInd.Self_Assesment = (string)config["Self_Assesment"];
                    PerfInd.Self_Score = (string)config["Self_Score"];
                    PerfInd.Supervisor_Assesment = (string)config["Supervisor_Assesment"];
                    PerfInd.Supervisors_Score = (string)config["Supervisors_Score"];
                    PerfInd.Agreed_Assesment_Results = (string)config["Agreed_Assesment_Results"];
                    PerfInd.Agreed_Score = (string)config["Agreed_Score"];
                    PerfInd.Appraisee_Comments = (string)config["Appraisee_Comments"];
                    PerfInd.Supervisor_Comments = (string)config["Supervisor_Comments"];
                    PerfInd.Level = Level;
                    LoadAssessment();
                }
            }
            return PartialView("~/Views/Appraisal/Partial Views/KPIReviewForm.cshtml", PerfInd);
        }
        public PartialViewResult AppraisalEvaluationForm(string DocNo, string LnNo)
        {
            EvaluationLines Ln = new EvaluationLines();

            string page = "AppraisalEvaluationLines?$filter=Appraisal_Code eq '" + DocNo + "' and Entry_No eq " + Convert.ToInt32(LnNo) + "&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {                    
                    Ln.Appraisal_No = (string)config["Appraisal_Code"];
                    Ln.Entry_No = (string)config["Entry_No"];
                    Ln.Objective = (string)config["Objective"];
                    Ln.Target = (string)config["Target"];
                    Ln.Activity = (string)config["Activity"];
                    Ln.Resources_Required = (string)config["Resources_Required"];
                    Ln.Expected_Results = (string)config["Expected_Results"];
                    Ln.Performance_Indicator = (string)config["Performance_Indicator"];
                    if ((string)config["Time_Frame"] == "Q1")
                    {
                        Ln.Time_Frame = "1";
                    }
                    else if ((string)config["Time_Frame"] == "Q2")
                    {
                        Ln.Time_Frame = "2";
                    }
                    else if ((string)config["Time_Frame"] == "Q3")
                    {
                        Ln.Time_Frame = "3";
                    }
                    else if ((string)config["Time_Frame"] == "Q4")
                    {
                        Ln.Time_Frame = "4";
                    }
                    else if ((string)config["Time_Frame"] == "Quarterly")
                    {
                        Ln.Time_Frame = "5";
                    }
                    else if ((string)config["Time_Frame"] == "By 25th of Every Month")
                    {
                        Ln.Time_Frame = "6";
                    }
                    else
                    {
                        Ln.Time_Frame = "5";
                    }
                    Ln.Appraisee_Score = (string)config["Appraisee_Score"];
                    Ln.Supervisor_Score = (string)config["Supervisor_Score"];
                    Ln.Agreed_Score = (string)config["Supervisor_Score"];
                }
            }
            return PartialView("~/Views/Appraisal/Partial Views/KPIReviewForm.cshtml", Ln);
        }
        protected void LoadAssessment()
        {
            try
            {
                List<DropdownList> dropdownList = new List<DropdownList>();
                for (int i = 1; i < 6; i++)
                {
                    DropdownList ddl = new DropdownList();
                    ddl.Value = i.ToString();
                    ddl.Text = i.ToString();
                    dropdownList.Add(ddl);
                }
                ViewBag.Assessment = dropdownList;
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }
        public PartialViewResult TrainingDevForm()
        {
            return PartialView("~/Views/Appraisal/Partial Views/TrainingDevForm.cshtml");
        }
        [HttpPost]
        public JsonResult SubmitKPIs(PerformanceIndicator kpi)
        {
            try
            {
                string Comments = "";
                if (kpi.Appraisee_Comments != null)
                {
                    Comments = kpi.Appraisee_Comments;
                }
                Credentials.ObjNav.SaveKPIs(kpi.Appraisal_No, kpi.Agreed_Performance_Targets, kpi.Key_Performance_Indicator, Comments,0,"",0);

                return Json(new { message = "Record Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SubmitKPIsEvaluation(PerformanceIndicator kpi)
        {
            try
            {
                string Comments = "";
                if (kpi.Appraisee_Comments != null)
                {
                    Comments = kpi.Appraisee_Comments;
                }
                Credentials.ObjNav.SaveKPIsReviews(kpi.Appraisal_No, Convert.ToInt32(kpi.Line_No), kpi.Key_Result_Areas_Output, Convert.ToInt32(kpi.Self_Assesment), Comments, 0);

                return Json(new { message = "KPI Review Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult DropKPIs(string DocNo, string LnNo)
        {
            try
            {
                Credentials.ObjNav.DropKPIs(DocNo, Convert.ToInt32(LnNo));

                return Json(new { message = "Record removed Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SaveTrainingDev(TrainingDev trDev)
        {
            try
            {
                DateTime startDate = DateTime.ParseExact(trDev.Expected_Start_Date.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                Credentials.ObjNav.SaveTrainingDevPlan(trDev.Appraisal_No, trDev.Name_of_the_Course, trDev.Duration_of_Course, startDate, trDev.Reaction, trDev.Learning_Obtained, trDev.Behavior_Changes_Adopted, trDev.Results_Obtained, trDev.Remarks_Appraisee);

                return Json(new { message = "Record Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult DropTrainingDev(string DocNo, string LnNo)
        {
            try
            {
                Credentials.ObjNav.DropTrainingDevPlan(DocNo, Convert.ToInt32(LnNo));

                return Json(new { message = "Record removed Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult CompValueEvaluationForm(string DocNo, string LnNo, string Level)
        {
            CompetenceValues CompV = new CompetenceValues();

            string page = "CompetenceValues?$filter=Appraisal_No eq '" + DocNo + "' and Line_No eq " + Convert.ToInt32(LnNo) + "&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {

                    CompV.Appraisal_No = (string)config["Appraisal_No"];
                    CompV.Description = (string)config["Description"];
                    CompV.Appraisal_Assesment = (string)config["Appraisal_Assesment"];
                    CompV.Score = (string)config["Score"];
                    CompV.Line_No = (string)config["Line_No"];
                    CompV.Level = Level;
                    LoadAssessment();
                }
            }
            return PartialView("~/Views/Appraisal/Partial Views/ValueCompetenceForm.cshtml", CompV);
        }
        [HttpPost]
        public JsonResult SubmitCompValueEvaluation(CompetenceValues CValues)
        {
            try
            {
                //Credentials.ObjNav.SaveCompetenceValuesReviews(CValues.Appraisal_No, Convert.ToInt32(CValues.Line_No), CValues.Appraisal_Assesment, Convert.ToInt32(CValues.Score), 0);

                return Json(new { message = "Competence and Value Review Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SendDocumentTo(string DocNo, string SendTo)
        {
            try
            {
                Credentials.ObjNav.AppraisalRequisitionOpenTo(DocNo, Convert.ToInt32(SendTo));
                string ToLevel = "";
                if (SendTo == "0")
                {
                    ToLevel = "Appraisal Requisition send to Appraisee";
                }
                if (SendTo == "1")
                {
                    ToLevel = "Appraisal Requisition send to Supervisor";
                }
                if (SendTo == "2")
                {
                    ToLevel = "Appraisal Requisition Closed";
                }
                return Json(new { message = ToLevel, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult MyReviewAppraisalList()
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
        public PartialViewResult AppraisalReviewListPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<AppraisalCardList> ListOfAppraisal = new List<AppraisalCardList>();

                string page = "AppraisalCard?$filter=Supervisor_No eq '" + StaffNo + "' and Status eq 'Supervisor'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        AppraisalCardList ApprList = new AppraisalCardList();
                        ApprList.ApprisalCode = (string)config["Appraisal_No"];
                        //ApprList.AppraisalType = (string)config["Appraisal_Type"];
                        ApprList.StaffName = (string)config["Employee_Name"];
                        ApprList.ApprisalPeriod = (string)config["Appraisal_Period"];
                        ApprList.Status = (string)config["Status"];
                        ApprList.OpenTo = (string)config["Status"];
                        ListOfAppraisal.Add(ApprList);
                    }
                }
                return PartialView("~/Views/Appraisal/AppraisalReview/AppraisalReviewList.cshtml", ListOfAppraisal);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public ActionResult AppraisalReviewScoreCard(string AppDoc)
        {
            string StaffNo = Session["Username"].ToString();
            AppraisalCardList AppDocDetails = new AppraisalCardList();

            string page = "AppraisalCard?$filter=Appraisal_No eq '" + AppDoc + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    AppDocDetails.ApprisalCode = (string)config["Appraisal_No"];
                    AppDocDetails.AppraisalDate = ((DateTime)config["Appraisal_Date"]).ToString("dd/MM/yyyy");
                    AppDocDetails.StaffNo = StaffNo;
                    AppDocDetails.StaffName = (string)config["Employee_Name"];
                    AppDocDetails.AppraisalStartDate = ((DateTime)config["Evaluation_Period_Start_Date"]).ToString("dd/MM/yyyy");
                    AppDocDetails.AppraisalEndDate = ((DateTime)config["Evaluation_Period_End_Date"]).ToString("dd/MM/yyyy");
                    AppDocDetails.AppointmentDate = ((DateTime)config["Date_of_First_Appointment"]).ToString("dd/MM/yyyy");
                    AppDocDetails.Designation = (string)config["Designation"];
                    AppDocDetails.Department = (string)config["Department_Name"];
                    AppDocDetails.ApprisalPeriod = (string)config["Appraisal_Period"];
                    AppDocDetails.AppraisalStage = (string)config["Appraisal_Stage"];
                }
            }
            return View("~/Views/Appraisal/AppraisalReview/ApprisalReviewDocument.cshtml", AppDocDetails);
        }
    }
}