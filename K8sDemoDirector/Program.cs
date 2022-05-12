using System;
using System.Threading.Tasks;
using K8sBackendShared.Extensions;
using K8sBackendShared.Utils;
using Microsoft.Extensions.Hosting;
[assembly: System.Reflection.AssemblyVersion("0.1.*")]

namespace K8sDemoDirector
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
