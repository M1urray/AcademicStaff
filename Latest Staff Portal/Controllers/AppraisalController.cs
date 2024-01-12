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
            return PartialView("~/Views/Appraisal/AppraisalListView.cshtml", ListOfAppraisal);
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
            return PartialView("~/Views/Appraisal/NewApprisalRequest.cshtml", NewAppl);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitAppraisalDocument(NewAppraisalDocument NewApp)
        {
            try
            {
                string DocNo = "";// Credentials.ObjNav.createAppraisalDocument(Session["username"].ToString(), NewApp.ApprisalPeriod, NewApp.ApprisalType, NewApp.Responsibility);
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
            return View(AppDocDetails);
        }
        public PartialViewResult LoadScoreCardData(string AppDoc)
        {
            string StaffNo = Session["Username"].ToString();
            List<ScoreCardObjectives> ListOfAppraisalObj = new List<ScoreCardObjectives>();

            string page = "ApprisalPartList?$filter=DocumentNo eq '" + AppDoc + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    ScoreCardObjectives AppObjCard = new ScoreCardObjectives();
                    AppObjCard.DocumentNo = (string)config["DocumentNo"];
                    AppObjCard.Objective = (string)config["Objective"];
                    AppObjCard.KeyPerformanceIndicator = (string)config["KeyPerformanceIndicator"];
                    AppObjCard.Targets = (string)config["Targets"];
                    AppObjCard.Achievements = (string)config["Achievements"];
                    AppObjCard.Ratings = (string)config["Ratings"];
                    AppObjCard.SupervisorRating = (string)config["SupervisorRating"];
                    ListOfAppraisalObj.Add(AppObjCard);
                }
            }
            return PartialView("~/Views/Appraisal/ScoreCardData.cshtml", ListOfAppraisalObj);
        }
        public PartialViewResult LoadAppraisalReviews(string AppDoc)
        {
            string StaffNo = Session["Username"].ToString();
            List<ScoreCardObjectives> ListOfAppraisalObj = new List<ScoreCardObjectives>();

            string page = "ApprisalPartList?$filter=DocumentNo eq '" + AppDoc + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    ScoreCardObjectives AppObjCard = new ScoreCardObjectives();
                    AppObjCard.DocumentNo = (string)config["DocumentNo"];
                    AppObjCard.Objective = (string)config["Objective"];
                    AppObjCard.KeyPerformanceIndicator = (string)config["KeyPerformanceIndicator"];
                    AppObjCard.Targets = (string)config["Targets"];
                    AppObjCard.Achievements = (string)config["Achievements"];
                    AppObjCard.Ratings = (string)config["Ratings"];
                    AppObjCard.SupervisorRating = (string)config["SupervisorRating"];
                    ListOfAppraisalObj.Add(AppObjCard);
                }
            }
            return PartialView("~/Views/Appraisal/ApprisalReviews.cshtml", ListOfAppraisalObj);
        }
        public PartialViewResult LoadCoreCompetenceValues(string AppDoc)
        {
            string StaffNo = Session["Username"].ToString();
            List<ScoreCardObjectives> ListOfAppraisalObj = new List<ScoreCardObjectives>();

            string page = "ApprisalPartList?$filter=DocumentNo eq '" + AppDoc + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    ScoreCardObjectives AppObjCard = new ScoreCardObjectives();
                    AppObjCard.DocumentNo = (string)config["DocumentNo"];
                    AppObjCard.Objective = (string)config["Objective"];
                    //AppObjCard.KeyPerformanceIndicator = (string)config["KeyPerformanceIndicator"];
                    //AppObjCard.Targets = (string)config["Targets"];
                    //AppObjCard.Achievements = (string)config["Achievements"];
                    //AppObjCard.Ratings = (string)config["Ratings"];
                    //AppObjCard.SupervisorRating = (string)config["SupervisorRating"];
                    ListOfAppraisalObj.Add(AppObjCard);
                }
            }
            return PartialView("~/Views/Appraisal/CoreCompetenceValues.cshtml", ListOfAppraisalObj);
        }
        public PartialViewResult LoadPImprovementPlan(string AppDoc)
        {
            string StaffNo = Session["Username"].ToString();
            List<ScoreCardObjectives> ListOfAppraisalObj = new List<ScoreCardObjectives>();

            string page = "ApprisalPartList?$filter=DocumentNo eq '" + AppDoc + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    ScoreCardObjectives AppObjCard = new ScoreCardObjectives();
                    AppObjCard.DocumentNo = (string)config["DocumentNo"];
                    AppObjCard.Objective = (string)config["Objective"];
                    AppObjCard.KeyPerformanceIndicator = (string)config["KeyPerformanceIndicator"];
                    AppObjCard.Targets = (string)config["Targets"];
                    AppObjCard.Achievements = (string)config["Achievements"];
                    AppObjCard.Ratings = (string)config["Ratings"];
                    AppObjCard.SupervisorRating = (string)config["SupervisorRating"];
                    ListOfAppraisalObj.Add(AppObjCard);
                }
            }
            return PartialView("~/Views/Appraisal/PerfImprovementPlan.cshtml", ListOfAppraisalObj);
        }
        public PartialViewResult LoadLearningDevelopments(string AppDoc)
        {
            string StaffNo = Session["Username"].ToString();
            List<ScoreCardObjectives> ListOfAppraisalObj = new List<ScoreCardObjectives>();

            string page = "ApprisalPartList?$filter=DocumentNo eq '" + AppDoc + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    ScoreCardObjectives AppObjCard = new ScoreCardObjectives();
                    AppObjCard.DocumentNo = (string)config["DocumentNo"];
                    AppObjCard.Objective = (string)config["Objective"];
                    AppObjCard.KeyPerformanceIndicator = (string)config["KeyPerformanceIndicator"];
                    AppObjCard.Targets = (string)config["Targets"];
                    AppObjCard.Achievements = (string)config["Achievements"];
                    AppObjCard.Ratings = (string)config["Ratings"];
                    AppObjCard.SupervisorRating = (string)config["SupervisorRating"];
                    ListOfAppraisalObj.Add(AppObjCard);
                }
            }
            return PartialView("~/Views/Appraisal/LearningDevmpnt.cshtml", ListOfAppraisalObj);
        }
        public PartialViewResult NewObjectiveLine()
        {
            string StaffNo = Session["Username"].ToString();
            NewApprisalRequest NewAppl = new NewApprisalRequest();

            //#region ApprisalTypes
            //List<AppraisalTypes> AppraisalTyp = new List<AppraisalTypes>();

            //string page = "AppraisalTypes?&format=json";

            //HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            //{
            //    var result = streamReader.ReadToEnd();

            //    var details = JObject.Parse(result);
            //    foreach (JObject config in details["value"])
            //    {
            //        AppraisalTypes AppTyp = new AppraisalTypes();
            //        AppTyp.Code = (string)config["Code"];
            //        AppTyp.Description = (string)config["Description"];
            //        AppraisalTyp.Add(AppTyp);
            //    }
            //}
            //#endregion

            //#region ApprisalPeriod
            //List<AppraisalPeriods> AppPeriod = new List<AppraisalPeriods>();

            //string pageReliever = "ApprisalPeriods?&format=json";

            //HttpWebResponse httpResponseReliever = Credentials.GetOdataData(pageReliever);
            //using (var streamReader = new StreamReader(httpResponseReliever.GetResponseStream()))
            //{
            //    var result = streamReader.ReadToEnd();

            //    var details = JObject.Parse(result);
            //    foreach (JObject config in details["value"])
            //    {
            //        AppraisalPeriods ApPeriod = new AppraisalPeriods();
            //        ApPeriod.Period = (string)config["Period"];
            //        AppPeriod.Add(ApPeriod);
            //    }
            //}
            //#endregion

            //#region Responsibility
            //List<RespCenter> RespCList = new List<RespCenter>();
            //string pageResC = "ResponsibilityCenters?$format=json";

            //HttpWebResponse httpResponseResC = Credentials.GetOdataData(pageResC);
            //using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
            //{
            //    var result = streamReader.ReadToEnd();

            //    var details = JObject.Parse(result);


            //    foreach (JObject config in details["value"])
            //    {
            //        RespCenter RCList = new RespCenter();
            //        RCList.Code = (string)config["Code"];
            //        RCList.Name = (string)config["Name"];
            //        RespCList.Add(RCList);
            //    }
            //}
            //#endregion
            //NewAppl = new NewApprisalRequest
            //{
            //    ListOfApprisalPeriods = AppPeriod.Select(x =>
            //                        new SelectListItem()
            //                        {
            //                            Text = x.Period,
            //                            Value = x.Period
            //                        }).ToList(),
            //    ListOfApprisalTypes = AppraisalTyp.Select(x =>
            //                         new SelectListItem()
            //                         {
            //                             Text = x.Description,
            //                             Value = x.Code
            //                         }).ToList(),
            //    ListOfResponsibility = RespCList.Select(x =>
            //                       new SelectListItem()
            //                       {
            //                           Text = x.Name,
            //                           Value = x.Code
            //                       }).ToList()
            //};
            return PartialView("~/Views/Appraisal/AddAppraisalObjectiveLine.cshtml", NewAppl);
        }
        public JsonResult SubmitAppraisalObjectiveLine(ApprisalObjective AppObjective)
        {
            try
            {
                Credentials.ObjNav.SaveObjectives(AppObjective.DocNo, AppObjective.AppraisalPeriod, AppObjective.Objective,"",
                    AppObjective.Target, Convert.ToInt32(AppObjective.TimeLines), 0, 1, Convert.ToInt32(AppObjective.Section));

                return Json(new { message = "Record Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult NewCoreCompetenceValue()
        {
            string StaffNo = Session["Username"].ToString();
            NewApprisalRequest NewAppl = new NewApprisalRequest();

            return PartialView("~/Views/Appraisal/AddCoreCompetenceValue.cshtml", NewAppl);
        }
        public JsonResult SubmitCoreCompetenceValue(ApprisalObjective AppObjective)
        {
            try
            {
                Credentials.ObjNav.SaveObjectives(AppObjective.DocNo, AppObjective.AppraisalPeriod, AppObjective.Objective,"",
                    AppObjective.Target, Convert.ToInt32(AppObjective.TimeLines), 0, 1, Convert.ToInt32(AppObjective.Section));

                return Json(new { message = "Record Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}