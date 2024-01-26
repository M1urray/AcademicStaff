using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Latest_Staff_Portal.ViewModel
{
    public class DocumentAttachment
    {
        public int TabelID { get; set; }
        public string No { get; set; }
        public string FileName { get; set; }
        public string FileExt { get; set; }
        public int ID { get; set; }
        public string LineNo { get; set; }
        public string DocType { get; set; }
    }
    public class DocumentAttachmentList
    {
        public string Status { get; set; }
        public List<DocumentAttachment> DocList { get; set; }
    }
}