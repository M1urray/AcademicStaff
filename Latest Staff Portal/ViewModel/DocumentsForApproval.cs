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
        public string ReqType { get; set; }
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
        public int TransportCount { get; set; }
        public int Clearance { get; set; }
        public int TransferOrder { get; set; }
        public int CafCount { get; set; }
        public int PVCount { get; set; }
    }
    public class StdDocumentCount
    {
        public string Status { get; set; }
        public int CampusTraCount { get; set; }
        public int ProgTransCount { get; set; }
        public int SExamCount { get; set; }
        public int DropCorsesCount { get; set; }
        public int MinorCount { get; set; }
        public int MajorCount { get; set; }
        public int DoubleConcCount { get; set; }
        public int ExemptionConcCount { get; set; }
        public int ExamChlngConcCount { get; set; }
        public int ClearanceCount { get; set; }
        public int SpecialExamCount { get; set; }
        public int SuppCount { get; set; }
    }
    public class DocumentApprovalCount
    {
        public DocumentCount DocCount { get; set; }
        public StdDocumentCount StdCount { get; set; }

    }
    public class DocumentRejectionComment
    {
        public bool CommentFound { get; set; }
        public string Comment { get; set; }
    }
}