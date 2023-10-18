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
    [CustomAuthorization(Role = "ALLUSERS")]
    public class AppraisalController : Controller
    {
        // GET: Appraisal
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

            string page = "AppraisalCard?$filter=Staff_No eq '" + StaffNo + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    AppraisalCardList ApprList = new AppraisalCardList();
                    ApprList.ApprisalCode = (string)config["Appraisal_Code"];
                    ApprList.AppraisalType = (string)config["Appraisal_Type"];
                    ApprList.StaffName = (string)config["Staff_Name"];
                    ApprList.ApprisalPeriod = (string)config["Appraisal_Period"];
                    ApprList.RespCenter = (string)config["Resp_Center"];
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

            #region ApprisalTypes
            List<AppraisalTypes> AppraisalTyp = new List<AppraisalTypes>();

            string page = "AppraisalTypes?&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    AppraisalTypes AppTyp = new AppraisalTypes();
                    AppTyp.Code = (string)config["Code"];
                    AppTyp.Description = (string)config["Description"];
                    AppraisalTyp.Add(AppTyp);
                }
            }
            #endregion

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
                    ApPeriod.Period = (string)config["Period"];
                    AppPeriod.Add(ApPeriod);
                }
            }
            #endregion

            #region Responsibility
            List<RespCenter> RespCList = new List<RespCenter>();
            string pageResC = "ResponsibilityCenters?$format=json";

            HttpWebResponse httpResponseResC = Credentials.GetOdataData(pageResC);
            using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);


                foreach (JObject config in details["value"])
                {
                    RespCenter RCList = new RespCenter();
                    RCList.Code = (string)config["Code"];
                    RCList.Name = (string)config["Name"];
                    RespCList.Add(RCList);
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
                ListOfApprisalTypes = AppraisalTyp.Select(x =>
                                     new SelectListItem()
                                     {
                                         Text = x.Description,
                                         Value = x.Code
                                     }).ToList(),
                ListOfResponsibility = RespCList.Select(x =>
                                   new SelectListItem()
                                   {
                                       Text = x.Name,
                                       Value = x.Code
                                   }).ToList()
            };
            return PartialView("~/Views/Appraisal/Partial Views/NewApprisalRequest.cshtml", NewAppl);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitAppraisalDocument(NewAppraisalDocument NewApp)
        {
            try
            {
                string DocNo = Credentials.ObjNav.createAppraisalDocument(Session["username"].ToString(), NewApp.ApprisalPeriod,"", "","");
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
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                string StaffNo = Session["Username"].ToString();
                AppraisalCardList AppDocDetails = new AppraisalCardList();

                string page = "AppraisalCard?$filter=Appraisal_Code eq '" + AppDoc + "'&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        AppDocDetails.ApprisalCode = (string)config["Appraisal_Code"];
                        AppDocDetails.AppraisalType = (string)config["Appraisal_Type"];
                        AppDocDetails.StaffNo = StaffNo;
                        AppDocDetails.StaffName = (string)config["Staff_Name"];
                        AppDocDetails.RespCenter = (string)config["Resp_Center"];
                        AppDocDetails.UserID = (string)config["User_ID"];
                        AppDocDetails.Department = (string)config["Department"];
                        AppDocDetails.ApprisalPeriod = (string)config["Appraisal_Period"];
                        AppDocDetails.RespCenter = (string)config["Resp_Center"];
                        AppDocDetails.Status = (string)config["Status"];
                    }
                }

                //string s = CommonClass.GetEmployeeJobCategory(StaffNo);

                if (AppDocDetails.AppraisalType == "HALFYEAR1")
                {
                    return View("~/Views/Appraisal/ScoreCardNonTeachingStaff.cshtml", AppDocDetails);
                }
                else if (AppDocDetails.AppraisalType == "HALFYEAR2")
                {
                    return View("~/Views/Appraisal/ScoreCardTeachingStaff.cshtml", AppDocDetails);
                }
                else
                {
                    return View("~/Views/Appraisal/ScoreCardGeneralStaff.cshtml", AppDocDetails);
                }
            }
        }
        public PartialViewResult LoadScoreCardObjectives(string AppDoc, string Type)
        {
            #region Objective List
            List<ScoreCardObjectives> ListOfAppraisalObj = new List<ScoreCardObjectives>();

            string page = "HRAppraisalObjectives?$filter=Document_No eq '" + AppDoc + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    ScoreCardObjectives AppObjCard = new ScoreCardObjectives();
                    AppObjCard.LineNo = (string)config["Line_No"];
                    AppObjCard.DocumentNo = (string)config["Document_No"];
                    AppObjCard.Objective = (string)config["Objective"];
                    AppObjCard.KeyPerformanceIndicator = (string)config["Key_Performance_Indicator"];
                    AppObjCard.Targets = (string)config["Targets"];
                    AppObjCard.Achievements = (string)config["Achievements"];
                    AppObjCard.Ratings = (string)config["Ratings"];
                    AppObjCard.SupervisorRating = (string)config["Supervisor_Rating"];
                    ListOfAppraisalObj.Add(AppObjCard);
                }
            }
            #endregion
            string[] s = new string[2];
            if (Type == "S")
            {
                s = CommonClass.GetAppraisalComments(AppDoc, "Appraisee_Comment", "Supervisor_Comment");
            }
            else
            {
                s = CommonClass.GetAppraisalComments(AppDoc, "Appraisee_Review_Comment", "Supervisor_Review_Comment");
            }
            SectionDetails newsection = new SectionDetails
            {
                AppraiseeComment = s[0],
                SuporvisorComment = s[1],
                ObjList = ListOfAppraisalObj
            };
            if (Type == "S")
            {
                return PartialView("~/Views/Appraisal/Partial Views/HRAppraisalObjectives.cshtml", newsection);
            }
            else
            {
                return PartialView("~/Views/Appraisal/Partial Views/ObjectiveEvauation.cshtml", newsection);
            }
        }
        public PartialViewResult LoadCoreSkillsCompetenceValues(string AppDoc)
        {
            #region Skill Compe
            List<ScoreCardObjectives> ListOfAppraisalObj = new List<ScoreCardObjectives>();

            string page = "HRAppraisalSkilsComp?$filter=Appraisal_No eq '" + AppDoc + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    ScoreCardObjectives AppObjCard = new ScoreCardObjectives();
                    AppObjCard.LineNo = (string)config["Line_No"];
                    AppObjCard.DocumentNo = (string)config["Appraisal_No"];
                    AppObjCard.Objective = (string)config["Competence"];
                    AppObjCard.Ratings = (string)config["Appraisee_Score"];
                    AppObjCard.SupervisorRating = (string)config["Appraiser_Score"];
                    ListOfAppraisalObj.Add(AppObjCard);
                }
            }
            #endregion
            string[] s = CommonClass.GetAppraisalComments(AppDoc, "Appraisee_Skills_Comp_Comment", "Appraiser_Skills_Comp_Comment");
            SectionDetails newsection = new SectionDetails
            {
                AppraiseeComment = s[0],
                SuporvisorComment = s[1],
                ObjList = ListOfAppraisalObj
            };
            return PartialView("~/Views/Appraisal/Partial Views/CoreSkillsCompetence.cshtml", newsection);
        }
        public PartialViewResult LoadPImprovementSkillsDevPlan(string AppDoc)
        {
            #region Skill Development
            List<ScoreCardObjectives> ListOfAppraisalObj = new List<ScoreCardObjectives>();

            string page = "HRAppraisalSkills_Dev?$filter=Appraisal_No eq '" + AppDoc + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    ScoreCardObjectives AppObjCard = new ScoreCardObjectives();
                    AppObjCard.LineNo = (string)config["Line_No"];
                    AppObjCard.DocumentNo = (string)config["Appraisal_No"];
                    AppObjCard.Objective = (string)config["Goals_Objectives"];
                    AppObjCard.weight = (string)config["Objective_Weight"];
                    ListOfAppraisalObj.Add(AppObjCard);
                }
            }
            #endregion
            string[] s = CommonClass.GetAppraisalComments(AppDoc, "Appraisee_Skills_Dev_Comment", "Appraiser_Skills_Dev_Comment");
            SectionDetails newsection = new SectionDetails
            {
                AppraiseeComment = s[0],
                SuporvisorComment = s[1],
                ObjList = ListOfAppraisalObj
            };
            return PartialView("~/Views/Appraisal/Partial Views/PerfImprovementPlan.cshtml", newsection);
        }
        public PartialViewResult LoadGeneralComments(string AppDoc)
        {
            GeneralComments generalComm = new GeneralComments();

            string[] s = CommonClass.GetAppraisalComments(AppDoc, "Appraisee_General_Comment", "Supervisor_General_Comment");
            generalComm.AppraiseeComment = s[0];
            generalComm.SuporvisorComment = s[1];
            return PartialView("~/Views/Appraisal/Partial Views/GeneralComments.cshtml", generalComm);
        }
        public PartialViewResult NewObjectiveLine()
        {
            return PartialView("~/Views/Appraisal/Partial Views/AddAppraisalObjectiveLine.cshtml");
        }
        [HttpPost]
        public JsonResult SubmitAppraisalObjectiveLine(ApprisalObjective AppObjective)
        {
            try
            {
                Credentials.ObjNav.SaveUpdateHRAppraisalObjective(0, AppObjective.DocNo, AppObjective.AppraisalPeriod, AppObjective.Objective,
                                                                  AppObjective.Target, AppObjective.KeyPerformance);

                return Json(new { message = "Record Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult DeleteAppraisalObjectiveLine(string LnNo, string DocNo)
        {
            try
            {
                Credentials.ObjNav.DeleteHRAppraisalObjective(Convert.ToInt32(LnNo), DocNo);

                return Json(new { message = "Record Removed Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SaveAppraisalObjectiveLineAchievement(string LnNo, string DocNo, string achvmnt, string ratings)
        {
            try
            {
                Credentials.ObjNav.HRAppraisalObjectiveReview(Convert.ToInt32(LnNo), DocNo, achvmnt, Convert.ToDecimal(ratings));

                return Json(new { message = "Objective achievement Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult EvaluateAppraisalObjectiveLine(string LnNo, string DocNo)
        {
            ScoreCardObjectives ObjLine = new ScoreCardObjectives();

            string page = "HRAppraisalObjectives?$filter=Document_No eq '" + DocNo + "' and Line_No eq " + Convert.ToInt32(LnNo) + "&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    ObjLine.LineNo = (string)config["Line_No"];
                    ObjLine.DocumentNo = (string)config["Document_No"];
                    ObjLine.Objective = (string)config["Objective"];
                    ObjLine.KeyPerformanceIndicator = (string)config["Key_Performance_Indicator"];
                    ObjLine.Targets = (string)config["Targets"];
                    ObjLine.Achievements = (string)config["Achievements"];
                    ObjLine.Ratings = (string)config["Ratings"];
                    ObjLine.SupervisorRating = (string)config["Supervisor_Rating"];
                }
            }
            return PartialView("~/Views/Appraisal/Partial Views/EvaluateObjective.cshtml", ObjLine);
        }
        public PartialViewResult NewCoreCompetenceValue()
        {
            List<ScoreCardObjectives> ListOfAppraisalObj = new List<ScoreCardObjectives>();

            string page = "HRAppraisalEvaluationAreas?$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    ScoreCardObjectives AppObjCard = new ScoreCardObjectives();
                    AppObjCard.DocumentNo = (string)config["Code"];
                    AppObjCard.Objective = (string)config["Description"];
                    ListOfAppraisalObj.Add(AppObjCard);
                }
            }
            return PartialView("~/Views/Appraisal/Partial Views/AddCoreCompetenceValue.cshtml", ListOfAppraisalObj);
        }
        public JsonResult SubmitCoreCompetenceValue(string DocNo, string AppPeriod, List<Array> skillComp)
        {
            try
            {
                int RowCount = skillComp.Count();
                for (int i = 0; i < RowCount; i++)
                {
                    string[] RowText = (string[])skillComp[i];

                    string Code = RowText[1].Trim();
                    string Description = RowText[2].Trim();
                    string Score = RowText[3].Trim();
                    Credentials.ObjNav.SaveUpdateHRAppraisalSkillsCompetence(0, DocNo, AppPeriod, Description, Code, Convert.ToDecimal(Score),0);
                }

                return Json(new { message = "Record Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult EditSkillCompetenceValue(string LnNo, string DocNo)
        {
            ScoreCardObjectives SkillComp = new ScoreCardObjectives();

            string page = "HRAppraisalSkilsComp?$filter=Appraisal_No eq '" + DocNo + "' and Line_No eq " + Convert.ToInt32(LnNo) + "&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    SkillComp.LineNo = (string)config["Line_No"];
                    SkillComp.DocumentNo = (string)config["Appraisal_No"];
                    SkillComp.Objective = (string)config["Competence"];
                    SkillComp.Ratings = (string)config["Appraisee_Score"];
                    SkillComp.SupervisorRating = (string)config["Appraiser_Score"];
                }
            }
            return PartialView("~/Views/Appraisal/Partial Views/ScoreSkillCompetence.cshtml", SkillComp);
        }
        [HttpPost]
        public JsonResult CoreCompetenceValue(string LnNo, string DocNo)
        {
            try
            {
                Credentials.ObjNav.DeleteHRAppraisalSkillsCompetence(Convert.ToInt32(LnNo), DocNo);

                return Json(new { message = "Record removed Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult NewSkillDevelopment()
        {
            return PartialView("~/Views/Appraisal/Partial Views/AddAreaOfImprovement.cshtml");
        }
        public JsonResult SubmitSkillDevelopmentValue(ApprisalObjective AppObjective)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                Credentials.ObjNav.SaveUpdateHRAppraisalSkillsDevelopment(0, AppObjective.DocNo, StaffNo, AppObjective.Objective, Convert.ToInt32(AppObjective.weight));

                return Json(new { message = "Record Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult DeleteSkillDevelopmentValue(string LnNo, string DocNo)
        {
            try
            {
                Credentials.ObjNav.DeleteHRAppraisalSkillsDevelopment(Convert.ToInt32(LnNo), DocNo);

                return Json(new { message = "Record removed Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SubmitSkillCompScore(string LnNo, string DocNo, string score)
        {
            try
            {
                Credentials.ObjNav.HRAppraisalSkillsCompetenceRatings(Convert.ToInt32(LnNo), DocNo, 0, Convert.ToDecimal(score));

                return Json(new { message = "Record Score Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SaveAppriseeComments(string DocNo, string Comment, string sectioon)
        {
            try
            {
                bool saved = false;
                if (sectioon == "1")
                {
                    Credentials.ObjNav.HRAppraisalObjectiveCommenst(DocNo, 0, Comment);
                    saved = true;
                }
                else if (sectioon == "2")
                {
                    Credentials.ObjNav.HRAppraisalObjectiveReviewCommenst(DocNo, 0, Comment);
                    saved = true;
                }
                else if (sectioon == "3")
                {
                    Credentials.ObjNav.HRAppraisalSkillsCompetenceCommenst(DocNo, 0, Comment);
                    saved = true;
                }
                else if (sectioon == "4")
                {
                    Credentials.ObjNav.HRAppraisalSkillsDevelopmentCommenst(DocNo, 0, Comment);
                    saved = true;
                }
                else if (sectioon == "5")
                {
                    Credentials.ObjNav.HRAppraisalGeneralCommenst(DocNo, 0, Comment);
                    saved = true;
                }
                else
                {
                    saved = false;
                }
                if (saved)
                {
                    return Json(new { message = "Comments Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "No section to save the Comments!", success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SendAppraisalForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.AppraisalRequisitionApprovalRequest(DocNo);
                return Json(new { message = "Appraisal Requisition send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult CancelAppraisalForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.HRCancelApprisalRequisition(DocNo);
                return Json(new { message = "Appraisal Requisition approval cancelled Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }       
    }
}
