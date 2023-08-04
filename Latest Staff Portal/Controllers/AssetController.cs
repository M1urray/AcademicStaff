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
    public class AssetController : Controller
    {
        // GET: Asset
        public ActionResult AssignedAssetList()
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
        public PartialViewResult AssignedAssetPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<FixedAsset> AssignedAssetList = new List<FixedAsset>();
                {
                    string page = "Fixed_Asset?$select=No,Description&$filter=Assigned_Employee eq '" + StaffNo + "'&$format=json";

                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        foreach (JObject config in details["value"])
                        {
                            FixedAsset AssgnedAsset = new FixedAsset();
                            AssgnedAsset.AssetNo = (string)config["No"];
                            AssgnedAsset.Description = (string)config["Description"];
                            AssignedAssetList.Add(AssgnedAsset);
                        }
                    }
                    return PartialView("~/Views/Asset/Partial Views/AssignedAssetList.cshtml", AssignedAssetList);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }

        public ActionResult AssetTransferList()
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
        public PartialViewResult AssetTransferPartialView()
        {
            try
            {
                string StaffNo = Session["Username"].ToString();
                List<AssetTransfer> AssignedAssetList = new List<AssetTransfer>();
                {
                    string page = "AssetTransferList?$filter=FromResponsibleEmployee eq '" + StaffNo + "'&$format=json";

                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var details = JObject.Parse(result);
                        foreach (JObject config in details["value"])
                        {
                            AssetTransfer AssgnedAsset = new AssetTransfer();
                            AssgnedAsset.No = (string)config["No"];
                            AssgnedAsset.RaisedBy = (string)config["RaisedBy"];
                            AssgnedAsset.TransferType = (string)config["TransferType"];
                            AssgnedAsset.Type = (string)config["Type"];
                            AssgnedAsset.AssettoTransfer = (string)config["AssettoTransfer"];
                            AssgnedAsset.AssetDescription = (string)config["AssetDescription"];
                            AssgnedAsset.Status = (string)config["Status"];
                            AssgnedAsset.Transferred = (string)config["Transferred"];
                            AssgnedAsset.Comments = (string)config["Comments"];
                            AssgnedAsset.FromLocation = (string)config["FromLocation"];
                            AssgnedAsset.ToLocation = (string)config["ToLocation"];
                            AssgnedAsset.FromResponsibleEmployee = (string)config["FromResponsibleEmployee"];
                            AssgnedAsset.ToResponsibleEmployee = (string)config["ToResponsibleEmployee"];
                            AssgnedAsset.FromDimension1Code = (string)config["FromDimension1Code"];
                            AssgnedAsset.FromDimension2Code = (string)config["FromDimension2Code"];
                            AssgnedAsset.ToDimension1Code = (string)config["ToDimension1Code"];
                            AssgnedAsset.ToDimension2Code = (string)config["ToDimension2Code"];
                            AssgnedAsset.DestinationLocation = (string)config["DestinationLocation"];
                            AssgnedAsset.ToEmployeeName = (string)config["ToEmployeeName"];
                            AssignedAssetList.Add(AssgnedAsset);
                        }
                    }
                    return PartialView("~/Views/Asset/Partial Views/TransferAssetList.cshtml", AssignedAssetList);
                }
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewAssetTransferRequest(string AssetNo)
        {
            try
            {
                NewTransferForm NewTransferForm = new NewTransferForm();

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

                #region Department List
                List<DimensionValues> Department = new List<DimensionValues>();
                string pageDepartment = "DimensionValues?$filter=Dimension_Code eq 'DEPARTMENT'&format=json";

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
                #region Division List
                List<DimensionValues> DivisionList = new List<DimensionValues>();
                string pageDivision = "DimensionValues?$filter=Dimension_Code eq 'CROP'&$format=json";

                HttpWebResponse httpResponseDivision = Credentials.GetOdataData(pageDivision);
                using (var streamReader = new StreamReader(httpResponseDivision.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimensionValues division = new DimensionValues();
                        division.Code = (string)config["Code"];
                        division.Name = (string)config["Name"];
                        DivisionList.Add(division);
                    }
                }
                #endregion
                AssetTransfer newTransfer = new AssetTransfer();
                NewTransferForm = new NewTransferForm
                {
                    AssetNo = AssetNo,
                    TransferDoc = newTransfer,
                    ListOfEmployee = EmployeeList.Select(x =>
                                         new SelectListItem()
                                         {
                                             Text = x.Text,
                                             Value = x.Value
                                         }).ToList(),
                    ListOfDepartment = Department.Select(x =>
                                        new SelectListItem()
                                        {
                                            Text = x.Name,
                                            Value = x.Code
                                        }).ToList(),
                    ListOfDivision = DivisionList.Select(x =>
                                        new SelectListItem()
                                        {
                                            Text = x.Name,
                                            Value = x.Code
                                        }).ToList()

                };
                return PartialView("~/Views/Asset/Partial Views/AssetTransferForm.cshtml", NewTransferForm);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitAssetTrasnferData(AssetTransfer AssTrans)
        {
            try
            {
                string UserID = Session["UserID"].ToString();
                string EmpNo = Session["Username"].ToString();
                Credentials.ObjNav.AssetTranferRequest(UserID, Convert.ToInt32(AssTrans.TransferType), 0, AssTrans.AssettoTransfer, AssTrans.Comments,
                    AssTrans.FromLocation, AssTrans.FromDimension1Code, AssTrans.FromDimension2Code, EmpNo, AssTrans.ToLocation, AssTrans.ToDimension1Code, AssTrans.ToDimension2Code,
                    AssTrans.ToResponsibleEmployee);

                return Json(new { message = "/Asset/AssetTransferList", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}