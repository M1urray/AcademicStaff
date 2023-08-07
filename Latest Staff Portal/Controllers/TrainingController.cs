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
    public class TrainingController : Controller
    {
        // GET: Training
        public ActionResult TrainingRequisitionList()
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
        public PartialViewResult TrainingRequisitionListPartialView()
        {
            string StaffNo = Session["Username"].ToString();
            List<TrainingList> TrainingList = new List<TrainingList>();

            string page = "HRTrainingApplicationCard?$filter=Employee_No eq '" + StaffNo + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    TrainingList TrList = new TrainingList();
                    TrList.ApplicationNo = (string)config["Application_No"];
                    TrList.TrainingCategory = (string)config["Training_Category"];
                    TrList.CourseTitle = (string)config["Course_Title"];
                    TrList.Status = (string)config["Status"];
                    TrainingList.Add(TrList);
                }
            }
            return PartialView("~/Views/Training/Partial Views/TRListView.cshtml", TrainingList);
        }
        public PartialViewResult NewTrainingApplication()
        {
            string StaffNo = Session["Username"].ToString();
            NewTrainingDocument NewAppl = new NewTrainingDocument();
            string Dim1 = "", Dim2 = "";
            #region Employee Data
            string pageData = "EmployeeList?$filter=No eq '" + StaffNo + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(pageData);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);

                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        Dim1 = (string)config["Campus"];
                        Dim2 = (string)config["Department_Code"];
                    }
                }
            }
            #endregion
            if (Dim1 == "")
            {
                Error erroMsg = new Error();
                erroMsg.Message = "Your Station not set. Contact HR";
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
            else if (Dim2 == "")
            {
                Error erroMsg = new Error();
                erroMsg.Message = "Your Department not set. Contact HR";
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
            else
            {
                #region Dim1 List
                List<DimensionValues> Dim1List = new List<DimensionValues>();
                string pageDepartment = "DimensionValues?$filter=Global_Dimension_No_ eq 1&$format=json";

                HttpWebResponse httpResponseDepartment = Credentials.GetOdataData(pageDepartment);
                using (var streamReader = new StreamReader(httpResponseDepartment.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues Department = new DimensionValues();
                        Department.Code = (string)config["Code"];
                        Department.Name = (string)config["Name"];
                        Dim1List.Add(Department);
                    }
                }
                #endregion

                #region dim2
                List<DimensionValues> Dim2List = new List<DimensionValues>();
                string pageDivision = "DimensionValues?$filter=Global_Dimension_No_ eq 2&$format=json";

                HttpWebResponse httpResponseDivision = Credentials.GetOdataData(pageDivision);
                using (var streamReader = new StreamReader(httpResponseDivision.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues DList = new DimensionValues();
                        DList.Code = (string)config["Code"];
                        DList.Name = (string)config["Name"];
                        Dim2List.Add(DList);
                    }
                }
                #endregion

                #region Courses
                List<DropdownList> CourseList = new List<DropdownList>();
                string pageResC = "HRCourseList?$format=json";

                HttpWebResponse httpResponseResC = Credentials.GetOdataData(pageResC);
                using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList CList = new DropdownList();
                        CList.Value = (string)config["Course_Code"];
                        CList.Text = (string)config["Course_Tittle"];
                        CourseList.Add(CList);
                    }
                }
                #endregion
                #region Trainers
                List<DropdownList> TrainerList = new List<DropdownList>();
                string pageTrainer = "HrTrainingProviders?$format=json";

                HttpWebResponse httpResponseTrainer = Credentials.GetOdataData(pageTrainer);
                using (var streamReader = new StreamReader(httpResponseTrainer.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList CList = new DropdownList();
                        CList.Value = (string)config["No"];
                        CList.Text = (string)config["Name"];
                        TrainerList.Add(CList);
                    }
                }
                #endregion

                NewAppl = new NewTrainingDocument
                {
                    Dim1 = Dim1,
                    Dim2 = Dim2,
                    ListOfDim1 = Dim1List.Select(x =>
                                       new SelectListItem()
                                       {
                                           Text = x.Name,
                                           Value = x.Code
                                       }).ToList(),
                    ListOfDim2 = Dim2List.Select(x =>
                                         new SelectListItem()
                                         {
                                             Text = x.Name,
                                             Value = x.Code
                                         }).ToList(),
                    ListOfCourses = CourseList.Select(x =>
                                       new SelectListItem()
                                       {
                                           Text = x.Text,
                                           Value = x.Value
                                       }).ToList(),
                    ListOfTrainers = TrainerList.Select(x =>
                                      new SelectListItem()
                                      {
                                          Text = x.Text,
                                          Value = x.Value
                                      }).ToList()
                };
                return PartialView("~/Views/Training/Partial Views/NewTrainingRequisition.cshtml", NewAppl);
            }
        }
        [HttpPost]
        public JsonResult SubmitTrainingDocument(TrainingList NewApp)
        {
            try
            {
                DateTime StartDate = DateTime.ParseExact(NewApp.StartDate.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime EndDate = DateTime.ParseExact(NewApp.EndDate.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string DocNo = Credentials.ObjNav.TrainingRequisitionCreate(Session["username"].ToString(), NewApp.Dim1, NewApp.Dim2, "",
                                  NewApp.CourseTitle, "", Convert.ToInt32(NewApp.TrainingCategory), Convert.ToInt32(NewApp.Sponsor), StartDate, EndDate,
                                  NewApp.Trainer, "", Convert.ToDecimal(NewApp.Cost), NewApp.Purpose,"","");

                string Redirect = "/Training/TrainingDocumentDetails?AppDoc=" + DocNo;

                return Json(new { message = Redirect, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult TrainingDocumentDetails(string AppDoc)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                string StaffNo = Session["Username"].ToString();
                TrainingList TranDoc = new TrainingList();
                #region Dim1 List
                List<DimensionValues> Dim1List = new List<DimensionValues>();
                string pageDepartment = "DimensionValues?$filter=Global_Dimension_No_ eq 1&$format=json";

                HttpWebResponse httpResponseDepartment = Credentials.GetOdataData(pageDepartment);
                using (var streamReader = new StreamReader(httpResponseDepartment.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues Department = new DimensionValues();
                        Department.Code = (string)config["Code"];
                        Department.Name = (string)config["Name"];
                        Dim1List.Add(Department);
                    }
                }
                #endregion

                #region dim2
                List<DimensionValues> Dim2List = new List<DimensionValues>();
                string pageDivision = "DimensionValues?$filter=Global_Dimension_No_ eq 2&$format=json";

                HttpWebResponse httpResponseDivision = Credentials.GetOdataData(pageDivision);
                using (var streamReader = new StreamReader(httpResponseDivision.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues DList = new DimensionValues();
                        DList.Code = (string)config["Code"];
                        DList.Name = (string)config["Name"];
                        Dim2List.Add(DList);
                    }
                }
                #endregion

                #region Courses
                List<DropdownList> CourseList = new List<DropdownList>();
                string pageResC = "HRCourseList?$format=json";

                HttpWebResponse httpResponseResC = Credentials.GetOdataData(pageResC);
                using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList CList = new DropdownList();
                        CList.Value = (string)config["Course_Code"];
                        CList.Text = (string)config["Course_Tittle"];
                        CourseList.Add(CList);
                    }
                }
                #endregion
                #region Trainers
                List<DropdownList> TrainerList = new List<DropdownList>();
                string pageTrainer = "HrTrainingProviders?$format=json";

                HttpWebResponse httpResponseTrainer = Credentials.GetOdataData(pageTrainer);
                using (var streamReader = new StreamReader(httpResponseTrainer.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList CList = new DropdownList();
                        CList.Value = (string)config["No"];
                        CList.Text = (string)config["Name"];
                        TrainerList.Add(CList);
                    }
                }
                #endregion
                string page = "HRTrainingApplicationCard?$filter=Application_No eq '" + AppDoc + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        TranDoc.ApplicationNo = (string)config["Application_No"];
                        TranDoc.ApplicationDate = Convert.ToDateTime((string)config["Application_Date"]).ToString("dd/MM/yyyy");
                        TranDoc.StartDate = Convert.ToDateTime((string)config["From_Date"]).ToString("dd/MM/yyyy");
                        TranDoc.EndDate = Convert.ToDateTime((string)config["To_Date"]).ToString("dd/MM/yyyy");
                        TranDoc.TrainingCategory = (string)config["Training_Category"];
                        TranDoc.CourseTitle = (string)config["Course_Title"];
                        TranDoc.CourseDesc = (string)config["Description"];
                        TranDoc.Dim1 = (string)config["Campus"];
                        TranDoc.Dim2 = (string)config["Department"];
                        TranDoc.Trainer = (string)config["Trainer"];
                        TranDoc.Purpose = (string)config["Purpose_of_Training"];
                        TranDoc.Status = (string)config["Status"];
                    }
                }
                TranDoc.ListOfDim1 = Dim1List.Select(x =>
                                      new SelectListItem()
                                      {
                                          Text = x.Name,
                                          Value = x.Code
                                      }).ToList();
                TranDoc.ListOfDim2 = Dim2List.Select(x =>
                                         new SelectListItem()
                                         {
                                             Text = x.Name,
                                             Value = x.Code
                                         }).ToList();
                TranDoc.ListOfCourses = CourseList.Select(x =>
                                       new SelectListItem()
                                       {
                                           Text = x.Text,
                                           Value = x.Value
                                       }).ToList();
                TranDoc.ListOfTrainers = TrainerList.Select(x =>
                                      new SelectListItem()
                                      {
                                          Text = x.Text,
                                          Value = x.Value
                                      }).ToList();

                return View(TranDoc);
            }
        }
        public PartialViewResult TrainingParticipants(string DocNo, string Status)
        {
            try
            {
                #region Training Lines
                List<Trainees> participantList = new List<Trainees>();
                string pageLine = "HrTrainingParticipants?$filter=TrainingCode eq '" + DocNo + "'&$format=json";
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
                TraineeList Lines = new TraineeList
                {
                    Status = Status,
                    ListOfTrainees = participantList
                };
                return PartialView("~/Views/Training/Partial Views/ParticipantList.cshtml", Lines);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewParticipantForm()
        {
            string StaffNo = Session["Username"].ToString();
            Trainees NewAppl = new Trainees();
            #region Employee List
            List<DropdownList> EmployeeList = new List<DropdownList>();
            string page = "EmployeeList?$&format=json";

            HttpWebResponse httpResponseCampus = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponseCampus.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);


                foreach (JObject config in details["value"])
                {
                    DropdownList ddl = new DropdownList();
                    ddl.Value = (string)config["No"];
                    ddl.Text = (string)config["FirstName"] + " " + (string)config["MiddleName"] + " " + (string)config["LastName"];
                    EmployeeList.Add(ddl);
                }
            }
            #endregion

            NewAppl.ListOfEmployee = EmployeeList.Select(x =>
                                           new SelectListItem()
                                           {
                                               Text = x.Text,
                                               Value = x.Value
                                           }).ToList();
            return PartialView("~/Views/Training/Partial Views/ParticipantForm.cshtml", NewAppl);
        }
        [HttpPost]
        public JsonResult SubmitTrainingParticipants(string DocNo, string ParticipantNo)
        {
            try
            {
                Credentials.ObjNav.InsertHRTrainingParticipants(DocNo, ParticipantNo);

                return Json(new { message = "Participant added successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RemoveParticipantLine(string DocNo, string Emp)
        {
            try
            {
                Credentials.ObjNav.RemoveHRTrainingParticipants(DocNo, Emp);
                return Json(new { message = "Participant Removed successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SendDocAppForApproval(string DocNo,string Redirect)
        {
            try
            {
                Credentials.ObjNav.TrainingRequisitionApprovalRequest(DocNo);
                if (Redirect == "Y")
                {
                    Session["SuccessMsg"] = "Store Requisition, Document No " + DocNo + " send for approval Successfully";
                }
                return Json(new { message = "Store Requisition, Document No " + DocNo + " send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CancelTrainingAppForApproval(string DocNo)
        {
            try
            {
                Credentials.ObjNav.HRCancelTrainingRequisition(DocNo);
                return Json(new { message = "Training Requisition approval cancelled Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}