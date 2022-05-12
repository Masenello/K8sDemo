using System;
using K8sBackendShared.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
[assembly: System.Reflection.AssemblyVersion("0.1.*")]

namespace K8sDemoHubManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"Started: {AppProperties.Instance.ApplicationName} with version: {AppProperties.Instance.ApplicationVersion}");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
