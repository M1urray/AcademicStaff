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

            string page = "HRTrainingApplication?$filter=Employee_No eq '" + StaffNo + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    TrainingList TrList = new TrainingList();
                    TrList.Application_No = (string)config["Application_No"];
                    TrList.Training_Category = (string)config["Training_Category"];
                    TrList.Course_Title = (string)config["Course_Title"];
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
            string Dir = "", Dep = "";
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
                        Dir = (string)config["_x003C_GlobSal_Dimension_1_Code_x003E_"];
                        Dep = (string)config["GlobalDimension2Code"];
                    }
                }
            }
            #endregion
            if (Dir == "")
            {
                Error erroMsg = new Error();
                erroMsg.Message = "Your directorate has not been set. Contact HR";
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
            else if (Dep == "")
            {
                Error erroMsg = new Error();
                erroMsg.Message = "Your department has not been set. Contact HR";
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
            else
            {
                #region Directorate List
                List<DimensionValues> DirectorateList = new List<DimensionValues>();
                string pageDir = "DimensionValues?$filter=Dimension_Code eq 'BRANCH'&$format=json";

                HttpWebResponse httpResponseDepartment = Credentials.GetOdataData(pageDir);
                using (var streamReader = new StreamReader(httpResponseDepartment.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues Directorate = new DimensionValues();
                        Directorate.Code = (string)config["Code"];
                        Directorate.Name = (string)config["Name"];
                        DirectorateList.Add(Directorate);
                    }
                }
                #endregion

                #region Department
                List<DimensionValues> DepartmentList = new List<DimensionValues>();
                string pageDepartment = "DimensionValues?$filter=Dimension_Code eq 'DEPARTMENT'&$format=json";

                HttpWebResponse httpResponseDivision = Credentials.GetOdataData(pageDepartment);
                using (var streamReader = new StreamReader(httpResponseDivision.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues Department = new DimensionValues();
                        Department.Code = (string)config["Code"];
                        Department.Name = (string)config["Name"];
                        DepartmentList.Add(Department);
                    }
                }
                #endregion

                #region Section List
                List<DimensionValues> SectionList = new List<DimensionValues>();
                string pageSection = "DimensionValues?$filter=Dimension_Code eq 'SECTION'&format=json";

                HttpWebResponse httpResponseSection = Credentials.GetOdataData(pageSection);
                using (var streamReader = new StreamReader(httpResponseSection.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues Section = new DimensionValues();
                        Section.Code = (string)config["Code"];
                        Section.Name = (string)config["Name"];
                        SectionList.Add(Section);
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

                #region Courses
                List<DropdownList> CourseList = new List<DropdownList>();
                string pageCourse = "HRCourseList?$filter=Closed eq false&$format=json";

                HttpWebResponse httpResponseCourse = Credentials.GetOdataData(pageCourse);
                using (var streamReader = new StreamReader(httpResponseCourse.GetResponseStream()))
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
                string pageTrainer = "VendorList?$format=json";

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
                    Directorate = Dir,
                    Department = Dep,
                    ListOfDirectorate = DirectorateList.Select(x =>
                                       new SelectListItem()
                                       {
                                           Text = x.Name,
                                           Value = x.Code
                                       }).ToList(),
                    ListOfDepartment = DepartmentList.Select(x =>
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
                string DocNo = Credentials.ObjNav.TrainingRequisitionCreate(Session["username"].ToString(), NewApp.Directorate, NewApp.Department, "",
                    NewApp.Course_Title, "", Convert.ToInt32(NewApp.Training_Category), Convert.ToInt32(NewApp.Sponsor), StartDate, EndDate,
                    NewApp.Trainer, "", Convert.ToDecimal(NewApp.Cost), NewApp.Purpose);

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
                #region Directorate List
                List<DimensionValues> DirectorateList = new List<DimensionValues>();
                string pageDir = "DimensionValues?$filter=Dimension_Code eq 'DIRECTORATES'&$format=json";

                HttpWebResponse httpResponseDepartment = Credentials.GetOdataData(pageDir);
                using (var streamReader = new StreamReader(httpResponseDepartment.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues Directorate = new DimensionValues();
                        Directorate.Code = (string)config["Code"];
                        Directorate.Name = (string)config["Name"];
                        DirectorateList.Add(Directorate);
                    }
                }
                #endregion

                #region Department
                List<DimensionValues> DepartmentList = new List<DimensionValues>();
                string pageDepartment = "DimensionValues?$filter=Dimension_Code eq 'DEPARTMENT'&$format=json";

                HttpWebResponse httpResponseDivision = Credentials.GetOdataData(pageDepartment);
                using (var streamReader = new StreamReader(httpResponseDivision.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues Department = new DimensionValues();
                        Department.Code = (string)config["Code"];
                        Department.Name = (string)config["Name"];
                        DepartmentList.Add(Department);
                    }
                }
                #endregion

                #region Section List
                List<DimensionValues> SectionList = new List<DimensionValues>();
                string pageSection = "DimensionValues?$filter=Dimension_Code eq 'SECTION'&format=json";

                HttpWebResponse httpResponseSection = Credentials.GetOdataData(pageSection);
                using (var streamReader = new StreamReader(httpResponseSection.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues Section = new DimensionValues();
                        Section.Code = (string)config["Code"];
                        Section.Name = (string)config["Name"];
                        SectionList.Add(Section);
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

                #region Courses
                List<DropdownList> CourseList = new List<DropdownList>();
                string pageCourse = "HRCourseList?$filter=Closed eq false&$format=json";

                HttpWebResponse httpResponseCourse = Credentials.GetOdataData(pageCourse);
                using (var streamReader = new StreamReader(httpResponseCourse.GetResponseStream()))
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
                string pageTrainer = "VendorList?$format=json";

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
                string page = "HRTrainingApplication?$filter=Application_No eq '" + AppDoc + "'&$format=json";

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
                        TranDoc.Directorate = (string)config["Global_Dimension_1"];
                        TranDoc.Department = (string)config["Global_Dimension_2"];
                        TranDoc.Trainer = (string)config["Trainer"];
                        TranDoc.Purpose = (string)config["Purpose_of_Training"];
                        TranDoc.Status = (string)config["Status"];
                    }
                }
                TranDoc.ListOfDirectorate = DirectorateList.Select(x =>
                                      new SelectListItem()
                                      {
                                          Text = x.Name,
                                          Value = x.Code
                                      }).ToList();
                TranDoc.ListOfDepartment = DepartmentList.Select(x =>
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
        public JsonResult SendDocAppForApproval(string DocNo, string Redirect)
        {
            try
            {
                Credentials.ObjNav.TrainingRequisitionApprovalRequest(DocNo);
                if (Redirect == "Y")
                {
                    Session["SuccessMsg"] = "Training Requisition, Document No " + DocNo + " send for approval Successfully";
                }
                return Json(new { message = "Training Requisition, Document No " + DocNo + " send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CancelTransportAppForApproval(string DocNo)
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
        [AcceptVerbs(HttpVerbs.Get)]
        public PartialViewResult FileUploadForm()
        {
            return PartialView("~/Views/Training/Partial Views/FileAttachmentForm.cshtml");
        }
    }
}