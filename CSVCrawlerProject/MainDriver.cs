using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ProgAssign1
{
    public class MainDriver
    {
        public static string CrawlPath = Directory.GetCurrentDirectory();

        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var worker = ActivatorUtilities.CreateInstance<Worker>(host.Services);
            try
            {
                worker.run();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Something has failed and hence Exiting the Program");
                Console.WriteLine("Printing the StackTrace for Informatuin");
                Console.WriteLine(ex.GetBaseException());
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Source);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args);
        }
    }
}