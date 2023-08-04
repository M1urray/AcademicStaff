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
    }
}