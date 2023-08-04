using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class NewStandingImprest
    {
        public string Date
        {
            get;
            set;
        }

        public string Dim1
        {
            get;
            set;
        }

        public string Dim11
        {
            get;
            set;
        }

        public string Dim2
        {
            get;
            set;
        }

        public string Dim21
        {
            get;
            set;
        }

        public string DocNo
        {
            get;
            set;
        }

        public List<SelectListItem> ListOfBankAccounts
        {
            get;
            set;
        }

        public List<SelectListItem> ListOfBankAccounts1
        {
            get;
            set;
        }

        public List<SelectListItem> ListOfDim1
        {
            get;
            set;
        }

        public List<SelectListItem> ListOfDim2
        {
            get;
            set;
        }

        public List<SelectListItem> ListOfResponsibility
        {
            get;
            set;
        }

        public string Receiving_Amount
        {
            get;
            set;
        }

        public string Receiving_BnkAccount
        {
            get;
            set;
        }

        public string Remarks
        {
            get;
            set;
        }

        public string RespC
        {
            get;
            set;
        }

        public string RespC1
        {
            get;
            set;
        }

        public string Sending_Amount
        {
            get;
            set;
        }

        public string Sending_BnkAccount
        {
            get;
            set;
        }

        public string Status
        {
            get;
            set;
        }
    }
}