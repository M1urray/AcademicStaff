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
    public class ProgrammeController : Controller
    {
        // GET: Programme
        public ActionResult ProgrammeList()
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
                    EmployeeDesignation EmpDes = new EmployeeDesignation();// CommonClass.EmployeeDesignation(StaffNo);

                    string page = "";
                    List<Programme> PList = new List<Programme>();

                    //if (EmpDes.IsHOD)
                    //{
                    //     page = "ProgrammeList?$select=Code,Description&$filter=OldCarriculum eq false and DepartmentCode eq '"+ EmpDes.EmpDepartment + "'&$format=json";
                    //}
                    //if (EmpDes.IsDean)
                    //{
                    //    page = "ProgrammeList?$select=Code,Description&$filter=OldCarriculum eq false and School eq '" + EmpDes.EmpSchool + "'&$format=json";
                    //}
                    //if (EmpDes.IsDirector)
                    //{
                    //    page = "ProgrammeList?$select=Code,Description&$filter=OldCarriculum eq false&$format=json";
                    //}
                    page = "ProgrammeList?$select=Code,Description&$filter=OldCarriculum eq false and Short_Course eq false&$format=json";
                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        foreach (JObject config in details["value"])
                        {
                            Programme ProgList = new Programme();
                            ProgList.Code = (string)config["Code"];
                            ProgList.Description = (string)config["Description"];
                            PList.Add(ProgList);
                        }
                    }
                    return View(PList);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public ActionResult ProgrammeStages(string Prog, string Descr)
        {
            try
            {
                if (Session["Username"] == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    ProgStages ProgStages = new ProgStages();
                    List<PStageList> PStages = new List<PStageList>();
                    string page = "ProgrammeStages?$filter=ProgrammeCode eq '" + Prog + "'&$format=json";

                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);


                        foreach (JObject config in details["value"])
                        {
                            PStageList PStageList = new PStageList();
                            PStageList.Code = (string)config["Code"];
                            PStageList.Description = (string)config["Description"];
                            PStages.Add(PStageList);
                        }
                    }
                    ProgStages = new ProgStages
                    {
                        ProgDescr = Descr,
                        ListOfProgrammeStages = PStages

                    };
                    return View(ProgStages);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public ActionResult ProgStageUnits(string Prog, string Stage)
        {
            try
            {
                if (Session["Username"] == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    List<StageUnits> PStagesUnits = new List<StageUnits>();
                    string page = "UnitsSubjects?$filter=ProgrammeCode eq '" + Prog + "' and StageCode eq '" + Stage + "'&format=json";

                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        foreach (JObject config in details["value"])
                        {
                            StageUnits StgUnit = new StageUnits();
                            StgUnit.Code = (string)config["Code"];
                            StgUnit.Desription = (string)config["Description"];
                            PStagesUnits.Add(StgUnit);
                        }
                    }
                    return View(PStagesUnits);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult GetReportFilters(string Prog, string Stage, string ReportType)
        {
            try
            {
                ViewProgrammeStageDocFilters Filters = new ViewProgrammeStageDocFilters();
                #region Semesters
                List<SemesterList> SemList = new List<SemesterList>();
                string pageSem = "SemesterList?$format=json";

                HttpWebResponse httpResponseSem = Credentials.GetOdataData(pageSem);
                using (var streamReader = new StreamReader(httpResponseSem.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        SemesterList SList = new SemesterList();
                        SList.Code = (string)config["Code"];
                        SList.Name = (string)config["Code"];
                        SemList.Add(SList);
                    }
                }
                #endregion
                #region Study Mode
                List<StudyModes> progOpList = new List<StudyModes>();
                string pageOption = "Programme_Study_Modes?$filter=Code eq '" + Prog + "'&$format=json";

                HttpWebResponse httpResponseOption = Credentials.GetOdataData(pageOption);
                using (var streamReader = new StreamReader(httpResponseOption.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        StudyModes OptionList = new StudyModes();
                        OptionList.Code = (string)config["Code"];
                        OptionList.Description = (string)config["Student_Type"];
                        progOpList.Add(OptionList);
                    }
                }
                #endregion

                #region Campus List
                List<DimensionValues> Campuses = new List<DimensionValues>();
                string pageCampus = "DimensionValues?$filter=Global_Dimension_No_ eq 3 and Blocked eq false&$format=json";

                HttpWebResponse httpResponseCampus = Credentials.GetOdataData(pageCampus);
                using (var streamReader = new StreamReader(httpResponseCampus.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues CmpList = new DimensionValues();
                        CmpList.Code = (string)config["Code"];
                        CmpList.Name = (string)config["Name"];
                        Campuses.Add(CmpList);
                    }
                }
                #endregion
                Filters = new ViewProgrammeStageDocFilters
                {
                    Prog = Prog,
                    Stage = Stage,
                    ReportType = ReportType,
                    ListOfProgrammeOption = progOpList.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Description,
                                              Value = x.Code
                                          }).ToList(),
                    ListOfSemester = SemList.Select(x =>
                        new SelectListItem()
                        {
                            Text = x.Name,
                            Value = x.Code
                        }).ToList(),
                    ListOfCampus = Campuses.Select(x =>
                                           new SelectListItem()
                                           {
                                               Text = x.Name,
                                               Value = x.Code
                                           }).ToList()

                };
                return PartialView("~/Views/Programme/ProgrammeStageDocFilters.cshtml", Filters);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public JsonResult GetProgrammeStageReport(ProgrammeStageDocFilters Filters)
        {
            try
            {
                string extn = "";
                if (Filters.DocType == "1")
                {
                    extn = ".pdf";
                }
                if (Filters.DocType == "2")
                {
                    extn = ".xlsx";
                }
                if (Filters.DocType == "3")
                {
                    extn = ".doc";
                }
                string ProgOption = "", Campus = "";
                if (Filters.ProgrammeOption != null)
                {
                    ProgOption = Filters.ProgrammeOption;
                }
                if (Filters.Campus != null)
                {
                    Campus = Filters.Campus;
                }
                string _filename = (Filters.Prog + "-" + Filters.Stage).Replace(@"/", @"");
                string message = "";
                string filename = "";
                bool success = false, view = false;
                bool iSDeviceMobile = Request.Browser.IsMobileDevice;
                if (Filters.ReportType == "CONSLMSHT")
                {
                    //message =Credentials.ObjNav.GenerateConsolidatedMarks(Filters.Prog, Filters.AcademicYear, Filters.Stage, ProgOption, Convert.ToInt32(Filters.DocType), Campus, "CONS MARKSHEET-" + _filename + extn);
                    string filePath = Server.MapPath("~/Downloads/"); 
                    filename = "CONS MARKSHEET-" + _filename + extn;
                    Credentials.SaveBase64DocumentAttachment(message, filePath + filename);
                    string DestinationPath = filePath + filename;
                    FileInfo file = new FileInfo(DestinationPath);
                    if (file.Exists)
                    {
                        success = true;
                    }
                    else
                    {
                        success = false;
                        message = "File Not Found";
                    }
                    if (success)
                    {
                        message = @"/Downloads/" + filename;
                    }
                }
                if (Filters.ReportType == "PASSLIST")
                {
                    //message=Credentials.ObjNav.GenerateSenateSummary(Filters.Prog, Filters.AcademicYear, Filters.Stage, ProgOption, Convert.ToInt32(Filters.DocType), "SENATEREPORT-" + _filename + extn);
                    string filePath = Server.MapPath("~/Downloads/");
                    filename = "SENATEREPORT-" + _filename + extn;
                    Credentials.SaveBase64DocumentAttachment(message, filePath + filename);
                    string DestinationPath = filePath + filename;
                    FileInfo file = new FileInfo(DestinationPath);
                    if (file.Exists)
                    {
                        success = true;
                    }
                    else
                    {
                        success = false;
                        message = "File Not Found";
                    }
                    if (success)
                    {
                        message = @"/Downloads/" + filename;
                    }
                }
                if (Filters.ReportType == "CLASSIFICATION")
                {
                    //message= Credentials.ObjNav.GenerateClassifications(Filters.Prog, Filters.AcademicYear, Filters.Stage, ProgOption, Convert.ToInt32(Filters.DocType), "CLASSIFICATION-" + _filename + extn);
                    string filePath = Server.MapPath("~/Downloads/");
                    filename = "CLASSIFICATION-" + _filename + extn;
                    Credentials.SaveBase64DocumentAttachment(message, filePath + filename);
                    string DestinationPath = filePath + filename;
                    FileInfo file = new FileInfo(DestinationPath);
                    if (file.Exists)
                    {
                        success = true;
                    }
                    else
                    {
                        success = false;
                        message = "File Not Found";
                    }
                    if (success)
                    {
                        message = @"/Downloads/" + filename;
                    }
                }
                if (Filters.ReportType == "AWARD")
                {
                    //Credentials.ObjNav.GenerateAwardList(Filters.Prog, Filters.AcademicYear, Filters.Stage, ProgOption, Convert.ToInt32(Filters.DocType), "AWARD-" + _filename + extn);
                    string filePath = Server.MapPath("~/Downloads/");
                    filename = "AWARD-" + _filename + extn;
                    Credentials.SaveBase64DocumentAttachment(message, filePath + filename);
                    string DestinationPath = filePath + filename;
                    FileInfo file = new FileInfo(DestinationPath);
                    if (file.Exists)
                    {
                        success = true;
                    }
                    else
                    {
                        success = false;
                        message = "File Not Found";
                    }
                    if (success)
                    {
                        message = @"/Downloads/" + filename;
                    }
                }
                if (success)
                {
                    if (view)
                    {
                        if (iSDeviceMobile)
                        {
                            view = false;
                            message = filename;
                        }
                        else
                        {
                            message = @"/Downloads/" + filename;
                        }
                    }
                    else
                    {
                        message = filename;
                    }
                }
                return Json(new { message = message, success, view }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public virtual ActionResult Download(string fileName)
        {
            try
            {
                string fullPath = "";// Credentials.fileDestinationPath + fileName;
                return File(fullPath, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public JsonResult GetUnitReports(string Prog, string Stage, string Unit, string ClassCode, string Campus, string ReportType)
        {
            try
            {
                string message = "";
                string filename = "";
                bool success = false;

                if (Session["CurrentSem"] == null)
                {
                    Session["CurrentSem"] = CommonClass.CurrentSemester();
                }

                string Sem = Session["CurrentSem"].ToString();

                string _filename = (Prog + "-" + Unit).Replace(@"/", @"");

                if (ReportType == "CLATT")
                {
                    //Credentials.ObjNav.PrintClassList(Prog, Unit, Stage, Sem, ClassCode, Campus,1, "CLASSLIST-" + _filename + ".pdf");
                    filename = "CLASSLIST-" + _filename + ".pdf";
                }
                if (ReportType == "EXAMATT")
                {
                    //Credentials.ObjNav.GenerateExamAttendanceList(Prog, Unit, Stage, Sem, ClassCode, Campus,1, "EXAMATTENDANCE-" + _filename + ".pdf");
                    filename = "EXAMATTENDANCE-" + _filename + ".pdf";
                }
                if (ReportType == "SCORESHEET")
                {
                    //Credentials.ObjNav.GenerateScoreSheet(Prog, Unit, Stage, Sem, "SCORESHEET-" + _filename + ".pdf");
                    filename = "SCORESHEET-" + _filename + ".pdf";
                }
                string fileDestinationPath = Server.MapPath("~/Downloads/");
                CommonClass.MoveFile(filename, fileDestinationPath);
                string DestinationPath = fileDestinationPath + filename;
                System.IO.FileInfo file = new System.IO.FileInfo(DestinationPath);
                if (file.Exists)
                {
                    success = true;
                    message = @"/Downloads/" + filename;
                }
                else
                {
                    success = false;
                    message = "File Not Found";
                }
                return Json(new { message = message, success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult GetScoreSheetReportFilters()
        {
            try
            {
                ViewScoreSheetFilters Filters = new ViewScoreSheetFilters();
                #region Semesters
                List<SemesterList> SemList = new List<SemesterList>();
                string page = "SemesterList?$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        SemesterList SList = new SemesterList();
                        SList.Code = (string)config["Code"];
                        SemList.Add(SList);
                    }
                }
                #endregion
                Filters = new ViewScoreSheetFilters
                {
                    ListOfSemesters = SemList.Select(x =>
                                         new SelectListItem()
                                         {
                                             Text = x.Code,
                                             Value = x.Code
                                         }).ToList()

                };
                return PartialView("~/Views/Programme/ScoreSheetSemesterFilter.cshtml", Filters);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public JsonResult GetScoreSheetReports(string Prog, string Stage, string Unit, string Sem)
        {
            try
            {
                string message = "";
                string filename = "";
                bool success = false;

                string _filename = (Prog + "-" + Unit).Replace(@"/", @"");

                //Credentials.ObjNav.GenerateScoreSheet(Prog, Unit, Stage, Sem, "SCORESHEET-" + _filename + ".pdf");
                filename = "SCORESHEET-" + _filename + ".pdf";
                string fileDestinationPath = Server.MapPath("~/Downloads/");
                CommonClass.MoveFile(filename, fileDestinationPath);
                string DestinationPath = fileDestinationPath + filename;
                System.IO.FileInfo file = new System.IO.FileInfo(DestinationPath);
                if (file.Exists)
                {
                    success = true;
                    message = @"/Downloads/" + filename;
                }
                else
                {
                    success = false;
                    message = "File Not Found";
                }

                return Json(new { message = message, success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ClassStatusList()
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
                    EmployeeDesignation EmpDes = new EmployeeDesignation();// CommonClass.EmployeeDesignation(StaffNo);
                    List<DimensionValues> School = new List<DimensionValues>();
                    //if (EmpDes.IsDirector)
                    //{
                    //    #region School
                    //    string pageSchool = "DimensionValues?$filter=Global_Dimension_No_ eq 3&$format=json";

                    //    HttpWebResponse httpResponseSchool = Credentials.GetOdataData(pageSchool);
                    //    using (var streamReader = new StreamReader(httpResponseSchool.GetResponseStream()))
                    //    {
                    //        var result = streamReader.ReadToEnd();

                    //        var details = JObject.Parse(result);


                    //        foreach (JObject config in details["value"])
                    //        {
                    //            DimensionValues SchoolList = new DimensionValues();
                    //            SchoolList.Code = (string)config["Code"];
                    //            SchoolList.Name = (string)config["Name"];
                    //            School.Add(SchoolList);
                    //        }
                    //    }
                    //    #endregion
                    //}
                    List<DimensionValues> Department = new List<DimensionValues>();
                    //if (EmpDes.IsDean || EmpDes.IsDirector)
                    //{
                    //    #region Department List
                    //    string pageDepartment = "DimensionValues?$filter=Global_Dimension_No_ eq 2&$format=json";

                    //    HttpWebResponse httpResponseDepartment = Credentials.GetOdataData(pageDepartment);
                    //    using (var streamReader = new StreamReader(httpResponseDepartment.GetResponseStream()))
                    //    {
                    //        var result = streamReader.ReadToEnd();

                    //        var details = JObject.Parse(result);


                    //        foreach (JObject config in details["value"])
                    //        {
                    //            DimensionValues DepartmentList = new DimensionValues();
                    //            DepartmentList.Code = (string)config["Code"];
                    //            DepartmentList.Name = (string)config["Name"];
                    //            Department.Add(DepartmentList);
                    //        }
                    //    }
                    //    #endregion
                    //}
                    #region Department List
                    string pageDepartment = "DimensionValues?$filter=Global_Dimension_No_ eq 2&$format=json";

                    HttpWebResponse httpResponseDepartment = Credentials.GetOdataData(pageDepartment);
                    using (var streamReader = new StreamReader(httpResponseDepartment.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);


                        foreach (JObject config in details["value"])
                        {
                            DimensionValues DepartmentList = new DimensionValues();
                            DepartmentList.Code = (string)config["Code"];
                            DepartmentList.Name = (string)config["Name"];
                            Department.Add(DepartmentList);
                        }
                    }
                    #endregion
                    ClassStatus newD = new ClassStatus
                    {
                        Employee_Desig = EmpDes,
                        Code = "",
                        ListOfDepartment = Department.Select(x =>
                                             new SelectListItem()
                                             {
                                                 Text = x.Name,
                                                 Value = x.Code
                                             }).ToList(),
                        ListOfSchool = School.Select(x =>
                                               new SelectListItem()
                                               {
                                                   Text = x.Name,
                                                   Value = x.Code
                                               }).ToList()

                    };

                    return View(newD);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult GetClassStatusList(EmployeeDesignation EmpDes)
        {
            try
            {
                string Sem = CommonClass.CurrentSemester();
                List<TimeTableView> timeTable = new List<TimeTableView>();
                string pageTimetable = "";

                if (EmpDes.EmpSchool == null)
                {
                    EmpDes.EmpSchool = "";
                }
                if (EmpDes.EmpDepartment == null)
                {
                    EmpDes.EmpDepartment = "";
                }

                //if (EmpDes.IsHOD)
                //{
                //    pageTimetable = "Timetable?$filter=Unit_Department eq '" + EmpDes.EmpDepartment + "' and Semester eq '" + Sem + "'&$format=json";
                //}
                //if (EmpDes.IsDean)
                //{
                //    if (EmpDes.EmpDepartment != "")
                //    {
                //        pageTimetable = "Timetable?$filter=Unit_School eq '" + EmpDes.EmpSchool + "' and Unit_Department eq '" + EmpDes.EmpDepartment + "' and Semester eq '" + Sem + "'&$format=json";
                //    }
                //    else
                //    {
                //        pageTimetable = "Timetable?$filter=Unit_School eq '" + EmpDes.EmpSchool + "' and Semester eq '" + Sem + "'&$format=json";
                //    }
                //}
                //if (EmpDes.IsDirector)
                //{
                //    if (EmpDes.EmpDepartment != "" && EmpDes.EmpSchool == "")
                //    {
                //        pageTimetable = "Timetable?$filter=Unit_Department eq '" + EmpDes.EmpDepartment + "' and Semester eq '" + Sem + "'&$format=json";
                //    }
                //    else if (EmpDes.EmpDepartment == "" && EmpDes.EmpSchool != "")
                //    {
                //        pageTimetable = "Timetable?$filter=Unit_School eq '" + EmpDes.EmpSchool + "' and Semester eq '" + Sem + "'&$format=json";
                //    }
                //    else if (EmpDes.EmpDepartment != "" && EmpDes.EmpSchool != "")
                //    {
                //        pageTimetable = "Timetable?$filter=Unit_School eq '" + EmpDes.EmpSchool + "' and Unit_Department eq '" + EmpDes.EmpDepartment + "' and Semester eq '" + Sem + "'&$format=json";
                //    }
                //    else
                //    {
                //        pageTimetable = "Timetable?$filter=Semester eq '" + Sem + "'&$format=json";
                //    }
                //}
                pageTimetable = "Timetable?$filter=Semester eq '" + Sem + "'&$format=json";
                HttpWebResponse httpResponseTimeTable = Credentials.GetOdataData(pageTimetable);
                using (var streamReaderTimeTable = new StreamReader(httpResponseTimeTable.GetResponseStream()))
                {
                    var resultTimeTable = streamReaderTimeTable.ReadToEnd();

                    var detailsTimeTable = JObject.Parse(resultTimeTable);

                    if (detailsTimeTable["value"].Count() > 0)
                    {
                        foreach (JObject config1 in detailsTimeTable["value"])
                        {
                            TimeTableView tmTable = new TimeTableView();
                            tmTable.Unit = (string)config1["Unit"];
                            tmTable.Description = (string)config1["Unit_Description"];
                            tmTable.Period = (string)config1["Period"];
                            tmTable.Semester = (string)config1["Semester"];
                            tmTable.Day_of_Week = (string)config1["DayofWeek"];
                            tmTable.Lecture_Room = (string)config1["Lecture_Room"];
                            tmTable.Lecturer = (string)config1["Lecturer_Name"];
                            tmTable.Campus = (string)config1["Campus_Code"];
                            tmTable.Campus = (string)config1["Campus_Code"];
                            tmTable.Section = (string)config1["Unit_Class"];
                            tmTable.Registered = (string)config1["Students_Count"];
                            timeTable.Add(tmTable);
                        }
                    }
                }

                return PartialView("~/Views/Programme/Partial View/LoadClassStatusList.cshtml", timeTable.OrderByDescending(x => x.Unit));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
    }
}