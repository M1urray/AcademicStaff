using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class UnitAllocationDropdownList
    {
        public List<SelectListItem> ListOfProgrammes { get; set; }
        public List<SelectListItem> ListOfSemesters { get; set; }
        public List<SelectListItem> ListOfCampus { get; set; }
        public List<SelectListItem> ListOfStudyModes { get; set; }
        public List<SelectListItem> ListOfCourseClasses { get; set; }
    }
}