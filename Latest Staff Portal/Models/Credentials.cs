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
        public static string fileSourcePath = @"C:\DBs\Portal Reports\";
        public static string fileDestinationPath = @"C:\PORTAL\Live\Downloads\";
        public static string fileUploadsPath = @"\\192.168.1.148\Document Uploads\";
        public static HttpWebResponse GetOdataData(string page)
        {
            HttpWebResponse httpResponse = null;
            string Url = ConfigurationManager.AppSettings["W_PWD"];
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["ODATA_URI"] + page);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["W_USER"],
                        ConfigurationManager.AppSettings["W_PWD"], ConfigurationManager.AppSettings["DOMAIN"]);

            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            return httpResponse;
        }
        public static WebService ObjNav
        {
            get
            {
                var ws = new WebService();

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

                using (PrincipalContext pContext = new PrincipalContext(ContextType.Domain, "192.168.2.156", AdminAccountName, AdminPassword))
                {
                    UserPrincipal up = UserPrincipal.FindByIdentity(pContext, username);
                    if (up != null)
                    {
                        up.SetPassword(newpass);
                        up.Save();
                        rval = "CHANGED";
                    }
                }
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
                File.WriteAllBytes(filePath, Convert.FromBase64String(base64String));
                if (CommonClass.IfFileExists(filePath))
                {
                    ObjNav.ImportStaffProfilePicture(StaffNo, filePath, fileName);
                }
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

                if (CommonClass.IfFileExists(filePath))
                {
                    ObjNav.UploadAttachedDocument(DocNo, filePath, base64String, TableID);
                }
               
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
                PicString = ObjNav.GetDocumentAttachment(TblID, DocNo, Id);
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