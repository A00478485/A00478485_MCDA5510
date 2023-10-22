using System;
using System.Text;

namespace ProgAssign1
{
    internal class Worker
    {

        private string rootPath;
        private string? inputPath;
        private string? outputPath;
        private string? logPath;
        private readonly DateTime startTime = DateTime.Now;
        private readonly string logFileName;
        private int maxDepth = 5;
        private FileOperation fo = new FileOperation();
        StringBuilder sbLog = new StringBuilder();
        List<String>? fileLst;
        public Worker()
        {
            logFileName = "Log_" + startTime.ToString("yyyyMMdd_HHmmss") + ".txt";
            try
            {
                rootPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName.ToString();
            }
            catch (Exception)
            {
                rootPath = Directory.GetCurrentDirectory().ToString();
            }

        }

        public void run()
        {
            //Console.WriteLine(logFileName);
            String phase = "Parameter";
            try
            {
                sbLog.Append("Starting initial parameter decision phase" + Environment.NewLine);
                decideParams();
                sbLog.Append("Starting Exploration phase" + Environment.NewLine);
                phase = "Exploration";
                decideFiles();

                DateTime dtCurr = DateTime.Now;
                Console.WriteLine("Time took from begining to decising CSV scope = " + dtCurr.Subtract(startTime) + Environment.NewLine);
                sbLog.Append("Time took from begining to decising CSV scope = " + (dtCurr.Subtract(startTime)) + Environment.NewLine);

                phase = "Processing";
                sbLog.Append("Starting data processing phase" + Environment.NewLine);
                Console.WriteLine("Starting data processing phase" + Environment.NewLine);
                processData("CUSTOMER_DATA");
            }
            catch (Exception ex)
            {
                Console.WriteLine(Environment.NewLine + "Error occured during " + phase);
                sbLog.Append("Error occured during " + phase + Environment.NewLine);

                fo.WriteLogFile(Path.Join(logPath, logFileName), sbLog);

                Console.WriteLine(ex.StackTrace);

                throw;
            }

            if (sbLog.Length > 0)
            {
                fo.WriteLogFile(Path.Join(logPath, logFileName), sbLog);
                sbLog.Clear();
            }
        }

        private void processData(string configKey)
        {
            if (configKey is null || configKey.Trim().Length < 1 || fileLst is null || fileLst.Count < 1)
            {
                Console.WriteLine("configKey or List of files are NULL");
                sbLog.Append("configKey or List of files are NULL" + Environment.NewLine);
                throw new Exception();
            }
            return;
        }

        private void decideFiles()
        {
            fileLst = GetFilesAndFolders(inputPath, "*.csv", maxDepth);
            //fileLst.AddRange(GetFilesAndFolders(inputPath, "*.CSV", maxDepth));

            if (fileLst is null || fileLst.Count == 0)
            {
                sbLog.Append("No CSV file found !!!" + Environment.NewLine);
                fo.WriteLogFile(Path.Join(logPath, logFileName), sbLog);
                sbLog.Clear();
                Console.WriteLine("No CSV File found and hence exiting the program !!");
                throw new Exception();
            }

            Console.WriteLine("Number of CSV file(s) found = " + fileLst.Count);
            sbLog.Append("Number of CSV file(s) found = " + fileLst.Count + Environment.NewLine);
            for (int i = 0; i < fileLst.Count; i++)
                sbLog.Append((i + 1) + " -> " + fileLst[i] + Environment.NewLine);
        }

        private void decideDepth()
        {
            String inp;
            int num;
            int origDep = maxDepth;

            do
            {
                Console.Clear();
                Console.Write(Environment.NewLine + "Deafult recursion depth is " +
                maxDepth + ". Hit enter to keep it or enter number to change it" +
                Environment.NewLine + ">");

                inp = Console.ReadLine().ToString().Trim();

                if (inp.Length > 0)
                {
                    num = Convert.ToInt16(inp);
                    if (num < 0 || num > 10)
                    {
                        num = -1;
                    }
                    else
                    {
                        maxDepth = num;
                    }
                }
                else
                {
                    num = origDep;
                }
            } while (num < 0);
        }

        private void decideParams()
        {
            rootPath = Path.GetFullPath(choosePath("root path", rootPath, true));

            inputPath = Path.Join(rootPath, "SampleInput");
            outputPath = Path.Join(rootPath, "FinalOutput");
            logPath = Path.Join(rootPath, "CustomLog");

            inputPath = choosePath("input path", inputPath, true);
            outputPath = choosePath("output path", outputPath, false);
            logPath = choosePath("log path", logPath, false);

            decideDepth();



            Console.Clear();
            sbLog.Append("Starting execution at " + startTime.ToString() + Environment.NewLine);
            sbLog.Append("Root Path = " + rootPath + Environment.NewLine);
            sbLog.Append("Input Path = " + inputPath + Environment.NewLine);
            sbLog.Append("Output Path = " + outputPath + Environment.NewLine);
            sbLog.Append("Log Path = " + logPath + Environment.NewLine);
            sbLog.Append("Log File = " + logFileName + Environment.NewLine);
            sbLog.Append("Max Depth = " + maxDepth + Environment.NewLine);

            fo.WriteLogFile(Path.Join(logPath, logFileName), sbLog);

            Console.WriteLine(sbLog.ToString());

            sbLog.Clear();
        }

        private string choosePath(string shortMsg, string pathLocal, Boolean force)
        {
            String newPath;
            String inp;
            do
            {
                Console.Clear();
                Console.WriteLine(Environment.NewLine + "Choose " + shortMsg.Trim() + " - Enter full path (Hit Enter to select Default path) : ");
                Console.WriteLine("Default = '" + pathLocal + "'");
                Console.Write(">");
                inp = Console.ReadLine().ToString();
                if (inp.Trim().Length > 0)
                {
                    if (inp == ".")
                        newPath = pathLocal;
                    else
                        newPath = inp;
                }
                else
                {
                    newPath = pathLocal;
                }

            } while (force && !fo.checkFolder(newPath));

            Console.WriteLine("Selected path = '" + newPath + "'");

            return newPath;

        }

        List<string> GetFilesAndFolders(string path, string pattern, int depth)
        {
            var list = new List<string>();
            foreach (var directory in Directory.EnumerateDirectories(path, pattern))
            {
                list.Add(directory);
                if (depth > 0)
                {
                    list.AddRange(GetFilesAndFolders(directory, pattern, depth - 1));
                }
            }

            list.AddRange(Directory.EnumerateFiles(path, pattern));

            return list;
        }

    }
}