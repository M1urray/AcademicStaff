using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Latest_Staff_Portal.Controllers
{
    public class ADSetupController : Controller
    {
        // GET: ADSetup
        public ActionResult ListofUsers()
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
        public ActionResult GetListofUsers()
        {
            try
            {
                DirectorySearcher ds = null;
                ds = ADClassLibrary.ADClass.GetListofADUsers();
                SearchResultCollection result = ds.FindAll();

                List<ADSetup> ADUserList = new List<ADSetup>();

                foreach (SearchResult de in result)
                {
                    ADSetup ADUser = new ADSetup();
                    if (de.Properties.Contains("samAccountName"))
                    {
                        ADUser.UserID = de.Properties["samAccountName"][0].ToString();
                    }
                    if (de.Properties.Contains("displayname"))
                    {
                        ADUser.UserName = de.Properties["displayname"][0].ToString();
                    }
                    if (de.Properties.Contains("sn"))
                    {
                        ADUser.Email = de.Properties["sn"][0].ToString();
                    }
                    if (de.Properties.Contains("mail"))
                    {
                        ADUser.Email = de.Properties["mail"][0].ToString();
                    }
                    if (de.Properties.Contains("telephonenumber"))
                    {
                        ADUser.PhoneNo = de.Properties["telephonenumber"][0].ToString();
                    }
                    if (de.Properties.Contains("userAccountControl"))
                    {
                        const int UF_ACCOUNTDISABLE = 0x0002;

                        int flags = (int)de.Properties["userAccountControl"][0];

                        if (Convert.ToBoolean(flags & UF_ACCOUNTDISABLE))
                        {
                            ADUser.Disabled = "Inactive";
                        }
                        else
                        {
                            ADUser.Disabled = "Active";
                        }
                    }
                    ADUserList.Add(ADUser);
                }
                return PartialView("~/Views/ADSetup/Partial View/ListOfADUsers.cshtml", ADUserList.OrderByDescending(x => x.UserID));
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult NewADUser()
        {
            try
            {
                UserDetails NewUser = new UserDetails();

                return PartialView("~/Views/ADSetup/Partial View/ADUserDetails.cshtml", NewUser);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitADUserDetails(UserDetails userD)
        {
            bool successVal = false;
            try
            {
                string msg = "";

                ADClassLibrary.UserDetails newUser = new ADClassLibrary.UserDetails();
                newUser.FirstName = userD.FirstName;
                newUser.Surname = userD.Surname;
                if (userD.EmailAddress != null)
                {
                    newUser.EmailAddress = userD.EmailAddress;
                }
                else
                {
                    newUser.EmailAddress = "";
                }
                newUser.SamAccountName = userD.SamAccountName.ToUpper();
                newUser.UserPrincipalName = userD.SamAccountName.ToUpper() + ConfigurationManager.AppSettings["DC_DOMAIN"];
                if (userD.TelephoneNumber != null)
                {
                    newUser.TelephoneNumber = userD.TelephoneNumber;
                }
                newUser.Password = userD.Password;

                ADClassLibrary.InformClass Inf = ADClassLibrary.ADClass.SaveAdUserRecord(newUser);
                if (!Inf.BooleanValue)
                {
                    msg = Inf.Message;
                    successVal = false;
                }
                else
                {
                    msg = Inf.Message;
                    successVal = true;
                }

                return Json(new { success = successVal, Message = msg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (successVal)
                {
                    Session["ErrorMsg"] = ex.Message.Replace("'", "");
                }
                return Json(new { message = ex.Message.Replace("'", ""), success = successVal }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult ChangePassword(string UserID, string DisplayName)
        {
            try
            {
                ChangePassword UserD = new ChangePassword();
                if (DisplayName == "")
                {
                    UserD.DisplayName = UserID;
                }
                else
                {
                    UserD.DisplayName = DisplayName;
                }
                UserD.UserID = UserID;

                return PartialView("~/Views/ADSetup/Partial View/ChangePassword.cshtml", UserD);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitChangedPassword(string UserID, string Password)
        {
            bool successVal = false;
            try
            {
                string msg = "";

                ADClassLibrary.InformClass Inf = ADClassLibrary.ADClass.ChangeUserPassword(UserID, Password);
                if (!Inf.BooleanValue)
                {
                    msg = Inf.Message;
                    successVal = false;
                }
                else
                {
                    successVal = true;
                    msg = Inf.Message;
                }

                return Json(new { success = successVal, Message = msg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (successVal)
                {
                    Session["ErrorMsg"] = ex.Message.Replace("'", "");
                }
                return Json(new { message = ex.Message.Replace("'", ""), success = successVal }, JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult ADUserDetails(string UserID)
        {
            try
            {
                SearchResult rs = null;
                UserDetails ADUser = new UserDetails();
                rs = ADClassLibrary.ADClass.GetUserDetails(UserID);
                if (rs != null)
                {
                    DirectoryEntry de = rs.GetDirectoryEntry();
                    if (de.Properties.Contains("samAccountName"))
                    {
                        ADUser.SamAccountName = de.Properties["samAccountName"][0].ToString();
                    }
                    if (de.Properties.Contains("givenName"))
                    {
                        ADUser.FirstName = de.Properties["givenName"][0].ToString();
                    }
                    if (de.Properties.Contains("sn"))
                    {
                        ADUser.Surname = de.Properties["sn"][0].ToString();
                    }
                    if (de.Properties.Contains("mail"))
                    {
                        ADUser.EmailAddress = de.Properties["mail"][0].ToString();
                    }
                    if (de.Properties.Contains("telephonenumber"))
                    {
                        ADUser.TelephoneNumber = de.Properties["telephonenumber"][0].ToString();
                    }
                    //if (de.Properties.Contains("userAccountControl"))
                    //{
                    //    const int UF_ACCOUNTDISABLE = 0x0002;

                    //    int flags = (int)de.Properties["userAccountControl"][0];

                    //    if (Convert.ToBoolean(flags & UF_ACCOUNTDISABLE))
                    //    {
                    //        ADUser.Disabled = "Inactive";
                    //    }
                    //    else
                    //    {
                    //        ADUser.Disabled = "Active";
                    //    }
                    //}
                }
                return PartialView("~/Views/ADSetup/Partial View/ADUserDetails.cshtml", ADUser);
            }
            catch (Exception ex)
            {
                Error erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateADUserDetails(UserDetails userD)
        {
            bool successVal = false;
            try
            {
                string msg = "";

                ADClassLibrary.UserDetails newUser = new ADClassLibrary.UserDetails();
                newUser.FirstName = userD.FirstName;
                newUser.Surname = userD.Surname;
                if (userD.EmailAddress != null)
                {
                    newUser.EmailAddress = userD.EmailAddress;
                }
                else
                {
                    newUser.EmailAddress = "";
                }
                newUser.SamAccountName = userD.SamAccountName.ToUpper();
                newUser.UserPrincipalName = userD.SamAccountName.ToUpper() + ConfigurationManager.AppSettings["DC_DOMAIN"];
                if (userD.TelephoneNumber != null)
                {
                    newUser.TelephoneNumber = userD.TelephoneNumber;
                }

                ADClassLibrary.InformClass Inf = ADClassLibrary.ADClass.UpdateDetails(newUser);
                if (!Inf.BooleanValue)
                {
                    msg = Inf.Message;
                    successVal = false;
                }
                else
                {
                    msg = Inf.Message;
                    successVal = true;
                }

                return Json(new { success = successVal, Message = msg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (successVal)
                {
                    Session["ErrorMsg"] = ex.Message.Replace("'", "");
                }
                return Json(new { message = ex.Message.Replace("'", ""), success = successVal }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}