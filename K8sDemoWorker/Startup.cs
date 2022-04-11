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
            services.AddHostedService<RabbitConnectorServiceDemoWorker>();
            // services.AddHostedService<TestCyclicWorkerService>(x =>
            //     new TestCyclicWorkerService(
            //             1000,
            //             new TestJob(),
            //             new RabbitConnectorServiceDemoWorker(x)
            //     )
                
            // );
            services.AddHostedService<culoService>();

            //services.AddHostedService<TestCyclicWorkerService>();
            

        }
    }
}