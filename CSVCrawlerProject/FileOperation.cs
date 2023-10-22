using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace ProgAssign1
{
    internal class FileOperation
    {
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
        }

    }
}
