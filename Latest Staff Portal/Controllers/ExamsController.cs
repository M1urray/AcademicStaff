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
    [CustomAuthorization(Role = "FULLTIME")]
    public class ExamsController : Controller
    {
        // GET: Exams
        public ActionResult ExamSetup()
        {
            if (Session["UserID"] == null || Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                if (!CommonClass.CanEditMarks(Session["UserID"].ToString()))
                {
                    return RedirectToAction("Dashboard", "Dashboard");
                }
                else
                {
                    return View();
                }
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetRegisteredSemesterd(string StdNo)
        {
            try
            {
                #region Semesters
                List<DropdownList> SemList = new List<DropdownList>();
                string page = "CourseReg?$select=Semester&$filter=StudentNo eq '" + StdNo + "' and Reversed eq false&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList s = new DropdownList();
                        s.Text = (string)config["Semester"];
                        s.Value = (string)config["Semester"];
                        SemList.Add(s);
                    }
                }
                #endregion
                DropdownListData SList = new DropdownListData
                {
                    ListOfddlData = SemList.Select(x =>
                                    new SelectListItem()
                                    {
                                        Text = x.Text,
                                        Value = x.Value
                                    }).ToList()
                };
                return Json(SList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetRegisteredUnits(string StdNo, string Sem)
        {
            try
            {
                #region Unit List
                List<DropdownList> UnitList = new List<DropdownList>();
                string page = "StudentUnits?$filter=Student_No eq '" + StdNo + "' and Semester eq '" + Sem + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList s = new DropdownList();
                        s.Text = (string)config["Unit"] + "-" + (string)config["Unit_Description"];
                        s.Value = (string)config["Unit"];
                        UnitList.Add(s);
                    }
                }
                #endregion
                DropdownListData UList = new DropdownListData
                {
                    ListOfddlData = UnitList.Select(x =>
                                    new SelectListItem()
                                    {
                                        Text = x.Text,
                                        Value = x.Value
                                    }).ToList()
                };
                return Json(UList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult LoadStudentSpecialUnit(string StdNo, string Sem, string Unit)
        {
            StudentRedUnit stdUnitD = new StudentRedUnit();
            List<CustomerList> studentlist = new List<CustomerList>();
            string Prog = "";
            string page = "StudentUnits?$filter=Student_No eq '" + StdNo + "' and Semester eq '" + Sem + "' and Unit eq '" + Unit + "'&$format=json";

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
                    Cust.Unit = (string)config["Unit"];
                    Cust.Semester = (string)config["Semester"];
                    Prog = (string)config["Programme"];
                    studentlist.Add(Cust);
                }
                stdUnitD = new StudentRedUnit
                {
                    Cust = studentlist.OrderBy(x => x.No).DistinctBy(x => x.No).ToList(),
                    Prog = Prog,
                    Unit = Unit,
                    Sem = Sem
                };
            }
            return PartialView("~/Views/Exams/LoadSpecialStudentUnit.cshtml", stdUnitD);
        }
        public PartialViewResult LoadStudentSuppUnit(string StdNo, string Sem, string Unit)
        {
            StudentRedUnit stdUnitD = new StudentRedUnit();
            List<CustomerList> studentlist = new List<CustomerList>();
            string Prog = "";
            string page = "StudentUnits?$filter=Student_No eq '" + StdNo + "' and Semester eq '" + Sem + "' and Unit eq '" + Unit + "' and Register_for eq 'Supplementary'&$format=json";

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
                    Cust.Unit = (string)config["Unit"];
                    Cust.Semester = (string)config["Semester"];
                    Cust.Marks = (string)config["Final_Score"];
                    Cust.Grade = (string)config["Grade"];
                    Prog = (string)config["Programme"];
                    studentlist.Add(Cust);
                }
                stdUnitD = new StudentRedUnit
                {
                    Cust = studentlist.OrderBy(x => x.No).DistinctBy(x => x.No).ToList(),
                    Prog = Prog,
                    Unit = Unit,
                    Sem = Sem
                };
            }
            return PartialView("~/Views/Exams/LoadSupplimentaryStudentUnit.cshtml", stdUnitD);
        }
        [HttpPost]
        public JsonResult SaveSpecialMarksStudentMarks(string Prog, string Sem, string Unit, List<Array> headers, List<Array> Rows)
        {
            try
            {
                string Lec = Session["username"].ToString();

                bool Assigned = false;
                string ProgC = "";

                ProgC = Credentials.ObjNav.GetExamCategory(Unit, Prog);
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
                            decimal mxmScore = 0, AssinedScore = 0, Contribution = 0;

                            string page = "ExamSetup?$filter=Category eq '" + ProgC + "' and Code eq '" + HeaderText[j].Trim() + "' and Type ne 'Special' and Type ne 'Supplementary'&$format=json";

                            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                var result = streamReader.ReadToEnd();
                                var details = JObject.Parse(result);
                                foreach (JObject config in details["value"])
                                {
                                    mxmScore = Convert.ToDecimal(config["Max_Score"].ToString());
                                    Contribution = Convert.ToDecimal(config["Contrib_Final_Score"].ToString());
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
                                        Contribution = (AssinedScore / mxmScore) * Contribution;
                                        Credentials.ObjNav.EnterSpecialExamMarks(
                                        prog: "",
                                        stage: "",
                                        unit: Unit,
                                        sem: Sem,
                                        score: AssinedScore,
                                        contrib: Math.Round(Contribution, 0),
                                        stdNo: studentNo,
                                        examType: examType,
                                        user: Session["username"].ToString(),
                                        entryType: HeaderText[j].Trim(),
                                        academicY: ""
                                        );
                                        Assigned = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if (Assigned)
                {
                    return Json(new { message = "Special Exam Marks Assigned Successfully", success = true, failed = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "No Student with Assined Marks found", success = true, failed = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SaveSuppMarksStudentMarks(string Prog, string Sem, string Unit, List<Array> headers, List<Array> Rows)
        {
            try
            {
                string Lec = Session["username"].ToString();

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
                int noOfPapers = CommonClass.ReturnNumberofSuppExamPapers(ProgC);
                string[] HeaderText = (string[])headers[0];
                int ColumnCount = HeaderText.Count();

                int RowCount = Rows.Count();

                for (int i = 0; i < RowCount; i++)
                {
                    string[] RowText = (string[])Rows[i];

                    string studentNo = RowText[1].Trim();
                    for (int j = 3; j < ColumnCount; j++)
                    {
                        string marks = "";
                        marks = RowText[j];
                        if (marks != "")
                        {
                            decimal mxmScore = 0, AssinedScore = 0, Contribution = 0;

                            string page = "ExamSetup?$filter=Category eq '" + ProgC + "' and Code eq '" + HeaderText[j].Trim() + "' and Type eq 'Supplementary'&$format=json";

                            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                var result = streamReader.ReadToEnd();
                                var details = JObject.Parse(result);
                                foreach (JObject config in details["value"])
                                {
                                    mxmScore = Convert.ToDecimal(config["Max_Score"].ToString());
                                    Contribution = Convert.ToDecimal(config["Contrib_Final_Score"].ToString());
                                    AssinedScore = Convert.ToDecimal(marks);
                                    if (AssinedScore > mxmScore)
                                    {
                                        return Json(new { message = HeaderText[j].Trim() + " assined score can not be greater than " + mxmScore + ", maximum allowed score", success = true, failed = true }, JsonRequestBehavior.AllowGet);
                                    }
                                    Contribution = ((AssinedScore / mxmScore) * Contribution) * 100;
                                    if (AssinedScore > 0)
                                    {
                                        Credentials.ObjNav.EnterSupplimentaryExamMarks(
                                                       prog: "",
                                                       stage: "",
                                                       unit: Unit,
                                                       sem: Sem,
                                                       score: AssinedScore,
                                                       contrib: AssinedScore,
                                                       stdNo: studentNo,
                                                       examType: "EXAM",
                                                       user: Session["username"].ToString(),
                                                       entryType: HeaderText[j].Trim(),
                                                       academicY: ""
                                                       );
                                        Assigned = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if (Assigned)
                {
                    return Json(new { message = "Supplimentary Exam marks Assigned Successfully", success = true, failed = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "No Student with Assined Marks found", success = true, failed = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}