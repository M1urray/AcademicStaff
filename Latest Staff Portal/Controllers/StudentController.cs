using Latest_Staff_Portal.CustomSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.Controllers
{
    [CustomeAuthentication]
    [CustomAuthorization(Role = "FULLTIME")]
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult StudentRequisitionLinks()
        {
            return View();
        }
        public ActionResult StudentRequisitionLink()
        {
            return View();
        }
    }
}