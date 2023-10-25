using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ProgAssign1
{

    internal class CSVParser
    {
        private readonly String logFile;
        private StringBuilder sbLog;
        private DateTime localStartTime;
        private FileOperation fo;
        private List<string> listOfFiles;
        private string outputFileNm;
        private StringBuilder outputContent;
        Dictionary<int, Dictionary<string, int>> fileInfo;
        Dictionary<string, int> globalInfo;
        private int sbCapacity;
        List<Customer> outputFile = new List<Customer>();
        Dictionary<string, List<string>> possibleColName = new Dictionary<string, List<string>>();
        string emailRegEx = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

        public CSVParser(string outputFileNm, string logFile, StringBuilder sb, List<string> Lst, FileOperation fo)
        {
            this.logFile = logFile;
            this.fo = fo;
            this.sbLog = sb;
            this.listOfFiles = Lst;
            this.outputFileNm = outputFileNm;
            this.localStartTime = DateTime.Now;
            this.outputContent = new StringBuilder();
            this.fileInfo = new Dictionary<int, Dictionary<string, int>>();
            this.globalInfo = new Dictionary<string, int>();
            sbCapacity = Math.Min(sbLog.MaxCapacity, 100000);

            this.globalInfo.Add("NUMBER_OF_FILE", this.listOfFiles.Count);
            this.globalInfo.Add("GLOBAL_PROCESSED_ROW", 0);
            this.globalInfo.Add("GLOBAL_SKIPPED_ROW", 0);

            possibleColName.Add("FIRST_NAME", new List<string>());
            possibleColName.Add("LAST_NAME", new List<string>());
            possibleColName.Add("STREET_NO", new List<string>());
            possibleColName.Add("STREET_NAME", new List<string>());
            possibleColName.Add("ZIP_CODE", new List<string>());
            possibleColName.Add("CITY", new List<string>());
            possibleColName.Add("PROVINCE", new List<string>());
            possibleColName.Add("COUNTRY", new List<string>());
            possibleColName.Add("PHONE", new List<string>());
            possibleColName.Add("EMAIL", new List<string>());

            possibleColName["FIRST_NAME"].Add("FIRST_NAME");
            possibleColName["FIRST_NAME"].Add("FIRSTNAME");

            possibleColName["LAST_NAME"].Add("LAST_NAME");
            possibleColName["LAST_NAME"].Add("LASTNAME");

            possibleColName["STREET_NO"].Add("STREET_NO");
            possibleColName["STREET_NO"].Add("STREETNO");
            possibleColName["STREET_NO"].Add("ST_NO");
            possibleColName["STREET_NO"].Add("STNO");
            possibleColName["STREET_NO"].Add("STREET_NUMBER");
            possibleColName["STREET_NO"].Add("STREETNUMBER");

            possibleColName["STREET_NAME"].Add("STREET");
            possibleColName["STREET_NAME"].Add("STREET_NAME");
            possibleColName["STREET_NAME"].Add("STREETNAME");
            possibleColName["STREET_NAME"].Add("STREET_NM");
            possibleColName["STREET_NAME"].Add("STREETNM");
            possibleColName["STREET_NAME"].Add("ST_NM");
            possibleColName["STREET_NAME"].Add("ST_NAME");
            possibleColName["STREET_NAME"].Add("STNM");
            possibleColName["STREET_NAME"].Add("STNAME");

            possibleColName["ZIP_CODE"].Add("ZIP_CODE");
            possibleColName["ZIP_CODE"].Add("ZIPCODE");
            possibleColName["ZIP_CODE"].Add("POST_CODE");
            possibleColName["ZIP_CODE"].Add("POSTCODE");
            possibleColName["ZIP_CODE"].Add("POSTAL_CODE");
            possibleColName["ZIP_CODE"].Add("POSTALCODE");

            possibleColName["CITY"].Add("CITY");

            possibleColName["PROVINCE"].Add("PROVINCE");
            possibleColName["PROVINCE"].Add("PRNVC");

            possibleColName["COUNTRY"].Add("COUNTRY");
            possibleColName["COUNTRY"].Add("CNTRY");

            possibleColName["PHONE"].Add("PHONE");
            possibleColName["PHONE"].Add("PHONE_NUMBER");
            possibleColName["PHONE"].Add("PHONENUMBER");
            possibleColName["PHONE"].Add("PHONE_NO");
            possibleColName["PHONE"].Add("PHONENO");
            possibleColName["PHONE"].Add("MOBILE");
            possibleColName["PHONE"].Add("CELL_NUMBER");
            possibleColName["PHONE"].Add("CELLNUMBER");
            possibleColName["PHONE"].Add("CELL_NO");
            possibleColName["PHONE"].Add("CELLNO");

            possibleColName["EMAIL"].Add("EMAIL");
            possibleColName["EMAIL"].Add("EMAIL_ADDRESS");
            possibleColName["EMAIL"].Add("EMAILADDRESS");
            possibleColName["EMAIL"].Add("EMAIL_ID");
            possibleColName["EMAIL"].Add("EMAILID");
            possibleColName["EMAIL"].Add("MAIL_ID");
            possibleColName["EMAIL"].Add("MAILID");
            possibleColName["EMAIL"].Add("MAIL");
        }

        public DateOnly? getDateFromPath(string path)
        {
            DateOnly dt;
            DateTime fl;

            string loop_val;

            try
            {
                DirectoryInfo fi = new DirectoryInfo(path);
                loop_val = fi.Parent.Name.ToString()+"/"+ fi.Parent.Parent.Name.ToString() + "/"+ fi.Parent.Parent.Parent.Name.ToString();
                //fl = DateTime.ParseExact(loop_val, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                dt = DateOnly.Parse(loop_val);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void run()
        {

            localStartTime = DateTime.Now;

            Dictionary<string, int> filelevelDetails;
            DateTime start;
            DateTime end;

            int ind = -1;

            foreach (string singleFile in listOfFiles)
            {
                ind++;
                start = DateTime.Now;

                List<dynamic> t = fo.readCSVFile(singleFile);
                sbLog.Append("STARTED processing of FILE NO - " + (ind + 1) + Environment.NewLine);
                sbLog.Append("File Location = " + singleFile + Environment.NewLine);

                DateOnly? dt = getDateFromPath(singleFile);
                if (dt is not null)
                {
                    sbLog.Append("File_Date = " + dt.ToString() + Environment.NewLine);
                }
                else
                {
                    sbLog.Append("File_Date = NULL" + Environment.NewLine);
                }

                filelevelDetails = processFile(t, dt);

                if (outputFile.Count > 1000)
                {
                    sbLog.Append("Flushing the CUSTOMER DATA ARRAY to File" + Environment.NewLine);
                    fo.writeCustomerToFile(outputFileNm, outputFile);
                }

                end = DateTime.Now;

                globalInfo["GLOBAL_PROCESSED_ROW"] = globalInfo["GLOBAL_PROCESSED_ROW"] + filelevelDetails["PROCESSED_ROW"];
                globalInfo["GLOBAL_SKIPPED_ROW"] = globalInfo["GLOBAL_SKIPPED_ROW"] + filelevelDetails["SKIPPED_ROW"];

                sbLog.Append("FINISHED processing of FILE NO - " + ind + " in " + (end - start) + Environment.NewLine);
                sbLog.Append("PROCESSED ROWS - " + filelevelDetails["PROCESSED_ROW"] + Environment.NewLine);
                sbLog.Append("SKIPPED ROWS - " + filelevelDetails["SKIPPED_ROW"] + Environment.NewLine);
                sbLog.Append("WRITTEN ROWS - " + (filelevelDetails["PROCESSED_ROW"] - filelevelDetails["SKIPPED_ROW"]) + Environment.NewLine);
                sbLog.Append(Environment.NewLine);

                if (sbCapacity < sbLog.Length)
                {
                    fo.WriteLogFile(logFile, sbLog);
                }

            }

            sbLog.Append("Flushing the CUSTOMER DATA ARRAY to File" + Environment.NewLine);
            fo.writeCustomerToFile(outputFileNm, outputFile);

            sbLog.Append(Environment.NewLine);
            sbLog.Append("Total processed row = " + globalInfo["GLOBAL_PROCESSED_ROW"] + Environment.NewLine);
            sbLog.Append("Total skipped row = " + globalInfo["GLOBAL_SKIPPED_ROW"] + Environment.NewLine);
            sbLog.Append("Total written row = " + (globalInfo["GLOBAL_PROCESSED_ROW"] - globalInfo["GLOBAL_SKIPPED_ROW"]) + Environment.NewLine);
            sbLog.Append(Environment.NewLine);


            Console.WriteLine();
            Console.WriteLine("Total processed row = " + globalInfo["GLOBAL_PROCESSED_ROW"]);
            Console.WriteLine("Total skipped row = " + globalInfo["GLOBAL_SKIPPED_ROW"]);
            Console.WriteLine("Total written row = " + (globalInfo["GLOBAL_PROCESSED_ROW"] - globalInfo["GLOBAL_SKIPPED_ROW"]));

            fo.WriteLogFile(logFile, sbLog);

        }

        private Dictionary<string, int> processFile(List<dynamic> file, DateOnly? dt)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            result.Add("PROCESSED_ROW", 0);
            result.Add("SKIPPED_ROW", 0);
            Customer cust;

            foreach (ExpandoObject row in file)
            {
                cust = null;
                result["PROCESSED_ROW"] = result["PROCESSED_ROW"] + 1;
                cust = validateAndCreateCustomer(row);
                if (cust == null)
                {
                    result["SKIPPED_ROW"] = result["SKIPPED_ROW"] + 1;
                    sbLog.Append("Skipped row was " + result["PROCESSED_ROW"] + Environment.NewLine);
                }
                else
                {

                    if (dt is not null)
                    {
                        cust.file_date = dt;
                    }


                    outputFile.Add(cust);
                }

                if (outputFile.Count > 1000)
                {
                    sbLog.Append("Flushing the CUSTOMER DATA ARRAY to File" + Environment.NewLine);
                    fo.writeCustomerToFile(outputFileNm, outputFile);
                }

            }



            fo.WriteLogFile(logFile, sbLog);
            //Console.WriteLine("Output Objects = " + outputFile.Count);

            return result;
        }

        private Customer? validateAndCreateCustomer(ExpandoObject row)
        {
            Customer cust = new Customer();
            Dictionary<string, Boolean> validation = new Dictionary<string, Boolean>();

            String newKey;

            /*Dictionary<string, List<string>> possibleColName = new Dictionary<string, List<string>>();*/

            foreach (KeyValuePair<string, object> kvp in row)
            {
                newKey = kvp.Key.ToUpper().Trim().Replace(" ", "_");

                //Console.WriteLine("{0} = {1} = {2}", kvp.Key, kvp.Value, kvp.Value.GetType());

                if (possibleColName["FIRST_NAME"].Contains(newKey))
                {
                    if (validation.ContainsKey("FIRST_NAME"))
                    {
                        sbLog.Append("Seeing duplicated column and hence skipping the row ...." + Environment.NewLine);
                        return null;
                    }
                    else
                    {
                        validation.Add("FIRST_NAME", true);
                        cust.first_name = (string)kvp.Value;
                        if (cust.first_name is null || cust.first_name.Trim().Length == 0)
                        {
                            sbLog.Append("Skipping the row due to NULL or 0 length data ..." + Environment.NewLine);
                            return null;
                        }

                    }
                }

                if (possibleColName["LAST_NAME"].Contains(newKey))
                {
                    if (validation.ContainsKey("LAST_NAME"))
                    {
                        sbLog.Append("Seeing duplicated column and hence skipping the row ...." + Environment.NewLine);
                        return null;
                    }
                    else
                    {
                        validation.Add("LAST_NAME", true);
                        cust.last_name = (string)kvp.Value;
                        if (cust.last_name is null || cust.last_name.Trim().Length == 0)
                        {
                            sbLog.Append("Skipping the row due to NULL or 0 length data ..." + Environment.NewLine);
                            return null;
                        }
                    }
                }

                if (possibleColName["STREET_NO"].Contains(newKey))
                {

                    int street_number;

                    try
                    {
                        street_number = Convert.ToInt16((string)kvp.Value);
                    }
                    catch (Exception)
                    {
                        street_number = -2;
                    }

                    if (validation.ContainsKey("STREET_NO") || street_number < 1)
                    {
                        sbLog.Append("Seeing duplicated column or street number as NULL ...." + Environment.NewLine);
                        return null;
                    }
                    else
                    {
                        validation.Add("STREET_NO", true);
                        cust.street_number = street_number;
                    }
                }

                if (possibleColName["STREET_NAME"].Contains(newKey))
                {
                    if (validation.ContainsKey("STREET_NAME"))
                    {
                        sbLog.Append("Seeing duplicated column and hence skipping the row ...." + Environment.NewLine);
                        return null;
                    }
                    else
                    {
                        validation.Add("STREET_NAME", true);
                        cust.street_name = (string)kvp.Value;
                        if (cust.street_name.Trim().Length == 0)
                        {
                            sbLog.Append("Skipping the row due to NULL or 0 length data ..." + Environment.NewLine);
                            return null;
                        }
                    }
                }

                if (possibleColName["ZIP_CODE"].Contains(newKey))
                {
                    if (validation.ContainsKey("ZIP_CODE"))
                    {
                        sbLog.Append("Seeing duplicated column and hence skipping the row ...." + Environment.NewLine);
                        return null;
                    }
                    else
                    {
                        validation.Add("ZIP_CODE", true);
                        cust.postol_code = (string)kvp.Value;
                        if (cust.postol_code.Trim().Length == 0)
                        {
                            sbLog.Append("Skipping the row due to NULL or 0 length data ..." + Environment.NewLine);
                            return null;
                        }
                    }
                }

                if (possibleColName["CITY"].Contains(newKey))
                {
                    if (validation.ContainsKey("CITY"))
                    {
                        sbLog.Append("Seeing duplicated column and hence skipping the row ...." + Environment.NewLine);
                        return null;
                    }
                    else
                    {
                        validation.Add("CITY", true);
                        cust.city = (string)kvp.Value;
                        if (cust.city.Trim().Length == 0)
                        {
                            sbLog.Append("Skipping the row due to NULL or 0 length data ..." + Environment.NewLine);
                            return null;
                        }
                    }
                }

                if (possibleColName["PROVINCE"].Contains(newKey))
                {
                    if (validation.ContainsKey("PROVINCE"))
                    {
                        sbLog.Append("Seeing duplicated column and hence skipping the row ...." + Environment.NewLine);
                        return null;
                    }
                    else
                    {
                        validation.Add("PROVINCE", true);
                        cust.province = (string)kvp.Value;
                        if (cust.province.Trim().Length == 0)
                        {
                            sbLog.Append("Skipping the row due to NULL or 0 length data ..." + Environment.NewLine);
                            return null;
                        }
                    }
                }

                if (possibleColName["COUNTRY"].Contains(newKey))
                {
                    if (validation.ContainsKey("COUNTRY"))
                    {
                        sbLog.Append("Seeing duplicated column and hence skipping the row ...." + Environment.NewLine);
                        return null;
                    }
                    else
                    {
                        validation.Add("COUNTRY", true);
                        cust.country = (string)kvp.Value;
                        if (cust.country.Trim().Length == 0)
                        {
                            sbLog.Append("Skipping the row due to NULL or 0 length data ..." + Environment.NewLine);
                            return null;
                        }
                    }
                }

                if (possibleColName["PHONE"].Contains(newKey))
                {

                    int street_number;

                    try
                    {
                        street_number = Convert.ToInt32(((string)kvp.Value).Replace("(", "").Replace(")", ""));
                    }
                    catch (Exception)
                    {
                        street_number = -2;
                    }

                    if (validation.ContainsKey("PHONE") || street_number < 1)
                    {
                        sbLog.Append("Seeing duplicated column or street number as NULL ...." + Environment.NewLine);
                        return null;
                    }
                    else
                    {
                        validation.Add("PHONE", true);
                        cust.phone_number = street_number;
                    }
                }

                if (possibleColName["EMAIL"].Contains(newKey))
                {
                    if (validation.ContainsKey("EMAIL") || !Regex.IsMatch((string)kvp.Value, emailRegEx, RegexOptions.IgnoreCase))
                    {
                        sbLog.Append("Seeing duplicated column or EMail Validation failed hence skipping the row ...." + Environment.NewLine);
                        return null;
                    }
                    else
                    {
                        validation.Add("EMAIL", true);
                        cust.email_address = (string)kvp.Value;
                    }
                }

            }

            /*            foreach (string key in validation.Keys)
                            Console.WriteLine(key);
                        Console.WriteLine("Validation count = " + validation.Count);*/

            if (validation.Count != 10)
            {
                sbLog.Append("Did not found 10 columns in this and hence need to skip the row ....");
                return null;
            }
            else
                return cust;

        }
    }

    public class Customer
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public int street_number { get; set; }
        public string street_name { get; set; }
        public string city { get; set; }
        public string postol_code { get; set; }
        public string province { get; set; }
        public string country { get; set; }
        public int phone_number { get; set; }
        public string email_address { get; set; }
        public DateOnly? file_date { get; set; }

        public Customer()
        {
            this.first_name = String.Empty;
            this.last_name = String.Empty;
            this.street_number = int.MinValue;
            this.street_name = String.Empty;
            this.city = String.Empty;
            this.province = String.Empty;
            this.postol_code = String.Empty;
            this.phone_number = int.MinValue;
            this.email_address = String.Empty;
            this.file_date = null;
        }

        public Customer(string first_name, string last_name, int street_number, string street_name, string city, string postol_code, string province, string country, int phone_number, string email_address)
        {
            this.first_name = first_name.Trim();
            this.last_name = last_name.Trim();
            this.street_number = street_number;
            this.street_name = street_name.Trim();
            this.city = city.Trim();
            this.country = country;
            this.postol_code = postol_code;
            this.province = province;
            this.phone_number = phone_number;
            this.email_address = email_address.Trim();
            this.file_date = DateOnly.FromDateTime(DateTime.Now);
        }

        private string printElement(string element)
        {
            if (element is null)
            {
                return "[EMPTY]";
            }
            else
            {
                return element;
            }
        }

        private string printElement(int element)
        {
            if (element == 0 || element < 0)
            {
                return "[EMPTY]";
            }
            else
            {
                return element.ToString();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("First_Name = " + printElement(this.first_name) + Environment.NewLine);
            sb.Append("Last_Name = " + printElement(this.last_name) + Environment.NewLine);
            sb.Append("Street_Number = " + printElement(this.street_number) + Environment.NewLine);
            sb.Append("Street_Name = " + printElement(this.street_name) + Environment.NewLine);
            sb.Append("City = " + printElement(this.city) + Environment.NewLine);
            sb.Append("Postol_Code = " + printElement(this.postol_code) + Environment.NewLine);
            sb.Append("Province = " + printElement(this.province) + Environment.NewLine);
            sb.Append("Country = " + printElement(this.country) + Environment.NewLine);
            sb.Append("Phone_Number = " + printElement(this.phone_number) + Environment.NewLine);
            sb.Append("Email = " + printElement(this.email_address) + Environment.NewLine);
            sb.Append("File_Date = " + this.file_date.ToString() + Environment.NewLine);

            return sb.ToString();
        }

    }

    public class CustomerMap : ClassMap<Customer>
    {
        public CustomerMap()
        {
            Map(m => m.first_name).Name("First Name").Index(0);
            Map(m => m.last_name).Name("Last Name").Index(2);
            Map(m => m.street_number).Name("Street Number").Index(4);
            Map(m => m.street_name).Name("Street Name").Index(6);
            Map(m => m.city).Name("City").Index(8);
            Map(m => m.postol_code).Name("Postal Code").Index(10);
            Map(m => m.province).Name("Province").Index(12);
            Map(m => m.country).Name("Country").Index(14);
            Map(m => m.phone_number).Name("Number").Index(16);
            Map(m => m.email_address).Name("Email Address").Index(18);
            Map(m => m.file_date).Name("File Date").Index(20);
        }
    }
}
