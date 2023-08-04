using Latest_Staff_Portal.CustomSecurity;
using Latest_Staff_Portal.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Latest_Staff_Portal.Controllers
{
    [CustomeAuthentication]
    [CustomAuthorization(Role = "FULLTIME,PARTTIME")]
    public class SettingsController : Controller
    {
        // GET: Settings
        public ActionResult ChangePassword()
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
        public JsonResult ChangePassord(string newpass)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return Json(new { message = "/Login/Login", success = false, redirect = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string UserID = Session["UserID"].ToString();

                    string ok = Credentials.ResetPassword(UserID, newpass);

                    if (ok == "CHANGED")
                    {
                        return Json(new { message = "Password Changed Successfully", success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { message = ok, success = false, redirect = false }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false, redirect = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}