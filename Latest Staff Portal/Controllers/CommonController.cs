using Latest_Staff_Portal.CustomSecurity;
using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Latest_Staff_Portal.Controllers
{
    [CustomeAuthentication]
    [CustomAuthorization(Role = "ALLUSERS")]
    public class CommonController : Controller
    {
        // GET: Common        
        public JsonResult GetServiceList()
        {
            try
            {
                #region Service List
                List<DropdownList> ddlList = new List<DropdownList>();
                string page = "Item_Service?$select=No,Name&$filter=Gen_Prod_Posting_Group eq 'SERVICES' and Account_Type eq 'Posting' and Direct_Posting eq true&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList dll = new DropdownList();
                        dll.Value = (string)config["No"];
                        dll.Text = (string)config["Name"];
                        ddlList.Add(dll);
                    }
                }
                #endregion
                DropdownListData DropDownData = new DropdownListData
                {
                    ListOfddlData = ddlList.Select(x =>
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
        public JsonResult GetItemList()
        {
            try
            {
                #region Items List
                List<DropdownList> ddlList = new List<DropdownList>();
                string page = "Item_List?$&orderby=Description&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList dll = new DropdownList();
                        dll.Value = (string)config["No"];
                        dll.Text = (string)config["Description"];
                        ddlList.Add(dll);
                    }
                }
                #endregion
                DropdownListData DropDownData = new DropdownListData
                {
                    ListOfddlData = ddlList.Select(x =>
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
        public JsonResult GetFixedAssetList()
        {
            try
            {
                #region Items List
                List<DropdownList> ddlList = new List<DropdownList>();
                string page = "FixedAssetsList?$&orderby=Description&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList dll = new DropdownList();
                        dll.Value = (string)config["No"];
                        dll.Text = (string)config["Description"] + "~" + (string)config["Search_Description"];
                        ddlList.Add(dll);
                    }
                }
                #endregion
                DropdownListData DropDownData = new DropdownListData
                {
                    ListOfddlData = ddlList.Select(x =>
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
        public void NullibySessions()
        {
            Session["SuccessMsg"] = null;
            Session["ErrorMsg"] = null;
        }
        public PartialViewResult DocumentApprovalTrail(string DocNo, string RecID)
        {
            string StaffNo = Session["Username"].ToString();
            List<ApprovalEntries> ApprovalTrail = new List<ApprovalEntries>();

            string page = "ApprovalEntries?$filter=Record_ID_to_Approve eq '" + RecID + "' and Status ne 'Canceled' and Status ne 'Rejected'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    ApprovalEntries AppTra = new ApprovalEntries();
                    AppTra.DocNo = DocNo;
                    AppTra.UserID = (string)config["ApproverNames"];
                    AppTra.DateSendForApproval = ((DateTime)config["Date_Time_Sent_for_Approval"]).ToString("dd/MM/yyyy");
                    AppTra.DueDate = ((DateTime)config["Due_Date"]).ToString("dd/MM/yyyy");
                    AppTra.Status = (string)config["Status"];
                    AppTra.Sequence = Convert.ToInt32((string)config["Sequence_No"]);
                    ApprovalTrail.Add(AppTra);
                }
            }
            return PartialView("~/Views/Shared/Partial Views/ApprovalTrail.cshtml", ApprovalTrail.OrderBy(x => x.Sequence).ToList());
        }
        string UrlEncode(string url)
        {
            Dictionary<string, string> toBeEncoded = new Dictionary<string, string>() { { "%", "%25" }, { "!", "%21" }, { "#", "%23" }, { " ", "%20" },
            { "$", "%24" }, { "&", "%26" }, { "'", "%27" }, { "(", "%28" }, { ")", "%29" }, { "*", "%2A" }, { "+", "%2B" }, { ",", "%2C" },
            { "/", "%2F" }, { ":", "%3A" }, { ";", "%3B" }, { "=", "%3D" }, { "?", "%3F" }, { "@", "%40" }, { "[", "%5B" }, { "]", "%5D" } };
            Regex replaceRegex = new Regex(@"[%!# $&'()*+,/:;=?@\[\]]");
            MatchEvaluator matchEval = match => toBeEncoded[match.Value];
            string encoded = replaceRegex.Replace(url, matchEval);
            return encoded;
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public PartialViewResult FileUploadForm()
        {
            return PartialView("~/Views/Shared/Partial Views/FileAttachmentForm.cshtml");
        }
        //[AcceptVerbs(HttpVerbs.Get)]
        //public JsonResult GetCommonDropdwnListData()
        //{
        //    try
        //    {
        //        #region Campus List
        //        List<DimensionValues> Campuses = new List<DimensionValues>();
        //        string pageCampus = "DimensionValues?$filter=Global_Dimension_No_ eq 1&$format=json";

        //        HttpWebResponse httpResponseCampus = Credentials.GetOdataData(pageCampus);
        //        using (var streamReader = new StreamReader(httpResponseCampus.GetResponseStream()))
        //        {
        //            var result = streamReader.ReadToEnd();

        //            var details = JObject.Parse(result);


        //            foreach (JObject config in details["value"])
        //            {
        //                DimensionValues CmpList = new DimensionValues();
        //                CmpList.Code = (string)config["Code"];
        //                CmpList.Name = (string)config["Name"];
        //                Campuses.Add(CmpList);
        //            }
        //        }
        //        #endregion
        //        #region School
        //        List<DimensionValues> School = new List<DimensionValues>();
        //        string pageSchool = "DimensionValues?$filter=Global_Dimension_No_ eq 3&$format=json";

        //        HttpWebResponse httpResponseSchool = Credentials.GetOdataData(pageSchool);
        //        using (var streamReader = new StreamReader(httpResponseSchool.GetResponseStream()))
        //        {
        //            var result = streamReader.ReadToEnd();

        //            var details = JObject.Parse(result);


        //            foreach (JObject config in details["value"])
        //            {
        //                DimensionValues SchoolList = new DimensionValues();
        //                SchoolList.Code = (string)config["Code"];
        //                SchoolList.Name = (string)config["Name"];
        //                School.Add(SchoolList);
        //            }
        //        }
        //        #endregion
        //        #region Department List
        //        List<DimensionValues> Department = new List<DimensionValues>();
        //        string pageDepartment = "DimensionValues?$filter=Global_Dimension_No_ eq 2&$format=json";

        //        HttpWebResponse httpResponseDepartment = Credentials.GetOdataData(pageDepartment);
        //        using (var streamReader = new StreamReader(httpResponseDepartment.GetResponseStream()))
        //        {
        //            var result = streamReader.ReadToEnd();

        //            var details = JObject.Parse(result);


        //            foreach (JObject config in details["value"])
        //            {
        //                DimensionValues DepartmentList = new DimensionValues();
        //                DepartmentList.Code = (string)config["Code"];
        //                DepartmentList.Name = (string)config["Name"];
        //                Department.Add(DepartmentList);
        //            }
        //        }
        //        #endregion
        //        #region Responsibility
        //        List<RespCenter> RespCList = new List<RespCenter>();
        //        string pageResC = "ResponsibilityCenters?$format=json";

        //        HttpWebResponse httpResponseResC = Credentials.GetOdataData(pageResC);
        //        using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
        //        {
        //            var result = streamReader.ReadToEnd();

        //            var details = JObject.Parse(result);


        //            foreach (JObject config in details["value"])
        //            {
        //                RespCenter RCList = new RespCenter();
        //                RCList.Code = (string)config["Code"];
        //                RCList.Name = (string)config["Name"];
        //                RespCList.Add(RCList);
        //            }
        //        }
        //        #endregion
        //        CommonDropDownList DropDownData = new CommonDropDownList
        //        {
        //            ListOfSchools = School.Select(x =>
        //                             new SelectListItem()
        //                             {
        //                                 Text = x.Name,
        //                                 Value = x.Code
        //                             }).ToList(),
        //            ListOfDepartments = Department.Select(x =>
        //                            new SelectListItem()
        //                            {
        //                                Text = x.Name,
        //                                Value = x.Code
        //                            }).ToList(),
        //            ListOfCampus = Campuses.Select(x =>
        //                               new SelectListItem()
        //                               {
        //                                   Text = x.Name,
        //                                   Value = x.Code
        //                               }).ToList(),
        //            ListOfRespC = RespCList.Select(x =>
        //                               new SelectListItem()
        //                               {
        //                                   Text = x.Name,
        //                                   Value = x.Code
        //                               }).ToList()
        //        };
        //        return Json(new { DropDownData, success = true }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
        //    }
        //}
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
                    docAttList.TabelID = (string)config["Table_ID"];
                    docAttList.No = (string)config["No"];
                    docAttList.FileName = (string)config["Name"];
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
                if (base64Upload != "" && Extn != "")
                {
                    string ext = Path.GetExtension(fileName);

                    if (ext.ToLower() == ".pdf" || ext.ToLower() == ".docx" || ext.ToLower() == ".doc" || ext.ToLower() == ".xlsx" ||
                        ext.ToLower() == ".jpeg" || ext.ToLower() == ".jpg" || ext.ToLower() == ".png")
                    {
                        string filePath = Server.MapPath("~/Uploads/" + fileName);
                        string s = Credentials.UploadDocumentAttachment(DocNo, base64Upload, filePath, TableID);
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
                    else
                    {
                        msg = "Only files with extensions(.pdf, .docx, .doc, .xlsx, .jpeg, .jpg, .png) can be uploaded";
                        successVal = false;
                    }
                }
                else
                {
                    msg = "Incorrect file!!";
                    successVal = false;
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
            string page = "DocumentAttachment?$filter=Table_ID eq " + TableID + " and No eq '" + DocNo + "'&$format=json";

            HttpWebResponse httpResponseResC = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);

                foreach (JObject config in details["value"])
                {
                    DocumentAttachment docAttList = new DocumentAttachment();
                    docAttList.TabelID = (string)config["Table_ID"];
                    docAttList.No = (string)config["No"];
                    docAttList.FileName = (string)config["Name"];
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
        [HttpGet]
        public virtual ActionResult AttachmentDownload(string fileName)
        {
            string fullPath = Server.MapPath("~/Uploads/" + fileName);
            return File(fullPath, "application/octet-stream", fileName);
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
                //if (ext == "doc" || ext == "docx" || ext == "xlsx" || ext == "csv")
                //{
                //    string fName = fileName + "." + ext;
                //    Byte[] bytes = Convert.FromBase64String(Attachment);
                //    string path = Server.MapPath("~/Uploads/" + fName);
                //    Credentials.DownloadAttachment(path, bytes);
                //    msg = fName;
                //    view = false;
                //    success = true;
                //    return Json(new { message = msg, success, view }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    //msg = Attachment;
                //    view = true;
                //    success = true;
                //    //JavaScriptSerializer serializer = new JavaScriptSerializer();
                //    //serializer.MaxJsonLength = Int32.MaxValue;
                //    //return Json(new { message = serializer.Serialize(Attachment), success, view }, JsonRequestBehavior.AllowGet);


                //}
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
        public PartialViewResult MyDocumentComments(string DocNo)
        {
            try
            {
                string userID = Session["UserID"].ToString();
                List<DocComments> docComments = new List<DocComments>();

                string page = "ApprovalComments?select=Comment&$filter=Document_No eq '" + DocNo + "' and User_ID eq '" + userID + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        DocComments comment = new DocComments();
                        comment.Comment = config["Comment"].ToString();
                        docComments.Add(comment);
                    }
                }
                return PartialView("~/Views/DocumentApproval/Document Approval Views/DocumentComments.cshtml", docComments);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult DocumentComments(string DocNo)
        {
            try
            {
                string userID = Session["UserID"].ToString();
                List<DocComments> docComments = new List<DocComments>();

                string page = "ApprovalComments?select=Comment,User_ID&$filter=Document_No eq '" + DocNo + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        DocComments comment = new DocComments();
                        comment.Comment = config["Comment"].ToString();
                        string[] s = config["User_ID"].ToString().Split('\\');
                        string EmplName = CommonClass.GetEmployeeName(s[1]);
                        if (EmplName != null)
                        {
                            comment.CommentBy = EmplName;
                        }
                        else
                        {
                            comment.CommentBy = (string)config["User_ID"];
                        }
                        docComments.Add(comment);
                    }
                }
                return PartialView("~/Views/Shared/Partial Views/ApprovalTrail.cshtml", docComments);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public ActionResult ErrorMessange()
        {
            return View();
        }
        public ActionResult Unauthorized()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetEmployeeList()
        {
            try
            {
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
                return Json(new { ddlList = EmployeeList, success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult DocumentApprovalComments(string RecID)
        {
            try
            {
                List<DocComments> ApprovalComments = new List<DocComments>();
                string page = "ApprovalComments?$select=Comment,User_ID&$filter=Record_ID_to_Approve eq '" + RecID + "'&$format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);
                    if (details["value"].Count() > 0)
                    {
                        foreach (JObject config in details["value"])
                        {
                            DocComments newComm = new DocComments();
                            newComm.Comment = (string)config["Comment"];
                            string[] s = config["User_ID"].ToString().Split('\\');
                            string EmplName = CommonClass.GetEmployeeName(s[1]);
                            if (EmplName == "")
                            {
                                newComm.CommentBy = (string)config["User_ID"];
                            }
                            else
                            {
                                newComm.CommentBy = EmplName;
                            }
                            //newComm.Sequence = (int)config["Sequence_No"];
                            ApprovalComments.Add(newComm);
                        }
                    }
                }
                return PartialView("~/Views/Shared/Partial Views/DocumentApprovalComments.cshtml", ApprovalComments.OrderBy(x => x.Sequence));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult CommonActions(string DocNo,string Status,string DocType)
        {
            try
            {
                DocumentNumber docNo = new DocumentNumber();
                docNo.Code = DocNo;
                docNo.Status = Status;
                docNo.DocType = DocType;
                return PartialView("~/Views/Common/CommonAction.cshtml", docNo);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult WorkPlanActionLinks(string DocNo, string Obj,string EntryNo)
        {
            try
            {
                IndividualObjectives docNo = new IndividualObjectives();
                docNo.Code = DocNo;
                docNo.Obj = Obj;
                docNo.EntryNo = EntryNo;
                return PartialView("~/Views/Common/WorkPlanActionLinks.cshtml", docNo);
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