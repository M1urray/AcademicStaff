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
                    List<Programme> PList = new List<Programme>();

                    string page = "ProgrammeList?$format=json";

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
                    string page = "ProgrammeStages?$filter=Programme_Code eq '" + Prog + "'&$format=json";

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
                    string page = "UnitsSubjects?$filter=Programme_Code eq '" + Prog + "' and Stage_Code eq '" + Stage + "'&$format=json";

                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        foreach (JObject config in details["value"])
                        {
                            StageUnits StgUnit = new StageUnits();
                            StgUnit.Code = (string)config["Code"];
                            StgUnit.Desription = (string)config["Desription"];
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
                #region AcademicYear
                List<AcademicYearList> AcademicYrList = new List<AcademicYearList>();
                string page = "AcademicYear?$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        AcademicYearList AcYrList = new AcademicYearList();
                        AcYrList.Code = (string)config["Code"];
                        AcademicYrList.Add(AcYrList);
                    }
                }
                #endregion

                #region Programme Option List
                List<ProgOptionList> progOpList = new List<ProgOptionList>();
                string pageOption = "ProgrammeOption?$filter=Programme_Code eq '" + Prog + "'&$format=json";

                HttpWebResponse httpResponseOption = Credentials.GetOdataData(pageOption);
                using (var streamReader = new StreamReader(httpResponseOption.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        ProgOptionList OptionList = new ProgOptionList();
                        OptionList.Code = (string)config["Code"];
                        OptionList.Desription = (string)config["Desription"];
                        progOpList.Add(OptionList);
                    }
                }
                #endregion

                #region Campus List
                List<DimensionValues> Campuses = new List<DimensionValues>();
                string pageCampus = "DimensionValues?$filter=Global_Dimension_No_ eq 1&$format=json";

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
                    ListOfAcademicYear = AcademicYrList.Select(x =>
                                         new SelectListItem()
                                         {
                                             Text = x.Code,
                                             Value = x.Code
                                         }).ToList(),
                    ListOfProgrammeOption = progOpList.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Desription,
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
                    Credentials.ObjNav.GenerateConsolidatedMarks(Filters.Prog, Filters.AcademicYear, Filters.Stage, ProgOption, Convert.ToInt32(Filters.DocType), Campus, "CONS MARKSHEET-" + _filename + extn);

                    filename = "CONS MARKSHEET-" + _filename + extn;
                    string fileDestinationPath = Server.MapPath("~/Downloads/");
                    CommonClass.MoveFile(filename, fileDestinationPath);
                    string DestinationPath = Credentials.fileDestinationPath + filename;
                    System.IO.FileInfo file = new System.IO.FileInfo(DestinationPath);
                    if (file.Exists)
                    {
                        if (Filters.DocType == "1")
                        {
                            view = true;
                        }
                        else
                        {
                            view = false;
                        }
                        success = true;
                    }
                    else
                    {
                        success = false;
                        message = "File Not Found";
                    }
                }
                if (Filters.ReportType == "PASSLIST")
                {
                    Credentials.ObjNav.GenerateSenateSummary(Filters.Prog, Filters.AcademicYear, Filters.Stage, ProgOption, Convert.ToInt32(Filters.DocType), "SENATEREPORT-" + _filename + extn);
                    filename = "SENATEREPORT-" + _filename + extn;
                    string fileDestinationPath = Server.MapPath("~/Downloads/");
                    CommonClass.MoveFile(filename, fileDestinationPath);
                    string DestinationPath = Credentials.fileDestinationPath + filename;
                    System.IO.FileInfo file = new System.IO.FileInfo(DestinationPath);
                    if (file.Exists)
                    {
                        if (Filters.DocType == "1")
                        {
                            view = true;
                        }
                        else
                        {
                            view = false;
                        }
                        success = true;
                    }
                    else
                    {
                        success = false;
                        message = "File Not Found";
                    }
                }
                if (Filters.ReportType == "CLASSIFICATION")
                {
                    Credentials.ObjNav.GenerateClassifications(Filters.Prog, Filters.AcademicYear, Filters.Stage, ProgOption, Convert.ToInt32(Filters.DocType), "CLASSIFICATION-" + _filename + extn);
                    filename = "CLASSIFICATION-" + _filename + extn;
                    string fileDestinationPath = Server.MapPath("~/Downloads/");
                    CommonClass.MoveFile(filename, fileDestinationPath);
                    string DestinationPath = Credentials.fileDestinationPath + filename;
                    System.IO.FileInfo file = new System.IO.FileInfo(DestinationPath);
                    if (file.Exists)
                    {
                        if (Filters.DocType == "1")
                        {
                            view = true;
                        }
                        else
                        {
                            view = false;
                        }
                        success = true;
                    }
                    else
                    {
                        success = false;
                        message = "File Not Found";
                    }
                }
                if (Filters.ReportType == "AWARD")
                {
                    Credentials.ObjNav.GenerateAwardList(Filters.Prog, Filters.AcademicYear, Filters.Stage, ProgOption, Convert.ToInt32(Filters.DocType), "AWARD-" + _filename + extn);
                    filename = "AWARD-" + _filename + extn;
                    string fileDestinationPath = Server.MapPath("~/Downloads/");
                    CommonClass.MoveFile(filename, fileDestinationPath);
                    string DestinationPath = Credentials.fileDestinationPath + filename;
                    System.IO.FileInfo file = new System.IO.FileInfo(DestinationPath);
                    if (file.Exists)
                    {
                        if (Filters.DocType == "1")
                        {
                            view = true;
                        }
                        else
                        {
                            view = false;
                        }
                        success = true;
                    }
                    else
                    {
                        success = false;
                        message = "File Not Found";
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
                string fullPath = Credentials.fileDestinationPath + fileName;
                return File(fullPath, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public JsonResult GetUnitReports(string Prog, string Stage, string Unit, string ReportType)
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
                    Credentials.ObjNav.PrintClassList(Prog, Unit, Stage, Sem,"","",1, "CLASSLIST-" + _filename + ".pdf");
                    filename = "CLASSLIST-" + _filename + ".pdf";
                }
                if (ReportType == "EXAMATT")
                {
                    Credentials.ObjNav.GenerateExamAttendanceList(Prog, Unit, Stage, Sem,"","",1, "EXAMATTENDANCE-" + _filename + ".pdf");
                    filename = "EXAMATTENDANCE-" + _filename + ".pdf";
                }
                if (ReportType == "SCORESHEET")
                {
                    //Credentials.ObjNav.GenerateScoreSheet(Prog, Unit, Stage, Sem, "SCORESHEET-" + _filename + ".pdf");
                    filename = "SCORESHEET-" + _filename + ".pdf";
                }
                string fileDestinationPath = Server.MapPath("~/Downloads/");
                CommonClass.MoveFile(filename, fileDestinationPath);
                string DestinationPath = Credentials.fileDestinationPath + filename;
                System.IO.FileInfo file = new System.IO.FileInfo(DestinationPath);
                if (file.Exists)
                {
                    success = true;
                    message = Credentials.fileDownloads + filename;
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
                string DestinationPath = Credentials.fileDestinationPath + filename;
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
    }
}