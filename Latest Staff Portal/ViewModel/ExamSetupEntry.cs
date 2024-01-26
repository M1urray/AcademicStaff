using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class ExamSetupEntry
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public string EntryType { get; set; }
        public string MaxScore { get; set; }
        public string Contr { get; set; }
        public string Semester { get; set; }
        public string Unit { get; set; }
        public string Unitclass { get; set; }
        public string Order { get; set; }
        public string EntryNo { get; set; }
    }
    public class MySetupEntry
    {
        public string Type { get; set; }
        public List<ExamSetupEntry> ListOfExamEntry { get; set; }
        public List<SelectListItem> ListOfExamCodes { get; set; }
        public bool IsAssLec { get; set; }
    }
}