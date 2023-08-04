using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Latest_Staff_Portal.ViewModel
{
    public class DocumentsForApproval
    {
        public string Entry_No { get; set; }
        public string TabelID { get; set; }
        public string Document_No { get; set; }
        public string Document_Type { get; set; }
        public string Sender_Name { get; set; }
        public string DateSend { get; set; }
        public string DueDate { get; set; }
        public string Status { get; set; }
        public string Sequence { get; set; }
        public string Sender { get; set; }
        public string Approver { get; set; }
        public bool CommentFound { get; set; }
        public string Comment { get; set; }
    }
    public class DocumentsForApprovalList
    {
        public string TableID { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string DocType { get; set; }
    }
    public class DocumentCount
    {
        public string Status { get; set; }
        public int LeaveCount { get; set; }
        public int PRNCount { get; set; }
        public int LPOCount { get; set; }
        public int SRNCount { get; set; }
        public int ImpCount { get; set; }
        public int SurrCount { get; set; }
        public int ClaimCount { get; set; }
        public int Transport { get; set; }
        public int TransferOrder { get; set; }
        public int Training { get; set; }
        public int PVCount { get; set; }
        public int PayrollCount { get; set; }
    }
    public class DocumentRejectionComment
    {
        public bool CommentFound { get; set; }
        public string Comment { get; set; }
    }
}