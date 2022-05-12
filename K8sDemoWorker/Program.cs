using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using K8sBackendShared.Extensions;
using System;
using K8sBackendShared.Utils;

[assembly: System.Reflection.AssemblyVersion("0.1.*")]

namespace K8sDemoWorker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine($"Started: {AppProperties.Instance.ApplicationName} with version: {AppProperties.Instance.ApplicationVersion}");
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
    }
}
