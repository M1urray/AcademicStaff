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
    public class BackToOfficeController : Controller
    {
        // GET: BackToOffice
        public ActionResult BackToOfficeRequisitionList()
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
        public PartialViewResult BackToOfficeRequisitionListPartialView()
        {
            string StaffNo = Session["Username"].ToString();
            List<BackToOfficeList> BackToOfficeList = new List<BackToOfficeList>();

            string page = "HRBackToOfficeList?$filter=EmployeeNo eq '" + StaffNo + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    BackToOfficeList TrList = new BackToOfficeList();
                    TrList.Application_No = (string)config["Application_No"];
                    TrList.BackToOffice_Category = (string)config["Trainingcategory"];
                    TrList.Course_Title = (string)config["CourseTitle"];
                    TrList.Status = (string)config["Status"];
                    BackToOfficeList.Add(TrList);
                }
            }
            return PartialView("~/Views/BackToOffice/Partial Views/TRListView.cshtml", BackToOfficeList);
        }
        public PartialViewResult NewBackToOfficeApplication()
        {
            string StaffNo = Session["Username"].ToString();
            NewBackToOfficeDocument NewAppl = new NewBackToOfficeDocument();
            string Campus = "", Dep = "";
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
                        Campus= (string)config["Campus"];
                        Dep = (string)config["Department_Code"];
                    }
                }
            }
            #endregion
            if (Campus == "")
            {
                Error erroMsg = new Error();
                erroMsg.Message = "Your campus has not been set. Contact HR";
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
                string pageDir = "DimensionValues?$filter=Dimension_Code eq 'CAMPUS'&$format=json";

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

                //#region Section List
                //List<DimensionValues> SectionList = new List<DimensionValues>();
                //string pageSection = "DimensionValues?$filter=Dimension_Code eq 'SECTION'&format=json";

                //HttpWebResponse httpResponseSection = Credentials.GetOdataData(pageSection);
                //using (var streamReader = new StreamReader(httpResponseSection.GetResponseStream()))
                //{
                //    var result = streamReader.ReadToEnd();

                //    var details = JObject.Parse(result);


                //    foreach (JObject config in details["value"])
                //    {
                //        DimensionValues Section = new DimensionValues();
                //        Section.Code = (string)config["Code"];
                //        Section.Name = (string)config["Name"];
                //        SectionList.Add(Section);
                //    }
                //}
                //#endregion

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

                NewAppl = new NewBackToOfficeDocument
                {
                    Directorate = Campus,
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
                return PartialView("~/Views/BackToOffice/Partial Views/NewBackToOfficeRequisition.cshtml", NewAppl);
            }
        }
        [HttpPost]
        public JsonResult SubmitBackToOfficeDocument(BackToOfficeList NewApp)
        {
            try
            {
                DateTime StartDate = DateTime.ParseExact(NewApp.StartDate.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime EndDate = DateTime.ParseExact(NewApp.EndDate.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string DocNo = "";//Credentials.ObjNav.BackToOfficeRequisitionCreate(Session["username"].ToString(), NewApp.Directorate, NewApp.Department, "",
                //    NewApp.Course_Title, "", Convert.ToInt32(NewApp.BackToOffice_Category), Convert.ToInt32(NewApp.Sponsor), StartDate, EndDate,
                //    NewApp.Trainer, "", Convert.ToDecimal(NewApp.Cost), NewApp.Purpose,"","");

                string Redirect = "/BackToOffice/BackToOfficeDocumentDetails?AppDoc=" + DocNo;

                return Json(new { message = Redirect, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult BackToOfficeDocumentDetails(string AppDoc)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                string StaffNo = Session["Username"].ToString();
                BackToOfficeList TranDoc = new BackToOfficeList();
                #region Directorate List
                List<DimensionValues> DirectorateList = new List<DimensionValues>();
                string pageDir = "DimensionValues?$filter=Dimension_Code eq 'CAMPUS'&$format=json";

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

                //#region Section List
                //List<DimensionValues> SectionList = new List<DimensionValues>();
                //string pageSection = "DimensionValues?$filter=Dimension_Code eq 'SECTION'&format=json";

                //HttpWebResponse httpResponseSection = Credentials.GetOdataData(pageSection);
                //using (var streamReader = new StreamReader(httpResponseSection.GetResponseStream()))
                //{
                //    var result = streamReader.ReadToEnd();

                //    var details = JObject.Parse(result);


                //    foreach (JObject config in details["value"])
                //    {
                //        DimensionValues Section = new DimensionValues();
                //        Section.Code = (string)config["Code"];
                //        Section.Name = (string)config["Name"];
                //        SectionList.Add(Section);
                //    }
                //}
                //#endregion

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
                string page = "HRBackToOfficeForm?$filter=Document_No eq '" + AppDoc + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        TranDoc.Application_No = (string)config["Document_No"];
                        //TranDoc.Application_Date = Convert.ToDateTime((string)config["Application_Date"]).ToString("dd/MM/yyyy");
                        TranDoc.StartDate = Convert.ToDateTime((string)config["From_Date"]).ToString("dd/MM/yyyy");
                        TranDoc.EndDate = Convert.ToDateTime((string)config["To_Date"]).ToString("dd/MM/yyyy");
                        TranDoc.Location = (string)config["Location"];
                        TranDoc.Course_Title = (string)config["Course_Title"];
                        TranDoc.Course_Desc = (string)config["Description"];
                        TranDoc.Directorate = (string)config["Campus"];
                        TranDoc.Department = (string)config["Department"];
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
        public PartialViewResult BackToOfficeParticipants(string DocNo, string Status)
        {
            try
            {
                #region BackToOffice Lines
                List<Trainees> participantList = new List<Trainees>();
                string pageLine = "HRBackToOfficePartcipants?$filter=BackToOfficeCode eq '" + DocNo + "'&$format=json";
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
                return PartialView("~/Views/BackToOffice/Partial Views/ParticipantList.cshtml", Lines);
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
                    ddl.Text = (string)config["First_Name"] + " " + (string)config["Middle_Name"] + " " + (string)config["Last_Name"];
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
            return PartialView("~/Views/BackToOffice/Partial Views/ParticipantForm.cshtml", NewAppl);
        }
        
        public JsonResult SendDocAppForApproval(string DocNo, string Redirect)
        {
            try
            {
                //Credentials.ObjNav.BackToOfficeRequisitionApprovalRequest(DocNo);
                if (Redirect == "Y")
                {
                    Session["SuccessMsg"] = "BackToOffice Requisition, Document No " + DocNo + " send for approval Successfully";
                }
                return Json(new { message = "BackToOffice Requisition, Document No " + DocNo + " send for approval Successfully", success = true }, JsonRequestBehavior.AllowGet);
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
                //Credentials.ObjNav.HRCancelBackToOfficeRequisition(DocNo);
                return Json(new { message = "BackToOffice Requisition approval cancelled Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public PartialViewResult FileUploadForm()
        {
            return PartialView("~/Views/BackToOffice/Partial Views/FileAttachmentForm.cshtml");
        }
    }
}