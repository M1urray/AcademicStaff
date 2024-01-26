using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net;
using System.Web.Mvc;
using System.Web.Caching;
using System.Web.UI;
using System.IO;
using Latest_Staff_Portal.NAVWS;
using System.DirectoryServices;
using System.Web.Configuration;
using System.DirectoryServices.AccountManagement;

namespace Latest_Staff_Portal.Models
{
    public class Credentials
    {
        private static DirectorySearcher dirSearch = null;
        public static string fileSourcePath = ConfigurationManager.AppSettings["FILEPATH"];
        public static HttpWebResponse GetOdataData(string page)
        {
            HttpWebResponse httpResponse = null;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["ODATA_URI"] + page);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["W_USER"],
                        ConfigurationManager.AppSettings["W_PWD"], ConfigurationManager.AppSettings["DOMAIN"]);

            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            return httpResponse;
        }
        public static HttpWebResponse WhatsUpText(string text)
        {
            HttpWebResponse httpResponse = null;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.whatsapp.com/send?phone=+254714562578&text=Alexander");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["W_USER"],
                        ConfigurationManager.AppSettings["W_PWD"], ConfigurationManager.AppSettings["DOMAIN"]);

            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            return httpResponse;
        }
        public static Webportal ObjNav
        {
            get
            {
                var ws = new Webportal();

                try
                {
                    var credentials = new NetworkCredential(ConfigurationManager.AppSettings["W_USER"],
                        ConfigurationManager.AppSettings["W_PWD"], ConfigurationManager.AppSettings["DOMAIN"]);

                    ws.Credentials = credentials;
                    ws.PreAuthenticate = true;

                }
                catch (Exception ex)
                {
                    ex.Data.Clear();
                }
                return ws;
            }
        }
        //public static PortalService ObjNav1
        //{
        //    get
        //    {
        //        var ws = new PortalService();

        //        try
        //        {
        //            var credentials = new NetworkCredential(ConfigurationManager.AppSettings["W_USER"],
        //                ConfigurationManager.AppSettings["W_PWD"], ConfigurationManager.AppSettings["DOMAIN"]);

        //            ws.Credentials = credentials;
        //            ws.PreAuthenticate = true;

        //        }
        //        catch (Exception ex)
        //        {
        //            ex.Data.Clear();
        //        }
        //        return ws;
        //    }
        //}
        private static SearchResult SearchUserExist(DirectorySearcher ds, string UserName)
        {
            ds.Filter = string.Format("(&(objectCategory=Person)(sAMAccountName={0}))", UserName);

            SearchResult userObject = ds.FindOne();
            if (userObject != null)
            {
                return userObject;
            }
            else
            {
                return null;
            }
        }
        private static DirectorySearcher GetDirectorySearch(string username, string password, string domain)
        {
            if (dirSearch == null)
            {
                try
                {
                    dirSearch = new DirectorySearcher(new DirectoryEntry("LDAP://dc=DaystarUniversity,dc=local", username, password));
                }
                catch (DirectoryServicesCOMException ex)
                {
                    ex.Data.Clear();
                    //cSite.Messaging.ShowAlert("Connection Credentials is wrong. Reason: " + ex.Message.ToString());
                }
                return dirSearch;
            }
            else
            {
                return dirSearch;
            }
        }
        public static string ResetPassword(string username, string newpass)
        {
            string rval = "";
            try
            {
                string UName = "";
                if (username.Contains(@"\\"))
                {
                    UName = username.Replace(@"\\", "").Trim();
                }
                else if (username.Contains(@"\"))
                {
                    UName = username.Replace(@"\", "").Trim();
                }
                else
                {
                    UName = username.Trim();
                }
                string AdminAccountName = WebConfigurationManager.AppSettings["AD_USER"];
                string AdminPassword = WebConfigurationManager.AppSettings["ADW_PWD"];
                string Domain = WebConfigurationManager.AppSettings["AD_DOMAIN"];

                using (PrincipalContext pContext = new PrincipalContext(ContextType.Domain, "DaystarUniversity.local", AdminAccountName, AdminPassword))
                {
                    UserPrincipal up = UserPrincipal.FindByIdentity(pContext, username);
                    if (up != null)
                    {
                        up.SetPassword(newpass);
                        up.Save();
                        rval = "CHANGED";
                    }
                }
                //string AdminAccountName = WebConfigurationManager.AppSettings["W_USER"];
                //string AdminPassword = WebConfigurationManager.AppSettings["W_PWD"];
                //string Domain = WebConfigurationManager.AppSettings["DOMAIN"];



                //SearchResult rs = null;
                //rs = SearchUserExist(GetDirectorySearch(AdminAccountName, AdminPassword, Domain), UName);
                //if (rs != null)
                //{
                //    try
                //    {
                //        DirectoryEntry user = rs.GetDirectoryEntry();
                //        user.Invoke("SetPassword", new object[] { "" + newpass + "" });
                //        user.CommitChanges();
                //        rval = "CHANGED";
                //    }
                //    catch (DirectoryServicesCOMException ex)
                //    {
                //        rval = ex.InnerException.Message;
                //    }
                //}
            }
            catch (Exception ex)
            {
                rval = ex.InnerException.Message;
            }
            return rval;
        }
        public static bool UploadProfilePic(string StaffNo, string base64String, string filePath, string fileName)
        {
            bool Uploaded = false;
            try
            {
                CommonClass.MoveUploadedFile(base64String, filePath, fileName);
                string UploadFilePath = "";// Credentials.fileUploadsPath + fileName;
                //ObjNav.ImportStaffProfilePicture(StaffNo, UploadFilePath, fileName);
                Uploaded = true;
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return Uploaded;
        }
        public static string UploadDocumentAttachment(string DocNo, string base64String, string filePath, int TableID)
        {
            string Uploaded = "";
            try
            {
                File.WriteAllBytes(filePath, Convert.FromBase64String(base64String));

                //ObjNav.UploadAttachedDocument(DocNo, filePath, base64String, TableID);
                Uploaded = "SUCCESS";
            }
            catch (Exception ex)
            {
                Uploaded = ex.Message;
            }
            return Uploaded;
        }
        public static string GetDocumentAttachmet(int TblID, string DocNo, int Id)
        {
            string PicString = "";
            try
            {
                //PicString = ObjNav.GetDocumentAttachment(TblID, DocNo, Id);
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return PicString;
        }
        public static void DownloadAttachment(string path, Byte[] bytes)
        {
            File.WriteAllBytes(path, bytes);
        }
    }
}