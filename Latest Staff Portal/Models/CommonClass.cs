using iTextSharp.text;
using iTextSharp.text.pdf;
using Latest_Staff_Portal.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace Latest_Staff_Portal.Models
{
    public class CommonClass
    {
        public static string ProfilePicture(string User)
        {
            string PicString = "";
            try
            {
                string StaffNo = User;
                PicString = Credentials.ObjNav.GetProfilePicture(StaffNo);
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return PicString;
        }
        public static string[] GetStaffDetails(string User)
        {
            string[] s = new string[2];
            try
            {
                string StaffNo = User;
                string page = "EmployeeList?$filter=No eq '" + StaffNo + "'&format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        s[0] = (string)config["Gender"];
                        s[1] = (string)config["FirstName"] + " " + (string)config["MiddleName"] + " " + (string)config["LastName"];
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return s;
        }
        public static bool IsICTStaff(string User)
        {
            bool s = false;
            try
            {
                string StaffNo = User;
                string page = "EmployeeList?$filter=No eq '" + StaffNo + "' and In_ICT_Dep eq true&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        s = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return s;
        }
        public static bool MoveFile(string FileName)
        {
            bool s = false;
            try
            {
                string sourcefile = Credentials.fileSourcePath + FileName;
                string destinationfile = Credentials.fileDestinationPath + FileName;
                if (System.IO.File.Exists(destinationfile) == true)
                {
                    System.IO.File.Delete(destinationfile);
                    System.IO.File.Move(sourcefile, destinationfile);
                }
                if (System.IO.File.Exists(destinationfile) == false)
                {
                    System.IO.File.Move(sourcefile, destinationfile);
                }
                s = true;
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return s;
        }
        public static bool MoveUploadedFile(string base64Upload, string FilePath, string FileName)
        {
            bool s = false;
            try
            {
                CommonClass.SaveUploadedFile(base64Upload, FilePath);

                string sourcefile = FilePath;
                string destinationfile = Credentials.fileUploadsPath + FileName;
                if (System.IO.File.Exists(destinationfile) == true)
                {
                    System.IO.File.Delete(destinationfile);
                    System.IO.File.Move(sourcefile, destinationfile);
                }
                if (System.IO.File.Exists(destinationfile) == false)
                {
                    System.IO.File.Move(sourcefile, destinationfile);
                }
                s = true;
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return s;
        }
        public static bool DeleteFile(string FilePath)
        {
            bool s = false;
            try
            {
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                    s = true;
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return s;
        }
        public static bool SendEmailAlert(string body, string recepient, string subject)
        {
            Boolean x = false;

            try
            {
                x = Credentials.ObjNav.SendEmail(recepient, subject, body);

                //string SMTPHost = "smtp.gmail.com";
                //string fromAddress = "testjooust@gmail.com";
                //string toAddress = recepient;
                //System.Net.Mail.MailMessage mail_ = new System.Net.Mail.MailMessage();
                //mail_.To.Add(toAddress);
                //mail_.Subject = subject;
                //mail_.From = new System.Net.Mail.MailAddress(fromAddress);
                //mail_.Body = body;
                //mail_.IsBodyHtml = true;

                //var smtp = new SmtpClient(SMTPHost, 587)
                //{
                //    Credentials = new NetworkCredential("testjooust@gmail.com", "123@Team"),
                //    EnableSsl = true
                //};
                //smtp.Send(mail_);
            }
            catch (Exception ex2)
            {
                ex2.Data.Clear();
            }
            return x;
        }
        public static string GetDimensionValue(string DimCode)
        {
            string DimVal = "";
            try
            {
                string page = "DimensionValues?$filter=Code eq '" + DimCode + "'&format=json";

                HttpWebResponse httpResponseResC = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        DimVal = (string)config["Name"];
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return DimVal;
        }
        public static string EmployeeDepartment(string StaffNo)
        {
            string Department = "";

            string page = "EmployeeList?$select=GlobalDimension2Code&$filter=No eq '" + StaffNo + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        Department = (string)config["GlobalDimension2Code"];
                    }
                }
            }
            return Department;
        }
        public static bool DisregardDirectorate(string StaffNo)
        {
            bool DisRegard = false;

            string page = "EmployeeList?$select=Disregard_Directorate&$filter=No eq '" + StaffNo + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        DisRegard = (bool)config["Disregard_Directorate"];
                    }
                }
            }
            return DisRegard;
        }
        public static string GetEmployeeName(string StaffNo)
        {
            string Name = "";

            string page = "EmployeeList?$select=FirstName,MiddleName,LastName&$filter=No eq '" + StaffNo + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        Name = (string)config["FirstName"] + " " + (string)config["MiddleName"] + " " + (string)config["LastName"];
                    }
                }
            }
            return Name;
        }
        public static string GetEmployeeNameByUserID(string UserID)
        {
            string Name = "";

            string page = "EmployeeList?$select=FirstName,MiddleName,LastName&$filter=EmployeeUserID eq '" + UserID + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        Name = (string)config["FirstName"] + " " + (string)config["MiddleName"] + " " + (string)config["LastName"];
                    }
                }
            }
            return Name;
        }
        public static string GetEmployeeGender(string StaffNo)
        {
            string gender = "";

            string page = "EmployeeList?$select=Gender&$filter=No eq '" + StaffNo + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        gender = (string)config["Gender"];
                    }
                }
            }
            return gender;
        }
        public static string GetEmployeeIDNo(string StaffNo)
        {
            string IDNo = "";

            string page = "EmployeeList?$select=IDNumber&$filter=No eq '" + StaffNo + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        IDNo = (string)config["IDNumber"];
                    }
                }
            }
            return IDNo;
        }
        public static bool SaveUploadedFile(string base64String, string filePath)
        {
            bool Uploaded = false;
            try
            {
                File.WriteAllBytes(filePath, Convert.FromBase64String(base64String));
                Uploaded = true;
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return Uploaded;
        }
        public static decimal GetStaffCustomerBal(string CustNo)
        {
            decimal Bal = 0;
            try
            {
                string page = "CustomerList?$filter=No eq '" + CustNo + "'&format=json";

                HttpWebResponse httpResponseResC = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
                {

                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    if (details["value"].Count() > 0)
                    {
                        foreach (JObject config in details["value"])
                        {
                            Bal = (decimal)config["Balance"];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return Bal;
        }
        public static bool IfFileExists(string FileName)
        {
            bool s = false;
            try
            {
                if (System.IO.File.Exists(FileName) == true)
                {
                    System.IO.File.Delete(FileName);
                }
                s = true;
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return s;
        }
        public static string GetDocRejectionComment(string DocNo, int SeqNo)
        {
            string comment = "";
            try
            {
                string page = "ApprovalComments?$select=Comment&$filter=Document_No eq '" + DocNo + "' and Sequence_No eq " + SeqNo + "&format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);
                    if (details["value"].Count() > 0)
                    {
                        foreach (JObject config in details["value"])
                        {
                            comment = (string)config["Comment"];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return comment;
        }
        public static decimal[] GetLeaveBal(string StaffNo,string LvType)
        {
            decimal[] LvDays = new decimal[5];
            try
            {
                LvDays = Credentials.ObjNav.GetLeaveBalances(StaffNo, LvType);
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return LvDays;
        }
        public static string GetFixedAssetDescription(string AssetNo)
        {
            string s = "";
            try
            {

                string page = "FixedAssetsList?$&select=Description&$filter=No eq '" + AssetNo + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        s = (string)config["Description"];
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return s;
        }
    }
}