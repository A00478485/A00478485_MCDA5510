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
        public CSVParser(string outputFileNm, string logFile, StringBuilder sb, List<string> Lst, FileOperation fo)
        {
            this.logFile = logFile;
            this.fo = fo;
            this.sbLog = sb;
            this.listOfFiles = Lst;
            this.outputFileNm = outputFileNm;
            localStartTime = DateTime.Now;
        }

        public void run()
        {

            localStartTime = DateTime.Now;

            Console.WriteLine("Starting processing at "+localStartTime.ToString());
            sbLog.Append("Starting processing at " + localStartTime.ToString() + "...." + Environment.NewLine);

            foreach (string singleFile in  listOfFiles)
            {
                List<dynamic> t = fo.readCSVFile(singleFile);

                Console.WriteLine(t.ToString());
            }

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
