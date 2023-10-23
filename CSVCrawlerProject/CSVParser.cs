using Microsoft.Extensions.Primitives;
using System.Text;

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
        private int sbCapacity ;
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
                sbLog.Append("STARTED processing of FILE NO - " + ind + Environment.NewLine);

                filelevelDetails = processFile(t);

                end = DateTime.Now;

                globalInfo["GLOBAL_PROCESSED_ROW"] = globalInfo["GLOBAL_PROCESSED_ROW"] + filelevelDetails["PROCESSED_ROW"];
                globalInfo["GLOBAL_SKIPPED_ROW"] = globalInfo["GLOBAL_SKIPPED_ROW"] + filelevelDetails["SKIPPED_ROW"];

                sbLog.Append("FINISHED processing of FILE NO - " + ind + " in " + (end - start) + Environment.NewLine);
                sbLog.Append("PROCESSED ROWS - " + filelevelDetails["PROCESSED_ROW"] + Environment.NewLine);
                sbLog.Append("SKIPPED ROWS - " + filelevelDetails["SKIPPED_ROW"] + Environment.NewLine);
                sbLog.Append("WRITTEN ROWS - " + (filelevelDetails["PROCESSED_ROW"] - filelevelDetails["SKIPPED_ROW"]) + Environment.NewLine);
                sbLog.Append(Environment.NewLine);

                if(sbCapacity < sbLog.Length)
                {
                    fo.WriteLogFile(logFile, sbLog);
                }

            }
            fo.WriteLogFile(logFile, sbLog);

        }

        private Dictionary<string, int> processFile(List<dynamic> t)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            result.Add("PROCESSED_ROW", 0);
            result.Add("SKIPPED_ROW", 0);
            return result;
        }
    }

    internal class Customer
    {
        string first_name;
        string last_name;
        int street_number = -1;
        string street_name;
        string city;
        string postol_code;
        string country;
        int phone_number = -1;
        string email_address;
        DateOnly file_date;

        public Customer(string first_name, string last_name, int street_number, string street_name, string city, string postol_code, string country, int phone_number, string email_address)
        {
            this.first_name = first_name.Trim();
            this.last_name = last_name.Trim();
            this.street_number = street_number;
            this.street_name = street_name.Trim();
            this.city = city.Trim();
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
            sb.Append("Country = " + printElement(this.country) + Environment.NewLine);
            sb.Append("Phone_Number = " + printElement(this.phone_number) + Environment.NewLine);
            sb.Append("Email = " + printElement(this.email_address) + Environment.NewLine);
            sb.Append("File_Date = " + this.file_date.ToString() + Environment.NewLine);

            return sb.ToString();
        }

    }
}
