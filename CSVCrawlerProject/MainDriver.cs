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
            worker.run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args);
        }
    }
}