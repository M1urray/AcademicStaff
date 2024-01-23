using Latest_Staff_Portal.CustomSecurity;
using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
using Microsoft.Ajax.Utilities;
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
    [CustomAuthorization(Role = "FULLTIME,PARTTIME")]
    public class LecturerController : Controller
    {
        // GET: Lecturer
        public ActionResult LecturerList()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                List<Lecturer> LectList = new List<Lecturer>();
                string page = "EmployeeList?$filter=Lecturer eq true&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        Lecturer newLec = new Lecturer();
                        newLec.No = (string)config["No"];
                        newLec.First_Name = (string)config["First_Name"];
                        newLec.Middle_Name = (string)config["Middle_Name"];
                        newLec.Last_Name = (string)config["Last_Name"];
                        newLec.Cellular_Phone_Number = (string)config["Cellular_Phone_Number"];
                        newLec.Gender = (string)config["Gender"];
                        newLec.Company_E_Mail = (string)config["E_Mail"];
                        LectList.Add(newLec);
                    }
                }
                return View(LectList);
            }
        }
        public ActionResult LecturerUnitAllocations()
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
        public PartialViewResult GetLecturerAssinedUnits(string Lecturer)
        {
            List<LecturerAssignedUnits> LectUnitAllocation = new List<LecturerAssignedUnits>();
            string page = "LectAllocatedUnits?$filter=Lecturer eq '" + Lecturer + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);

                foreach (JObject config in details["value"])
                {
                    LecturerAssignedUnits Lec = new LecturerAssignedUnits();
                    Lec.Code = (string)config["Code"];
                    Lec.Stage = (string)config["Stage"];
                    Lec.Semester = (string)config["Semester"];
                    Lec.Unit = (string)config["Unit"];
                    Lec.Unit_Name = (string)config["Description"];
                    Lec.Campus_Code = (string)config["Campus_Code"];
                    Lec.Name = (string)config["Name"];
                    Lec.Student_Type = (string)config["Student_Type"];
                    Lec.LnNo = (string)config["Line_No"];
                    //Lec.CourseClass = (string)config["Class"];
                    LectUnitAllocation.Add(Lec);
                }
            }

            return PartialView("~/Views/Lecturer/AssignedUnitsList.cshtml", LectUnitAllocation.DistinctBy(x => new { x.Unit, x.Semester, x.Student_Type, x.Campus_Code }).OrderBy(x => x.Code).ToList());
        }
        public ActionResult CourseAllocationUnits()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                string StaffNo = Session["Username"].ToString();
                List<LecturerAssignedUnits> LectUnitAllocation = new List<LecturerAssignedUnits>();
                string Sem = CommonClass.CurrentSemester();
                string page = "LecturerUnitsAllocation?$filter=Lecturer eq '" + StaffNo + "' and Semester eq '" + Sem + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        LecturerAssignedUnits Lec = new LecturerAssignedUnits();
                        Lec.Code = (string)config["Programme"];
                        Lec.Stage = (string)config["Stage"];
                        Lec.Semester = (string)config["Semester"];
                        Lec.Unit = (string)config["Unit"];
                        Lec.Unit_Name = (string)config["Description"];
                        Lec.Campus_Code = (string)config["CampusCode"];
                        Lec.Name = (string)config["CampusCode"];
                        Lec.Student_Type = (string)config["StudentType"];
                        Lec.LnNo = (string)config["Line_No"];
                        //Lec.CourseClass = (string)config["Class"];
                        LectUnitAllocation.Add(Lec);
                    }
                }
                return View(LectUnitAllocation.DistinctBy(x => new { x.Code, x.Stage, x.Campus_Code, x.Unit, x.Semester }).OrderBy(x => x.Code).ToList());
            }
        }
        public ActionResult MarkEntryAllocationUnits()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                string StaffNo = Session["Username"].ToString();
                List<LecturerAssignedUnits> LectUnitAllocation = new List<LecturerAssignedUnits>();
                string Sem = CommonClass.ExamSemester();
                string page = "LecturerUnitsAllocation?$filter=Lecturer eq '" + StaffNo + "' and Semester eq '" + Sem + "' and CampusCode ne ''&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        LecturerAssignedUnits Lec = new LecturerAssignedUnits();
                        Lec.Code = (string)config["Programme"];
                        Lec.Stage = (string)config["Stage"];
                        Lec.Semester = (string)config["Semester"];
                        Lec.Unit = (string)config["Unit"];
                        Lec.Unit_Name = (string)config["Description"];
                        Lec.Campus_Code = (string)config["CampusCode"];
                        Lec.Name = (string)config["CampusCode"];
                        Lec.Student_Type = (string)config["StudentType"];
                        Lec.LnNo = (string)config["Line_No"];
                        //Lec.CourseClass = (string)config["Class"];
                        LectUnitAllocation.Add(Lec);
                    }
                }
                return View(LectUnitAllocation.DistinctBy(x => new { x.Code, x.Stage, x.Campus_Code, x.Unit, x.Semester }).OrderBy(x => x.Code).ToList());
            }
        }
        public JsonResult AssignAssignedUnitSessions(UnitStudentListFilters StudentUnitFilters)
        {
            try
            {
                Session["Prog"] = StudentUnitFilters.Prog.Trim();
                Session["Stage"] = StudentUnitFilters.Stage.Trim();
                Session["Sem"] = StudentUnitFilters.Sem.Trim();
                Session["Unit"] = StudentUnitFilters.Unit.Trim();
                Session["UnitName"] = StudentUnitFilters.UnitName.Trim();
                if (StudentUnitFilters.Campus != null)
                {
                    Session["Campus"] = StudentUnitFilters.Campus.Trim();
                }
                else
                {
                    Session["Campus"] = "";
                }

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult StudentUnitList()
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
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ProgrammeExamSetup(string Prog, string Unit)
        {
            try
            {
                string ProgC = "";
                ProgC = Credentials.ObjNav.GetExamCategory(Unit, Prog);
                if (ProgC != "")
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "Programme exam category not set!", success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult LoadStudentUnitList()
        {
            if (Session["Prog"] != null && Session["Stage"] != null && Session["Sem"] != null && Session["Unit"] != null)
            {
                string Prog = Session["Prog"].ToString();
                string Stage = Session["Stage"].ToString();
                string Sem = Session["Sem"].ToString();
                string Unit = Session["Unit"].ToString();
                string Campus = Session["Campus"].ToString();

                List<CustomerList> studentlist = new List<CustomerList>();
                string page = "";
                if (Campus != null && Campus != "")
                {
                    page = "StudentUnits?$filter=Programme eq '" + Prog + "' and Stage eq '" + Stage + "' and Semester eq '" + Sem + "' and Unit eq '" + Unit + "' and Global_Dimension_1_Code eq '" + Campus + "'&$format=json";
                }
                else
                {
                    page = "StudentUnits?$filter=Programme eq '" + Prog + "' and Stage eq '" + Stage + "' and Semester eq '" + Sem + "' and Unit eq '" + Unit + "'&$format=json";
                }
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        CustomerList Cust = new CustomerList();
                        Cust.No = (string)config["Student_No"];
                        Cust.Name = (string)config["Name"];
                        Cust.Campus = (string)config["Campus_Code"];
                        Cust.GradePrefix = (string)config["Grade_Prefix"];
                        studentlist.Add(Cust);
                    }
                }

                bool PFUnit = Credentials.ObjNav.GetDefaultExamcategoryUnit(Unit);

                if (PFUnit)
                {
                    #region ExamRules
                    List<DropdownList> ExamR = new List<DropdownList>();
                    string pageExamR = "ExamRules?$select=Code,Description&$filter=Block_Online eq false&$format=json";

                    HttpWebResponse httpResponseExamR = Credentials.GetOdataData(pageExamR);
                    using (var streamReader = new StreamReader(httpResponseExamR.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);


                        foreach (JObject config in details["value"])
                        {
                            DropdownList rule = new DropdownList();
                            rule.Value = (string)config["Code"];
                            rule.Text = (string)config["Description"];
                            ExamR.Add(rule);
                        }
                    }
                    #endregion

                    StudentUnitList newList = new StudentUnitList
                    {
                        StudList = studentlist.DistinctBy(x => x.No).ToList(),
                        ListOfExamRules = ExamR.Select(x =>
                                             new SelectListItem()
                                             {
                                                 Text = x.Text,
                                                 Value = x.Value
                                             }).ToList()
                    };
                    return PartialView("~/Views/Lecturer/PassFail.cshtml", newList);
                }
                else
                {
                    return PartialView("~/Views/Lecturer/LoadStudentList.cshtml", studentlist.DistinctBy(x => x.No).ToList());
                }
            }
            else
            {
                return PartialView();
            }
        }
        public JsonResult SaveStudentMarks(List<Array> headers, List<Array> Rows)
        {
            try
            {
                if (Session["Prog"] != null && Session["Stage"] != null && Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null)
                {
                    string Prog = Session["Prog"].ToString();
                    string Stage = Session["Stage"].ToString();
                    string Sem = Session["Sem"].ToString();
                    string Unit = Session["Unit"].ToString();
                    string Campus = Session["Campus"].ToString();

                    bool Assigned = false;

                    string ProgC = "";
                    if (Session["ProgCate"] == null)
                    {
                        ProgC = Credentials.ObjNav.GetExamCategory(Unit, Prog);
                        Session["ProgCate"] = ProgC;
                    }
                    else
                    {
                        ProgC = Session["ProgCate"].ToString();
                    }
                    string[] HeaderText = (string[])headers[0];
                    int ColumnCount = HeaderText.Count();

                    int RowCount = Rows.Count();

                    for (int i = 0; i < RowCount; i++)
                    {
                        string[] RowText = (string[])Rows[i];

                        string studentNo = RowText[1].Trim();
                        for (int j = 3; j < ColumnCount - 1; j++)
                        {
                            string marks = "", examType = "";

                            marks = RowText[j];
                            if (marks != "")
                            {
                                examType = HeaderText[j].Trim();
                                decimal mxmScore = 0, AssinedScore = 0;

                                string page = "ExamSetup?$filter=Category eq '" + ProgC + "' and Code eq '" + HeaderText[j].Trim() + "'&format=json";

                                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                                {
                                    var result = streamReader.ReadToEnd();
                                    var details = JObject.Parse(result);
                                    foreach (JObject config in details["value"])
                                    {
                                        mxmScore = Convert.ToDecimal(config["Max_Score"].ToString());
                                        AssinedScore = Convert.ToDecimal(marks);
                                        if (AssinedScore > mxmScore)
                                        {
                                            return Json(new { message = HeaderText[j].Trim() + " assined score for <b>" + studentNo + "</b> can not be greater than " + mxmScore + ", maximum allowed score", success = true, failed = true }, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            if (examType.Contains("EXAM"))
                                            {
                                                examType = "EXAM";
                                            }
                                            else
                                            {
                                                examType = "CAT";
                                            }

                                            Credentials.ObjNav.EnterRowMarks(
                                            prog: Prog,
                                            stage: Stage,
                                            unit: Unit,
                                            sem: Sem,
                                            score: AssinedScore,
                                            contrib: AssinedScore,
                                            stdNo: studentNo,
                                            examType: examType,
                                            lecturer: Session["username"].ToString(),
                                            entryType: HeaderText[j].Trim()
                                            );
                                            Assigned = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                examType = HeaderText[j].Trim();
                                Credentials.ObjNav.CancelMarkEntry(
                                           studentNo: studentNo,
                                           exam_Code: "",
                                           unit: Unit,
                                           semester: Sem,
                                           entryType: HeaderText[j].Trim()
                                           );
                            }
                        }
                    }
                    if (Assigned)
                    {
                        return Json(new { message = "Marks Submitted  Successfully", success = true, failed = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { message = "No Student with Assined Marks found", success = true, failed = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { message = "", success = true, failed = true, redirect = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetUnitReports(string ReportType)
        {
            try
            {
                string message = "";
                string filename = "";
                bool success = false;
                if (Session["Prog"] != null && Session["Stage"] != null && Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null && Session["SettlementType"] != null)
                {
                    string Prog = Session["Prog"].ToString();
                    string Stage = Session["Stage"].ToString();
                    string Sem = Session["Sem"].ToString();
                    string Unit = Session["Unit"].ToString();
                    string Campus = Session["Campus"].ToString();
                    string ClassCode = "";// Session["ClassCode"].ToString();
                    string SettlementType = Session["SettlementType"].ToString();


                    string _filename = (Prog + "-" + Unit).Replace(@"/", @"");

                    if (ReportType == "CLATT")
                    {
                        Credentials.ObjNav.PrintClassList(Prog, Unit, Stage, Sem, ClassCode, Campus, 1, "CLASSLIST-" + _filename + ".pdf");
                        filename = "CLASSLIST-" + _filename + ".pdf";
                    }
                    if (ReportType == "EXAMATT")
                    {
                        Credentials.ObjNav.GenerateExamAttendanceList("", Unit, Stage, Sem, "", Campus, "EXAMATTENDANCE-" + _filename + ".pdf");
                        filename = "EXAMATTENDANCE-" + _filename + ".pdf";
                    }

                    string DestPath = Server.MapPath("~/Downloads/");
                    CommonClass.MoveFile(filename, DestPath);
                    string DestinationPath = DestPath + filename;
                    CommonClass.MoveFile(filename, DestinationPath);

                    System.IO.FileInfo file = new System.IO.FileInfo(DestinationPath);
                    if (file.Exists)
                    {
                        success = true;
                        message = Credentials.fileDownLoads + filename;
                    }
                    else
                    {
                        success = false;
                        message = "File Not Found";
                    }

                    return Json(new { message = message, success }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "File Not Found", success }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetCommonDropdwnListData(string Categ)
        {
            try
            {
                #region Programme List
                List<Programme> ProgList = new List<Programme>();
                string page = "ProgrammeList?$filter=Category eq '" + Categ + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        Programme PList = new Programme();
                        PList.Code = (string)config["Code"];
                        PList.Description = (string)config["Description"];
                        ProgList.Add(PList);
                    }
                }
                #endregion
                #region Semester List
                List<SemesterList> SemList = new List<SemesterList>();
                string pageSem = "SemesterList?$filter=CurrentSemester eq true&format=json";
                //string pageSem = "SemesterList?&format=json";

                HttpWebResponse httpResponseSem = Credentials.GetOdataData(pageSem);
                using (var streamReader = new StreamReader(httpResponseSem.GetResponseStream()))
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
                #region Campus List
                List<DimensionValues> Campuses = new List<DimensionValues>();
                string pageCampus = "DimensionValues?$select=Code,Name&$filter=Dimension_Code eq 'CAMPUS' and Blocked eq false&$format=json";

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
                #region Settlement Type
                List<DropdownList> SettmntTList = new List<DropdownList>();
                string pageClasses = "SettlementTypes?$select=Code&format=json";

                HttpWebResponse httpResponseClasses = Credentials.GetOdataData(pageClasses);
                using (var streamReader = new StreamReader(httpResponseClasses.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList st = new DropdownList();
                        st.Value = (string)config["Code"];
                        st.Text = (string)config["Code"];
                        SettmntTList.Add(st);
                    }
                }
                #endregion
                UnitAllocationDropdownList DropDownData = new UnitAllocationDropdownList
                {
                    ListOfProgrammes = ProgList.Select(x =>
                                     new SelectListItem()
                                     {
                                         Text = x.Description,
                                         Value = x.Code
                                     }).ToList(),
                    ListOfSemesters = SemList.Select(x =>
                                    new SelectListItem()
                                    {
                                        Text = x.Code,
                                        Value = x.Code
                                    }).ToList(),
                    ListOfCampus = Campuses.Select(x =>
                                       new SelectListItem()
                                       {
                                           Text = x.Name,
                                           Value = x.Code
                                       }).ToList(),
                    ListOfStudyModes = SettmntTList.Select(x =>
                                       new SelectListItem()
                                       {
                                           Text = x.Text,
                                           Value = x.Value
                                       }).ToList()
                };
                return Json(new { DropDownData, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetProgrammeStages(string Prog)
        {
            try
            {
                #region Programme List
                List<PStageList> ProgStageList = new List<PStageList>();
                string page = "ProgrammeStages?$filter=Programme_Code eq '" + Prog + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        PStageList PSList = new PStageList();
                        PSList.Code = (string)config["Code"];
                        PSList.Description = (string)config["Code"];
                        ProgStageList.Add(PSList);
                    }
                }
                #endregion
                ProgStageList PStage = new ProgStageList
                {
                    ListOfProgrammesStages = ProgStageList.Select(x =>
                                     new SelectListItem()
                                     {
                                         Text = x.Description,
                                         Value = x.Code
                                     }).ToList()
                };
                return Json(new { PStage, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetProgrammeStageUnits(string Prog, string Stage)
        {
            try
            {
                #region Programme Stage Units
                List<StageUnits> StageUnitList = new List<StageUnits>();
                string page = "UnitsSubjects?$filter=Programme_Code eq '" + Prog + "' &Stage_Code eq '" + Stage + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        StageUnits SUnitList = new StageUnits();
                        SUnitList.Code = (string)config["Code"];
                        SUnitList.Desription = (string)config["Code"] + "-" + (string)config["Desription"];
                        StageUnitList.Add(SUnitList);
                    }
                }
                #endregion
                ProgStageUnitList PSatgeUnits = new ProgStageUnitList
                {
                    ListOfProgrammesStageUnits = StageUnitList.Select(x =>
                                    new SelectListItem()
                                    {
                                        Text = x.Desription,
                                        Value = x.Code
                                    }).ToList()
                };
                return Json(new { PSatgeUnits, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult AssignLecturerUnit(SubmitLecAssignedUnit UnitDetails)
        {
            try
            {
                Credentials.ObjNav.AssignLecturerUnit(UnitDetails.Lect, UnitDetails.Prog, UnitDetails.Stage, UnitDetails.Sem, UnitDetails.Unit, UnitDetails.Campus, "", "", 0);
                return Json(new { message = "Unit Assigned Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PartTimeRequistionList()
        {
            PartTimeLecturerRegList PartTimeReqList = new PartTimeLecturerRegList();

            #region Parttime Req List
            List<PartTimeLecturerDetails> LecList = new List<PartTimeLecturerDetails>();
            string page = "PartTimeRequisition?$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);


                foreach (JObject config in details["value"])
                {
                    PartTimeLecturerDetails Lec = new PartTimeLecturerDetails();
                    Lec.IDNo = (string)config["ID_Number"];
                    Lec.Surname = (string)config["Surname"];
                    Lec.MiddelName = (string)config["Middle_Name"];
                    Lec.LastName = (string)config["Last_Name"];
                    LecList.Add(Lec);
                }
            }
            #endregion

            PartTimeReqList = new PartTimeLecturerRegList
            {
                ListOfPartTimeRequisition = LecList
            };
            return View(PartTimeReqList);
        }
        public ActionResult NewPartTimeRequistion()
        {
            ParttimeView LectUnitAllocation = new ParttimeView();

            #region Emp List
            List<EmpInitial> EmpInitials = new List<EmpInitial>();
            string pageEmpInitials = "HREmployeeInitial?$format=json";

            HttpWebResponse httpResponseEmpInitials = Credentials.GetOdataData(pageEmpInitials);
            using (var streamReader = new StreamReader(httpResponseEmpInitials.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);


                foreach (JObject config in details["value"])
                {
                    EmpInitial EmpIn = new EmpInitial();
                    EmpIn.Code = (string)config["Code"];
                    EmpInitials.Add(EmpIn);
                }
            }
            #endregion
            #region Department List
            List<DimensionValues> Department = new List<DimensionValues>();
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
            #region School List
            List<DimensionValues> Schools = new List<DimensionValues>();
            string pageSchools = "DimensionValues?$filter=Global_Dimension_No_ eq 3&$format=json";

            HttpWebResponse httpResponseSchools = Credentials.GetOdataData(pageSchools);
            using (var streamReader = new StreamReader(httpResponseSchools.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);


                foreach (JObject config in details["value"])
                {
                    DimensionValues SchlList = new DimensionValues();
                    SchlList.Code = (string)config["Code"];
                    SchlList.Name = (string)config["Name"];
                    Schools.Add(SchlList);
                }
            }
            #endregion

            LectUnitAllocation = new ParttimeView
            {
                ListOfHREmpInitials = EmpInitials.Select(x =>
                                     new SelectListItem()
                                     {
                                         Text = x.Code,
                                         Value = x.Code
                                     }).ToList(),
                ListOfCampus = Campuses.Select(x =>
                                     new SelectListItem()
                                     {
                                         Text = x.Name,
                                         Value = x.Code
                                     }).ToList(),
                ListOfDepartment = Department.Select(x =>
                                      new SelectListItem()
                                      {
                                          Text = x.Name,
                                          Value = x.Code
                                      }).ToList(),
                ListOfSchool = Schools.Select(x =>
                                     new SelectListItem()
                                     {
                                         Text = x.Name,
                                         Value = x.Code
                                     }).ToList()

            };
            return View(LectUnitAllocation);
        }
        public ActionResult PartTimeNewUnit()
        {
            try
            {
                if (Session["Username"] == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    return PartialView("~/Views/Shared/Partial Views/NewPartTimeLecUnit.cshtml");
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        [HttpPost]
        public JsonResult DropAssignedUnit(SubmitLecAssignedUnit UnitD)
        {
            try
            {
                Credentials.ObjNav.DeleteLecturerUnit(UnitD.Lect, UnitD.Prog, UnitD.Stage, UnitD.Sem, UnitD.Unit, UnitD.Campus, "", Convert.ToInt32(UnitD.LnNo));
                return Json(new { message = "Unit Deleted Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult UploadStudentMarks(string base64Upload, string fileName, string Extn)
        {
            bool successVal = false;
            string msg = "";
            if (base64Upload != "")
            {
                if (Session["Prog"] != null && Session["Unit"] != null)
                {
                    string Prog = Session["Prog"].ToString();
                    string Unit = Session["Unit"].ToString();
                    string Stage = Session["Stage"].ToString();
                    string Sem = Session["Sem"].ToString();
                    string Lec = Session["Username"].ToString();

                    string filePath = Server.MapPath(@"/Attachments/" + fileName);

                    bool NotExist = CommonClass.IfFileExists(filePath);

                    bool Uploaded = false;
                    if (NotExist)
                    {
                        Uploaded = CommonClass.SaveUploadedFile(base64Upload, filePath);
                    }
                    if (Uploaded)
                    {
                        string strFilename = filePath;

                        if (string.IsNullOrEmpty(strFilename))
                        {
                            msg = "Upload Students Marks";
                            successVal = false;
                        }
                        else if (Path.GetExtension(strFilename) != ".csv")
                        {
                            msg = "Your file should be a cvs (Comma delimited)";
                            successVal = false;
                        }
                        else
                        {
                            string ProgC = "";
                            if (Session["ProgCate"] == null)
                            {
                                ProgC = Credentials.ObjNav.GetExamCategory(Unit, Prog);
                                Session["ProgCate"] = ProgC;
                            }
                            else
                            {
                                ProgC = Session["ProgCate"].ToString();
                            }

                            string[] s = CommonClass.UploadStudentMarks(Unit, Stage, Sem, Lec, ProgC, strFilename);
                            if (s[1] == "T")
                            {
                                msg = s[0];
                                successVal = true;
                            }
                            else
                            {
                                msg = s[0];
                                successVal = false;
                            }
                        }
                    }
                    else
                    {
                        msg = "Problem Encountered while uploading File";
                        successVal = false;
                    }
                }
            }
            return Json(new { message = msg, success = successVal }, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PrintScoreSheet(string Type)
        {
            bool successVal = false;
            string msg = "";
            try
            {
                if (Session["Prog"] != null && Session["Stage"] != null && Session["Sem"] != null && Session["Unit"] != null)
                {
                    string Lec = Session["Username"].ToString();
                    string Prog = Session["Prog"].ToString();
                    string Stage = Session["Stage"].ToString();
                    string Sem = Session["Sem"].ToString();
                    string Unit = Session["Unit"].ToString();
                    string UnitName = Session["UnitName"].ToString();
                    string Campus = Session["Campus"].ToString();
                    string classCode = "";// Session["ClassCode"].ToString();

                    string rptpath = Server.MapPath("~/Downloads/");
                    string ProgC = "";
                    if (Session["ProgCate"] == null)
                    {
                        ProgC = Credentials.ObjNav.GetExamCategory(Unit, Prog);
                        Session["ProgCate"] = ProgC;
                    }
                    else
                    {
                        ProgC = Session["ProgCate"].ToString();
                    }
                    Error success = CommonClass.StartMarkSheettReport(Lec, Prog, Stage, Sem, Unit, UnitName, Campus, classCode, rptpath);
                    if (success.success)
                    {
                        string DestinationPath = rptpath + success.Message;
                        System.IO.FileInfo file = new System.IO.FileInfo(DestinationPath);
                        if (file.Exists)
                        {
                            successVal = true;
                            msg = Credentials.fileDownLoads + success.Message;
                        }
                        else
                        {
                            successVal = false;
                            msg = "File Not Found";
                        }
                    }
                    else
                    {
                        msg = success.Message;
                        successVal = success.success;
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;

            }
            return Json(new { message = msg, success = successVal }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SavePassFailStudentMarks(List<Array> Rows)
        {
            try
            {
                if (Session["Prog"] != null && Session["Stage"] != null && Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null)
                {
                    string Stage = Session["Stage"].ToString();
                    string Sem = Session["Sem"].ToString();
                    string Unit = Session["Unit"].ToString();


                    int RowCount = Rows.Count();

                    for (int i = 0; i < RowCount; i++)
                    {
                        string[] RowText = (string[])Rows[i];

                        string studentNo = RowText[1].Trim();
                        string GradePrefix = RowText[3].Trim();
                        if (GradePrefix == "")
                        {
                            GradePrefix = "NGR";
                        }
                        Credentials.ObjNav.fnSubmitExamPassFail(studentNo, Sem, Unit, GradePrefix);
                    }
                    return Json(new { message = "Student Grades Submitted successfully", success = true, failed = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "", success = true, failed = true, redirect = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ClassAttendanceList()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                //List<DropdownList> ddlList = new List<DropdownList>();
                //string page = "SettlementTypes?$select=Code,Description&&$format=json";

                //HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                //{
                //    var result = streamReader.ReadToEnd();

                //    var details = JObject.Parse(result);

                //    if (details["value"].Count() > 0)
                //    {
                //        foreach (JObject config in details["value"])
                //        {
                //            DropdownList ddl = new DropdownList();
                //            ddl.Text = (string)config["Description"];
                //            ddl.Value = (string)config["Code"];
                //            ddlList.Add(ddl);
                //        }
                //    }
                //}
                //SettlementList s = new SettlementList
                //{
                //    Code = "",
                //    ListOfSettlements = ddlList.Select(x =>
                //                          new SelectListItem()
                //                          {
                //                              Text = x.Text,
                //                              Value = x.Value
                //                          }).ToList()
                //};
                return View();
            }
        }
        public PartialViewResult LoadClassAttendanceList()
        {
            if (Session["Sem"] != null && Session["Unit"] != null)
            {
                string Lec = Session["username"].ToString();
                string Prog = Session["Prog"].ToString();
                string Stage = Session["Stage"].ToString();
                string Sem = Session["Sem"].ToString();
                string Unit = Session["Unit"].ToString();
                string Campus = Session["Campus"].ToString();
                //string SettlementType = Session["SettlementType"].ToString();

                List<ClassAttendanceEntries> AttendanceList = new List<ClassAttendanceEntries>();
                string page = "ClassAttendanceHeader?$filter=LecturerCode eq '" + Lec + "' and UnitCode eq '" + Unit + "' and SemesterCode eq '"
                    + Sem + "' and CampusCode eq '" + Campus + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        ClassAttendanceEntries Att = new ClassAttendanceEntries();
                        Att.DocNo = (string)config["Code"];
                        Att.Week = (string)config["WeekCode"];
                        //Att.SectionWK = (string)config["Lesson"];
                        Att.Campus = (string)config["CampusCode"];
                        //Att.SettmtT = (string)config["Settlement_Types"];
                        Att.PresentCount = (string)config["Present_Count"];
                        Att.AbsentCount = (string)config["Absent_Count"];
                        Att.Posted = (bool)config["Posted"];
                        AttendanceList.Add(Att);
                    }
                }
                return PartialView("~/Views/Lecturer/LoadAtendanceEntries.cshtml", AttendanceList.OrderBy(x => x.Week).ToList());
            }
            else
            {
                return PartialView();
            }
        }
        public ActionResult LoadClassAtteanceStudents(string DocNo)
        {
            if (Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null && Session["SettlementType"] != null)
            {
                string StaffNo = Session["Username"].ToString();
                string Lec = Session["username"].ToString();
                string Prog = Session["Prog"].ToString();
                string Stage = Session["Stage"].ToString();
                string Sem = Session["Sem"].ToString();
                string Unit = Session["Unit"].ToString();
                string Campus = Session["Campus"].ToString();
                //string SettlementType = Session["SettlementType"].ToString();

                List<CustomerList> studentlist = new List<CustomerList>();
                string page = "";
                if (DocNo != "")
                {
                    page = "ClassAttendanceLines?$filter=Code eq '" + DocNo + "'&$format=json";
                }
                else
                {
                    page = "StudentUnits?$filter=Semester eq '" + Sem + "' and Unit eq '" + Unit + "' and Global_Dimension_1_Code eq '" + Campus + "'&$format=json";
                }
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        CustomerList Cust = new CustomerList();
                        if (DocNo != "")
                        {
                            Cust.No = (string)config["StudentNo"];
                            Cust.Name = (string)config["Names"];
                            string Atte = CommonClass.StudentAttendance(DocNo, (string)config["StudentNo"]);
                            if (Atte == "0")
                            {
                                Cust.Attendance = "2";
                            }
                            else
                            {
                                Cust.Attendance = "1";
                            }
                        }
                        else
                        {
                            Cust.No = (string)config["Student_No"];
                            Cust.Name = (string)config["Name"];
                            Cust.Attendance = "1";
                        }
                        studentlist.Add(Cust);
                    }
                }
                string wk = "", lesson = "";
                List<DropdownList> WeekList = new List<DropdownList>();
                List<DropdownList> dropdownList = new List<DropdownList>();
                if (DocNo != "")
                {
                    #region Week and Day
                    string pageAtt = "ClassAttendanceHeader?$select=WeekCode,Day_Code,Lesson&$filter=Code eq '" + DocNo + "'&$format=json";

                    HttpWebResponse httpResponseAtt = Credentials.GetOdataData(pageAtt);
                    using (var streamReader = new StreamReader(httpResponseAtt.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);

                        foreach (JObject config in details["value"])
                        {
                            wk = (string)config["WeekCode"];
                            lesson = (string)config["Lesson"];
                        }
                    }
                    #endregion
                    #region Weeks List                    
                    string pageWk = "WeeksList?$select=Code&$filter=Inactive eq false&$format=json";

                    HttpWebResponse httpResponseWk = Credentials.GetOdataData(pageWk);
                    using (var streamReader = new StreamReader(httpResponseWk.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);

                        foreach (JObject config in details["value"])
                        {
                            DropdownList ddl = new DropdownList();
                            ddl.Value = (string)config["Code"];
                            ddl.Text = (string)config["Code"];
                            WeekList.Add(ddl);
                        }
                    }
                    #endregion
                    #region Lessons
                    for (int i = 1; i < 6; i++)
                    {
                        DropdownList ddl = new DropdownList();
                        ddl.Value = i.ToString();
                        ddl.Text = "Lesson " + i.ToString();
                        dropdownList.Add(ddl);
                    }
                    #endregion
                }
                else
                {
                    DocNo = "";
                    #region Weeks List
                    string pageWk = "WeeksList?$select=Code&$filter=Inactive eq false&$format=json";

                    HttpWebResponse httpResponseWk = Credentials.GetOdataData(pageWk);
                    using (var streamReader = new StreamReader(httpResponseWk.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);

                        foreach (JObject config in details["value"])
                        {
                            //if (!ClassAttendanceWeekTaken(StaffNo, Sem, Unit, Campus, (string)config["Code"]))
                            //{
                            DropdownList ddl = new DropdownList();
                            ddl.Value = (string)config["Code"];
                            ddl.Text = (string)config["Code"];
                            WeekList.Add(ddl);
                            //}
                        }
                    }
                    #endregion
                }
                ClassAttendance newAtt = new ClassAttendance
                {
                    Week = wk,
                    DocNo = DocNo,
                    StudentList = studentlist.DistinctBy(x => x.No).OrderBy(x => x.No).ToList(),
                    ListOfWeeks = WeekList.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Text,
                                              Value = x.Value
                                          }).ToList()
                };
                return PartialView("~/Views/Lecturer/LoadClassAttendance.cshtml", newAtt);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        protected bool ClassAttendanceWeekTaken(string StaffNo, string Semester, string Unit, string Campus, string Wk, string SettT)
        {
            bool taken = false;
            try
            {
                string pageAtt = "ClassAttendanceHeader?$select=WeekCode&$filter=LecturerCode eq '" + StaffNo + "' and SemesterCode eq '" + Semester + "' and UnitCode eq '" + Unit + "' and CampusCode eq '" + Campus + "' and WeekCode eq '" + Wk + "' and Settlement_Types eq '" + SettT + "'&$format=json";

                HttpWebResponse httpResponseAtt = Credentials.GetOdataData(pageAtt);
                using (var streamReader = new StreamReader(httpResponseAtt.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    if (details["value"].Count() > 0)
                    {
                        taken = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return taken;
        }
        protected bool ClassAttendanceLessonTaken(string StaffNo, string Semester, string Unit, string Campus, string Wk, int Lesson, string SettT)
        {
            bool taken = false;
            try
            {
                string pageAtt = "ClassAttendanceHeader?$select=Lesson&$filter=LecturerCode eq '" + StaffNo + "' and SemesterCode eq '" + Semester + "' and UnitCode eq '"
                    + Unit + "' and CampusCode eq '" + Campus + "' and WeekCode eq '" + Wk + "' and Lesson eq " + Lesson + " and Settlement_Types eq '" + SettT + "'&$format=json";

                HttpWebResponse httpResponseAtt = Credentials.GetOdataData(pageAtt);
                using (var streamReader = new StreamReader(httpResponseAtt.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    if (details["value"].Count() > 0)
                    {
                        taken = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return taken;
        }
        [HttpPost]
        public JsonResult SaveClassAttendance(string DocNo, string Wk, string Lesson, List<Array> Rows)
        {
            try
            {
                if (Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null && Session["SettlementType"] != null)
                {
                    string Lec = Session["username"].ToString();
                    string Prog = Session["Prog"].ToString();
                    string Stage = Session["Stage"].ToString();
                    string Sem = Session["Sem"].ToString();
                    string Unit = Session["Unit"].ToString();
                    string Campus = Session["Campus"].ToString();
                    string SettlMnt = Session["SettlementType"].ToString();

                    //string[] s = Wk.Split('~');
                    //Wk = s[0].Trim();
                    string Code = "", SmT = "";

                    if (SettlMnt != null && SettlMnt != "")
                    {
                        SmT = SettlMnt;
                    }
                    if (DocNo != null && DocNo != "")
                    {
                        Code = DocNo;
                    }
                    else
                    {
                        Code = Credentials.ObjNav.InserClassAtteHeader(Prog, Stage, Unit, Sem, Wk, Lec, Campus, "", "");
                    }
                    int RowCount = Rows.Count();

                    for (int i = 0; i < RowCount; i++)
                    {
                        string[] RowText = (string[])Rows[i];

                        string studentNo = RowText[1].Trim();
                        string Attendance = RowText[3].Trim();

                        Credentials.ObjNav.InsertClassListAttendance(Code, studentNo, Convert.ToInt32(Attendance), "", Unit, Sem, Wk, Lec, Campus);
                    }
                    return Json(new { message = "Class Attendance for week " + Wk + " saved Successfully", success = true, failed = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string Redirect = "/Lecturer/ClassAttendanceList";
                    return Json(new { message = Redirect, success = true, failed = true, redirect = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult PostClassAttendance(string DocNo, string Wk)
        {
            try
            {
                if (Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null)
                {
                    string Lec = Session["username"].ToString();

                    Credentials.ObjNav.PostAttendanceList(DocNo, Lec);

                    return Json(new { message = "Class Attendance for week " + Wk + " Posted Successfully", success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string Redirect = "/Lecturer/ClassAttendanceList";
                    return Json(new { message = Redirect, success = true, failed = true, redirect = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteAttendance(string DocNo)
        {
            try
            {
                string Lec = Session["username"].ToString();
                //Credentials.ObjNav.DeleteAttendance(DocNo, Lec);
                return Json(new { message = "Attendance deleted successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}