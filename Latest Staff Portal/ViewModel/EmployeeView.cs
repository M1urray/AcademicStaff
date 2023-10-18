using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Latest_Staff_Portal.ViewModel
{
    public class EmployeeView
    {
        public string No { get; set; }
        public string Name { get; set; }
        public string IDNo { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string Nationality { get; set; }
        public string County { get; set; }
        public string DoB { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string HomeTelNo { get; set; }
        public string PhoneNo { get; set; }
        public string WorkTel { get; set; }
        public string CompanyEmail { get; set; }
        public string PersonalEmail { get; set; }
        public string DateOfJoin { get; set; }
        public string ContractStartDate { get; set; }
        public string ContractEndtDate { get; set; }
        public string ProbationDate { get; set; }
        public string ProbationEndDate { get; set; }
        public string PenSchemeJoinDate { get; set; }
        public string JobTitle { get; set; }
        public string EmpStatus { get; set; }
        public string JobCat { get; set; }
        public string Department { get; set; }
        public string Campus { get; set; }
        public string School { get; set; }
        public string Bank { get; set; }
        public string Branch { get; set; }
        public string AccountNo { get; set; }
        public string PinNo { get; set; }
        public string NSSFNo { get; set; }
        public string NHIFNo { get; set; }
        public int NotfCount { get; set; }
        public ListOfInternalMemos ListInternalMemos { get; set; }
    }
    public class EmpInitial
    {
        public string Code { get; set; }
    }
    public class InternalMemos
    {
        public string description { get; set; }
        public string Remarks { get; set; }
        public string Date { get; set; }
    }
    public class ListOfInternalMemos
    {
        public List<InternalMemos> ListInternalMemos { get; set; }
        public List<DocumentAttachment> ListOfIntMemos { get; set; }        
        public bool hasFiles { get; set; }
    }

    public class EmpQualification
    {
        public string Qualification { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Description { get; set; }
        public string Institute { get; set; }
        public string Specialization { get; set; }
    }
}