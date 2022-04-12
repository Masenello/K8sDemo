using System.Threading;
using K8sBackendShared.RabbitConnector;
using K8sBackendShared.Workers;
using K8sDemoHubWorker.Services;
using K8sDemoWorker.Jobs;
using K8sDemoWorker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace K8sDemoWorker
{
    public class Startup
    {
        IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<RabbitConnectorServiceDemoWorker>();
            services.AddHostedService<TestCyclicWorkerService>(x =>
                new TestCyclicWorkerService(
                        x.GetRequiredService<RabbitConnectorServiceDemoWorker>(),
                        1000,
                        new TestJob()  
                )
            );
        }
    }
}