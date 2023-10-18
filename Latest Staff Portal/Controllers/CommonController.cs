using Latest_Staff_Portal.CustomSecurity;
using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
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
    public class CommonController : Controller
    {
        // GET: Common
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetService(string Item)
        {
            try
            {
                List<string> ItemList = new List<string>();
                var details = (JObject)Session["Servicedetails"];
                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        if (((string)config["Name"]).ToLower().Contains(Item.ToLower()))
                        {
                            ItemList.Add((string)config["No"] + "~" + (string)config["Name"]);
                        }
                    }
                }
                if (ItemList.Count < 1)
                {
                    ItemList.Add("No Service has been found. Contact procurement");
                }
                return Json(new { ItemList, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetItem(string Item)
        {
            try
            {
                List<string> ItemList = new List<string>();
                var details = (JObject)Session["Itemdetails"];
                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        if (((string)config["Description"]).ToLower().Contains(Item.ToLower()))
                        {
                            ItemList.Add((string)config["No"] + "~" + (string)config["Description"]);
                        }
                    }
                }
                if (ItemList.Count < 1)
                {
                    ItemList.Add("No Item has been found. Contact procurement");
                }
                return Json(new { ItemList, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetFixedAsset(string Item)
        {
            try
            {
                List<string> ItemList = new List<string>();
                var details = (JObject)Session["FAdetails"];
                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        if (((string)config["Description"]).ToLower().Contains(Item.ToLower()))
                        {
                            ItemList.Add((string)config["No"] + "~" + (string)config["Description"]);
                        }
                    }
                }
                if (ItemList.Count < 1)
                {
                    ItemList.Add("No Item has been found. Contact procurement");
                }
                return Json(new { ItemList, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetServiceList()
        {
            try
            {
                if (CommonClass.GetListServices())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "No services found", success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetItemList()
        {
            try
            {
                if (CommonClass.GetListItems())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "No Items found", success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetFixedAssetList()
        {
            try
            {
                if (CommonClass.GetListFixedAssets())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "No Items found", success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public void NullibySessions()
        {
            Session["SuccessMsg"] = null;
            Session["ErrorMsg"] = null;
        }
        public PartialViewResult DocumentApprovalTrail(string DocNo)
        {
            string StaffNo = Session["Username"].ToString();
            List<ApprovalEntries> ApprovalTrail = new List<ApprovalEntries>();

            string page = "ApprovalEntries?select=Approver_ID,Date_Time_Sent_for_Approval,Due_Date,Status,Sequence_No&$filter=Document_No eq '" + DocNo + "' and Status ne 'Canceled' and Status ne 'Rejected'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    ApprovalEntries AppTra = new ApprovalEntries();
                    AppTra.DocNo = DocNo;
                    //string[] s = config["Approver_ID"].ToString().Split('\\');
                    //string EmplName = CommonClass.GetEmployeeName(s[1]);
                    string EmplName = config["Approver_ID"].ToString();
                    if (EmplName != null)
                    {
                        AppTra.UserID = EmplName;
                    }
                    else
                    {
                        AppTra.UserID = (string)config["Approver_ID"];
                    }
                    AppTra.DateSendForApproval = Convert.ToDateTime((string)config["Date_Time_Sent_for_Approval"]).ToString("dd/MM/yyyy");
                    AppTra.DueDate = Convert.ToDateTime((string)config["Due_Date"]).ToString("dd/MM/yyyy");
                    AppTra.Status = (string)config["Status"];
                    AppTra.Sequence = Convert.ToInt32((string)config["Sequence_No"]);
                    ApprovalTrail.Add(AppTra);
                }
            }
            return PartialView("~/Views/Shared/Partial Views/ApprovalTrail.cshtml", ApprovalTrail.OrderBy(x => x.Sequence));
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetCommonDropdwnListData()
        {
            try
            {
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
                #region School
                List<DimensionValues> School = new List<DimensionValues>();
                string pageSchool = "DimensionValues?$filter=Global_Dimension_No_ eq 3&$format=json";

                HttpWebResponse httpResponseSchool = Credentials.GetOdataData(pageSchool);
                using (var streamReader = new StreamReader(httpResponseSchool.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues SchoolList = new DimensionValues();
                        SchoolList.Code = (string)config["Code"];
                        SchoolList.Name = (string)config["Name"];
                        School.Add(SchoolList);
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
                CommonDropDownList DropDownData = new CommonDropDownList
                {
                    ListOfSchools = School.Select(x =>
                                     new SelectListItem()
                                     {
                                         Text = x.Name,
                                         Value = x.Code
                                     }).ToList(),
                    ListOfDepartments = Department.Select(x =>
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
                                       }).ToList(),
                    ListOfRespC = RespCList.Select(x =>
                                       new SelectListItem()
                                       {
                                           Text = x.Name,
                                           Value = x.Code
                                       }).ToList()
                };
                return Json(new { DropDownData, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult DocumentAttachments(string DocNo, int TableID, string Status)
        {
            #region Document Attachment
            List<DocumentAttachment> DocAttachment = new List<DocumentAttachment>();
            string page = "DocumentAttachment?$filter=Table_ID eq " + TableID + " and No eq '" + DocNo + "'&format=json";

            HttpWebResponse httpResponseResC = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);

                foreach (JObject config in details["value"])
                {
                    DocumentAttachment docAttList = new DocumentAttachment();
                    docAttList.TabelID = (int)config["Table_ID"];
                    docAttList.No = (string)config["No"];
                    docAttList.FileName = (string)config["File_Name"];
                    docAttList.FileExt = (string)config["File_Extension"];
                    docAttList.ID = (int)config["ID"];
                    docAttList.LineNo = (string)config["Line_No"];
                    docAttList.DocType = (string)config["Document_Type"];
                    DocAttachment.Add(docAttList);
                }
            }
            #endregion
            DocumentAttachmentList DocumentList = new DocumentAttachmentList
            {
                Status = Status,
                DocList = DocAttachment
            };
            return PartialView("~/Views/Shared/Partial Views/ImportantDocs.cshtml", DocumentList);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SaveAttachedFile(string DocNo, string base64Upload, string fileName, string Extn, int TableID)
        {
            try
            {
                bool successVal = false;
                string msg = "";
                if (base64Upload != "")
                {
                    string filePath = Server.MapPath("~/Uploads/" + fileName);
                    CommonClass.MoveUploadedFile(filePath, fileName);
                    string UploadFilePath = Credentials.fileUploadsPath + fileName;
                    if (CommonClass.IfFileExists(UploadFilePath))
                    {
                        string s = Credentials.UploadDocumentAttachment(DocNo, base64Upload, UploadFilePath, TableID);
                        if (s == "SUCCESS")
                        {
                            msg = "Attachment File Uploaded Successfully";
                            successVal = true;
                        }
                        else
                        {
                            msg = s;
                            successVal = false;
                        }
                    }
                }
                return Json(new { message = msg, success = successVal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult DocumentAttachmentsToApprove(string DocNo, int TableID)
        {
            #region Document Attachment
            List<DocumentAttachment> DocAttachment = new List<DocumentAttachment>();
            string page = "DocumentAttachment?$filter=Table_ID eq " + TableID + " and No eq '" + DocNo + "'&format=json";

            HttpWebResponse httpResponseResC = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);

                foreach (JObject config in details["value"])
                {
                    DocumentAttachment docAttList = new DocumentAttachment();
                    docAttList.TabelID = (int)config["Table_ID"];
                    docAttList.No = (string)config["No"];
                    docAttList.FileName = (string)config["File_Name"];
                    docAttList.FileExt = (string)config["File_Extension"];
                    docAttList.ID = (int)config["ID"];
                    docAttList.LineNo = (string)config["Line_No"];
                    docAttList.DocType = (string)config["Document_Type"];
                    DocAttachment.Add(docAttList);
                }
            }
            #endregion
            return PartialView("~/Views/Shared/Partial Views/ImportantDocsToApprove.cshtml", DocAttachment);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DocumentAttachmentview(int tblID, string No, int ID, string fileName, string ext)
        {
            try
            {
                bool success = false, view = false;
                string msg = "";
                string Attachment = Credentials.GetDocumentAttachmet(tblID, No, ID);

                string fName = fileName + "." + ext;
                Byte[] bytes = Convert.FromBase64String(Attachment);
                string path = Server.MapPath("~/Uploads/" + fName);
                Credentials.DownloadAttachment(path, bytes);
                msg = fName;
                view = false;
                success = true;
                
                return Json(new { message = msg, success, view }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false, view = false }, JsonRequestBehavior.AllowGet); ;
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult CourseDocumentAttachmentview(int tblID, string No, int ID, string fileName, string ext)
        {
            try
            {
                bool success = false, view = false;
                string msg = "";
                string Attachment = Credentials.GetCourseDocumentAttachmet(tblID, No);

                string fName = fileName + "." + ext;
                Byte[] bytes = Convert.FromBase64String(Attachment);
                string path = Server.MapPath("~/Uploads/" + fName);
                Credentials.DownloadAttachment(path, bytes);
                msg = fName;
                view = false;
                success = true;

                return Json(new { message = msg, success, view }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false, view = false }, JsonRequestBehavior.AllowGet); ;
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteAttachedDocument(string DocNo, int tblID, int DocID)
        {
            try
            {
                Credentials.ObjNav.DeleteDocumentAttachment(DocNo, tblID, DocID);
                return Json(new { message = "Attachmet file deleted successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public virtual ActionResult AttachmentDownload(string fileName)
        {
            string fullPath = Server.MapPath("~/Uploads/" + fileName);
            return File(fullPath, "application/octet-stream", fileName);
        }
        public ActionResult ErrorMessange()
        {
            return View();
        }
        public PartialViewResult NotificationMessages()
        {
            List<Notifications> notList = new List<Notifications>();
            string pageLine = "CompayInformation?$select=Notificaion,Start_date&$filter=Category eq 'Staff'&$format=json";
            HttpWebResponse httpResponse = Credentials.GetOdataData(pageLine);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    Notifications newNotif = new Notifications();
                    newNotif.Message = (string)config["Notificaion"];
                    newNotif.StartDate = (DateTime)config["Start_date"];
                    notList.Add(newNotif);
                }
            }
            return PartialView("~/Views/Common/Notification.cshtml", notList.OrderByDescending(x => x.StartDate).ToList());
        }
        public ActionResult GetNotice()
        {
            List<NoticeBoard> notList = new List<NoticeBoard>();
            string pageLine = "StudentNoticeBoard?$select=Description,Campus,Active,Date_Posted&$format=json";
            HttpWebResponse httpResponse = Credentials.GetOdataData(pageLine);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    bool check = ((bool)config["Active"]);
                    NoticeBoard noticeBoard = new NoticeBoard();
                    if (check)
                    {
                        noticeBoard.Description = (string)config["Description"];
                        noticeBoard.Campus = (string)config["Campus"];
                        noticeBoard.DatePosted = (string)config["Date_Posted"];
                        notList.Add(noticeBoard);
                    }
                }
            }

            return View(notList.OrderByDescending(x => x.DatePosted).ToList());
        }
    }
}