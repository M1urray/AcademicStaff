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
    public class IndividualWorkPlanController : Controller
    {
        // GET: IndividualWorkPlan

        #region set Work Plan
        public ActionResult IndividualWorkPlanList()
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
        public PartialViewResult IndividualWorkPlanListView()
        {
            string StaffNo = Session["Username"].ToString();
            List<IndividualWorkPlan> ListOfAppraisal = new List<IndividualWorkPlan>();

            string page = "IndividualWorklan?$filter=Staff_No eq '" + StaffNo + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    IndividualWorkPlan P = new IndividualWorkPlan();
                    P.DocNo = (string)config["Code"];
                    P.StaffNo = (string)config["Staff_No"];
                    P.StaffName = (string)config["Employee_Name"];
                    P.ApprisalPeriod = (string)config["Appraisal_Period"];
                    P.Dim1 = (string)config["Dim1"];
                    P.Dim2 = (string)config["Dim2"];
                    P.Status = (string)config["Open_To"];
                    ListOfAppraisal.Add(P);
                }
            }
            return PartialView("~/Views/IndividualWorkPlan/Partial View/IndividualWKListView.cshtml", ListOfAppraisal);
        }
        public PartialViewResult NewRequisition()
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
        public JsonResult SubmitIndividualWkPDocument(string AppPeriod, string Supervisor)
        {
            try
            {
                string StaffNo = Session["username"].ToString();
                string DocNo = Credentials.ObjNav.InsertIndividualWorkplan(StaffNo, Supervisor, AppPeriod);
                string Redirect = "/IndividualWorkPlan/IndividualWorkPlanCard?AppDoc=" + DocNo;

                return Json(new { message = Redirect, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult IndividualWorkPlanCard(string AppDoc)
        {
            string StaffNo = Session["Username"].ToString();
            IndividualWorkPlan AppDocDetails = new IndividualWorkPlan();

            string page = "IndividualWorkPlanCard?$filter=Code eq '" + AppDoc + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    AppDocDetails.DocNo = (string)config["Code"];
                    AppDocDetails.StaffNo = (string)config["Staff_No"];
                    AppDocDetails.StaffName = (string)config["Employee_Name"];
                    AppDocDetails.ApprisalPeriod = (string)config["Appraisal_Period"];
                    AppDocDetails.Dim1 = (string)config["Dim1"];
                    AppDocDetails.Dim2 = (string)config["Dim2"];
                    AppDocDetails.Open_To = (string)config["Open_To"];
                }
            }
            return View(AppDocDetails);
        }
        public PartialViewResult LoadObjectives(string AppDoc)
        {
            List<IndividualObjectives> ListOfAppraisalObj = new List<IndividualObjectives>();

            string page = "IndividualWorkPlanObj?$filter=Code eq '" + AppDoc + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    IndividualObjectives AppObjCard = new IndividualObjectives();
                    AppObjCard.Code = AppDoc;
                    AppObjCard.Obj = (string)config["Objective"];
                    AppObjCard.Remarks = (string)config["Supervisor_Remarks"];
                    AppObjCard.EntryNo = (string)config["Entry_No"];
                    ListOfAppraisalObj.Add(AppObjCard);
                }
            }
            return PartialView("~/Views/IndividualWorkPlan/Partial View/IndividualObjective.cshtml", ListOfAppraisalObj);
        }
        public ActionResult ObjectiveTargetList(string AppDoc, string ObjEntryNo)
        {
            DocTarget DocTarget = new DocTarget();
            #region Get Objective
            string page = "IndividualWorkPlanObj?$filter=Code eq '" + AppDoc + "' and Entry_No eq " + Convert.ToInt32(ObjEntryNo) + "&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    DocTarget.Objective = (string)config["Objective"];
                    DocTarget.Remarks = (string)config["Supervisor_Remarks"];
                }
            }
            #endregion
            DocTarget.DocNo = AppDoc;
            DocTarget.ObjEntryNo = ObjEntryNo;
            return View(DocTarget);
        }
        public ActionResult LoadObjectiveTarget(string AppDoc, string ObjEntryNo)
        {
            #region Targets
            List<ObjTarget> ListOfObjTargets = new List<ObjTarget>();

            string pageTarget = "WorkPlanTarget?$filter=Code eq '" + AppDoc + "' and Objective_Entry_No eq " + Convert.ToInt32(ObjEntryNo) + "&$format=json";

            HttpWebResponse httpResponseTarget = Credentials.GetOdataData(pageTarget);
            using (var streamReader = new StreamReader(httpResponseTarget.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    ObjTarget tar = new ObjTarget();
                    tar.Target = (string)config["Objective_Target"];
                    tar.ObjEntryNo = (string)config["Objective_Entry_No"];
                    tar.EntryNo = (string)config["Entry_No"];
                    #region Activities
                    List<TargetActivities> ListOfActivities = new List<TargetActivities>();

                    string pageActivities = "IndividualwkActivities?$filter=Code eq '" + AppDoc + "' and Objective_Entry_No eq " + (int)config["Objective_Entry_No"] + " and Target_Entry_No eq " + (int)config["Entry_No"] + "&$format=json";

                    HttpWebResponse httpResponseActivities = Credentials.GetOdataData(pageActivities);
                    using (var streamReaderAct = new StreamReader(httpResponseActivities.GetResponseStream()))
                    {
                        var resultAct = streamReaderAct.ReadToEnd();

                        var detailsAct = JObject.Parse(resultAct);
                        foreach (JObject config1 in detailsAct["value"])
                        {
                            TargetActivities act = new TargetActivities();
                            act.Activity = (string)config1["Activity"];
                            act.Resources_Required = (string)config1["Resources_Required"];
                            act.Expected_Results = (string)config1["Expected_Results"];
                            act.Time_Frame = (string)config1["Time_Frame"];
                            act.Performance_Indicator = (string)config1["Performance_Indicator"];
                            act.Objective_Entry_No = (string)config1["Objective_Entry_No"];
                            act.Target_Entry_No = (string)config1["Target_Entry_No"];
                            act.Entry_No = (string)config1["Entry_No"];
                            ListOfActivities.Add(act);
                        }
                    }
                    #endregion
                    tar.ListOfActivities = ListOfActivities;
                    ListOfObjTargets.Add(tar);
                }
            }
            #endregion
            return PartialView("~/Views/IndividualWorkPlan/Partial View/ObjectiveTargetListView.cshtml", ListOfObjTargets);
        }
        [HttpPost]
        public JsonResult SendDocumentTo(string DocNo, string SendTo)
        {
            try
            {
                Credentials.ObjNav.OpenIndividualWorkPlanTo(DocNo, Convert.ToInt32(SendTo));
                string ToLevel = "";
                if (SendTo == "0")
                {
                    ToLevel = "Individual Work Plan, Document No. " + DocNo + " send to Appraisee";
                }
                if (SendTo == "1")
                {
                    ToLevel = "Individual Work Plan, Document No. " + DocNo + " send to Supervisor";
                }
                if (SendTo == "2")
                {
                    ToLevel = "Individual Work Plan, Document No. "+ DocNo + " Closed";
                }
                Session["SuccessMsg"] = ToLevel; 
                return Json(new { message = ToLevel, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult NewWkObjectiveForm(string Objective,string EntryNo)
        {
            try
            {
                IndividualObjectives Obj = new IndividualObjectives();
                Obj.Obj = Objective;
                Obj.EntryNo = EntryNo;
                return PartialView("~/Views/IndividualWorkPlan/Partial View/NewObjectiveForm.cshtml", Obj);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public JsonResult SubmitObjective(string AppNo, string Objective)
        {
            try
            {
                Credentials.ObjNav.InsertWorkPlanObjective(AppNo, Objective);
                return Json(new { message = "Objective Submitted successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateObjective(string AppNo,string EntryNo, string Objective)
        {
            try
            {
                Credentials.ObjNav.UpdateWorkPlanObjective(AppNo, Convert.ToInt32(EntryNo), Objective);
                return Json(new { message = "Objective Updated successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteObjective(string AppNo, string EntryNo)
        {
            try
            {
                Credentials.ObjNav.DeleteWorkPlanObjective(AppNo, Convert.ToInt32(EntryNo));
                return Json(new { message = "Objective deleted successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult NewObjectiveTargetForm(string Target, string EntryNo)
        {
            try
            {
                ObjTarget target = new ObjTarget();
                target.Target = Target;
                target.EntryNo = EntryNo;
                return PartialView("~/Views/IndividualWorkPlan/Partial View/NewTargetForm.cshtml", target);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitObjectiveTarget(string AppNo, string ObjEntryNo, string Target)
        {
            try
            {
                Credentials.ObjNav.InsertWorkPlanObjectiveTarget(AppNo, Convert.ToInt32(ObjEntryNo), Target);
                return Json(new { message = "Target Submitted successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateObjectiveTarget(string AppNo, string EntryNo, string Target)
        {
            try
            {
                Credentials.ObjNav.UpdateWorkPlanObjectiveTarget(AppNo, Convert.ToInt32(EntryNo), Target);
                return Json(new { message = "Target Updated successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteObjectiveTarget(string AppNo, string EntryNo)
        {
            try
            {
                Credentials.ObjNav.DeleteWorkPlanObjectiveTarget(AppNo, Convert.ToInt32(EntryNo));
                return Json(new { message = "Target Deleted successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult NewTargetActivityForm(string TargetEntryNo)
        {
            try
            {
                TargetActivities TActivity = new TargetActivities();
                TActivity.Target_Entry_No = TargetEntryNo;
                LoadQuartorsList();
                return PartialView("~/Views/IndividualWorkPlan/Partial View/NewActivityForm.cshtml", TActivity);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult UpdateTargetActivityForm(string EntryNo)
        {
            try
            {
                TargetActivities TActivity = new TargetActivities();
                #region Activities
                List<TargetActivities> ListOfActivities = new List<TargetActivities>();

                string pageActivities = "IndividualwkActivities?$filter=Entry_No eq " + Convert.ToInt32(EntryNo) + "&$format=json";

                HttpWebResponse httpResponseActivities = Credentials.GetOdataData(pageActivities);
                using (var streamReaderAct = new StreamReader(httpResponseActivities.GetResponseStream()))
                {
                    var resultAct = streamReaderAct.ReadToEnd();

                    var detailsAct = JObject.Parse(resultAct);
                    foreach (JObject config1 in detailsAct["value"])
                    {
                        TActivity.Activity = (string)config1["Activity"];
                        TActivity.Resources_Required = (string)config1["Resources_Required"];
                        TActivity.Expected_Results = (string)config1["Expected_Results"];
                        if ((string)config1["Time_Frame"] == "Q1")
                        {
                            TActivity.Time_Frame = "1";
                        }
                        else if ((string)config1["Time_Frame"] == "Q2")
                        {
                            TActivity.Time_Frame = "2";
                        }
                        else if ((string)config1["Time_Frame"] == "Q3")
                        {
                            TActivity.Time_Frame = "3";
                        }
                        else if ((string)config1["Time_Frame"] == "Q4")
                        {
                            TActivity.Time_Frame = "4";
                        }
                        else if ((string)config1["Time_Frame"] == "Quarterly")
                        {
                            TActivity.Time_Frame = "5";
                        }
                        else if ((string)config1["Time_Frame"] == "By 25th of Every Month")
                        {
                            TActivity.Time_Frame = "6";
                        }
                        else
                        {
                            TActivity.Time_Frame = "5";
                        }
                        TActivity.Performance_Indicator = (string)config1["Performance_Indicator"];
                        TActivity.Objective_Entry_No = (string)config1["Objective_Entry_No"];
                        TActivity.Target_Entry_No = (string)config1["Target_Entry_No"];
                        TActivity.Entry_No = (string)config1["Entry_No"];
                        ListOfActivities.Add(TActivity);
                    }
                }
                #endregion
                LoadQuartorsList();
                return PartialView("~/Views/IndividualWorkPlan/Partial View/NewActivityForm.cshtml", TActivity);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitTargetActivity(string AppNo, TargetActivities act)
        {
            try
            {
                Credentials.ObjNav.InsertWorkPlanObjectiveTargetActivity(AppNo, Convert.ToInt32(act.Objective_Entry_No), Convert.ToInt32(act.Target_Entry_No),
                    act.Activity, act.Resources_Required, act.Expected_Results, Convert.ToInt32(act.Time_Frame), act.Performance_Indicator);
                return Json(new { message = "Target Activity Submitted successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateTargetActivity(string AppNo,string EntryNo, TargetActivities act)
        {
            try
            {
                Credentials.ObjNav.UpdateWorkPlanObjectiveTargetActivity(AppNo, Convert.ToInt32(EntryNo),
                    act.Activity, act.Resources_Required, act.Expected_Results, Convert.ToInt32(act.Time_Frame), act.Performance_Indicator);
                return Json(new { message = "Target Activity Updated successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteTargetActivity(string AppNo, string EntryNo)
        {
            try
            {
                Credentials.ObjNav.DeleteWorkPlanObjectiveTargetActivity(AppNo, Convert.ToInt32(EntryNo));
                return Json(new { message = "Target Activity Deleted successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        protected void LoadQuartorsList()
        {
            try
            {
                List<DropdownList> dropdownList = new List<DropdownList>();
                for (int i = 1; i < 8; i++)
                {
                    DropdownList ddl = new DropdownList()
                    {
                        Value = i.ToString()
                    };
                    if (i == 1)
                    {
                        ddl.Text = "Q1";
                    }
                    else if (i == 2)
                    {
                        ddl.Text = "Q2";
                    }
                    else if (i == 3)
                    {
                        ddl.Text = "Q3";
                    }
                    else if (i == 4)
                    {
                        ddl.Text = "Q4";
                    }
                    else if (i == 5)
                    {
                        ddl.Text = "Quarterly";
                    }
                    else if (i == 6)
                    {
                        ddl.Text = "By 25th of Every Month";
                    }
                    else
                    {
                        ddl.Text = "By 14th Day After End of Quarter";
                    }
                    dropdownList.Add(ddl);
                }
                ((dynamic)base.ViewBag).QList = dropdownList;
            }
            catch (Exception exception)
            {
                exception.Data.Clear();
            }
        }
        #endregion
        #region Work Plan Reviews
        public ActionResult IndividualWorkPlanReviewList()
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
        public PartialViewResult IndividualWorkPlanReviewListView()
        {
            string StaffNo = Session["Username"].ToString();
            List<IndividualWorkPlan> ListOfAppraisal = new List<IndividualWorkPlan>();

            string page = "IndividualWorklan?$filter=Supervisor_No eq '" + StaffNo + "' and Open_To eq 'Supervisor'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    IndividualWorkPlan P = new IndividualWorkPlan();
                    P.DocNo = (string)config["Code"];
                    P.StaffNo = (string)config["Staff_No"];
                    P.StaffName = (string)config["Employee_Name"];
                    P.ApprisalPeriod = (string)config["Appraisal_Period"];
                    P.Dim1 = (string)config["Dim1"];
                    P.Dim2 = (string)config["Dim2"];
                    P.Status = (string)config["Open_To"];
                    ListOfAppraisal.Add(P);
                }
            }
            return PartialView("~/Views/IndividualWorkPlan/Partial Review/IndividualWKReviewListView.cshtml", ListOfAppraisal);
        }
        public ActionResult IndividualWorkPlanReviewCard(string AppDoc)
        {
            string StaffNo = Session["Username"].ToString();
            IndividualWorkPlan AppDocDetails = new IndividualWorkPlan();

            string page = "IndividualWorkPlanCard?$filter=Code eq '" + AppDoc + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    AppDocDetails.DocNo = (string)config["Code"];
                    AppDocDetails.StaffNo = (string)config["Staff_No"];
                    AppDocDetails.StaffName = (string)config["Employee_Name"];
                    AppDocDetails.ApprisalPeriod = (string)config["Appraisal_Period"];
                    AppDocDetails.Dim1 = (string)config["Dim1"];
                    AppDocDetails.Dim2 = (string)config["Dim2"];
                }
            }
            return View(AppDocDetails);
        }
        public PartialViewResult LoadObjectivesReviewList(string AppDoc)
        {
            List<IndividualObjectives> ListOfAppraisalObj = new List<IndividualObjectives>();

            string page = "IndividualWorkPlanObj?$filter=Code eq '" + AppDoc + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    IndividualObjectives AppObjCard = new IndividualObjectives();
                    AppObjCard.Code = AppDoc;
                    AppObjCard.Obj = (string)config["Objective"];
                    AppObjCard.Remarks = (string)config["Supervisor_Remarks"];
                    AppObjCard.EntryNo = (string)config["Entry_No"];
                    ListOfAppraisalObj.Add(AppObjCard);
                }
            }
            return PartialView("~/Views/IndividualWorkPlan/Partial Review/IndividualObjectiveReview.cshtml", ListOfAppraisalObj);
        }
        public ActionResult ObjectiveTargetReviewList(string AppDoc, string ObjEntryNo)
        {
            DocTarget DocTarget = new DocTarget();
            #region Get Objective
            string page = "IndividualWorkPlanObj?$filter=Code eq '" + AppDoc + "' and Entry_No eq " + Convert.ToInt32(ObjEntryNo) + "&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    DocTarget.Objective = (string)config["Objective"];
                    DocTarget.Remarks = (string)config["Supervisor_Remarks"];
                }
            }
            #endregion
            DocTarget.DocNo = AppDoc;
            DocTarget.ObjEntryNo = ObjEntryNo;
            return View(DocTarget);
        }
        public ActionResult LoadObjectiveReviewTarget(string AppDoc, string ObjEntryNo)
        {
            #region Targets
            List<ObjTarget> ListOfObjTargets = new List<ObjTarget>();

            string pageTarget = "WorkPlanTarget?$filter=Code eq '" + AppDoc + "' and Objective_Entry_No eq " + Convert.ToInt32(ObjEntryNo) + "&$format=json";

            HttpWebResponse httpResponseTarget = Credentials.GetOdataData(pageTarget);
            using (var streamReader = new StreamReader(httpResponseTarget.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    ObjTarget tar = new ObjTarget();
                    tar.Target = (string)config["Objective_Target"];
                    tar.ObjEntryNo = (string)config["Objective_Entry_No"];
                    tar.EntryNo = (string)config["Entry_No"];
                    #region Activities
                    List<TargetActivities> ListOfActivities = new List<TargetActivities>();

                    string pageActivities = "IndividualwkActivities?$filter=Code eq '" + AppDoc + "' and Objective_Entry_No eq " + (int)config["Objective_Entry_No"] + " and Target_Entry_No eq " + (int)config["Entry_No"] + "&$format=json";

                    HttpWebResponse httpResponseActivities = Credentials.GetOdataData(pageActivities);
                    using (var streamReaderAct = new StreamReader(httpResponseActivities.GetResponseStream()))
                    {
                        var resultAct = streamReaderAct.ReadToEnd();

                        var detailsAct = JObject.Parse(resultAct);
                        foreach (JObject config1 in detailsAct["value"])
                        {
                            TargetActivities act = new TargetActivities();
                            act.Activity = (string)config1["Activity"];
                            act.Resources_Required = (string)config1["Resources_Required"];
                            act.Expected_Results = (string)config1["Expected_Results"];
                            act.Time_Frame = (string)config1["Time_Frame"];
                            act.Performance_Indicator = (string)config1["Performance_Indicator"];
                            act.Objective_Entry_No = (string)config1["Objective_Entry_No"];
                            act.Target_Entry_No = (string)config1["Target_Entry_No"];
                            act.Entry_No = (string)config1["Entry_No"];
                            ListOfActivities.Add(act);
                        }
                    }
                    #endregion
                    tar.ListOfActivities = ListOfActivities;
                    ListOfObjTargets.Add(tar);
                }
            }
            #endregion
            return PartialView("~/Views/IndividualWorkPlan/Partial Review/ObjectiveTargetReviewList.cshtml", ListOfObjTargets);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SaveWorkplanObjectiveRemarks(string AppNo, string EntryNo,string Remarks)
        {
            try
            {
                Credentials.ObjNav.InsertWorkPlanObjectiveRemarks(AppNo, Convert.ToInt32(EntryNo), Remarks);
                return Json(new { message = "Target Activity Deleted successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}