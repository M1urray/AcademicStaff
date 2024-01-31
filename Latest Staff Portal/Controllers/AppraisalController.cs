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

            string page = "AppraisalCard?$filter=Staff_No eq '" + StaffNo + "'&$format=json";

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
                    //ApprList.RespCenter = (string)config["Resp_Center"];
                    ApprList.Status = (string)config["Status"];
                    ApprList.OpenTo = (string)config["Open_To"];
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

            string page = "AppraisalTypes?&$format=json";

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

            string pageAppPeriods = "ApprisalPeriods?&filter=Current eq true&$format=json";

            HttpWebResponse httpResponseAppPeriods = Credentials.GetOdataData(pageAppPeriods);
            using (var streamReader = new StreamReader(httpResponseAppPeriods.GetResponseStream()))
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
            #region Employee List
            List<RelieverList> EmpList = new List<RelieverList>();

            string pageEmp = "EmployeeList?$select=No,First_Name,Middle_Name,Last_Name&$filter=No ne '" + StaffNo + "'&$format=json";

            HttpWebResponse httpResponseEmp = Credentials.GetOdataData(pageEmp);
            using (var streamReader = new StreamReader(httpResponseEmp.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    if ((string)config["First_Name"] != "" || (string)config["Last_Name"] != "")
                    {
                        RelieverList Rlist = new RelieverList();
                        Rlist.No = (string)config["No"];
                        Rlist.Name = (string)config["First_Name"] + " " + (string)config["Middle_Name"] + " " + (string)config["Last_Name"];
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
                ListOfApprisalTypes = AppraisalTyp.Select(x =>
                                     new SelectListItem()
                                     {
                                         Text = x.Description,
                                         Value = x.Code
                                     }).OrderBy(x => x.Text).ToList(),
                ListOfEmployee = EmpList.Select(x =>
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
                string DocNo = "";// Credentials.ObjNav.createAppraisalDocument(Session["username"].ToString(), NewApp.ApprisalPeriod, NewApp.ApprisalType, "", NewApp.Supervisor);
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

            string page = "AppraisalCard?$filter=Appraisal_Code eq '" + AppDoc + "'&$format=json";

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
                    AppDocDetails.Supervisor = (string)config["Supervisor"];
                    AppDocDetails.SupervisorName = (string)config["Appraiser_Names"];
                    AppDocDetails.Peer = (string)config["Peer"];
                    AppDocDetails.PeerName = (string)config["Peer_Name"];
                    AppDocDetails.PeerExplored = (bool)config["Explored_By_Appraisee"];
                    AppDocDetails.SupervisorExplored = (bool)config["Explored_By_Supervisor"];
                    AppDocDetails.PeerExplored = (bool)config["Explored_By_Peer"];
                    AppDocDetails.OpenTo = (string)config["Open_To"];
                }
            }

            string s = "";//CommonClass.GetEmployeeJobCategory(StaffNo);
            if (s == "N")
            {
                return View("~/Views/Appraisal/Partial Views/ScoreCardNonTeachingStaff.cshtml", AppDocDetails);
            }
            else
            {
                return View("~/Views/Appraisal/Partial Views/ScoreCardTeachingStaff.cshtml", AppDocDetails);
            }
        }
        public PartialViewResult LoadScoreCardObjectives(string AppDoc, string Type, string Level)
        {
            #region Objective List
            List<ScoreCardObjectives> ListOfAppraisalObj = new List<ScoreCardObjectives>();

            string page = "HRAppraisalObjectives?$filter=Document_No eq '" + AppDoc + "'&$format=json";

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
                    AppObjCard.PeerRating = (string)config["Peer_Ratings"];
                    AppObjCard.SupervisorRating = (string)config["Supervisor_Rating"];
                    ListOfAppraisalObj.Add(AppObjCard);
                }
            }
            #endregion
            string[] s = new string[2];
            if (Type == "S")
            {
                //s = CommonClass.GetAppraisalComments(AppDoc, "Appraisee_Comment", "Supervisor_Comment");
            }
            else
            {
                //s = CommonClass.GetAppraisalComments(AppDoc, "Appraisee_Review_Comment", "Supervisor_Review_Comment");
            }
            SectionDetails newsection = new SectionDetails
            {
                AppraiseeComment = s[0],
                SuporvisorComment = s[1],
                ObjList = ListOfAppraisalObj,
                Level = Level
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
        public PartialViewResult LoadCoreSkillsCompetenceValues(string AppDoc, string Level)
        {
            #region Skill Compe
            List<ScoreCardObjectives> ListOfAppraisalObj = new List<ScoreCardObjectives>();

            string page = "HRAppraisalSkilsComp?$filter=Appraisal_No eq '" + AppDoc + "'&$format=json";

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
                    AppObjCard.PeerRating = (string)config["Peer_Score"];
                    AppObjCard.SupervisorRating = (string)config["Appraiser_Score"];
                    ListOfAppraisalObj.Add(AppObjCard);
                }
            }
            #endregion
            //string[] s = CommonClass.GetAppraisalComments(AppDoc, "Appraisee_Skills_Comp_Comment", "Appraiser_Skills_Comp_Comment");
            SectionDetails newsection = new SectionDetails
            {
                //AppraiseeComment = s[0],
                //SuporvisorComment = s[1],
                ObjList = ListOfAppraisalObj,
                Level = Level
            };
            return PartialView("~/Views/Appraisal/Partial Views/CoreSkillsCompetence.cshtml", newsection);
        }
        public PartialViewResult LoadPImprovementSkillsDevPlan(string AppDoc, string Level)
        {
            #region Skill Development
            List<ScoreCardObjectives> ListOfAppraisalObj = new List<ScoreCardObjectives>();

            string page = "HRAppraisalSkills_Dev?$filter=Appraisal_No eq '" + AppDoc + "'&$format=json";

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
            //string[] s = CommonClass.GetAppraisalComments(AppDoc, "Appraisee_Skills_Dev_Comment", "Appraiser_Skills_Dev_Comment");
            SectionDetails newsection = new SectionDetails
            {
                //AppraiseeComment = s[0],
                //SuporvisorComment = s[1],
                ObjList = ListOfAppraisalObj,
                Level = Level
            };
            return PartialView("~/Views/Appraisal/Partial Views/PerfImprovementPlan.cshtml", newsection);
        }
        public PartialViewResult LoadGeneralComments(string AppDoc, string Level)
        {
            GeneralComments generalComm = new GeneralComments();

            //string[] s = CommonClass.GetAppraisalComments(AppDoc, "Appraisee_General_Comment", "Supervisor_General_Comment");
            //generalComm.AppraiseeComment = s[0];
            //generalComm.SuporvisorComment = s[1];
            generalComm.Level = Level;
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
                //Credentials.ObjNav.SaveUpdateHRAppraisalObjective(0, AppObjective.DocNo, AppObjective.AppraisalPeriod, AppObjective.Objective,
                //                                                  AppObjective.Target, AppObjective.KeyPerformance);

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
        public JsonResult SaveAppraisalObjectiveLineAchievement(string LnNo, string DocNo, string achvmnt, string ratings, string Level)
        {
            try
            {
                bool val = false;
                string msg = "";
                if (Level == "A")
                {
                    Credentials.ObjNav.HRAppraisalObjectiveReview(Convert.ToInt32(LnNo), DocNo, achvmnt, Convert.ToDecimal(ratings));
                    val = true;
                }
                else
                {
                    if (Level == "S" || Level == "P")
                    {
                        int from = 0;
                        if (Level == "S")
                        {
                            from = 1;
                        }
                        else if (Level == "P")
                        {
                            from = 2;
                        }
                        else
                        {
                            from = 0;
                        }
                        Credentials.ObjNav.HRAppraisalObjectiveRatings(Convert.ToInt32(LnNo), DocNo, Convert.ToDecimal(ratings), from);
                        val = true;
                    }
                    else
                    {
                        val = false;
                    }
                }
                if (val)
                {
                    msg = "Objective review Saved Successfully";
                }
                else
                {
                    msg = "Review Level not defined";
                }
                return Json(new { message = msg, success = val }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult EvaluateAppraisalObjectiveLine(string LnNo, string DocNo, string Level)
        {
            ScoreCardObjectives ObjLine = new ScoreCardObjectives();

            string page = "HRAppraisalObjectives?$filter=Document_No eq '" + DocNo + "' and Line_No eq " + Convert.ToInt32(LnNo) + "&$format=json";

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
                    if (Level == "A")
                    {
                        ObjLine.Ratings = (string)config["Ratings"];
                    }
                    else if (Level == "S")
                    {
                        ObjLine.Ratings = (string)config["Supervisor_Rating"];
                    }
                    else if (Level == "P")
                    {
                        ObjLine.Ratings = (string)config["Peer_Ratings"];
                    }
                    else
                    {
                        ObjLine.Ratings = "";
                    }
                    ObjLine.Level = Level;
                }
            }
            return PartialView("~/Views/Appraisal/Partial Views/EvaluateObjective.cshtml", ObjLine);
        }
        public PartialViewResult NewCoreCompetenceValue(string apprisalType)
        {
            List<ScoreCardObjectives> ListOfAppraisalObj = new List<ScoreCardObjectives>();

            string page = "HRAppraisalEvaluationAreas?$select=Code,Description,Score&$filter=Appraisal_Type eq '" + apprisalType + "'&$format=json";

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
                    AppObjCard.weight = (string)config["Score"];
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
                    Credentials.ObjNav.SaveUpdateHRAppraisalSkillsCompetence(0, DocNo, AppPeriod, Description, Code, Convert.ToDecimal(Score), 0);
                }

                return Json(new { message = "Record Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult EditSkillCompetenceValue(string LnNo, string DocNo, string Level)
        {
            ScoreCardObjectives SkillComp = new ScoreCardObjectives();

            string page = "HRAppraisalSkilsComp?$filter=Appraisal_No eq '" + DocNo + "' and Line_No eq " + Convert.ToInt32(LnNo) + "&$format=json";

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
                    if (Level == "A")
                    {
                        SkillComp.Ratings = (string)config["Appraisee_Score"];
                    }
                    else if (Level == "S")
                    {
                        SkillComp.Ratings = (string)config["Appraiser_Score"];
                    }
                    else if (Level == "P")
                    {
                        SkillComp.Ratings = (string)config["Peer_Score"];
                    }
                    else
                    {
                        SkillComp.SupervisorRating = "";
                    }                    
                    SkillComp.Level = Level;
                }
            }
            return PartialView("~/Views/Appraisal/Partial Views/ScoreSkillCompetence.cshtml", SkillComp);
        }
        [HttpPost]
        public JsonResult DeleteCoreCompetenceValue(string LnNo, string DocNo)
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
        public JsonResult SubmitSkillCompScore(string LnNo, string DocNo, string score, string from)
        {
            try
            {
                Credentials.ObjNav.HRAppraisalSkillsCompetenceRatings(Convert.ToInt32(LnNo), DocNo, Convert.ToInt32(from), Convert.ToDecimal(score));

                return Json(new { message = "Record Score Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SaveAppraisalComments(string DocNo, string Comment, string section, string from)
        {
            try
            {
                bool saved = false;
                if (section == "1")
                {
                    Credentials.ObjNav.HRAppraisalObjectiveCommenst(DocNo, Convert.ToInt32(from), Comment);
                    saved = true;
                }
                else if (section == "2")
                {
                    Credentials.ObjNav.HRAppraisalObjectiveReviewCommenst(DocNo, Convert.ToInt32(from), Comment);
                    saved = true;
                }
                else if (section == "3")
                {
                    Credentials.ObjNav.HRAppraisalSkillsCompetenceCommenst(DocNo, Convert.ToInt32(from), Comment);
                    saved = true;
                }
                else if (section == "4")
                {
                    Credentials.ObjNav.HRAppraisalSkillsDevelopmentCommenst(DocNo, Convert.ToInt32(from), Comment);
                    saved = true;
                }
                else if (section == "5")
                {
                    Credentials.ObjNav.HRAppraisalGeneralCommenst(DocNo, Convert.ToInt32(from), Comment);
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
                    return Json(new { message = "No section to save the Comments! ", success = true }, JsonRequestBehavior.AllowGet);
                }
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
                    ToLevel = "Appraisee";
                }
                if (SendTo == "1")
                {
                    ToLevel = "Supervisor";
                }
                if (SendTo == "2")
                {
                    ToLevel = "Peer";
                }
                if (SendTo == "3")
                {
                    ToLevel = "HR";
                }
                return Json(new { message = "Appraisal Requisition send " + ToLevel, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult MyReviewAppraisalList(string Level)
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
        public PartialViewResult AppraisalReviewListPartialView(string Level)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<AppraisalCardList> ListOfAppraisal = new List<AppraisalCardList>();

                string page = "";
                if (Level != "")
                {
                    if (Level == "S")
                    {
                        page = "AppraisalCard?$filter=Supervisor eq '" + StaffNo + "' and Open_To eq 'Supervisor'&$format=json";
                    }
                    if (Level == "P")
                    {
                        page = "AppraisalCard?$filter=Peer eq '" + StaffNo + "' and Open_To eq 'Peer'&$format=json";
                    }

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
                            ApprList.Status = (string)config["Status"];
                            ApprList.OpenTo = (string)config["Open_To"];
                            ListOfAppraisal.Add(ApprList);
                        }
                    }
                    return PartialView("~/Views/Appraisal/AppraisalReview/AppraisalReviewList.cshtml", ListOfAppraisal);
                }
                else
                {
                    Error erroMsg = new Error();
                    erroMsg.Message = "Cannot access the page!!";
                    return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public ActionResult AppraisalReviewScoreCard(string AppDoc, string Level)
        {
            string StaffNo = Session["Username"].ToString();
            AppraisalCardList AppDocDetails = new AppraisalCardList();

            string page = "AppraisalCard?$filter=Appraisal_Code eq '" + AppDoc + "'&$format=json";

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
                    AppDocDetails.Supervisor = (string)config["Supervisor"];
                    AppDocDetails.SupervisorName = (string)config["Appraiser_Names"];
                    AppDocDetails.Peer = (string)config["Peer"];
                    AppDocDetails.No = (string)config["Peer"];
                    AppDocDetails.PeerName = (string)config["Peer_Name"];
                    AppDocDetails.PeerExplored = (bool)config["Explored_By_Appraisee"];
                    AppDocDetails.SupervisorExplored = (bool)config["Explored_By_Supervisor"];
                    AppDocDetails.PeerExplored = (bool)config["Explored_By_Peer"];
                    #region Employee List
                    List<RelieverList> EmpList = new List<RelieverList>();

                    string pageEmp = "EmployeeList?$select=No,First_Name,Middle_Name,Last_Name&$filter=No ne '" + StaffNo + "' and No ne '" + AppDocDetails.Supervisor + "'&$format=json";

                    HttpWebResponse httpResponseEmp = Credentials.GetOdataData(pageEmp);
                    using (var streamReaderEmp = new StreamReader(httpResponseEmp.GetResponseStream()))
                    {
                        var resultEmp = streamReaderEmp.ReadToEnd();

                        var detailsEmp = JObject.Parse(resultEmp);
                        foreach (JObject config1 in detailsEmp["value"])
                        {
                            if ((string)config1["First_Name"] != "" || (string)config1["Last_Name"] != "")
                            {
                                RelieverList Rlist = new RelieverList();
                                Rlist.No = (string)config1["No"];
                                Rlist.Name = (string)config1["First_Name"] + " " + (string)config1["Middle_Name"] + " " + (string)config1["Last_Name"];
                                EmpList.Add(Rlist);
                            }
                        }

                    }
                    #endregion
                    AppDocDetails.ListOfEmployee = EmpList.Select(x =>
                                   new SelectListItem()
                                   {
                                       Text = x.Name,
                                       Value = x.No
                                   }).ToList();
                }
            }
            return View("~/Views/Appraisal/AppraisalReview/ApprisalReviewDocument.cshtml", AppDocDetails);
        }
        [HttpPost]
        public JsonResult UpdatePeerReviewer(string DocNo, string Peer)
        {
            try
            {
                Credentials.ObjNav.UpdateScoreCardPeerReviewer(DocNo, Peer);
                return Json(new { message = "Peer Updated Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
