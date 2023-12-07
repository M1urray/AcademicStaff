using Latest_Staff_Portal.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

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
        public static string Current_HR_Calender()
        {
            string code = "";
            try
            {
                string page = "HRLeaveCalender?$select=Code&$filter=Current eq true&format=json";

                HttpWebResponse httpResponseResC = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponseResC.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        code = (string)config["Code"];
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return code;
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
                PicString = Credentials.ObjNav.GetProfilePicture(User);
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
                        s[4] = (string)config["Director"];
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
                if (File.Exists(destinationfile) == true)
                {
                    File.Delete(destinationfile);
                    File.Move(sourcefile, destinationfile);
                }
                if (File.Exists(destinationfile) == false)
                {
                    File.Move(sourcefile, destinationfile);
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
                x = Credentials.ObjNav.SendEmail(ref recepient, subject, body);
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
        public static string[] UploadStudentMarks(string Unit, string Sem, string Lec, string Campus, string ClassCode, string filePath)
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
                    if (HasStudentRegForUnit(studentNo, Unit, Sem))
                    {
                        string page = "ExamEntrySetup?$filter=Lecturer eq '" + Lec + "' and Unit eq '" + Unit + "' and Semester eq '" + Sem
                            + "' and Class_Code eq '" + ClassCode + "' and Deleted eq false&$format=json";

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
                                        decimal mxmScore = 0;
                                        decimal weight = 0;
                                        decimal AssinedScore = 0;

                                        if (!string.IsNullOrEmpty(ExamMark))
                                        {
                                            mxmScore = Convert.ToDecimal(config["Max_Score"].ToString());
                                            weight = Convert.ToDecimal(config["Contribution"].ToString());
                                            AssinedScore = Convert.ToDecimal(ExamMark);

                                            if (AssinedScore > mxmScore)
                                            {
                                                Uploaded[0] = ((string)config["Entry_Code"] + " assined score for <b>" + studentNo + "</b> can not be greater than " + mxmScore + ", maximum allowed score");
                                                Uploaded[1] = "F";
                                                return Uploaded;
                                            }
                                            else
                                            {
                                                if (AssinedScore > 0)
                                                {
                                                    string examType = "";
                                                    if ((string)config["No_of_Times"] == "Once")
                                                    {
                                                        examType = "EXAM";
                                                    }
                                                    else
                                                    {
                                                        examType = "CAT";
                                                    }
                                                    decimal Contrb = (AssinedScore / mxmScore) * weight;

                                                    Credentials.ObjNav.EnterRowMarks(
                                                    prog: "",
                                                    stage: "",
                                                    unit: Unit,
                                                    sem: Sem,
                                                    score: AssinedScore,
                                                    contrib: Contrb,
                                                    stdNo: studentNo,
                                                    examType: examType,
                                                    lecturer: Lec,
                                                    entryType: (string)config["Entry_Code"]
                                                    );

                                                    if (!UpScc)
                                                    {
                                                        i++;
                                                        UpScc = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    j++;
                                }
                            }
                        }
                    }
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
                //string rptpath = Server.MapPath("~/MarkSheets/");
                //#region Variables

                //Font TitleReport = FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK);
                //Font tableTh = FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK);
                //Font tableTd = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK);
                //Font fnttableHeader = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLUE);

                //BaseColor RowTh = new BaseColor(191, 219, 255);
                //BaseColor EvenTd = new BaseColor(227, 234, 235);
                //BaseColor OddTd = new BaseColor(255, 255, 255);

                //#endregion

                //#region =============================== CREATE PDF  ===============================

                //string fileName = String.Format("Mark_Sheet_{0}.pdf", Lec.Replace("/", "_"));
                //string filenamePath = String.Format("{0}{1}", rptpath, Unit.Replace("/", "_") + "-" + fileName);
                ////string ImagePath = Server.MapPath("~/images");

                //#region Check If File exist
                //try
                //{
                //    if (File.Exists(filenamePath))
                //    {
                //        File.Delete(filenamePath);
                //    }
                //}
                //catch (Exception Ex)
                //{
                //    Ex.Data.Clear();
                //}
                //#endregion

                //Document doc_LOAN_CALCULATOR_RPT = new Document(PageSize.A4);
                //doc_LOAN_CALCULATOR_RPT.SetMargins(20f, 10f, 20f, 10f); // Left, Bottom,Top,Right
                //doc_LOAN_CALCULATOR_RPT.HtmlStyleClass = "background:red";


                //MemoryStream pdfStream = new MemoryStream();
                //using (MemoryStream myMemoryStream = new MemoryStream())
                //{
                //    //PdfWriter pdfWriter = PdfWriter.GetInstance(doc_LOAN_CALCULATOR_RPT, new FileStream(filenamePath, FileMode.Create));
                //    PdfWriter pdfWriter = PdfWriter.GetInstance(doc_LOAN_CALCULATOR_RPT, myMemoryStream);
                //    doc_LOAN_CALCULATOR_RPT.Open();

                //    #endregion

                //    #region ++++++++++++++++++ REPORT TABLE Logo++++++++++++++++++++++++++++++++++


                //    string Logo_Path = String.Format("{0}/logo2.png", ImagePath);

                //    PdfPTable tableFirstApplicationLogo = new PdfPTable(3) { TotalWidth = 560f, LockedWidth = true };
                //    float[] widthsLogo = new float[] { 225f, 110f, 225f };
                //    tableFirstApplicationLogo.SetWidths(widthsLogo);
                //    tableFirstApplicationLogo.DefaultCell.Border = PdfPCell.NO_BORDER;


                //    PdfPCell Logo_a6 = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK))) { Border = 0 };
                //    tableFirstApplicationLogo.AddCell(Logo_a6);

                //    iText.text.Image Logo_jpg = iText.text.Image.GetInstance(Logo_Path);
                //    Logo_jpg.ScaleToFit(80f, 60f);
                //    Logo_jpg.Border = 0;
                //    Logo_jpg.BorderWidth = 0;
                //    Logo_jpg.UseVariableBorders = false;
                //    Logo_jpg.Alignment = Element.ALIGN_CENTER;

                //    tableFirstApplicationLogo.AddCell(Logo_jpg);

                //    DateTime dt = DateTime.Now;
                //    string daydatetome = String.Format("{0:f}", dt);

                //    PdfPCell Logo_c = new PdfPCell(new Phrase("" + daydatetome, FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK))) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT };
                //    tableFirstApplicationLogo.AddCell(Logo_c);

                //    PdfPCell Title2 = new PdfPCell(new Phrase("Individual Mark Sheet", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Colspan = 3, BorderWidthRight = 0f, BorderWidthLeft = 0f, BorderWidthTop = 0f, BorderWidthBottom = 0.6f, HorizontalAlignment = Element.ALIGN_CENTER };
                //    tableFirstApplicationLogo.AddCell(Title2);


                //    doc_LOAN_CALCULATOR_RPT.Add(tableFirstApplicationLogo);


                //    #endregion

                //    #region CREATE Header

                //    PdfPTable tableHeader = new PdfPTable(4) { TotalWidth = 560f, LockedWidth = true, SpacingBefore = 5f, };
                //    tableHeader.DefaultCell.Border = PdfPCell.NO_BORDER;

                //    float[] widthsHeader = new float[] { 120f, 200f, 100f, 100f };
                //    tableHeader.SetWidths(widthsHeader);


                //    #region Header Details

                //    PdfPCell cellLoanType = new PdfPCell(new Phrase("Lecturer Name :  ", tableTd)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(cellLoanType);

                //    PdfPCell cellLoanTypeb = new PdfPCell(new Phrase(CommonClass.GetEmployeeName(Lec), fnttableHeader)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(cellLoanTypeb);

                //    PdfPCell cellLoanTypec = new PdfPCell(new Phrase("", tableTd)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(cellLoanTypec);

                //    PdfPCell AdmNo = new PdfPCell(new Phrase("", fnttableHeader)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(AdmNo);

                //    PdfPCell cellLoanType1 = new PdfPCell(new Phrase("Lecturer Number :  ", tableTd)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(cellLoanType1);

                //    PdfPCell cellLoanTypeb1 = new PdfPCell(new Phrase(Lec, fnttableHeader)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(cellLoanTypeb1);

                //    PdfPCell cellLoanTypec1 = new PdfPCell(new Phrase("", tableTd)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(cellLoanTypec1);

                //    PdfPCell AdmNo1 = new PdfPCell(new Phrase("", fnttableHeader)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(AdmNo1);

                //    PdfPCell cellLoanType2 = new PdfPCell(new Phrase("Campus :  ", tableTd)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(cellLoanType2);

                //    PdfPCell cellLoanTypeb2 = new PdfPCell(new Phrase(Campus, fnttableHeader)) { Colspan = 2, Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(cellLoanTypeb2);

                //    PdfPCell cellLoanTypec2 = new PdfPCell(new Phrase("", tableTd)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(cellLoanTypec2);


                //    //String.Format("{0:0,0.00}", Monthly_Payment)

                //    PdfPCell lblDept = new PdfPCell(new Phrase("Programme :  ", tableTd)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(lblDept);

                //    PdfPCell DeptName = new PdfPCell(new Phrase(Prog + "-" + CommonClass.GetProgrammeName(Prog), fnttableHeader)) { Colspan = 3, Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(DeptName);

                //    PdfPCell cellRepaymentPeriod = new PdfPCell(new Phrase("Unit :  ", tableTd)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(cellRepaymentPeriod);


                //    PdfPCell cellRepaymentPeriodb = new PdfPCell(new Phrase(Unit + " - " + UnitName, fnttableHeader)) { Colspan = 3, Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(cellRepaymentPeriodb);

                //    PdfPCell cellClassCode = new PdfPCell(new Phrase("Course Class :  ", tableTd)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(cellClassCode);


                //    PdfPCell cellClassCodedata = new PdfPCell(new Phrase(classCode, fnttableHeader)) { Colspan = 3, Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(cellClassCodedata);

                //    PdfPCell cellSem = new PdfPCell(new Phrase("Semester:  ", tableTd)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(cellSem);


                //    PdfPCell cellSemester = new PdfPCell(new Phrase(Sem, fnttableHeader)) { Colspan = 3, Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableHeader.AddCell(cellSemester);


                //    #endregion

                //    #region Horizontal line

                //    PdfPCell hrlive = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Colspan = 4, Border = 0, BorderWidthRight = 0f, BorderWidthLeft = 0f, BorderWidthTop = 0f, BorderWidthBottom = 0.5f, HorizontalAlignment = Element.ALIGN_CENTER };
                //    tableHeader.AddCell(hrlive);


                //    #endregion

                //    doc_LOAN_CALCULATOR_RPT.Add(tableHeader);

                //    #endregion

                //    #region +++++++++++ REPORT BODY +++++++++++++++

                //    int Counter = ReturnNumberofExams(Lec, Unit, Sem, classCode);
                //    int CellsNo = Counter + 5;
                //    PdfPTable tableBody = new PdfPTable(CellsNo) { TotalWidth = 560f, LockedWidth = true, SpacingBefore = 5f, SplitRows = false };
                //    tableBody.DefaultCell.Border = PdfPCell.NO_BORDER;

                //    //float[] widthsBody = new float[] { 40f, 90f, 120f, 50f, 50f, 50f, 50f };

                //    List<float> lst = new List<float>();
                //    lst.Add(40f);
                //    lst.Add(90f);
                //    lst.Add(120f);
                //    for (int j = 0; j < Counter; j++)
                //    {
                //        lst.Add(50f);
                //    }
                //    lst.Add(50f);
                //    lst.Add(50f);
                //    float[] widthsBody = lst.ToArray();
                //    tableBody.SetWidths(widthsBody);

                //    #region Items
                //    PdfPCell ItemsHeader1 = new PdfPCell(new Phrase("#", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK))) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableBody.AddCell(ItemsHeader1);

                //    PdfPCell ItemsHeader2 = new PdfPCell(new Phrase("Reg. No", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK))) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableBody.AddCell(ItemsHeader2);

                //    PdfPCell ItemsHeader3 = new PdfPCell(new Phrase("Name of Candidate", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK))) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableBody.AddCell(ItemsHeader3);

                //    var details = new JObject();
                //    string page = "ExamEntrySetup?$filter=Lecturer eq '" + Lec + "' and Unit eq '" + Unit + "' and Semester eq '" + Sem + "' and Class_Code eq '" + classCode + "' and Deleted eq false&$format=json";
                //    HttpWebResponse httpResponse = Credentials.GetOdataData(page);
                //    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                //    {
                //        var result = streamReader.ReadToEnd();
                //        details = JObject.Parse(result);
                //    }

                //    if (details["value"].Count() > 0)
                //    {
                //        foreach (JObject config in details["value"])
                //        {
                //            PdfPCell ItemsHeader4 = new PdfPCell(new Phrase((string)config["Entry_Code"], FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK))) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
                //            tableBody.AddCell(ItemsHeader4);
                //        }
                //    }

                //    PdfPCell ItemsHeader8 = new PdfPCell(new Phrase("Total", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK))) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
                //    tableBody.AddCell(ItemsHeader8);

                //    PdfPCell ItemsHeader9 = new PdfPCell(new Phrase("Grade", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK))) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
                //    tableBody.AddCell(ItemsHeader9);


                //    #region Horizontal line

                //    PdfPCell hrline = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Colspan = CellsNo, Border = 0, BorderWidthRight = 0f, BorderWidthLeft = 0f, BorderWidthTop = 0f, BorderWidthBottom = 0.5f, HorizontalAlignment = Element.ALIGN_CENTER };
                //    tableBody.AddCell(hrline);

                //    //BorderWidthRight = 0f, BorderWidthLeft = 0f, BorderWidthTop = 0f,

                //    #endregion

                //    //Sp get grades
                //    int A = 0, B = 0, C = 0, D = 0, F = 0, IncCount = 0;

                //    #region Get Grades
                //    int i = 1, No = 0;

                //    //string pageStudentList = "StudentUnits?$filter=Programme eq '" + Prog + "' and Stage eq '" + Stage + "' and Semester eq '" + Sem + "' and Unit eq '" + Unit + "'&$format=json";
                //    string pageStudentList = "StudentUnits?$filter=Semester eq '" + Sem + "' and Unit eq '" + Unit + "' and Unit_Class_Code eq '" + classCode + "' and Campus eq '" + Campus + "'&$format=json";
                //    HttpWebResponse httpResponseStudentList = Credentials.GetOdataData(pageStudentList);
                //    using (var streamReader = new StreamReader(httpResponseStudentList.GetResponseStream()))
                //    {
                //        var result = streamReader.ReadToEnd();

                //        var Studentdetails = JObject.Parse(result);

                //        if (Studentdetails["value"].Count() > 0)
                //        {
                //            foreach (JObject config in Studentdetails["value"])
                //            {
                //                string Name = (string)config["Name"];
                //                string StudentNo = (string)config["Student_No"];
                //                decimal Total = 0;

                //                PdfPCell col1 = new PdfPCell(new Phrase(i.ToString(), fnttableHeader)) { BorderWidthRight = 0f, BorderWidthLeft = 0f, HorizontalAlignment = Element.ALIGN_LEFT };
                //                tableBody.AddCell(col1);

                //                PdfPCell col2 = new PdfPCell(new Phrase(StudentNo, fnttableHeader)) { BorderWidthRight = 0f, BorderWidthLeft = 0f, HorizontalAlignment = Element.ALIGN_LEFT };
                //                tableBody.AddCell(col2);

                //                PdfPCell col3 = new PdfPCell(new Phrase(Name, fnttableHeader)) { BorderWidthRight = 0f, BorderWidthLeft = 0f, HorizontalAlignment = Element.ALIGN_LEFT };
                //                tableBody.AddCell(col3);
                //                bool MarkExist = false, MarkDoesNotExist = true;
                //                decimal ExamMark = 0;

                //                if (details["value"].Count() > 0)
                //                {
                //                    foreach (JObject config1 in details["value"])
                //                    {
                //                        string TextValue = "";
                //                        string pageResults = "ExamResults?$top=1&$filter=Student_No eq '" + StudentNo + "' and Semester eq '" + Sem + "' and ExamType eq '" + config1["Entry_Code"].ToString() + "' and Unit eq '" + Unit + "' and Cancelled eq false&$format=json";
                //                        HttpWebResponse httpResponseResults = Credentials.GetOdataData(pageResults);
                //                        using (var streamReaderResults = new StreamReader(httpResponseResults.GetResponseStream()))
                //                        {
                //                            var result1 = streamReaderResults.ReadToEnd();
                //                            var details1 = JObject.Parse(result1);
                //                            if (details1["value"].Count() > 0)
                //                            {
                //                                foreach (JObject config2 in details1["value"])
                //                                {
                //                                    TextValue = "";
                //                                    ExamMark = 0;
                //                                    TextValue = Math.Round(Convert.ToDecimal(config2["Contribution"])).ToString();
                //                                    PdfPCell col4 = new PdfPCell();
                //                                    if (TextValue != "")
                //                                    {
                //                                        MarkExist = true;
                //                                        ExamMark = Convert.ToDecimal(TextValue);
                //                                        Total = Total + ExamMark;
                //                                        col4 = new PdfPCell(new Phrase(ExamMark.ToString(), fnttableHeader)) { BorderWidthRight = 0f, BorderWidthLeft = 0f, HorizontalAlignment = Element.ALIGN_CENTER };
                //                                        tableBody.AddCell(col4);
                //                                    }
                //                                    else
                //                                    {
                //                                        MarkDoesNotExist = false;
                //                                        col4 = new PdfPCell(new Phrase("-", fnttableHeader)) { BorderWidthRight = 0f, BorderWidthLeft = 0f, HorizontalAlignment = Element.ALIGN_CENTER };
                //                                        tableBody.AddCell(col4);
                //                                    }
                //                                }
                //                            }
                //                            else
                //                            {
                //                                PdfPCell col4 = new PdfPCell();
                //                                MarkDoesNotExist = false;
                //                                col4 = new PdfPCell(new Phrase("-", fnttableHeader)) { BorderWidthRight = 0f, BorderWidthLeft = 0f, HorizontalAlignment = Element.ALIGN_CENTER };
                //                                tableBody.AddCell(col4);
                //                            }
                //                        }
                //                    }
                //                }

                //                string Grade = "";
                //                if (MarkExist && MarkDoesNotExist)
                //                {
                //                    Grade = "";// Credentials.ObjNav.GetGrade(Convert.ToDecimal(Total), Unit, Prog);
                //                    if (Grade == "A")
                //                    {
                //                        A++;
                //                    }
                //                    if (Grade == "B")
                //                    {
                //                        B++;
                //                    }
                //                    if (Grade == "C")
                //                    {
                //                        C++;
                //                    }
                //                    if (Grade == "D")
                //                    {
                //                        D++;
                //                    }
                //                    if (Grade == "F")
                //                    {
                //                        F++;
                //                    }
                //                }
                //                else
                //                {
                //                    Grade = "Inc";
                //                    IncCount++;

                //                }
                //                PdfPCell col8 = new PdfPCell();
                //                if (MarkExist)
                //                {
                //                    col8 = new PdfPCell(new Phrase(Math.Round(Total, 2).ToString(), fnttableHeader)) { BorderWidthRight = 0f, BorderWidthLeft = 0f, HorizontalAlignment = Element.ALIGN_CENTER };
                //                    tableBody.AddCell(col8);
                //                }
                //                else
                //                {
                //                    col8 = new PdfPCell(new Phrase("-", fnttableHeader)) { BorderWidthRight = 0f, BorderWidthLeft = 0f, HorizontalAlignment = Element.ALIGN_CENTER };
                //                    tableBody.AddCell(col8);
                //                }

                //                PdfPCell col9 = new PdfPCell(new Phrase(Grade, fnttableHeader)) { BorderWidthRight = 0f, BorderWidthLeft = 0f, HorizontalAlignment = Element.ALIGN_CENTER };
                //                tableBody.AddCell(col9);

                //                #region Horizontal line
                //                i++;
                //                No++;
                //                #endregion
                //            }
                //        }
                //    }

                //    PdfPCell border3 = new PdfPCell(new Phrase("", fnttableHeader)) { Colspan = CellsNo, PaddingBottom = 20, Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableBody.AddCell(border3);
                //    #endregion


                //    doc_LOAN_CALCULATOR_RPT.Add(tableBody);

                //    #endregion

                //    #region GradeSummery
                //    PdfPTable tableGradeSummery = new PdfPTable(4) { TotalWidth = 560f, LockedWidth = true, SpacingBefore = 5f, };
                //    tableGradeSummery.DefaultCell.Border = PdfPCell.NO_BORDER;
                //    tableGradeSummery.KeepTogether = true;

                //    float[] widthsGradeSummery = new float[] { 40f, 60f, 60f, 40f };
                //    tableGradeSummery.SetWidths(widthsGradeSummery);

                //    PdfPCell SpaceGradeSum = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Colspan = 4, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
                //    tableHeader.AddCell(SpaceGradeSum);


                //    #region GradeKeySummery

                //    PdfPCell HeaderSpace = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_MIDDLE };
                //    tableGradeSummery.AddCell(HeaderSpace);

                //    PdfPCell GradeHeader = new PdfPCell(new Phrase("GRADES SUMMERY", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Colspan = 2, Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_MIDDLE };
                //    tableGradeSummery.AddCell(GradeHeader);

                //    PdfPCell GKAGrade = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableGradeSummery.AddCell(GKAGrade);

                //    PdfPCell space1 = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_MIDDLE };
                //    tableGradeSummery.AddCell(space1);

                //    PdfPCell GAGrade = new PdfPCell(new Phrase("A", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { BorderWidthLeft = 1, BorderWidthRight = 0, BorderWidthTop = 1, BorderWidthBottom = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_MIDDLE };
                //    tableGradeSummery.AddCell(GAGrade);

                //    PdfPCell GA = new PdfPCell(new Phrase(A.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { BorderWidthLeft = 0, BorderWidthTop = 1, BorderWidthRight = 1, BorderWidthBottom = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableGradeSummery.AddCell(GA);

                //    PdfPCell GKBGrade = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableGradeSummery.AddCell(GKBGrade);

                //    PdfPCell space2 = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_MIDDLE };
                //    tableGradeSummery.AddCell(space2);

                //    PdfPCell GBGrade = new PdfPCell(new Phrase("B", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { BorderWidthLeft = 1, BorderWidthRight = 0, BorderWidthTop = 1, BorderWidthBottom = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableGradeSummery.AddCell(GBGrade);

                //    PdfPCell GB = new PdfPCell(new Phrase(B.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { BorderWidthLeft = 0, BorderWidthTop = 1, BorderWidthRight = 1, BorderWidthBottom = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableGradeSummery.AddCell(GB);

                //    PdfPCell GKCGrade = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableGradeSummery.AddCell(GKCGrade);

                //    PdfPCell space3 = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_MIDDLE };
                //    tableGradeSummery.AddCell(space3);

                //    PdfPCell GCGrade = new PdfPCell(new Phrase("C", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { BorderWidthLeft = 1, BorderWidthRight = 0, BorderWidthTop = 1, BorderWidthBottom = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableGradeSummery.AddCell(GCGrade);

                //    PdfPCell GC = new PdfPCell(new Phrase(C.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { BorderWidthLeft = 0, BorderWidthTop = 1, BorderWidthRight = 1, BorderWidthBottom = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableGradeSummery.AddCell(GC);

                //    PdfPCell GKGrade = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableGradeSummery.AddCell(GKGrade);

                //    PdfPCell space4 = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_MIDDLE };
                //    tableGradeSummery.AddCell(space4);

                //    PdfPCell GDGrade = new PdfPCell(new Phrase("D", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { BorderWidthLeft = 1, BorderWidthRight = 0, BorderWidthTop = 1, BorderWidthBottom = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableGradeSummery.AddCell(GDGrade);

                //    PdfPCell GD = new PdfPCell(new Phrase(D.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { BorderWidthLeft = 0, BorderWidthTop = 1, BorderWidthRight = 1, BorderWidthBottom = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableGradeSummery.AddCell(GD);

                //    PdfPCell GEKGrade = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableGradeSummery.AddCell(GEKGrade);

                //    PdfPCell space5 = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_MIDDLE };
                //    tableGradeSummery.AddCell(space5);

                //    PdfPCell GEGrade = new PdfPCell(new Phrase("F", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { BorderWidthLeft = 1, BorderWidthRight = 0, BorderWidthTop = 1, BorderWidthBottom = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableGradeSummery.AddCell(GEGrade);

                //    PdfPCell GE = new PdfPCell(new Phrase(F.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { BorderWidthLeft = 0, BorderWidthTop = 1, BorderWidthRight = 1, BorderWidthBottom = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableGradeSummery.AddCell(GE);

                //    PdfPCell space6 = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_MIDDLE };
                //    tableGradeSummery.AddCell(space6);

                //    PdfPCell space10 = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_MIDDLE };
                //    tableGradeSummery.AddCell(space10);

                //    PdfPCell GincGrade = new PdfPCell(new Phrase("Incomplete", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { BorderWidthLeft = 1, BorderWidthRight = 0, BorderWidthTop = 1, BorderWidthBottom = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableGradeSummery.AddCell(GincGrade);

                //    PdfPCell GInc = new PdfPCell(new Phrase(IncCount.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { BorderWidthLeft = 0, BorderWidthTop = 1, BorderWidthRight = 1, BorderWidthBottom = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableGradeSummery.AddCell(GInc);

                //    PdfPCell space7 = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_MIDDLE };
                //    tableGradeSummery.AddCell(space7);

                //    PdfPCell space11 = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_MIDDLE };
                //    tableGradeSummery.AddCell(space11);

                //    PdfPCell GTotal = new PdfPCell(new Phrase("Total", FontFactory.GetFont("Arial", 12, Font.BOLD, BaseColor.BLACK))) { BorderWidthLeft = 1, BorderWidthRight = 0, BorderWidthTop = 1, BorderWidthBottom = 1, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableGradeSummery.AddCell(GTotal);

                //    PdfPCell GTotalCount = new PdfPCell(new Phrase(No.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { BorderWidthLeft = 0, BorderWidthTop = 1, BorderWidthRight = 1, BorderWidthBottom = 1, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableGradeSummery.AddCell(GTotalCount);

                //    PdfPCell space12 = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_MIDDLE };
                //    tableGradeSummery.AddCell(space12);
                //    #endregion
                //    #endregion

                //    doc_LOAN_CALCULATOR_RPT.Add(tableGradeSummery);
                //    #region Signature

                //    PdfPTable tableFooter = new PdfPTable(4) { TotalWidth = 560f, LockedWidth = true, SpacingBefore = 5f, };
                //    tableFooter.DefaultCell.Border = PdfPCell.NO_BORDER;

                //    float[] widthsFooter = new float[] { 80f, 200f, 130f, 150f };
                //    tableFooter.SetWidths(widthsFooter);

                //    PdfPCell Space = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK))) { Colspan = 4, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
                //    tableHeader.AddCell(Space);

                //    PdfPCell CheckIn = new PdfPCell(new Phrase("Signed by: ......................................................................", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK))) { Colspan = 2, Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableFooter.AddCell(CheckIn);
                //    #region Horizontal line
                //    PdfPCell hrLine = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))) { Colspan = 4, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
                //    tableHeader.AddCell(hrLine);
                //    #endregion
                //    PdfPCell Signature2 = new PdfPCell(new Phrase("Signed by: ....................................................................", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK))) { Colspan = 2, Border = 0, PaddingTop = 10, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableFooter.AddCell(Signature2);

                //    PdfPCell DateSignature = new PdfPCell(new Phrase("Internal Examiner                  /Date", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK))) { Colspan = 2, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
                //    tableFooter.AddCell(DateSignature);

                //    PdfPCell signature3 = new PdfPCell(new Phrase("External Examiner                  /Date", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK))) { Colspan = 2, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
                //    tableFooter.AddCell(signature3);

                //    PdfPCell emptycell5 = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, PaddingBottom = 10, Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableFooter.AddCell(emptycell5);

                //    PdfPCell CheckIn2 = new PdfPCell(new Phrase("Signed by: ......................................................................", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK))) { Colspan = 2, Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableFooter.AddCell(CheckIn2);

                //    PdfPCell Signature4 = new PdfPCell(new Phrase("Signed by: ....................................................................", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK))) { Colspan = 2, Border = 0, HorizontalAlignment = Element.ALIGN_LEFT };
                //    tableFooter.AddCell(Signature4);

                //    PdfPCell DateSignature2 = new PdfPCell(new Phrase("Head of Department                  /Date", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK))) { Colspan = 2, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
                //    tableFooter.AddCell(DateSignature2);

                //    PdfPCell signature6 = new PdfPCell(new Phrase("Dean/Director                  /Date", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK))) { Colspan = 2, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
                //    tableFooter.AddCell(signature6);

                //    doc_LOAN_CALCULATOR_RPT.Add(tableFooter);
                //    #endregion

                //    #endregion

                //    pdfWriter.CloseStream = true;
                //    doc_LOAN_CALCULATOR_RPT.Close();

                //    byte[] content = myMemoryStream.ToArray();

                //    // Write out PDF from memory stream.
                //    using (FileStream fs = File.Create(filenamePath))
                //    {
                //        fs.Write(content, 0, (int)content.Length);
                //    }
                //}
                string fileName = Unit.Replace("/", "_") + "-" + String.Format("Mark_Sheet_{0}.pdf", Lec.Replace("/", "_"));
                string filenamePath = String.Format("{0}{1}", rptpath, fileName);
                Credentials.ObjNav.GenerateScoreSheet("", Unit, "", Sem, classCode, Campus, fileName);
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