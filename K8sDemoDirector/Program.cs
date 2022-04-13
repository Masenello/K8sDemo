using System.Threading.Tasks;
using K8sBackendShared.Extensions;
using Microsoft.Extensions.Hosting;

namespace K8sDemoDirector
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
