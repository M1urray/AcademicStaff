using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Latest_Staff_Portal.Controllers
{
    public class SettingsController : Controller
    {
        // GET: Settings
        public ActionResult ChangePassword()
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
        public JsonResult ChangePassord(string newpass)
        {
            try
            {
                if (Session["Username"] == null)
                {
                    return Json(new { message = "/Login/Login", success = false, redirect = true }, JsonRequestBehavior.AllowGet);
                }
                //else if (Session["UserID"] == null)
                //{
                //    return Json(new { message = "/Login/Login", success = false, redirect = true }, JsonRequestBehavior.AllowGet);
                //}
                else
                {
                    string StaffNo = Session["Username"].ToString();

                    string ok = Credentials.ResetPassword(StaffNo, newpass);

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