using Latest_Staff_Portal.CustomSecurity;
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
    [CustomeAuthentication]
    [CustomAuthorization(Role = "ALLUSERS")]
    public class ICTController : Controller
    {
        // GET: ICT
        #region ICT Req
        public ActionResult ICTRequisitionList()
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
        public PartialViewResult ICTListPartialView()
        {
            string StaffNo = Session["Username"].ToString();
            List<ICTRequest> ICTReqList = new List<ICTRequest>();

            string page = "ICTRequisition?$filter=Requested_By eq '" + StaffNo + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    ICTRequest ICTList = new ICTRequest();
                    ICTList.No = (string)config["No"];
                    ICTList.Date = Convert.ToDateTime((string)config["Date"]).ToString("dd/MM/yyyy");
                    ICTList.ReqCat = (string)config["Requisition_Category"];
                    ICTList.Directorate = (string)config["Global_Dimension_1_Code"];
                    ICTList.Department = (string)config["Global_Dimension_2_Code"];
                    ICTList.Description = (string)config["General_Description"];
                    ICTList.Urgency = (string)config["Urgency_Priority"];
                    ICTList.RequiredDate = Convert.ToDateTime((string)config["Required_Date"]).ToString("dd/MM/yyyy");
                    ICTList.Status = (string)config["Resolution_Status"];
                    ICTList.Assignee = (string)config["Assignee_Name"];
                    ICTList.Resoltion_Remarks = (string)config["Resolution_Remarks"];
                    ICTReqList.Add(ICTList);
                }
            }
            return PartialView("~/Views/ICT/ICTListView.cshtml", ICTReqList);
        }
        public PartialViewResult NewICTRequest()
        {
            string StaffNo = Session["Username"].ToString();
            string Dir = "", Dep = "";
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
                        Dir = (string)config["_x003C_GlobSal_Dimension_1_Code_x003E_"];
                        Dep = (string)config["GlobalDimension2Code"];
                    }
                }
            }
            #endregion
            if (Dir == "")
            {
                Error erroMsg = new Error();
                erroMsg.Message = "Your directorate has not been set. Contact HR";
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
                NewICTRequisition NewICTReq = new NewICTRequisition();
                #region Directorate List
                List<DimensionValues> DirectorateList = new List<DimensionValues>();
                string pageDir = "DimensionValues?$filter=Dimension_Code eq 'DIRECTORATES'&$format=json";

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
                #region Categories
                List<DropdownList> CategoryList = new List<DropdownList>();
                string pageResC = "ICTRequisitionCategory?$format=json";

                HttpWebResponse httpResponseResC = Credentials.GetOdataData(pageResC);
                using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList CatList = new DropdownList();
                        CatList.Value = (string)config["Code"];
                        CatList.Text = (string)config["Description"];
                        CategoryList.Add(CatList);
                    }
                }
                #endregion
                NewICTReq = new NewICTRequisition
                {
                    Directorate = Dir,
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
                    ListOfCategory = CategoryList.Select(x =>
                                       new SelectListItem()
                                       {
                                           Text = x.Text,
                                           Value = x.Value
                                       }).ToList()
                };
                return PartialView("~/Views/ICT/NewICTRequest.cshtml", NewICTReq);
            }
        }
        public PartialViewResult CancelICTRequestForm(string DocNo)
        {
            ICTCancel c = new ICTCancel();
            c.DocNo = DocNo;
            return PartialView("~/Views/ICT/CancelRemarks.cshtml", c);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitICTRequest(ICTRequest NewReq)
        {
            try
            {
                DateTime requireddate = DateTime.ParseExact(NewReq.RequiredDate.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var username = Session["username"].ToString();

                string DocNo = Credentials.ObjNav.ICTRequisitionCreate(username, NewReq.Directorate,
                     NewReq.Department, Convert.ToInt32(NewReq.Urgency), requireddate, NewReq.Description, NewReq.ReqCat);
                //if (DocNo != "")
                //{
                //    foreach (var c in ICTReqLines)
                //    {
                //        string Descriprion = c.Description.Trim();
                //        string Quantity = c.Quantity.Trim();
                //        Credentials.ObjNav.InsertICTRequisitionLines(DocNo, Descriprion, Convert.ToInt32(Quantity));
                //    }
                //}
                return Json(new { message = "ICT Requisition DocNo " + DocNo + " Submitted Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult CancelICTRequest(string DocNo,string CancelR)
        {
            try
            {
                Credentials.ObjNav.CancelICTRequisitionCreate(DocNo, CancelR);
               
                return Json(new { message = "ICT Requisition DocNo " + DocNo + " cancelled Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult ConfirmICTRequestForm(string DocNo)
        {
            ICTCancel c = new ICTCancel();
            c.DocNo = DocNo;
            return PartialView("~/Views/ICT/ConfirmRemarks.cshtml", c);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ConfirmICTRequest(string DocNo,string Resolved, string ConfirmR)
        {
            try
            {
                Credentials.ObjNav.ConfirmClosureOfICTRequisition(DocNo, ConfirmR);

                return Json(new { message = "Confirmation Submitted Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region ICT Asste Req
        public ActionResult ICTAssetTransferList()
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
        public PartialViewResult ICTAssetTransferListPartialView()
        {
            string StaffNo = Session["Username"].ToString();
            List<ICTAssetRequest> ICTAssetReqList = new List<ICTAssetRequest>();

            string page = "AssetMvtCard?$filter=Requestor eq '" + StaffNo + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    ICTAssetRequest RegList = new ICTAssetRequest();
                    RegList.DocNo = (string)config["Doc_No"];
                    RegList.Asset = (string)config["Asset_No"];
                    RegList.Description = (string)config["Asset_Description"];
                    RegList.Requestor_No = (string)config["Requestor"];
                    RegList.Requestor_Name = (string)config["Requestor_Name"];
                    RegList.Date_Requested = Convert.ToDateTime((string)config["Date_Requested"]).ToString("dd/MM/yyyy");
                    RegList.Date_Moved = Convert.ToDateTime((string)config["Date_Moved"]).ToString("dd/MM/yyyy");
                    RegList.Date_Returned = Convert.ToDateTime((string)config["Date_Returned"]).ToString("dd/MM/yyyy");
                    RegList.Status = (string)config["Status"];
                    RegList.Remarks = (string)config["Remarks"];
                    ICTAssetReqList.Add(RegList);
                }
            }
            return PartialView("~/Views/ICT/ICTAssetTransferListView.cshtml", ICTAssetReqList.OrderByDescending(x => x.DocNo));
        }
        public PartialViewResult NewICTTransferRequest()
        {
            string StaffNo = Session["Username"].ToString();
            string Dir = "", Dep = "";
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
                        Dir = (string)config["_x003C_GlobSal_Dimension_1_Code_x003E_"];
                        Dep = (string)config["GlobalDimension2Code"];
                    }
                }
            }
            #endregion
            if (Dir == "")
            {
                Error erroMsg = new Error();
                erroMsg.Message = "Your directorate has not been set. Contact HR";
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
                NewICTRequisition NewICTReq = new NewICTRequisition();
                #region Directorate List
                List<DimensionValues> DirectorateList = new List<DimensionValues>();
                string pageDir = "DimensionValues?$filter=Dimension_Code eq 'DIRECTORATES'&$format=json";

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
                NewICTReq = new NewICTRequisition
                {
                    Directorate = Dir,
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
                                        }).ToList()
                };
                return PartialView("~/Views/ICT/NewICTAssetRequest.cshtml", NewICTReq);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitICTTransferRequest(ICTAssetRequest NewReq)
        {
            try
            {
                DateTime requireddate = DateTime.ParseExact(NewReq.Date_Requested.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                string DocNo = Credentials.ObjNav.SubmitICTAssetMovement(Session["username"].ToString(), requireddate, NewReq.Description,NewReq.reason);

                return Json(new { message = "ICT Asset Requisition DocNo " + DocNo + " Submitted Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult ICTAssetTransferDocView()
        {
            string StaffNo = Session["Username"].ToString();
            ICTAssetRequest RegDoc = new ICTAssetRequest();

            string page = "AssetMvtCard?$filter=Requestor eq '" + StaffNo + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    RegDoc.DocNo = (string)config["Doc_No"];
                    RegDoc.Asset = (string)config["Asset_No"];
                    RegDoc.Description = (string)config["Asset_Description"];
                    RegDoc.Requestor_No = (string)config["Requestor"];
                    RegDoc.Requestor_Name = (string)config["Requestor_Name"];

                    DateTime d = Convert.ToDateTime(new DateTime(0));
                    if ((DateTime)config["Date_Requested"] != new DateTime(0))
                    {
                        RegDoc.Date_Requested = Convert.ToDateTime((string)config["Date_Requested"]).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        RegDoc.Date_Requested = "";
                    }
                    if ((DateTime)config["Date_Requested"] != new DateTime(0))
                    {
                        RegDoc.Date_Moved = Convert.ToDateTime((string)config["Date_Moved"]).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        RegDoc.Date_Moved = "";
                    }
                    if ((DateTime)config["Date_Requested"] != new DateTime(0))
                    {
                        RegDoc.Date_Returned = Convert.ToDateTime((string)config["Date_Returned"]).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        RegDoc.Date_Returned = "";
                    }
                    RegDoc.Status = (string)config["Status"];
                    RegDoc.Remarks = (string)config["Remarks"];
                }
            }
            return PartialView("~/Views/ICT/ICTAssetRequestDocument.cshtml", RegDoc);
        }
        #endregion

        #region ICT Service Req
        public ActionResult ICTServMntList()
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
        public PartialViewResult ICTServMntListPartialView()
        {
            string StaffNo = Session["Username"].ToString();
            List<ICTServiceRequest> ServReqList = new List<ICTServiceRequest>();

            string page = "ICT_Service_Maintenance_Card?$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    ICTServiceRequest Reg = new ICTServiceRequest();
                    Reg.DocNo = (string)config["Doc_No"];
                    Reg.Asset = (string)config["Asset_No"];
                    Reg.Description = (string)config["Asset_Description"];
                    Reg.ServiceDate = Convert.ToDateTime((string)config["Service_Date"]).ToString("dd/MM/yyyy");
                    Reg.LastServiceDate = Convert.ToDateTime((string)config["Last_Service_Date"]).ToString("dd/MM/yyyy");
                    Reg.NextSeviceDate = Convert.ToDateTime((string)config["Next_Service_Date"]).ToString("dd/MM/yyyy");
                    Reg.Status = (string)config["Service_Status"];
                    Reg.Remarks = (string)config["Service_Details"];
                    ServReqList.Add(Reg);
                }
            }
            return PartialView("~/Views/ICT/ICTAssetServiceReqListView.cshtml", ServReqList.OrderByDescending(x => x.DocNo));
        }
        public PartialViewResult NewICTServMntRequest()
        {
            string StaffNo = Session["Username"].ToString();
            string Dir = "", Dep = "";
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
                        Dir = (string)config["_x003C_GlobSal_Dimension_1_Code_x003E_"];
                        Dep = (string)config["GlobalDimension2Code"];
                    }
                }
            }
            #endregion
            if (Dir == "")
            {
                Error erroMsg = new Error();
                erroMsg.Message = "Your directorate has not been set. Contact HR";
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
                NewICTRequisition NewICTReq = new NewICTRequisition();
                #region Directorate List
                List<DimensionValues> DirectorateList = new List<DimensionValues>();
                string pageDir = "DimensionValues?$filter=Dimension_Code eq 'DIRECTORATES'&$format=json";

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
                #region ICT Asset List
                List<DropdownList> ICTAssetList = new List<DropdownList>();
                string pageICTAsset = "ICTAssetRegister?$format=json";

                HttpWebResponse httpResponseICTAsset = Credentials.GetOdataData(pageICTAsset);
                using (var streamReader = new StreamReader(httpResponseICTAsset.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DropdownList Asset = new DropdownList();
                        Asset.Value = (string)config["Asset_No"];
                        Asset.Text = (string)config["Asset_Description"];
                        ICTAssetList.Add(Asset);
                    }
                }
                #endregion
                NewICTReq = new NewICTRequisition
                {
                    Code = "",
                    Directorate = Dir,
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
                    ListOfICTAsset = ICTAssetList.Select(x =>
                                        new SelectListItem()
                                        {
                                            Text = x.Text,
                                            Value = x.Value
                                        }).ToList()
                };
                return PartialView("~/Views/ICT/NewICTServiceRequest.cshtml", NewICTReq);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitICTServmntRequest(ICTServiceRequest NewReq)
        {
            try
            {
                DateTime ServiceD = DateTime.ParseExact(NewReq.ServiceDate.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                string DocNo = Credentials.ObjNav.SubmitICTService_mainRequest(Session["username"].ToString(), NewReq.Asset, ServiceD,
                    Convert.ToInt32(NewReq.Category), NewReq.Description);

                return Json(new { message = "ICT Asset Requisition DocNo " + DocNo + " Submitted Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult ICTServMntDocView(string DocNo)
        {
            string StaffNo = Session["Username"].ToString();
            ICTServiceRequest RegDoc = new ICTServiceRequest();

            string page = "ICT_Service_Maintenance_Card?$filter=Doc_No eq '" + DocNo + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    RegDoc.DocNo = (string)config["Doc_No"];
                    RegDoc.Asset = (string)config["Asset_No"];
                    RegDoc.Description = (string)config["Asset_Description"];
                    RegDoc.Date = Convert.ToDateTime((string)config["Date_Created"]).ToString("dd/MM/yyyy");
                    RegDoc.ServiceDate = Convert.ToDateTime((string)config["Service_Date"]).ToString("dd/MM/yyyy");
                    RegDoc.LastServiceDate = Convert.ToDateTime((string)config["Last_Service_Date"]).ToString("dd/MM/yyyy");
                    RegDoc.NextSeviceDate = Convert.ToDateTime((string)config["Next_Service_Date"]).ToString("dd/MM/yyyy");
                    RegDoc.Status = (string)config["Service_Status"];
                    RegDoc.Remarks = (string)config["Service_Details"];
                }
            }
            return PartialView("~/Views/ICT/ServiceDocView.cshtml", RegDoc);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateICTServmntRequest(string DocNo,string LServDate,string NxtServDate)
        {
            try
            {
                DateTime LSDte = DateTime.ParseExact(LServDate.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime NSDate = DateTime.ParseExact(NxtServDate.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);

              Credentials.ObjNav.UpdateICTService_mainRequest(DocNo,"",DateTime.Today, LSDte, NSDate);

                return Json(new { message = "ICT Asset Requisition DocNo " + DocNo + " Submitted Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        public PartialViewResult DocumentCommentsView()
        {
            string StaffNo = Session["Username"].ToString();
            ICTAssetRequest RegDoc = new ICTAssetRequest();

            string page = "AssetMvtCard?$filter=Requestor eq '" + StaffNo + "'&format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                foreach (JObject config in details["value"])
                {
                    RegDoc.DocNo = (string)config["Doc_No"];
                    RegDoc.Asset = (string)config["Asset_No"];
                    RegDoc.Description = (string)config["Asset_Description"];
                    RegDoc.Requestor_No = (string)config["Requestor"];
                    RegDoc.Requestor_Name = (string)config["Requestor_Name"];

                    DateTime d = Convert.ToDateTime(new DateTime(0));
                    if ((DateTime)config["Date_Requested"] != new DateTime(0))
                    {
                        RegDoc.Date_Requested = Convert.ToDateTime((string)config["Date_Requested"]).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        RegDoc.Date_Requested = "";
                    }
                    if ((DateTime)config["Date_Requested"] != new DateTime(0))
                    {
                        RegDoc.Date_Moved = Convert.ToDateTime((string)config["Date_Moved"]).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        RegDoc.Date_Moved = "";
                    }
                    if ((DateTime)config["Date_Requested"] != new DateTime(0))
                    {
                        RegDoc.Date_Returned = Convert.ToDateTime((string)config["Date_Returned"]).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        RegDoc.Date_Returned = "";
                    }
                    RegDoc.Status = (string)config["Status"];
                    RegDoc.Remarks = (string)config["Remarks"];
                }
            }
            return PartialView("~/Views/ICT/DocumentComments.cshtml", RegDoc);
        }
    }
}
