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
    public class GradeComplaintController : Controller
    {
        // GET: GradeComplaint
        public ActionResult Grade_Complaint()
        {
            return View();
        }
        public ActionResult Grade_Complaint_View()
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
                    List<Grade_Compaint> newDocList = new List<Grade_Compaint>();
                    string pageReg = "GradeComplaint?$filter=Lecturer eq '" + Lec + "' and Status eq 'Open' and MarksSubmitted eq false&$format=json";
                    List<TimeTableView> timeTable = new List<TimeTableView>();
                    HttpWebResponse httpResponse = Credentials.GetOdataData(pageReg);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);

                        if (details["value"].Count() > 0)
                        {
                            foreach (JObject config in details["value"])
                            {
                                Grade_Compaint newDoc = new Grade_Compaint();
                                newDoc.Doc_No = (string)config["DocNo"];
                                newDoc.Student_No = (string)config["StudentNo"];
                                newDoc.Student_Name = (string)config["StudentName"];
                                newDoc.Unit = (string)config["Unit"];
                                newDoc.Unit_Name = (string)config["UnitDescription"];
                                newDoc.Semester = (string)config["Semester"];
                                newDoc.Date_Applied = ((DateTime)config["DateApplied"]).ToString("dd/MM/yyyy");
                                newDoc.Current_Grade = (string)config["OriginalGrade"];
                                newDocList.Add(newDoc);
                            }
                        }
                    }
                    return View("~/Views/GradeComplaint/Partial View/Grade_Complaint_List_Views.cshtml", newDocList);
                }
            }
            catch (Exception ex)
            {
                Error error = new Error();
                error.Message = ex.Message.Replace("'", "");
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", error);
            }
        }
        public ActionResult Grade_Complaint_Form(string DocNo)
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
                    Grade_Compaint newDoc = new Grade_Compaint();
                    string pageReg = "GradeComplaint?$filter=DocNo eq '" + DocNo + "'&$format=json";
                    List<TimeTableView> timeTable = new List<TimeTableView>();
                    HttpWebResponse httpResponse = Credentials.GetOdataData(pageReg);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);

                        if (details["value"].Count() > 0)
                        {
                            foreach (JObject config in details["value"])
                            {
                                newDoc.Doc_No = (string)config["DocNo"];
                                newDoc.Student_No = (string)config["StudentNo"];
                                newDoc.Student_Name = (string)config["StudentName"];
                                newDoc.Unit = (string)config["Unit"];
                                newDoc.Unit_Name = (string)config["UnitDescription"];
                                newDoc.Semester = (string)config["Semester"];
                                newDoc.Date_Applied = ((DateTime)config["DateApplied"]).ToString("dd/MM/yyyy");
                                newDoc.Current_Grade = (string)config["OriginalGrade"];
                            }
                        }
                    }
                    return View("~/Views/GradeComplaint/Partial View/Grade_Compain_Form.cshtml", newDoc);
                }
            }
            catch (Exception ex)
            {
                Error error = new Error();
                error.Message = ex.Message.Replace("'", "");
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", error);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitGradeComplain(string DocNo, string Marks, string Reason, string OtherReason, string base64Upload, string fileName, string Extn, int TableID)
        {
            try
            {
                string StaffNo = Session["Username"].ToString();

                bool successVal = false;
                string msg = "";
                if (base64Upload != "" && Extn != "")
                {
                    string ext = Path.GetExtension(fileName);

                    if (ext.ToLower() == ".pdf" || ext.ToLower() == ".docx" || ext.ToLower() == ".doc" || ext.ToLower() == ".xlsx" ||
                        ext.ToLower() == ".jpeg" || ext.ToLower() == ".jpg" || ext.ToLower() == ".png")
                    {
                        if (Reason == null || Reason == "")
                        {
                            Reason = "0";
                        }
                        if (OtherReason == null)
                        {
                            OtherReason = "";
                        }
                        // Credentials.ObjNav.LecturerSubmitGradeCompain(DocNo, Convert.ToDecimal(Marks), Convert.ToInt32(Reason), OtherReason);

                        string filePath = Server.MapPath("~/Uploads/" + fileName);
                        string s = Credentials.UploadDocumentAttachment(DocNo, base64Upload, filePath, TableID);
                        if (s == "SUCCESS")
                        {
                            msg = "Grade Complaint Submitted successfully";
                            successVal = true;
                        }
                        else
                        {
                            msg = s;
                            successVal = false;
                        }
                    }
                    else
                    {
                        msg = "Only files with extensions(.pdf, .docx, .doc, .xlsx, .jpeg, .jpg, .png) can be uploaded";
                        successVal = false;
                    }
                }

                return Json(new { message = msg, success = successVal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}