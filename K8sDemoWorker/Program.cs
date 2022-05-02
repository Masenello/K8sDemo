using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using K8sBackendShared.Extensions;

namespace K8sDemoWorker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseStartup<Startup>(); 
    }
}
