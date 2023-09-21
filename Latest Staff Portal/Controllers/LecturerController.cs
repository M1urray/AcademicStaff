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
using System.Web.Mvc;

namespace Latest_Staff_Portal.Controllers
{
    [CustomeAuthentication]
    [CustomAuthorization(Role = "ALLUSERS")]
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
                string StaffNo = Session["Username"].ToString();
                EmployeeDesignation EmpDes = CommonClass.EmployeeDesignation(StaffNo);

                string page = "";
                if (EmpDes.IsHOD)
                {
                    page = "EmployeeList?$filter=Lecturer eq true and Department_Code eq '" + EmpDes.EmpDepartment + "'&format=json";
                }
                if (EmpDes.IsDean)
                {
                    page = "EmployeeList?$filter=Lecturer eq true and Schools eq '" + EmpDes.EmpSchool + "'&format=json";
                }
                if (EmpDes.IsDirector)
                {
                    page = "EmployeeList?$filter=Lecturer eq true&format=json";
                }

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
            string page = "LectAllocatedUnits?$filter=Lecturer eq '" + Lecturer + "' and Class ne ''&format=json";

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
                    Lec.Unit_Name = (string)config["Unit_Name"];
                    Lec.Campus_Code = (string)config["Campus_Code"];
                    Lec.Name = (string)config["Name"];
                    Lec.Student_Type = (string)config["Student_Type"];
                    Lec.CourseClass = (string)config["Class"];
                    LectUnitAllocation.Add(Lec);
                }
            }

            return PartialView("~/Views/Lecturer/AssignedUnitsList.cshtml", LectUnitAllocation.DistinctBy(x => new { x.Unit, x.Semester, x.CourseClass, x.Campus_Code }).ToList());
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
                List<LecturerAssignedUnits> LectAssociateUnits = new List<LecturerAssignedUnits>();
                string Sem = CommonClass.CurrentSemester();
                string page = "Timetable?$filter=Lecturer eq '" + StaffNo + "' and Semester eq '" + Sem + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        LecturerAssignedUnits Lec = new LecturerAssignedUnits();
                        //Lec.Code = (string)config["Code"];
                        //Lec.Stage = (string)config["Stage"];
                        Lec.Semester = (string)config["Semester"];
                        Lec.Unit = (string)config["Unit"];
                        Lec.Unit_Name = (string)config["Unit_Description"];
                        Lec.Campus_Code = (string)config["Campus_Code"];
                        Lec.Day = (string)config["DayofWeek"];
                        Lec.Period = (string)config["Period"];
                        Lec.Room = (string)config["Lecture_Room"];
                        Lec.CourseClass = (string)config["Unit_Class"];
                        Lec.stdCount = (string)config["Students_Count"];
                        Session["ClassCode"] = (string)config["Unit_Class"];
                        LectUnitAllocation.Add(Lec);
                    }
                }
                string pageLec = "UnitAssociateLecturers?$filter=Lecturer eq '" + StaffNo + "' and Semester eq '" + Sem + "'&$format=json";

                HttpWebResponse httpResponseLec = Credentials.GetOdataData(pageLec);
                using (var streamReader = new StreamReader(httpResponseLec.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        string pageT = "Timetable?$filter=Semester eq '" + Sem + "' and Unit eq '" + (string)config["Unit"] + "' and Campus_Code eq '" + (string)config["Campus_Code"] + "' and Unit_Class eq '" + (string)config["Unit_Class"] + "'&$format=json";

                        HttpWebResponse httpResponseT = Credentials.GetOdataData(pageT);
                        using (var streamReaderT = new StreamReader(httpResponseT.GetResponseStream()))
                        {
                            var resultT = streamReaderT.ReadToEnd();

                            var detailsT = JObject.Parse(resultT);

                            foreach (JObject configT in detailsT["value"])
                            {
                                LecturerAssignedUnits Lec = new LecturerAssignedUnits();
                                Lec.Semester = (string)configT["Semester"];
                                Lec.Unit = (string)configT["Unit"];
                                Lec.Unit_Name = (string)configT["Unit_Description"];
                                Lec.Campus_Code = (string)configT["Campus_Code"];
                                Lec.Day = (string)configT["DayofWeek"];
                                Lec.Period = (string)configT["Period"];
                                Lec.Room = (string)configT["Lecture_Room"];
                                Lec.CourseClass = (string)configT["Unit_Class"];
                                Lec.stdCount = (string)configT["Students_Count"];
                                LectAssociateUnits.Add(Lec);
                            }
                        }
                    }
                }

                LecAssignedUnits LecUnits = new LecAssignedUnits
                {
                    Code = "",
                    ListOfAssignedUnits = LectUnitAllocation.DistinctBy(x => new { x.Unit, x.Semester, x.CourseClass, x.Campus_Code }).ToList(),
                    ListOfAssociateUnits = LectAssociateUnits.DistinctBy(x => new { x.Unit, x.Semester, x.CourseClass, x.Campus_Code }).ToList()
                };
                return View(LecUnits);
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
                List<LecturerAssignedUnits> LectAssociateUnits = new List<LecturerAssignedUnits>();
                string Sem = CommonClass.ExamSemester();
                string page = "Timetable?$filter=Lecturer eq '" + StaffNo + "' and Semester eq '" + Sem + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        LecturerAssignedUnits Lec = new LecturerAssignedUnits();
                        //Lec.Code = (string)config["Code"];
                        //Lec.Stage = (string)config["Stage"];
                        Lec.Semester = (string)config["Semester"];
                        Lec.Unit = (string)config["Unit"];
                        Lec.Unit_Name = (string)config["Unit_Description"];
                        Lec.Campus_Code = (string)config["Campus_Code"];
                        Lec.Day = (string)config["DayofWeek"];
                        Lec.Period = (string)config["Period"];
                        Lec.Room = (string)config["Lecture_Room"];
                        Lec.CourseClass = (string)config["Unit_Class"];
                        Lec.stdCount = (string)config["Students_Count"];
                        LectUnitAllocation.Add(Lec);
                    }
                }
                string pageLec = "UnitAssociateLecturers?$filter=Lecturer eq '" + StaffNo + "' and Semester eq '" + Sem + "'&$format=json";

                HttpWebResponse httpResponseLec = Credentials.GetOdataData(pageLec);
                using (var streamReader = new StreamReader(httpResponseLec.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        string pageT = "Timetable?$filter=Semester eq '" + Sem + "' and Unit eq '" + (string)config["Unit"] + "' and Campus_Code eq '" + (string)config["Campus_Code"] + "' and Unit_Class eq '" + (string)config["Unit_Class"] + "'&$format=json";

                        HttpWebResponse httpResponseT = Credentials.GetOdataData(pageT);
                        using (var streamReaderT = new StreamReader(httpResponseT.GetResponseStream()))
                        {
                            var resultT = streamReaderT.ReadToEnd();

                            var detailsT = JObject.Parse(resultT);

                            foreach (JObject configT in detailsT["value"])
                            {
                                LecturerAssignedUnits Lec = new LecturerAssignedUnits();
                                Lec.Semester = (string)configT["Semester"];
                                Lec.Unit = (string)configT["Unit"];
                                Lec.Unit_Name = (string)configT["Unit_Description"];
                                Lec.Campus_Code = (string)configT["Campus_Code"];
                                Lec.Day = (string)configT["DayofWeek"];
                                Lec.Period = (string)configT["Period"];
                                Lec.Room = (string)configT["Lecture_Room"];
                                Lec.CourseClass = (string)configT["Unit_Class"];
                                Lec.stdCount = (string)configT["Students_Count"];
                                LectAssociateUnits.Add(Lec);
                            }
                        }
                    }
                }

                LecAssignedUnits LecUnits = new LecAssignedUnits
                {
                    Code = "",
                    ListOfAssignedUnits = LectUnitAllocation.DistinctBy(x => new { x.Unit, x.Semester, x.CourseClass, x.Campus_Code }).ToList(),
                    ListOfAssociateUnits = LectAssociateUnits.DistinctBy(x => new { x.Unit, x.Semester, x.CourseClass, x.Campus_Code }).ToList()
                };
                return View(LecUnits);
            }
        }
        public ActionResult CourseAllocationLinks()
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
        public JsonResult AssignAssignedUnitSessions(UnitStudentListFilters StudentUnitFilters)
        {
            try
            {
                if (StudentUnitFilters.Prog != null)
                {
                    Session["Prog"] = StudentUnitFilters.Prog.Trim();
                }
                if (StudentUnitFilters.Stage != null)
                {
                    Session["Stage"] = StudentUnitFilters.Stage.Trim();
                }
                if (StudentUnitFilters.Sem != null)
                {
                    Session["Sem"] = StudentUnitFilters.Sem.Trim();
                }
                if (StudentUnitFilters.Unit != null)
                {
                    Session["Unit"] = StudentUnitFilters.Unit.Trim();
                }
                if (StudentUnitFilters.UnitName != null)
                {
                    Session["UnitName"] = StudentUnitFilters.UnitName.Trim();
                }
                if (StudentUnitFilters.Campus != null)
                {
                    Session["Campus"] = StudentUnitFilters.Campus.Trim();
                }
                if (StudentUnitFilters.ClassCode != null)
                {
                    Session["ClassCode"] = StudentUnitFilters.ClassCode.Trim();
                }
                Session["IsLecAss"] = StudentUnitFilters.IsLecAssociate;

                return Json(new { message = "", success = true }, JsonRequestBehavior.AllowGet);
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
        public JsonResult ProgrammeExamSetup()
        {
            try
            {
                string Lec = Session["Username"].ToString();

                string Sem = Session["Sem"].ToString();
                string Unit = Session["Unit"].ToString();
                string classCode = Session["ClassCode"].ToString();
                bool IsAss = (bool)Session["IsLecAss"];

                bool succV = false;
                string msg = "";

                List<ExamSetupEntry> ExamEntrySetuplist = new List<ExamSetupEntry>();

                string page = "";
                if (IsAss)
                {
                    page = "ExamEntrySetup?$filter=Unit eq '" + Unit + "' and Semester eq '" + Sem + "' and Deleted eq false&$format=json";
                }
                else
                {
                    page = "ExamEntrySetup?$filter=Lecturer eq '" + Lec + "' and Unit eq '" + Unit + "' and Semester eq '" + Sem + "' and Deleted eq false&$format=json";
                }

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    if (details["value"].Count() > 0)
                    {
                        succV = true;
                    }
                    else
                    {
                        succV = false;
                        msg = "You have not set up exam entry for this unit!";
                    }
                }

                return Json(new { message = msg, success = succV }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult LoadStudentUnitList()
        {
            if (Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null || Session["ClassCode"] != null)
            {
                // string Prog = Session["Prog"].ToString();
                //string Stage = Session["Stage"].ToString();
                string Sem = Session["Sem"].ToString();
                string Unit = Session["Unit"].ToString();
                string Campus = Session["Campus"].ToString();
                string classCode = "";
                bool IsAss = (bool)Session["IsLecAss"];

                List<CustomerList> studentlist = new List<CustomerList>();
                //string page = "StudentUnits?$filter=Programme eq '" + Prog + "' and Stage eq '" + Stage + "' and Semester eq '" + Sem + "' and Unit eq '" + Unit + "' and Global_Dimension_1_Code eq '" + Campus + "'&$format=json";
                //string page = "StudentUnits?$filter=Programme eq '" + Prog + "' and Stage eq '" + Stage + "' and Semester eq '" + Sem + "' and Unit eq '" + Unit + "' and Class_Code eq '" + classCode + "' and Class_Code ne ''&format=json";
                string page = "";//
                if (Session["ClassCode"].ToString() == "")
                {
                    page = "StudentUnits?$filter=Semester eq '" + Sem + "' and Unit eq '" + Unit + "' and Campus eq '" + Campus + "'&$format=json";
                }
                else
                {
                    classCode = Session["ClassCode"].ToString();
                    page = "StudentUnits?$filter=Semester eq '" + Sem + "' and Unit eq '" + Unit + "' and Unit_Class_Code eq '" + classCode + "' and Campus eq '" + Campus + "'&$format=json";
                }
                               
                //string page = "StudentUnits?$filter=Semester eq '" + Sem + "' and Unit eq '" + Unit + "'&$format=json";

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
                        studentlist.Add(Cust);
                    }
                }
                return PartialView("~/Views/Lecturer/LoadStudentList.cshtml", studentlist.OrderBy(x => x.No).DistinctBy(x => x.No).ToList());
            }
            else
            {
                return PartialView();
            }
        }
        public ActionResult SpecialExamList()
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
        public PartialViewResult LoadSpecialExamStudentUnitList()
        {
            if (Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null || Session["ClassCode"] != null)
            {
                string StaffNo = Session["Username"].ToString();
                string Sem = Session["Sem"].ToString();
                string Unit = Session["Unit"].ToString();
                string Campus = Session["Campus"].ToString();
                //string classCode = Session["ClassCode"].ToString();

                List<CustomerList> studentlist = new List<CustomerList>();
                string page = "StudentRequisitionLines?$filter=Semester eq '" + Sem + "' and Assigned_Lecture eq '" + StaffNo + "' and Unit_Code eq '" + Unit + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        CustomerList Cust = new CustomerList();
                        Cust.No = (string)config["Student_No"];
                        Cust.Name = (string)config["Student_Name"];
                        Cust.ApplicationNo = (string)config["Application_No"];
                        Cust.Marks = (string)config["Marks_Score"];
                        studentlist.Add(Cust);
                    }
                }
                return PartialView("~/Views/Lecturer/LoadSpecialExamStudentList.cshtml", studentlist.OrderBy(x => x.No).DistinctBy(x => x.No).ToList());
            }
            else
            {
                return PartialView();
            }
        }
        [HttpPost]
        public JsonResult SaveStudentMarks(List<Array> headers, List<Array> Rows)
        {
            try
            {
                if (Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null)
                {
                    string Lec = Session["username"].ToString();
                    //string Prog = Session["Prog"].ToString();
                    //string Stage = Session["Stage"].ToString();
                    string Sem = Session["Sem"].ToString();
                    string Unit = Session["Unit"].ToString();
                    string Campus = Session["Campus"].ToString();
                    //string classCode = Session["ClassCode"].ToString();

                    bool Assigned = false;

                    //string ProgC = "";
                    //if (Session["ProgCate"] == null)
                    //{
                    //    ProgC = Credentials.ObjNav.GetExamCategory(Unit, Prog);
                    //    Session["ProgCate"] = ProgC;
                    //}
                    //else
                    //{
                    //    ProgC = Session["ProgCate"].ToString();
                    //}
                    string[] HeaderText = (string[])headers[0];
                    int ColumnCount = HeaderText.Count();

                    int RowCount = Rows.Count();

                    for (int i = 0; i < RowCount; i++)
                    {
                        string[] RowText = (string[])Rows[i];

                        string studentNo = RowText[1].Trim();
                        for (int j = 3; j < ColumnCount; j++)
                        {
                            string marks = "", examType = "";

                            marks = RowText[j];
                            if (marks != "")
                            {
                                examType = HeaderText[j].Trim();
                                decimal mxmScore = 0, weight = 0, AssinedScore = 0, Contrb = 0; ;

                                string page = "ExamEntrySetup?$filter=Lecturer eq '" + Lec + "' and Unit eq '" + Unit + "' and Semester eq '"
                                    + Sem + "'  and Entry_Code eq '" + HeaderText[j].Trim() + "' and Deleted eq false&$format=json";

                                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                                {
                                    var result = streamReader.ReadToEnd();
                                    var details = JObject.Parse(result);
                                    foreach (JObject config in details["value"])
                                    {
                                        mxmScore = Convert.ToDecimal(config["Max_Score"].ToString());
                                        weight = Convert.ToDecimal(config["Contribution"].ToString());
                                        AssinedScore = Convert.ToDecimal(marks);
                                        if (AssinedScore > mxmScore)
                                        {
                                            return Json(new { message = HeaderText[j].Trim() + " assined score for <b>" + studentNo + "</b> can not be greater than " + mxmScore + ", maximum allowed score", success = true, failed = true }, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            if (AssinedScore > 0)
                                            {
                                                if ((string)config["No_of_Times"] == "Once")
                                                {
                                                    examType = "EXAM";
                                                }
                                                else
                                                {
                                                    examType = "CAT";
                                                }

                                                Contrb = (AssinedScore / mxmScore) * weight;

                                                Credentials.ObjNav.EnterRowMarks(
                                                prog: "",
                                                stage: "",
                                                unit: Unit,
                                                sem: Sem,
                                                score: AssinedScore,
                                                contrib: Contrb,
                                                stdNo: studentNo,
                                                examType: examType,
                                                lecturer: Lec,
                                                entryType: HeaderText[j].Trim()
                                                );
                                                Assigned = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (Assigned)
                    {
                        return Json(new { message = "Unit Assigned Successfully", success = true, failed = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { message = "No Student with Assined Marks found", success = true, failed = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    string Redirect = "/Lecturer/CourseAllocationUnits";
                    return Json(new { message = Redirect, success = true, failed = true, redirect = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SaveSpecialExamMarks(List<Array> Rows)
        {
            try
            {
                if (Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null)
                {
                    string Lec = Session["username"].ToString();
                    string Sem = Session["Sem"].ToString();
                    string Unit = Session["Unit"].ToString();
                    string Campus = Session["Campus"].ToString();
                    //string classCode = Session["ClassCode"].ToString();

                    int RowCount = Rows.Count();
                    string msg = "";
                    bool SuccV = false;
                    int count = 0;
                    for (int i = 0; i < RowCount; i++)
                    {
                        string[] RowText = (string[])Rows[i];

                        string studentNo = RowText[1].Trim();
                        string Marks = RowText[3].Trim();
                        string AppNo = RowText[4].Trim();
                        if (Marks != "")
                        {
                            if (Convert.ToDecimal(Marks) > 100)
                            {
                                msg = "Assigned score for <b>" + studentNo + "</b> can not be greater than 100%, maximum allowed score";
                                SuccV = false;
                                break;
                            }
                            else
                            {
                                Credentials.ObjNav.InsertSpecialExamMark(AppNo, Unit, Lec, Convert.ToDecimal(Marks));
                                SuccV = true;
                                count = count + 1;
                                msg = "Special exam mark for " + count.ToString() + " saved successfully";
                            }
                        }
                    }
                    return Json(new { message = msg, success = SuccV }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string Redirect = "/Lecturer/CourseAllocationUnits";
                    return Json(new { message = Redirect, success = true, failed = true, redirect = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetUnitReports(string ReportType, string RType)
        {
            try
            {
                string message = "";
                string filename = "";
                bool success = false;
                if (Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null)
                {
                    //string Prog = Session["Prog"].ToString();
                    //string Stage = Session["Stage"].ToString();
                    string Sem = Session["Sem"].ToString();
                    string Unit = Session["Unit"].ToString();
                    string Campus = Session["Campus"].ToString();
                    string ClassCode = "";
                    
                    string extn = "";

                    if (RType == "1")
                    {
                        extn = ".pdf";
                    }
                    if (RType == "2")
                    {
                        extn = ".xlsx";
                    }
                    if (RType == "3")
                    {
                        extn = ".doc";
                    }
                    string _filename = Unit.Replace(@"/", @"");
                    if (Session["ClassCode"].ToString() == "")
                    {
                        if (ReportType == "CLATT")
                        {
                            Credentials.ObjNav.PrintClassList("", Unit, "", Sem, "", Campus, Convert.ToInt32(RType), "CLASSLIST-" + _filename + extn);
                            filename = "CLASSLIST-" + _filename + extn;
                        }
                        if (ReportType == "EXAMATT")
                        {
                            Credentials.ObjNav.GenerateExamAttendanceList("", Unit, "", Sem,"", Campus, Convert.ToInt32(RType), "EXAMATTENDANCE-" + _filename + extn);
                            filename = "EXAMATTENDANCE-" + _filename + extn;

                        }
                    }
                    else
                    {
                        ClassCode = Session["ClassCode"].ToString();
                        if (ReportType == "CLATT")
                        {
                            Credentials.ObjNav.PrintClassList("", Unit, "", Sem, ClassCode, Campus, Convert.ToInt32(RType), "CLASSLIST-" + _filename + extn);
                            filename = "CLASSLIST-" + _filename + extn;
                        }
                        if (ReportType == "EXAMATT")
                        {
                            Credentials.ObjNav.GenerateExamAttendanceList("", Unit, "", Sem, ClassCode, Campus, Convert.ToInt32(RType), "EXAMATTENDANCE-" + _filename + extn);
                            filename = "EXAMATTENDANCE-" + _filename + extn;

                        }
                    }
                    string fileDestinationPath = "";
                    if (RType != "1")
                    {
                        fileDestinationPath = Server.MapPath("~/Uploads/");
                    }
                    else
                    {
                        fileDestinationPath = Server.MapPath("~/Downloads/");
                    }
                    CommonClass.MoveFile(filename, fileDestinationPath);
                    System.IO.FileInfo file = new System.IO.FileInfo(fileDestinationPath + filename);
                    if (file.Exists)
                    {
                        success = true;
                        if (RType != "1")
                        {
                            message = filename;
                        }
                        else
                        {
                            message = @"/Downloads/" + filename;
                        }
                    }
                    else
                    {
                        success = false;
                        message = "No students registered";
                    }

                    return Json(new { message = message, success }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "No students registered", success }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = "Problem Encountered while reading report. Contact ICT", success = false }, JsonRequestBehavior.AllowGet);
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
                //string pageSem = "SemesterList?$filter=CurrentSemester eq true&format=json";
                string pageSem = "SemesterList?&format=json";

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
                #region Course Classes
                List<DropdownList> CourseClasses = new List<DropdownList>();
                string pageClasses = "CourseClasses?$select=Code&format=json";

                HttpWebResponse httpResponseClasses = Credentials.GetOdataData(pageClasses);
                using (var streamReader = new StreamReader(httpResponseClasses.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList ClassList = new DropdownList();
                        ClassList.Value = (string)config["Code"];
                        ClassList.Text = (string)config["Code"];
                        CourseClasses.Add(ClassList);
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
                    ListOfCourseClasses = CourseClasses.Select(x =>
                                       new SelectListItem()
                                       {
                                           Text = x.Text,
                                           Value = x.Value
                                       }).ToList()
                };
                return Json(new { DropDownData, success = false }, JsonRequestBehavior.AllowGet);
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
                        PSList.Description = (string)config["Description"];
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
                return Json(PStage, JsonRequestBehavior.AllowGet);
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
                string page = "UnitSubject?$filter=ProgrammeCode eq '" + Prog + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        StageUnits SUnitList = new StageUnits();
                        SUnitList.Code = (string)config["Code"];
                        SUnitList.Desription = (string)config["Desription"];
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
                return Json(PSatgeUnits, JsonRequestBehavior.AllowGet);
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
                Credentials.ObjNav.AssignLecturerUnit(UnitDetails.Lect, UnitDetails.Prog, UnitDetails.Stage, UnitDetails.Sem, UnitDetails.Unit, UnitDetails.Campus, "", UnitDetails.CourseClass, 0, false);
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
                ex.Data.Clear();
                return PartialView();
            }
        }
        [HttpPost]
        public JsonResult DropAssignedUnit(SubmitLecAssignedUnit UnitDetails)
        {
            try
            {
                //Credentials.ObjNav.DeleteLecturerUnit(UnitDetails.Lect, UnitDetails.Prog, UnitDetails.Stage, UnitDetails.Sem, UnitDetails.Unit, UnitDetails.Campus, "");
                return Json(new { message = "Unit Assigned Successfully", success = true }, JsonRequestBehavior.AllowGet);
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
                if (Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null)
                {
                    string Lec = Session["username"].ToString();
                    string Sem = Session["Sem"].ToString();
                    string Unit = Session["Unit"].ToString();
                    string Campus = Session["Campus"].ToString();
                    //string classCode = Session["ClassCode"].ToString();

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
                            string[] s = CommonClass.UploadStudentMarks(Unit, Sem, Lec, Campus, "", strFilename);
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
        public JsonResult PrintScoreSheet()
        {
            bool successVal = false;
            string msg = "";
            try
            {
                if (Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null)
                {
                    string Lec = Session["Username"].ToString();
                    //string Prog = Session["Prog"].ToString();
                    //string Stage = Session["Stage"].ToString();
                    string Sem = Session["Sem"].ToString();
                    string Unit = Session["Unit"].ToString();
                    string UnitName = Session["UnitName"].ToString();
                    string Campus = Session["Campus"].ToString();
                    string classCode = Session["ClassCode"].ToString();
                    bool IsAss = (bool)Session["IsLecAss"];

                    if (IsAss)
                    {
                        Credentials.ObjNav.ConfirmExamEntrySetup("", Sem, Unit, "");
                    }
                    else
                    {
                        Credentials.ObjNav.ConfirmExamEntrySetup(Lec, Sem, Unit, "");
                    }
                    string rptpath = Server.MapPath("~/Downloads/");
                    string ImagePath = Server.MapPath("~/assets/images");

                    Error success = CommonClass.StartMarkSheettReport(Lec, "", Sem, Unit, UnitName, Campus, classCode, ImagePath, rptpath);
                    if (success.success)
                    {
                        string DestinationPath = rptpath + success.Message;
                        System.IO.FileInfo file = new System.IO.FileInfo(DestinationPath);
                        if (file.Exists)
                        {
                            successVal = true;
                            msg = @"/Downloads/" + success.Message;
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
                successVal = false;
            }
            return Json(new { message = msg, success = successVal }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PartTimeClaimRequisitionList()
        {
            try
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
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult PartTimeClaimRequisitionListPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<StaffClaimList> ClaimList = new List<StaffClaimList>();

                string page = "StaffClaimList?$filter=Employee_No eq '" + StaffNo + "' and Payment_Type eq 'Parttime Claim'&format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        StaffClaimList CList = new StaffClaimList();
                        CList.No = (string)config["No"];
                        CList.ReqDate = Convert.ToDateTime((string)config["Date"]).ToString("dd/MM/yyyy");
                        CList.Purpose = (string)config["Purpose"];
                        CList.RespC = (string)config["Responsibility_Center"];
                        CList.Status = (string)config["Status"];
                        ClaimList.Add(CList);
                    }
                }
                return PartialView("~/Views/Lecturer/PartTimeClaimReqListView.cshtml", ClaimList.OrderByDescending(x => x.No));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [HttpPost]
        public JsonResult NewPartTimeClaimRequisition()
        {
            try
            {
                string LecNo = Session["Username"].ToString();
                //string Sem = CommonClass.CurrentSemester();
                string DocNo = "";//Credentials.ObjNav.GenerateParttimeClaim(LecNo, Sem);
                Credentials.ObjNav.StaffClaimRequisitionApprovalRequest(DocNo);
                return Json(new { message = "Part-Time Claim Document No :" + DocNo + " created successfully and send for Appproval. Do you want to view the details?", DocNo = DocNo, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PartTimeClaimDocumentView(string DocNo)
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
                    #region Staff Claim Header
                    StaffClaimHeader ClaimDoc = new StaffClaimHeader();

                    string page = "StaffClaimCard?$filter=No eq '" + DocNo + "'&$format=json";
                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        foreach (JObject config in details["value"])
                        {
                            ClaimDoc.No = (string)config["No"];
                            ClaimDoc.DateRequested = Convert.ToDateTime((string)config["Date"]).ToString("dd/MM/yyyy");
                            ClaimDoc.Remarks = (string)config["Purpose"];
                            ClaimDoc.school = (string)config["Shortcut_Dimension_3_Code"];
                            ClaimDoc.schoolName = (string)config["Dim3"];
                            ClaimDoc.Campus = (string)config["Global_Dimension_1_Code"];
                            ClaimDoc.CampusName = (string)config["Function_Name"];
                            ClaimDoc.Department = (string)config["Shortcut_Dimension_2_Code"];
                            ClaimDoc.DepartmentName = (string)config["Budget_Center_Name"];
                            ClaimDoc.RespC = (string)config["Responsibility_Center"];
                            ClaimDoc.TotalAmount = Convert.ToDecimal((string)config["Total_Net_Amount"]).ToString("#,##0.00");
                            ClaimDoc.Status = (string)config["Status"];
                        }
                    }
                    #endregion
                    return View(ClaimDoc);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ExamEntrySetupExist()
        {
            bool successVal = false;
            string msg = "";
            try
            {
                if (Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null)
                {
                    string Lec = Session["Username"].ToString();
                    //string Prog = Session["Prog"].ToString();
                    //string Stage = Session["Stage"].ToString();
                    string Sem = Session["Sem"].ToString();
                    string Unit = Session["Unit"].ToString();
                    //string classCode = Session["ClassCode"].ToString();

                    List<EmpInitial> EmpInitials = new List<EmpInitial>();
                    string pageEmpInitials = "ExamEntrySetup?$filter=Lecturer eq '" + Lec + "' and Unit eq '" + Unit + "' and Semester eq '" + Sem + "'&$format=json";

                    HttpWebResponse httpResponseEmpInitials = Credentials.GetOdataData(pageEmpInitials);
                    using (var streamReader = new StreamReader(httpResponseEmpInitials.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);

                        if (details["value"].Count() > 0)
                        {
                            successVal = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { message = msg, success = successVal }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExamEntrySetupForm()
        {
            try
            {
                if (Session["Username"] == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    string Lec = Session["Username"].ToString();
                    string Sem = Session["Sem"].ToString();
                    string Unit = Session["Unit"].ToString();
                    //string classCode = Session["ClassCode"].ToString();
                    bool IsAss = (bool)Session["IsLecAss"];

                    List<DropdownList> ddlExamCodeList = new List<DropdownList>();
                    if (!IsAss)
                    {
                        #region Exam Codes List
                        string page = "ExamTypeCodes?$format=json";
                        HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);

                            foreach (JObject config in details["value"])
                            {
                                DropdownList ddl = new DropdownList();
                                ddl.Value = (string)config["Code"];
                                ddl.Text = (string)config["Code"];
                                ddlExamCodeList.Add(ddl);
                            }
                        }
                        #endregion
                    }
                    #region Exam setup Entries
                    List<ExamSetupEntry> ExamEntrySetuplist = new List<ExamSetupEntry>();

                    string pageS = "";
                    if (IsAss)
                    {
                        pageS = "ExamEntrySetup?$filter=Unit eq '" + Unit + "' and Semester eq '"
                           + Sem + "' and Deleted eq false&$format=json";
                    }
                    else
                    {
                        pageS = "ExamEntrySetup?$filter=Lecturer eq '" + Lec + "' and Unit eq '" + Unit + "' and Semester eq '"
                           + Sem + "' and Deleted eq false&$format=json";
                    }

                    HttpWebResponse httpResponseS = Credentials.GetOdataData(pageS);
                    using (var streamReader = new StreamReader(httpResponseS.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);

                        if (details["value"].Count() > 0)
                        {
                            foreach (JObject config in details["value"])
                            {
                                ExamSetupEntry Entry = new ExamSetupEntry();
                                Entry.EntryType = (string)config["Exam_Type"];
                                Entry.Type = (string)config["Entry_Code"];
                                Entry.Description = (string)config["Description"];
                                Entry.MaxScore = (string)config["Max_Score"];
                                Entry.Contr = (string)config["Contribution"];
                                Entry.Order = (string)config["Order"];
                                Entry.EntryNo = (string)config["Entry_No"];
                                ExamEntrySetuplist.Add(Entry);
                            }
                        }
                    }
                    #endregion
                    MySetupEntry newList = new MySetupEntry
                    {
                        Type = "",
                        IsAssLec = IsAss,
                        ListOfExamCodes = ddlExamCodeList.Select(x =>
                                          new SelectListItem()
                                          {
                                              Text = x.Text,
                                              Value = x.Value
                                          }).ToList(),
                        ListOfExamEntry = ExamEntrySetuplist.OrderBy(x => x.EntryType).ThenBy(x => x.Order).ToList()
                    };
                    return PartialView("~/Views/Lecturer/Exam Entry Setup/NewExamEntrySetup.cshtml", newList);
                    //return PartialView("~/Views/Lecturer/Exam Entry Setup/NewExamEntrySetupForm.cshtml");
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitExamSetupEntry(List<Array> Rows, bool Insert)
        {
            bool successVal = false;
            string Msg = "";
            try
            {
                string LecNo = Session["Username"].ToString();
                //string Prog = Session["Prog"].ToString();
                string Sem = Session["Sem"].ToString();
                string Unit = Session["Unit"].ToString();
                string ClassCode = "";

                int RowCount = Rows.Count();
                decimal Totalweight = 0;
                for (int i = 0; i < RowCount; i++)
                {
                    string[] RowText = (string[])Rows[i];
                    if (RowText[1] != null && RowText[2] != null && RowText[1] != "" && RowText[2] != "")
                    {
                        Totalweight = Totalweight + Convert.ToDecimal(RowText[2].Trim());
                    }
                }
                if (Totalweight != 100)
                {
                    Msg = "Incorrect setup. Enter maximum score for every weighted score and ensure that your total weight is 100%. Confirm the setup";
                    successVal = false;
                }
                else
                {
                    int count = 1;
                    int exNo1 = 1, exNo2 = 1;
                    for (int i = 0; i < RowCount; i++)
                    {
                        string[] RowText = (string[])Rows[i];

                        string Type = RowText[0].Trim();
                        string Extype = "";

                        decimal maxScore = 0, weight = 0;

                        maxScore = Convert.ToDecimal(RowText[1].Trim());
                        weight = Convert.ToDecimal(RowText[2].Trim());

                        if (Type.Contains("ASS"))
                        {
                            Type = "Assignment";
                            Extype = "ASS" + exNo1;
                            exNo1++;
                        }
                        if (Type.Contains("CAT"))
                        {
                            Type = "CAT";
                            Extype = "CAT" + exNo2;
                            exNo2++;
                        }
                        if (Type.Contains("EXAM"))
                        {
                            Type = "EXAM";
                            Extype = "EXAM";
                        }
                        if (Insert)
                        {
                            //Credentials.ObjNav.InsertExamSetupEntry(Type,"", maxScore, weight, LecNo, Sem, Unit, ClassCode);
                            Msg = "SetUp Submitted Successfully";
                        }
                        else
                        {
                            //Credentials.ObjNav.ModifyExamSetupEntry(Type, Extype, maxScore, weight, LecNo, Sem, Unit, ClassCode, count);
                            Msg = "SetUp Modified Successfully";
                        }
                        count++;
                    }
                    successVal = true;
                }
            }
            catch (Exception ex)
            {

                if (successVal)
                {
                    Session["ErrorMsg"] = ex.Message.Replace("'", "");
                }
                Msg = ex.Message.Replace("'", "");
            }
            return Json(new { message = Msg, success = successVal }, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult InsertExamSetuEnry(string ExamType, string Description)
        {
            bool successVal = false;
            string Msg = "";
            try
            {
                string LecNo = Session["Username"].ToString();
                //string Prog = Session["Prog"].ToString();
                string Sem = Session["Sem"].ToString();
                string Unit = Session["Unit"].ToString();
                string ClassCode = "";

                Credentials.ObjNav.InsertLecExamSetupEntry(ExamType, LecNo, Sem, Unit, ClassCode, Description);
                successVal = true;
                Msg = "SetUp Submitted Successfully";
            }
            catch (Exception ex)
            {
                successVal = false;
                Msg = ex.Message.Replace("'", "");
            }
            return Json(new { message = Msg, success = successVal }, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ModifyExamSetupEntry(List<Array> Rows, bool Insert)
        {
            bool successVal = false;
            string Msg = "";
            try
            {
                string LecNo = Session["Username"].ToString();
                //string Prog = Session["Prog"].ToString();
                string Sem = Session["Sem"].ToString();
                string Unit = Session["Unit"].ToString();
                //string ClassCode = Session["ClassCode"].ToString();

                int RowCount = Rows.Count();
                decimal Totalweight = 0;
                for (int i = 0; i < RowCount; i++)
                {
                    string[] RowText = (string[])Rows[i];
                    if (RowText[4] != null && RowText[4] != null && RowText[4] != "" && RowText[4] != "")
                    {
                        Totalweight = Totalweight + Convert.ToDecimal(RowText[4].Trim());
                    }
                }
                if (Totalweight > 100)
                {
                    Msg = "Incorrect setup. Enter weighted score and ensure that your total weighted score does not exceed 100%. Confirm the setup";
                    successVal = false;
                }
                else
                {
                    int count = 1;
                    for (int i = 0; i < RowCount; i++)
                    {
                        string[] RowText = (string[])Rows[i];

                        string ExamT = RowText[0].Trim();
                        string Type = RowText[1].Trim();
                        //string Extype = "";

                        decimal maxScore = 0, weight = 0;
                        int EntryNo = 0;

                        maxScore = Convert.ToDecimal(RowText[3].Trim());
                        weight = Convert.ToDecimal(RowText[4].Trim());
                        weight = Convert.ToDecimal(RowText[4].Trim());
                        EntryNo = Convert.ToInt32(RowText[5].Trim());

                        Credentials.ObjNav.ModifyExamSetupEntry(ExamT, Type, maxScore, weight, LecNo, Sem, Unit, "", EntryNo);
                        Msg = "Entry Setup Modified Successfully";
                        count++;
                    }
                    successVal = true;
                }
            }
            catch (Exception ex)
            {
                Msg = ex.Message.Replace("'", "");
                successVal = false;
            }
            return Json(new { message = Msg, success = successVal }, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteExamSetupEntry(string ExamT, string EntryNo)
        {
            bool successVal = false;
            string Msg = "";
            try
            {
                string LecNo = Session["Username"].ToString();
                //string Prog = Session["Prog"].ToString();
                string Sem = Session["Sem"].ToString();
                string Unit = Session["Unit"].ToString();
                //string ClassCode = Session["ClassCode"].ToString();

                Credentials.ObjNav.DeleteExamSetupEntry(ExamT, LecNo, Sem, Unit, "", Convert.ToInt32(EntryNo));
                Msg = "SetUp Submitted Successfully";
                successVal = true;
            }
            catch (Exception ex)
            {

                if (successVal)
                {
                    Session["ErrorMsg"] = ex.Message.Replace("'", "");
                }
                Msg = ex.Message.Replace("'", "");
            }
            return Json(new { message = Msg, success = successVal }, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult ExamEntrySetupLines()
        {
            string Lec = Session["Username"].ToString();
            string Sem = Session["Sem"].ToString();
            string Unit = Session["Unit"].ToString();
            //string classCode = Session["ClassCode"].ToString();

            List<ExamSetupEntry> ExamEntrySetuplist = new List<ExamSetupEntry>();
            string page = "ExamEntrySetup?$filter=Lecturer eq '" + Lec + "' and Unit eq '" + Unit + "' and Semester eq '" + Sem + "' and Deleted eq false&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);

                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        ExamSetupEntry Entry = new ExamSetupEntry();
                        Entry.Type = (string)config["Entry_Code"];
                        Entry.MaxScore = (string)config["Max_Score"];
                        Entry.Contr = (string)config["Contribution"];
                        ExamEntrySetuplist.Add(Entry);
                    }
                }
            }
            return PartialView("~/Views/Lecturer/Exam Entry Setup/ExamEntrySetupLines.cshtml", ExamEntrySetuplist);
        }
        public ActionResult ClassAttendanceList(string Section)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                bool IsAssociateLec = (bool)Session["IsLecAss"];

                IsLecAssociate IsAss = new IsLecAssociate();
                IsAss.IsAssociateLec = IsAssociateLec;
                IsAss.Section = Section;
                return View(IsAss);
            }
        }
        public PartialViewResult LoadClassAttendanceList()
        {
            if (Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null)
            {
                string Lec = Session["username"].ToString();
                //string Prog = Session["Prog"].ToString();
                //string Stage = Session["Stage"].ToString();
                string Sem = Session["Sem"].ToString();
                string Unit = Session["Unit"].ToString();
                string Campus = Session["Campus"].ToString();
                //string classCode = Session["ClassCode"].ToString();
                bool IsAss = (bool)Session["IsLecAss"];

                List<ClassAttendanceEntries> AttendanceList = new List<ClassAttendanceEntries>();
                string page = "";
                if (IsAss)
                {
                    page = "ClassAttendanceHeader?$filter=UnitCode eq '" + Unit + "' and SemesterCode eq '"
                       + Sem + "' and CampusCode eq '" + Campus + "'&$format=json";
                }
                else
                {
                    page = "ClassAttendanceHeader?$filter=LecturerCode eq '" + Lec + "' and UnitCode eq '" + Unit + "' and SemesterCode eq '"
                       + Sem + "' and CampusCode eq '" + Campus + "'&$format=json";
                }
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
                        Att.Campus = (string)config["CampusCode"];
                        Att.PresentCount = (string)config["Present_Count"];
                        Att.AbsentCount = (string)config["Absent_Count"];
                        Att.Posted = (bool)config["Posted"];
                        Att.IsAss = IsAss;
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
        public PartialViewResult LoadClassAttendanceStudents(string DocNo)
        {
            if (Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null)
            {
                string Lec = Session["Username"].ToString();
                string Sem = Session["Sem"].ToString();
                string Unit = Session["Unit"].ToString();
                string Campus = Session["Campus"].ToString();
                //string classCode = Session["ClassCode"].ToString();


                List<CustomerList> studentlist = new List<CustomerList>();

                string page = "StudentUnits?$filter=Semester eq '" + Sem + "' and Unit eq '" + Unit + "' and Campus eq '" + Campus + "'&$format=json";

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
                        if (DocNo != "")
                        {
                            string Atte = CommonClass.StudentAttendance(DocNo, (string)config["Student_No"]);
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
                            Cust.Attendance = "1";
                        }
                        studentlist.Add(Cust);
                    }
                }
                List<DropdownList> WeekList = new List<DropdownList>();
                string wk = "", Code = "";
                if (DocNo != "")
                {
                    #region Week and Day
                    string pageAtt = "ClassAttendanceHeader?$select=WeekCode,Day_Code&$filter=Code eq '" + DocNo + "'&$format=json";

                    HttpWebResponse httpResponseAtt = Credentials.GetOdataData(pageAtt);
                    using (var streamReader = new StreamReader(httpResponseAtt.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);

                        foreach (JObject config in details["value"])
                        {
                            wk = (string)config["WeekCode"] + "~" + (string)config["Day_Code"];
                            Code = (string)config["WeekCode"];
                        }
                    }
                    #endregion
                    #region Weeks List
                    string pageTmTSub = "Timetable?$select=DayofWeek&$filter=Unit eq '" + Unit + "' and Semester eq '" + Sem +
                    "' and Campus_Code eq '" + Campus + "' and Lecturer eq '" + Lec + "' &$format=json";

                    HttpWebResponse httpResponseTmTSub = Credentials.GetOdataData(pageTmTSub);
                    using (var streamReaderTmT = new StreamReader(httpResponseTmTSub.GetResponseStream()))
                    {
                        var resultTmT = streamReaderTmT.ReadToEnd();

                        var detailsTmT = JObject.Parse(resultTmT);

                        foreach (JObject config1 in detailsTmT["value"])
                        {
                            string tday = DateTime.UtcNow.ToString("o");
                            string pageWk = "WeeksList?$select=Code,Start_Date&$filter=Code eq '" + Code + "' and Day eq '" + (string)config1["DayofWeek"] +
                                "' and Inactive eq false and Start_Date le " + tday + "&$format=json";

                            HttpWebResponse httpResponseWk = Credentials.GetOdataData(pageWk);
                            using (var streamReader = new StreamReader(httpResponseWk.GetResponseStream()))
                            {
                                var result = streamReader.ReadToEnd();

                                var details = JObject.Parse(result);

                                foreach (JObject config in details["value"])
                                {
                                    if (ClassAttendanceWeekTaken(Lec, Sem, Unit, Campus, (string)config["Code"]))
                                    {
                                        DropdownList ddl = new DropdownList();
                                        ddl.Value = (string)config["Code"] + "~" + (string)config1["DayofWeek"];
                                        ddl.Text = (string)config["Code"] + "-" + (string)config1["DayofWeek"] + "-" + ((DateTime)config["Start_Date"]).ToString("dd/MM/yyyy");
                                        WeekList.Add(ddl);
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Weeks List                
                    string pageTmT = "Timetable?$select=DayofWeek&$filter=Unit eq '" + Unit + "' and Semester eq '" + Sem +
                    "' and Campus_Code eq '" + Campus + "' and Lecturer eq '" + Lec + "' &$format=json";

                    HttpWebResponse httpResponseTmT = Credentials.GetOdataData(pageTmT);
                    using (var streamReaderTmT = new StreamReader(httpResponseTmT.GetResponseStream()))
                    {
                        var resultTmT = streamReaderTmT.ReadToEnd();

                        var detailsTmT = JObject.Parse(resultTmT);

                        foreach (JObject config1 in detailsTmT["value"])
                        {
                            string tday = DateTime.UtcNow.ToString("o");
                            string pageWk = "WeeksList?$select=Code,Start_Date&$filter=Day eq '" + (string)config1["DayofWeek"] +
                                "' and Inactive eq false and Start_Date le " + tday + " and Semester eq '" + Sem + "'&$format=json";

                            HttpWebResponse httpResponseWk = Credentials.GetOdataData(pageWk);
                            using (var streamReader = new StreamReader(httpResponseWk.GetResponseStream()))
                            {
                                var result = streamReader.ReadToEnd();

                                var details = JObject.Parse(result);

                                foreach (JObject config in details["value"])
                                {
                                    if (!ClassAttendanceWeekTaken(Lec, Sem, Unit, Campus, (string)config["Code"]))
                                    {
                                        DropdownList ddl = new DropdownList();
                                        ddl.Value = (string)config["Code"] + "~" + (string)config1["DayofWeek"];
                                        ddl.Text = (string)config["Code"] + "-" + (string)config1["DayofWeek"] + "-" + ((DateTime)config["Start_Date"]).ToString("dd/MM/yyyy");
                                        WeekList.Add(ddl);
                                    }
                                }
                            }
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
                return PartialView();
            }
        }
        protected bool ClassAttendanceWeekTaken(string StaffNo, string Semester, string Unit, string Campus, string Wk)
        {
            bool taken = false;
            try
            {
                string pageAtt = "ClassAttendanceHeader?$select=WeekCode&$filter=LecturerCode eq '" + StaffNo + "' and SemesterCode eq '" + Semester + "' and UnitCode eq '" +
                    Unit + "' and CampusCode eq '" + Campus + "' and WeekCode eq '" + Wk + "'&$format=json";

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
        public JsonResult SaveClassAttendance(string DocNo, string Wk, List<Array> Rows)
        {
            try
            {
                if (Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null)
                {
                    string Lec = Session["username"].ToString();
                    //string Prog = Session["Prog"].ToString();
                    //string Stage = Session["Stage"].ToString();
                    string Sem = Session["Sem"].ToString();
                    string Unit = Session["Unit"].ToString();
                    string Campus = Session["Campus"].ToString();
                    //string classCode = Session["ClassCode"].ToString();

                    string[] s = Wk.Split('~');
                    Wk = s[0].Trim();
                    string Code = "";

                    if (DocNo != null && DocNo != "")
                    {
                        Code = DocNo;
                    }
                    else
                    {
                        Code = Credentials.ObjNav.InserClassAtteHeader("", Unit, Sem, Wk, Lec, Campus, s[1].Trim(), "");
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
        public PartialViewResult LoadPercClassAttendanceStudents()
        {
            if (Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null )
            {
                string Lec = Session["username"].ToString();
                string Sem = Session["Sem"].ToString();
                string Unit = Session["Unit"].ToString();
                string Campus = Session["Campus"].ToString();
                string classCode = "";

                List<CustomerList> studentlist = new List<CustomerList>();

                string page = "StudentUnits?$filter=Semester eq '" + Sem + "' and Unit eq '" + Unit + "' and Unit_Class_Code eq '" + classCode + "' and Campus eq '" + Campus + "'&$format=json";

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
                        decimal MaxA = 0;
                        MaxA = Convert.ToDecimal(CommonClass.GetMaximumNoOfAttendance(Sem, (string)config["Unit"], (string)config["Campus"], (string)config["Unit_Class_Code"]));
                        if (MaxA > 0)
                        {
                            Cust.Per_Attendance = Math.Round(CommonClass.GetPercentageAttendance(Cust.No, Sem, Unit, MaxA), 0).ToString() + "%";
                        }
                        else
                        {
                            Cust.Per_Attendance = "0";
                        }
                        studentlist.Add(Cust);
                    }
                }

                return PartialView("~/Views/Lecturer/PercAttendanceList.cshtml", studentlist.OrderBy(x => x.No).ToList());
            }
            else
            {
                return PartialView();
            }
        }
        public PartialViewResult LoadStudentAttendanceENTRY(string StudentNo)
        {
            if (Session["Sem"] != null && Session["Unit"] != null && Session["Campus"] != null || Session["ClassCode"] != null)
            {
                string Lec = Session["username"].ToString();
                string Sem = Session["Sem"].ToString();
                string Unit = Session["Unit"].ToString();
                string Campus = Session["Campus"].ToString();
                string classCode = "";

                List<StudentAttendance> studentAtte = new List<StudentAttendance>();

                string page = "ClassAttendanceLines?$filter=StudentNo eq '" + StudentNo + "' and Semester eq '" + Sem + "' and UnitCode eq '" + Unit + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        StudentAttendance Att = new StudentAttendance();
                        Att.Week = (string)config["WeekCode"];
                        Att.Attendance = (string)config["AttendanceType"];

                        studentAtte.Add(Att);
                    }
                }

                return PartialView("~/Views/Lecturer/StudentAttendanceEntry.cshtml", studentAtte.OrderBy(x => x.Week).ToList());
            }
            else
            {
                return PartialView();
            }
        }
        public ActionResult CourseEvaluationReports()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                string StaffNo = Session["Username"].ToString();
                List<DropdownList> semesterList = new List<DropdownList>();
                string page = "Timetable?$filter=Lecturer eq '" + StaffNo + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        if (CommonClass.AllowLecEvaluationOnlineViewing((string)config["Semester"]))
                        {
                            DropdownList ddl = new DropdownList();
                            ddl.Value = (string)config["Semester"];
                            ddl.Text = (string)config["Semester"];
                            semesterList.Add(ddl);
                        }
                    }
                }
                CourseEval doc = new CourseEval
                {
                    Code = "",
                    ListOfSemester = semesterList.Select(x =>
                                                 new SelectListItem()
                                                 {
                                                     Text = x.Text,
                                                     Value = x.Value
                                                 }).DistinctBy(x => x.Value).OrderBy(x => x.Value).ToList()
                };
                return View(doc);
            }
        }
        public ActionResult EvaluationRListCourse(string Semester)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                string StaffNo = Session["Username"].ToString();
                List<LecturerAssignedUnits> LectUnitAllocation = new List<LecturerAssignedUnits>();
                string page = "Timetable?$filter=Lecturer eq '" + StaffNo + "' and Semester eq '" + Semester + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        LecturerAssignedUnits Lec = new LecturerAssignedUnits();
                        Lec.Semester = (string)config["Semester"];
                        Lec.Unit = (string)config["Unit"];
                        Lec.Unit_Name = (string)config["Unit_Description"];
                        Lec.Campus_Code = (string)config["Campus_Code"];
                        Lec.Day = (string)config["DayofWeek"];
                        Lec.Period = (string)config["Period"];
                        Lec.Room = (string)config["Lecture_Room"];
                        Lec.CourseClass = (string)config["Unit_Class"];
                        Lec.stdCount = (string)config["Students_Count"];
                        LectUnitAllocation.Add(Lec);
                    }
                }
                return View("~/Views/Lecturer/LecturerCourseEvaluationList.cshtml", LectUnitAllocation.DistinctBy(x => new { x.Unit, x.Semester, x.CourseClass, x.Campus_Code }).ToList());
            }
        }
        [HttpPost]
        public JsonResult GetEvaluationReport(string Unit, string Sem, string Section, string Campus, string RType)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                string _filename = (StaffNo).Replace(@"/", @"");
                string message = "";
                string filename = "";
                bool success = false, view = false;

                if (RType == "1")
                {
                    Credentials.ObjNav.GenerateLecturerEvaluationReport("", Unit, "", Sem, Section, "", 0, "COURSE_EVALUATION_" + _filename + ".pdf");

                    filename = "COURSE_EVALUATION_" + _filename + ".pdf";
                }
                if (RType == "2")
                {
                    Credentials.ObjNav.GetLecturerEvaluationCommentsReport("", Unit, "", Sem, Section, "", 0, "COURSE_COMMENTS_" + _filename + ".pdf");

                    filename = "COURSE_COMMENTS_" + _filename + ".pdf";
                }
                string fileDestinationPath = Server.MapPath("~/Downloads/");
                CommonClass.MoveFile(filename, fileDestinationPath);
                string DestinationPath = fileDestinationPath + filename;
                System.IO.FileInfo file = new System.IO.FileInfo(DestinationPath);
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
                return Json(new { message = message, success, view }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}