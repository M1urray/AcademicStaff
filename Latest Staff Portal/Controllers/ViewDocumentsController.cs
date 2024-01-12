using Latest_Staff_Portal.CustomSecurity;
using Latest_Staff_Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.Controllers
{
    [CustomeAuthentication]
    [CustomAuthorization(Role = "ALLUSERS")]
    public class ViewDocumentsController : Controller
    {
        // GET: ViewDocuments
        public ActionResult ViewDocuments()
        {
            return View();
        }
        public JsonResult VoteBook()
        {
            bool success = false;
            try
            {
                string message = "";

                if (Session["Username"] == null)
                {
                    Response.Redirect(Url.Action("Login", "Login"));
                }
                else
                {
                    string StaffDepartment = CommonClass.EmployeeDepartment(Session["username"].ToString());

                    string filename = StaffDepartment.Replace("/", "");

                    Credentials.ObjNav.PrintVoteBookBalance(Session["username"].ToString(), StaffDepartment, "VOTE BAL-" + filename + ".pdf");
                    filename = "VOTE BAL-" + filename + ".pdf";
                    string fileDestinationPath = Server.MapPath("~/Downloads/");
                    CommonClass.MoveFile(filename, fileDestinationPath);
                    string DestinationPath = fileDestinationPath + filename;
                    System.IO.FileInfo file = new System.IO.FileInfo(DestinationPath);
                    if (file.Exists)
                    {
                        success = true;
                        message = @"/Downloads/" + filename;
                    }
                    else
                    {
                        success = false;
                        message = "File Not Found";
                    }
                }
                return Json(new { message = message, success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}