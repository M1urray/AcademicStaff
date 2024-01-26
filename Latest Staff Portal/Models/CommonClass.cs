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
        public static string CurrentSemester()
        {
            string CSem = "";
            try
            {
                string page = "SemesterList?$filter=CurrentSemester eq true&$format=json";

                HttpWebResponse httpResponseResC = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        CSem = (string)config["Code"];
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return CSem;
        }
        public static bool[] BlockMarkEntry(string Sem)
        {
            bool[] CSem = new bool[2];
            try
            {
                string page = "SemesterList?$filter=Code eq '" + Sem + "'&$format=json";

                HttpWebResponse httpResponseResC = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        CSem[0] = (bool)config["LockCATEditting"];
                        CSem[1] = (bool)config["LockExamEditting"];
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return CSem;
        }
        public static string ExamSemester()
        {
            string CSem = "";
            try
            {
                string page = "SemesterList?$filter=ExamSemester eq true&$format=json";

                HttpWebResponse httpResponseResC = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        CSem = (string)config["Code"];
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return CSem;
        }
        public static string ProfilePicture(string User)
        {
            string PicString = "";
            try
            {
                string StaffNo = User;
                //PicString = Credentials.ObjNav.GetProfilePicture(StaffNo);
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return PicString;
        }
        public static string[] GetStaffDetails(string User)
        {
            string[] s = new string[7];
            try
            {
                string StaffNo = User;
                string page = "EmployeeList?$filter=No eq '" + StaffNo + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        s[0] = (string)config["Gender"];
                        s[1] = (string)config["First_Name"] + " " + (string)config["Middle_Name"] + " " + (string)config["Last_Name"];
                        s[2] = (string)config["HOD"];
                        s[3] = (string)config["Dean"];
                        s[4] = "";
                        s[5] = (string)config["Lecturer"];
                        s[6] = (string)config["Part_Time"];
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return s;
        }
        public static bool MoveFile(string FileName, string DestinationPath)
        {
            bool s = false;
            try
            {
                string sourcefile = Credentials.fileSourcePath + FileName;
                string destinationfile = DestinationPath + FileName;
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
                string destinationfile = "";// Credentials.fileUploadsPath + FileName;
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
                //x = Credentials.ObjNav.SendEmail(ref recepient, subject, body);
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
                string page = "DimensionValues?$filter=Code eq '" + DimCode + "'&$format=json";

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
        public static EmployeeDesignation EmployeeDesignation(string StaffNo)
        {
            EmployeeDesignation EmpDes = new EmployeeDesignation();

            string page = "EmployeeList?$select=HOD,Dean,Manager,Director,Schools,Department_Code&$filter=No eq '" + StaffNo + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        EmpDes.IsHOD = (bool)config["HOD"];
                        EmpDes.IsDean = (bool)config["Dean"];
                        EmpDes.IsDirector = (bool)config["Director"];
                        if (EmpDes.IsHOD)
                        {
                            EmpDes.EmpDepartment = (string)config["Department_Code"];
                        }
                        else
                        {
                            EmpDes.EmpDepartment = "";
                        }
                        if (EmpDes.IsDean)
                        {
                            EmpDes.EmpSchool = (string)config["Schools"];
                        }
                        else
                        {
                            EmpDes.EmpSchool = "";
                        }
                    }
                }
            }
            return EmpDes;
        }
        public static string EmployeeDepartment(string StaffNo)
        {
            string Department = "";

            string page = "EmployeeList?$select=Department_Code&$filter=No eq '" + StaffNo + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        Department = (string)config["Department_Code"];
                    }
                }
            }
            return Department;
        }
        public static string GetEmployeeName(string StaffNo)
        {
            string Name = "";

            string page = "EmployeeList?$select=First_Name,Middle_Name,Last_Name&$filter=No eq '" + StaffNo + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        Name = (string)config["First_Name"] + " " + (string)config["Middle_Name"] + " " + (string)config["Last_Name"];
                    }
                }
            }
            return Name;
        }
        public static string[] GetEmployeeByUserID(string UserID)
        {
            string[] det = new string[2];

            string page = "EmployeeList?$select=No,First_Name,Middle_Name,Last_Name&$filter=User_ID eq '" + UserID + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        det[0] = (string)config["No"];
                        det[1] = (string)config["First_Name"] + " " + (string)config["Middle_Name"] + " " + (string)config["Last_Name"];
                    }
                }
            }
            return det;
        }
        public static string GetEmployeeIDNo(string StaffNo)
        {
            string IDNo = "";

            string page = "EmployeeList?$select=ID_Number&$filter=No eq '" + StaffNo + "'&$format=json";

            HttpWebResponse httpResponse = Credentials.GetOdataData(page);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                if (details["value"].Count() > 0)
                {
                    foreach (JObject config in details["value"])
                    {
                        IDNo = (string)config["ID_Number"];
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
                string page = "CustomerList?$select=Balance_LCY&$filter=No eq '" + CustNo + "'&$format=json";

                HttpWebResponse httpResponseResC = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
                {

                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    if (details["value"].Count() > 0)
                    {
                        foreach (JObject config in details["value"])
                        {
                            Bal = (decimal)config["Balance_LCY"];
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
        public static string[] UploadStudentMarks(string Prog, string Unit, string Stage, string Sem, string Lec, string ProgC, string filePath)
        {
            string[] Uploaded = new string[2];
            try
            {
                var details = new JObject();

                string[] lines = File.ReadAllLines(filePath);
                //Remove Header line
                lines = lines.Skip(1).ToArray();
                int i = 0;
                foreach (var line in lines)
                {
                    var fields = line.Split(new char[] { ',' });
                    var studentNo = fields[1].Replace("\"", "");
                    //if (HasStudentRegForUnit(studentNo, Prog, Unit, Sem))
                    //{
                    string page = "ExamSetup?$filter=Category eq '" + ProgC + "'&$format=json";

                    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        details = JObject.Parse(result);

                        int j = 3;
                        bool UpScc = false;
                        string ExamMark = "";
                        if (details["value"].Count() > 0)
                        {
                            foreach (JObject config in details["value"])
                            {
                                if (!String.IsNullOrEmpty(fields[j]))
                                {
                                    ExamMark = fields[j].Replace("\"", "");

                                    if (!string.IsNullOrEmpty(ExamMark))
                                    {

                                        if (Convert.ToDecimal(ExamMark) > Convert.ToDecimal((string)config["Max_Score"]))
                                        {
                                            Uploaded[0] = ((string)config["Code"] + " Mark cannot be greater than " + (string)config["Max_Score"] + ", the maximum set mark, for student " + studentNo + "");
                                            Uploaded[1] = "F";
                                            return Uploaded;
                                        }
                                        else
                                        {
                                            Credentials.ObjNav.EnterRowMarks(
                                                     prog: Prog,
                                                     stage: Stage,
                                                     unit: Unit,
                                                     sem: Sem,
                                                     score: Convert.ToDecimal(ExamMark),
                                                     contrib: Convert.ToDecimal(ExamMark),
                                                     stdNo: studentNo,
                                                     lecturer: Lec,
                                                     examType: (string)config["Code"]
                                                     );
                                            if (!UpScc)
                                            {
                                                i++;
                                                UpScc = true;
                                            }
                                        }
                                    }
                                }
                                j++;
                            }
                        }
                    }
                    //}
                }
                Uploaded[0] = "Data for " + i.ToString() + " students Uploaded Successfully";
                Uploaded[1] = "T";
                return Uploaded;
            }
            catch (Exception ex)
            {
                Uploaded[0] = ex.Message.Replace("'", "");
                Uploaded[1] = "F";
                return Uploaded;
            }
        }
        private static bool HasStudentRegForUnit(string StdNo, string Prog, string Unit, string Sem)
        {
            bool Registered = false;
            try
            {
                string page = "StudentUnits?$filter=Student_No eq '" + StdNo + "' and Programme eq '" + Prog + "' and Semester eq '" + Sem + "' and Unit eq '" + Unit + "'&format=json";

                HttpWebResponse httpResponseResC = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    if (details["value"].Count() > 0)
                    {
                        Registered = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return Registered;
        }
        public static string GetProgrammeName(string Prog)
        {
            string ProgName = "";
            try
            {
                string page = "ProgrammeList?$filter=Code eq '" + Prog + "'&$format=json";

                HttpWebResponse httpResponseResC = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        ProgName = (string)config["Description"];
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return ProgName;
        }
        private static bool HasStudentRegForUnit(string StdNo, string Unit, string Sem)
        {
            bool Registered = false;
            try
            {
                string page = "StudentUnits?$filter=Student_No eq '" + StdNo + "' and Semester eq '" + Sem + "' and Unit eq '" + Unit + "'&$format=json";

                HttpWebResponse httpResponseResC = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    if (details["value"].Count() > 0)
                    {
                        Registered = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return Registered;
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
        public static Error StartMarkSheettReport(string Lec, string Prog, string Sem, string Unit, string UnitName, string Campus, string classCode, string ImagePath, string rptpath)
        {
            Error error = new Error();
            try
            {
                string fileName = Unit.Replace("/", "_") + "-" + String.Format("Mark_Sheet_{0}.pdf", Lec.Replace("/", "_"));
                string filenamePath = String.Format("{0}{1}", rptpath, fileName);
                Credentials.ObjNav.GenerateScoreSheet(Prog, Unit, "", Sem, "", "", fileName);
                CommonClass.MoveFile(fileName, rptpath);
                error.success = true;
                error.Message = fileName;
            }
            catch (Exception ex)
            {
                error.success = false;
                error.Message = ex.Message;
            }
            return error;
        }
        protected static int ReturnNumberofExams(string Lec, string Unit, string Sem, string classCode)
        {
            int i = 0;
            try
            {
                string page = "ExamEntrySetup?$filter=Lecturer eq '" + Lec + "' and Unit eq '" + Unit + "' and Semester eq '" + Sem + "' and Class_Code eq '" + classCode + "'&$format=json";
                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);
                    if (details["value"].Count() > 0)
                    {
                        i = details["value"].Count();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return i;
        }
        public static string GetDocRejectionComment(string DocNo, int SeqNo)
        {
            string comment = "";
            try
            {
                string page = "ApprovalComments?$select=Comment&$filter=Document_No eq '" + DocNo + "' and Sequence_No eq " + SeqNo + "&$format=json";
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
        public static string StudentStatus(string custNo)
        {
            string s = "";
            try
            {
                string page = "StudentCard?$select=Status&$filter=No eq '" + custNo + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        s = (string)config["Status"];
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return s;
        }
        public static string StudentAttendance(string DocNo, string StdNo)
        {
            string s = "";
            try
            {
                string page = "ClassAttendanceLines?$select=Attendance&$filter=Code eq '" + DocNo + "' and StudentNo eq '" + StdNo + "'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        s = (string)config["Attendance"];
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return s;
        }
        public static decimal[] GetLeaveBal(string StaffNo, string LvType)
        {
            decimal[] LvDays = new decimal[5];
            try
            {
                LeaveBalance newBal = new LeaveBalance();

                #region LeaveBal
                List<DropDownBalance> Lvbal = new List<DropDownBalance>();
                string page = "HRLeaveLedger?$filter=EmployeeNo eq '" + StaffNo + "' and LeaveType eq '" + LvType + "' and Closed eq false&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        if ((string)config["TransactionType"] == "Positive Adjustment" && (string)config["EntryType"] == "Allocation")
                        {
                            LvDays[0] = LvDays[0] + (decimal)config["NoofDays"];
                        }
                        if ((string)config["TransactionType"] == "Negative Adjustment" && (string)config["EntryType"] == "Allocation")
                        {
                            LvDays[0] = LvDays[0] - Math.Abs((decimal)config["NoofDays"]);
                        }
                        if ((string)config["TransactionType"] == "Carry Forward" && (string)config["EntryType"] == "Allocation")
                        {
                            LvDays[1] = LvDays[1] + (decimal)config["NoofDays"];
                        }
                        if ((string)config["TransactionType"] == "Positive Adjustment" && (string)config["EntryType"] == "Reimbursement")
                        {
                            LvDays[2] = LvDays[2] + (decimal)config["NoofDays"];

                        }
                        if ((string)config["TransactionType"] == "Negative Adjustment" && (string)config["EntryType"] == "Application")
                        {
                            LvDays[3] = LvDays[3] + Math.Abs((decimal)config["NoofDays"]);
                        }
                    }
                    LvDays[4] = (LvDays[0] + LvDays[1] + LvDays[2] - LvDays[3]);
                }
                #endregion
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return LvDays;
        }
        public static int GetMaximumNoOfAttendance(string Sem, string Unit, string Campus, string Section)
        {
            int maxA = 0;

            string page = "Timetable?$filter=Semester eq '" + Sem + "' and Unit eq '" + Unit + "' and Campus_Code eq '" + Campus + "' and Unit_Class eq '" + Section + "'&$format=json";
            HttpWebResponse httpResponsel = Credentials.GetOdataData(page);
            using (var streamReaderl = new StreamReader(httpResponsel.GetResponseStream()))
            {
                var resultl = streamReaderl.ReadToEnd();

                var detailsl = JObject.Parse(resultl);
                if (detailsl["value"].Count() > 0)
                {
                    foreach (JObject configl in detailsl["value"])
                    {
                        string pageLine = "WeeksList?$count=true&$filter=Semester eq '" + Sem + "' and Day eq '" + (string)configl["DayofWeek"] + "' and Inactive eq false&$format=json";
                        HttpWebResponse httpResponse = Credentials.GetOdataData(pageLine);
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();

                            var details = JObject.Parse(result);
                            if ((int)details["@odata.count"] > 0)
                            {
                                maxA = (int)details["@odata.count"];
                            }
                        }
                    }
                }
            }
            return maxA;
        }
        public static int GetTotalAttendance(string StdNo, string Sem, string Unit)
        {
            int count = 0;
            string pageLine = "ClassAttendanceLines?$count=true&$filter=StudentNo eq '" + StdNo + "' and Semester eq '" + Sem + "' and UnitCode eq '" + Unit + "' and Attendance eq 1 and Posted eq true&$format=json";
            HttpWebResponse httpResponse = Credentials.GetOdataData(pageLine);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var details = JObject.Parse(result);
                if ((int)details["@odata.count"] > 0)
                {
                    count = (int)details["@odata.count"];
                }
            }
            return count;
        }
        public static decimal GetPercentageAttendance(string StdNo, string Sem, string Unit, decimal MaxA)
        {
            decimal Perc = 0;
            try
            {
                decimal TotalA = Convert.ToDecimal(GetTotalAttendance(StdNo, Sem, Unit));
                Perc = (TotalA / MaxA) * 100;
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return Perc;
        }
        public static bool AllowLecEvaluationOnlineViewing(string Sem)
        {
            bool allow = false;
            try
            {
                string page = "SemesterList?$filter=Code eq '" + Sem + "' and Allow_Lec_Eva_Online eq true&format=json";

                HttpWebResponse httpResponseResC = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    if (details["value"].Count() > 0)
                    {
                        allow = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return allow;
        }
    }
}