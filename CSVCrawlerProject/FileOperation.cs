using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using System.Formats.Asn1;
using System.Globalization;
using System.Text;

namespace ProgAssign1
{
    internal class FileOperation
    {

        Boolean flg = true;
        public Boolean checkFolder(string path)
        {

            if(path is null || path.Trim().Length == 0)
            {
                return false;
            }
            if(!Directory.Exists(path))
            {
                return false;
            }
            if(File.Exists(path))
            {
                return false;
            }
            try
            {
                String FlNam = Path.Join(path.Trim(), Path.GetRandomFileName());

                var fs = new FileStream(FlNam, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Delete);
                StreamWriter st =  new StreamWriter(fs);

                st.Write("Something");
                st.Close();
                fs.Close();
                File.Delete(FlNam);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Trying to handle file operation");
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public void WriteLogFile(string filePath, StringBuilder sb)
        {
            FileInfo fi = new FileInfo(filePath);

/*            Console.WriteLine(fi.DirectoryName);
            Console.WriteLine(fi.Name + Environment.NewLine);*/

            if (!Directory.Exists(fi.DirectoryName))
                Directory.CreateDirectory(fi.DirectoryName);

            FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
            sw.Close();
            fs.Close();
            sb.Clear();
        }

        public List<dynamic> readCSVFile(string path)
        {
            if (path is null || path.Trim().Length < 1)
                throw new Exception("CSV Path is null .....");
            else if (!File.Exists(path))
                throw new Exception("File doesn't exist..." + path);

            StreamReader reader = new StreamReader(path);
            CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            List< dynamic> res = csv.GetRecords<dynamic>().ToList();

            /*foreach (KeyValuePair<string, object> kvp in res[0]) // enumerating over it exposes the Properties and Values as a KeyValuePair
                Console.WriteLine("{0} = {1}", kvp.Key, kvp.Value);*/

            csv.Dispose();
            reader.Close();

            return res;
        }

        public void writeCustomerToFile(String path, List<Customer> lst)
        {
            FileInfo fi = new FileInfo(path);
            Boolean headerFlg = false;

            if (!Directory.Exists(fi.DirectoryName))
                Directory.CreateDirectory(fi.DirectoryName);

            FileStream fs;
            if (flg)
            {
                flg = false;
                fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                headerFlg = true;
            }
            else if (File.Exists(path))
            {
                fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                headerFlg = false;
            }
            else
            {
                fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                headerFlg = true;
            }

            StreamWriter writer = new StreamWriter(fs);
            CsvWriter csvWriter;

            if (!headerFlg)
            {
                headerFlg = false;
                csvWriter = new CsvWriter(writer,new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false });
            }
            else
            {
                csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
            }
            new CsvWriter(writer, CultureInfo.InvariantCulture);

            csvWriter.Context.RegisterClassMap<CustomerMap>();

            

            csvWriter.WriteRecords(lst);
            csvWriter.Flush();
            writer.Close();
            lst.Clear();
        }

    }
}
