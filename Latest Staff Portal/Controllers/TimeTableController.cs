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
    public class TimeTableController : Controller
    {
        // GET: TimeTable
        public ActionResult TimeTableProjections()
        {
            return View();
        }
        public PartialViewResult TimeTableProjectionList()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();

                List<TimeTableView> ProjList = new List<TimeTableView>();

                //EmployeeDesignation EmpDes = CommonClass.EmployeeDesignation(StaffNo);
                string page = "", Msg = "";

                //if (EmpDes.IsHOD)
                //{
                //    if (EmpDes.EmpDepartment != "")
                //    {
                //        page = "TimetableProjections?$filter=Department eq '" + EmpDes.EmpDepartment + "'&$sformat=json";
                //    }
                //    else
                //    {
                //        Msg = "Your department has not be set in HR";
                //    }
                //}

                //if (EmpDes.IsDean)
                //{
                //    if (EmpDes.EmpSchool != "")
                //    {
                //        page = "TimetableProjections?$filter=School eq '" + EmpDes.EmpSchool + "'&$format=json";
                //    }
                //    else
                //    {
                //        Msg = "Your school has not be set in HR";
                //    }
                //}
                //if (EmpDes.IsDirector)
                //{
                //    page = "TimetableProjections?$format=json";
                //}
                page = "TimetableProjections?$format=json";
                if (page != "")
                {
                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        if (details["value"].Count() > 0)
                        {
                            foreach (JObject config in details["value"])
                            {
                                TimeTableView Tm = new TimeTableView();
                                Tm.Campus = (string)config["Campus"];
                                Tm.Semester = (string)config["Semester"];
                                Tm.Unit = (string)config["Unit"];
                                Tm.Description = (string)config["Unit_Name"];
                                Tm.Period = (string)config["Period"];
                                Tm.Day_of_Week = (string)config["Day_of_Week"];
                                Tm.Section = (string)config["Section"];
                                Tm.Lecture_Room = (string)config["Lecture_Room"];
                                Tm.Lecturer = (string)config["Lecturer"];
                                Tm.LecturerName = (string)config["Lecturer_Name"];
                                Tm.ClassSize = (string)config["Class_Size"];
                                Tm.EntryNo = (string)config["Entry_No"];
                                ProjList.Add(Tm);
                            }
                        }
                    }
                    return PartialView("~/Views/TimeTable/Partial View/TimeTableProjecion.cshtml", ProjList.OrderByDescending(x => x.Unit));
                }
                else
                {
                    Error erroMsg = new Error();
                    erroMsg.Message = Msg;
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
        public ActionResult NewTimeTableProjection()
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
                    string Msg = "";
                    EmployeeDesignation EmpDes = CommonClass.EmployeeDesignation(StaffNo);

                    if (EmpDes.IsHOD && EmpDes.EmpDepartment == "")
                    {
                        Msg = "Your department has not be set in HR";
                        Error erroMsg = new Error();
                        erroMsg.Message = Msg;
                        return View("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
                    }
                    else if (EmpDes.IsDean && EmpDes.EmpSchool == "")
                    {
                        Msg = "Your school has not be set in HR";
                        Error erroMsg = new Error();
                        erroMsg.Message = Msg;
                        return View("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
                    }
                    else
                    {
                        NewTimeTable NewDoc = new NewTimeTable();
                        #region Campus List
                        List<DimensionValues> Campuses = new List<DimensionValues>();
                        string pageCampus = "DimensionValues?$filter=Global_Dimension_No_ eq 1 and Blocked eq false&$format=json";

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
                        #region Units
                        List<DropdownList> UnitList = new List<DropdownList>();

                        string pageUnit = "";

                        if (EmpDes.IsHOD)
                        {
                            if (EmpDes.EmpDepartment != "")
                            {
                                pageUnit = "Courses_Master?$filter=Department_Code eq '" + EmpDes.EmpDepartment + "'&$format=json";
                            }
                        }
                        if (EmpDes.IsDean)
                        {
                            if (EmpDes.EmpSchool != "")
                            {
                                pageUnit = "Courses_Master?$filter=School_Code eq '" + EmpDes.EmpSchool + "'&$format=json";
                            }
                        }

                        if (pageUnit != "")
                        {
                            HttpWebResponse httpResponseUnit = Credentials.GetOdataData(pageUnit);
                            using (var streamReader = new StreamReader(httpResponseUnit.GetResponseStream()))
                            {
                                var result = streamReader.ReadToEnd();

                                var details = JObject.Parse(result);


                                foreach (JObject config in details["value"])
                                {
                                    DropdownList U = new DropdownList();
                                    U.Value = (string)config["Code"];
                                    U.Text = (string)config["Code"] + "-" + (string)config["Description"];
                                    UnitList.Add(U);
                                }
                            }
                        }
                        #endregion
                        #region Days of Week
                        List<DropdownList> WkDayList = new List<DropdownList>();
                        string pageDoW = "DaysOfWeek?$format=json";

                        HttpWebResponse httpResponseDoW = Credentials.GetOdataData(pageDoW);
                        using (var streamReader = new StreamReader(httpResponseDoW.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);


                            foreach (JObject config in details["value"])
                            {
                                DropdownList day = new DropdownList();
                                day.Value = (string)config["Day"];
                                day.Text = (string)config["Day"];
                                WkDayList.Add(day);
                            }
                        }
                        #endregion
                        #region Period List
                        List<DropdownList> PeriodList = new List<DropdownList>();
                        string pagePeriod = "PeriodList?$format=json";

                        HttpWebResponse httpResponsePeriod = Credentials.GetOdataData(pagePeriod);
                        using (var streamReader = new StreamReader(httpResponsePeriod.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);


                            foreach (JObject config in details["value"])
                            {
                                DropdownList p = new DropdownList();
                                p.Value = (string)config["Code"];
                                p.Text = (string)config["Code"];
                                PeriodList.Add(p);
                            }
                        }
                        #endregion
                        #region Sections
                        List<DropdownList> SectionList = new List<DropdownList>();
                        string pageSec = "CourseClasses?$format=json";

                        HttpWebResponse httpResponseSection = Credentials.GetOdataData(pageSec);
                        using (var streamReader = new StreamReader(httpResponseSection.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);


                            foreach (JObject config in details["value"])
                            {
                                DropdownList RCList = new DropdownList();
                                RCList.Value = (string)config["Code"];
                                RCList.Text = (string)config["Code"];
                                SectionList.Add(RCList);
                            }
                        }
                        #endregion
                        #region Lecturer List
                        List<DropdownList> LecList = new List<DropdownList>();
                        string pageLec = "EmployeeList?$filter=Lecturer eq true and Status eq 'Active'&$format=json";

                        HttpWebResponse httpResponseLec = Credentials.GetOdataData(pageLec);
                        using (var streamReader = new StreamReader(httpResponseLec.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);


                            foreach (JObject config in details["value"])
                            {
                                if ((string)config["First_Name"] != "" || (string)config["Middle_Name"] != "" || (string)config["Last_Name"] != "")
                                {
                                    DropdownList Lec = new DropdownList();
                                    Lec.Value = (string)config["No"];
                                    Lec.Text = (string)config["No"] + "-" + (string)config["First_Name"] + " " + (string)config["Middle_Name"] + " " + (string)config["Last_Name"];
                                    LecList.Add(Lec);
                                }
                            }
                        }
                        #endregion

                        TimeTableView TmV = new TimeTableView();
                        NewDoc = new NewTimeTable
                        {
                            Edit = false,
                            TableDoc = TmV,
                            ListOfCampus = Campuses.Select(x =>
                                                 new SelectListItem()
                                                 {
                                                     Text = x.Name,
                                                     Value = x.Code
                                                 }).DistinctBy(x => x.Value).ToList(),
                            ListOfUnits = UnitList.Select(x =>
                                                 new SelectListItem()
                                                 {
                                                     Text = x.Text,
                                                     Value = x.Value
                                                 }).DistinctBy(x => x.Value).ToList(),
                            ListOfDaysOfWeek = WkDayList.Select(x =>
                                                new SelectListItem()
                                                {
                                                    Text = x.Text,
                                                    Value = x.Value
                                                }).DistinctBy(x => x.Value).ToList(),
                            ListOfPeriods = PeriodList.Select(x =>
                                               new SelectListItem()
                                               {
                                                   Text = x.Text,
                                                   Value = x.Value
                                               }).DistinctBy(x => x.Value).OrderBy(x => x.Value).ToList(),
                            ListOfSections = SectionList.Select(x =>
                                               new SelectListItem()
                                               {
                                                   Text = x.Text,
                                                   Value = x.Value
                                               }).DistinctBy(x => x.Value).ToList(),
                            ListOfLecturers = LecList.Select(x =>
                                               new SelectListItem()
                                               {
                                                   Text = x.Text,
                                                   Value = x.Value
                                               }).DistinctBy(x => x.Value).ToList()
                        };
                        return PartialView("~/Views/TimeTable/Partial View/NewTimeTableProjection.cshtml", NewDoc);
                    }
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public ActionResult EditTimeTableProjection(TimeTableView TmV)
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
                    string Msg = "";
                    EmployeeDesignation EmpDes = CommonClass.EmployeeDesignation(StaffNo);

                    if (EmpDes.IsHOD && EmpDes.EmpDepartment == "")
                    {
                        Msg = "Your department has not be set in HR";
                        Error erroMsg = new Error();
                        erroMsg.Message = Msg;
                        return View("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
                    }
                    else if (EmpDes.IsDean && EmpDes.EmpSchool == "")
                    {
                        Msg = "Your school has not be set in HR";
                        Error erroMsg = new Error();
                        erroMsg.Message = Msg;
                        return View("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
                    }
                    else
                    {
                        NewTimeTable NewDoc = new NewTimeTable();
                        #region Campus List
                        List<DimensionValues> Campuses = new List<DimensionValues>();
                        string pageCampus = "DimensionValues?$filter=Global_Dimension_No_ eq 1 and Blocked eq false&$format=json";

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
                        #region Units
                        List<DropdownList> UnitList = new List<DropdownList>();

                        string pageUnit = "";

                        if (EmpDes.IsHOD)
                        {
                            if (EmpDes.EmpDepartment != "")
                            {
                                pageUnit = "Courses_Master?$filter=Department_Code eq '" + EmpDes.EmpDepartment + "'&$format=json";
                            }
                        }
                        if (EmpDes.IsDean)
                        {
                            if (EmpDes.EmpSchool != "")
                            {
                                pageUnit = "Courses_Master?$filter=School_Code eq '" + EmpDes.EmpSchool + "'&$format=json";
                            }
                        }

                        if (pageUnit != "")
                        {
                            HttpWebResponse httpResponseUnit = Credentials.GetOdataData(pageUnit);
                            using (var streamReader = new StreamReader(httpResponseUnit.GetResponseStream()))
                            {
                                var result = streamReader.ReadToEnd();

                                var details = JObject.Parse(result);


                                foreach (JObject config in details["value"])
                                {
                                    DropdownList U = new DropdownList();
                                    U.Value = (string)config["Code"];
                                    U.Text = (string)config["Code"] + "-" + (string)config["Description"];
                                    UnitList.Add(U);
                                }
                            }
                        }
                        #endregion
                        #region Days of Week
                        List<DropdownList> WkDayList = new List<DropdownList>();
                        string pageDoW = "DaysOfWeek?$format=json";

                        HttpWebResponse httpResponseDoW = Credentials.GetOdataData(pageDoW);
                        using (var streamReader = new StreamReader(httpResponseDoW.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);


                            foreach (JObject config in details["value"])
                            {
                                DropdownList day = new DropdownList();
                                day.Value = (string)config["Day"];
                                day.Text = (string)config["Day"];
                                WkDayList.Add(day);
                            }
                        }
                        #endregion
                        #region Period List
                        List<DropdownList> PeriodList = new List<DropdownList>();
                        string pagePeriod = "PeriodList?$format=json";

                        HttpWebResponse httpResponsePeriod = Credentials.GetOdataData(pagePeriod);
                        using (var streamReader = new StreamReader(httpResponsePeriod.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);


                            foreach (JObject config in details["value"])
                            {
                                DropdownList p = new DropdownList();
                                p.Value = (string)config["Code"];
                                p.Text = (string)config["Code"];
                                PeriodList.Add(p);
                            }
                        }
                        #endregion
                        #region Sections
                        List<DropdownList> SectionList = new List<DropdownList>();
                        string pageSec = "CourseClasses?$format=json";

                        HttpWebResponse httpResponseSection = Credentials.GetOdataData(pageSec);
                        using (var streamReader = new StreamReader(httpResponseSection.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);


                            foreach (JObject config in details["value"])
                            {
                                DropdownList RCList = new DropdownList();
                                RCList.Value = (string)config["Code"];
                                RCList.Text = (string)config["Code"];
                                SectionList.Add(RCList);
                            }
                        }
                        #endregion
                        #region Lecturer List
                        List<DropdownList> LecList = new List<DropdownList>();
                        string pageLec = "EmployeeList?$filter=Lecturer eq true and Status eq 'Active'&$format=json";

                        HttpWebResponse httpResponseLec = Credentials.GetOdataData(pageLec);
                        using (var streamReader = new StreamReader(httpResponseLec.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);


                            foreach (JObject config in details["value"])
                            {
                                if ((string)config["First_Name"] != "" || (string)config["Middle_Name"] != "" || (string)config["Last_Name"] != "")
                                {
                                    DropdownList Lec = new DropdownList();
                                    Lec.Value = (string)config["No"];
                                    Lec.Text = (string)config["No"] + "-" + (string)config["First_Name"] + " " + (string)config["Middle_Name"] + " " + (string)config["Last_Name"];
                                    LecList.Add(Lec);
                                }
                            }
                        }
                        #endregion

                        NewDoc = new NewTimeTable
                        {
                            TableDoc = TmV,
                            Edit = true,
                            ListOfCampus = Campuses.Select(x =>
                                                 new SelectListItem()
                                                 {
                                                     Text = x.Name,
                                                     Value = x.Code
                                                 }).DistinctBy(x => x.Value).ToList(),
                            ListOfUnits = UnitList.Select(x =>
                                                 new SelectListItem()
                                                 {
                                                     Text = x.Text,
                                                     Value = x.Value
                                                 }).DistinctBy(x => x.Value).ToList(),
                            ListOfDaysOfWeek = WkDayList.Select(x =>
                                                new SelectListItem()
                                                {
                                                    Text = x.Text,
                                                    Value = x.Value
                                                }).DistinctBy(x => x.Value).ToList(),
                            ListOfPeriods = PeriodList.Select(x =>
                                               new SelectListItem()
                                               {
                                                   Text = x.Text,
                                                   Value = x.Value
                                               }).DistinctBy(x => x.Value).OrderBy(x => x.Value).ToList(),
                            ListOfSections = SectionList.Select(x =>
                                               new SelectListItem()
                                               {
                                                   Text = x.Text,
                                                   Value = x.Value
                                               }).DistinctBy(x => x.Value).ToList(),
                            ListOfLecturers = LecList.Select(x =>
                                               new SelectListItem()
                                               {
                                                   Text = x.Text,
                                                   Value = x.Value
                                               }).DistinctBy(x => x.Value).ToList()
                        };
                        return PartialView("~/Views/TimeTable/Partial View/NewTimeTableProjection.cshtml", NewDoc);
                    }
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return View("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetLecturerRooms(string Campus)
        {
            try
            {
                #region Lec Rooms
                List<DropdownList> roomList = new List<DropdownList>();
                string page = "Lecture_Rooms?$filter=GlobalDimension1 eq '" + Campus + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList LR = new DropdownList();
                        LR.Value = (string)config["Code"];
                        LR.Text = (string)config["Code"];
                        roomList.Add(LR);
                    }
                }
                #endregion
                LecRooms lecRm = new LecRooms
                {
                    ListOfLecRooms = roomList.Select(x =>
                                    new SelectListItem()
                                    {
                                        Text = x.Text,
                                        Value = x.Value
                                    }).ToList()
                };
                return Json(lecRm, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitTimeTableProjection(TimeTableView Tm)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                string LecRoom = "", Lecturer = "";
                if (Tm.Lecture_Room != null)
                {
                    LecRoom = Tm.Lecture_Room;
                }
                if (Tm.Lecturer != null)
                {
                    Lecturer = Tm.Lecturer;
                }
                Credentials.ObjNav.InsertTimetableProjections(StaffNo, Tm.Unit, Tm.Period, Tm.Day_of_Week, LecRoom, Tm.Section, Lecturer, Tm.Campus, Convert.ToInt32(Tm.ClassSize));

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateTimeTableProjection(TimeTableView Tm)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                string LecRoom = "", Lecturer = "";
                if (Tm.Lecture_Room != null)
                {
                    LecRoom = Tm.Lecture_Room;
                }
                if (Tm.Lecturer != null)
                {
                    Lecturer = Tm.Lecturer;
                }
                //Credentials.ObjNav.UpdateTimetableProjections(StaffNo, Tm.Unit, Tm.Period, Tm.Day_of_Week, LecRoom, Tm.Section, Lecturer, Tm.Campus, Convert.ToInt32(Tm.ClassSize), Convert.ToInt32(Tm.EntryNo));

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeleteTimeTableProjection(string EntryNo)
        {
            try
            {
                //Credentials.ObjNav.DeleteTimetableProjections(Convert.ToInt32(EntryNo));

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}