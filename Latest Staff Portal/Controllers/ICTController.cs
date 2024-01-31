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
    [CustomAuthorization(Role = "FULLTIME,PARTTIME")]
    public class ICTController : Controller
    {
        // GET: ICT
        public ActionResult ICTRequisitionList()
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
        public PartialViewResult ICTListPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<ICTRequest> ICTReqList = new List<ICTRequest>();

                string page = "ICTRequisition?$filter=Requested_By eq '" + StaffNo + "'&$format=json";

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
                        ICTList.Campus = (string)config["Global_Dimension_1_Code"];
                        ICTList.Department = (string)config["Global_Dimension_2_Code"];
                        ICTList.Description = (string)config["General_Description"];
                        ICTList.Urgency = (string)config["Urgency_Priority"];
                        ICTList.RequiredDate = Convert.ToDateTime((string)config["Required_Date"]).ToString("dd/MM/yyyy");
                        ICTList.Status = (string)config["Resolution_Status"];
                        ICTList.Resoltion_Remarks = (string)config["Resoltion_Remarks"];
                        ICTReqList.Add(ICTList);
                    }
                }
                return PartialView("~/Views/ICT/ICTListView.cshtml", ICTReqList.OrderByDescending(x => x.No));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewICTRequest()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                NewICTRequisition NewICTReq = new NewICTRequisition();
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
                    ListOfCategory = CategoryList.Select(x =>
                                       new SelectListItem()
                                       {
                                           Text = x.Text,
                                           Value = x.Value
                                       }).ToList()
                };
                return PartialView("~/Views/ICT/NewICTRequest.cshtml", NewICTReq);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitICTRequest(ICTRequest NewReq, List<ICTRequestLines> ICTReqLines)
        {
            try
            {
                DateTime requireddate = DateTime.ParseExact(NewReq.RequiredDate.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                string DocNo = Credentials.ObjNav.ICTRequisitionCreate(Session["username"].ToString(), NewReq.Campus,
                     NewReq.Department, Convert.ToInt32(NewReq.Urgency), requireddate, NewReq.Description, NewReq.ReqCat);
                if (DocNo != "")
                {
                    foreach (var c in ICTReqLines)
                    {
                        string Descriprion = c.Description.Trim();
                        string Quantity = c.Quantity.Trim();
                        Credentials.ObjNav.InsertICTRequisitionLines(DocNo, Descriprion,Convert.ToInt32(Quantity));
                    }
                }
                return Json(new { message = "ICT Requisition DocNo " + DocNo + " Submitted Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
