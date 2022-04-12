using System.Threading;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Logging;
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
            services.AddSingleton<ILogger,RabbitLoggerService>();
            services.AddSingleton<IRabbitPublisher, RabbitConnectorServiceDemoWorker>();
            services.AddHostedService<TestCyclicWorkerService>(x =>
                new TestCyclicWorkerService(
                        x.GetRequiredService<IRabbitPublisher>(),
                        x.GetRequiredService<ILogger>(),
                        1000,
                        new TestJob(x.GetRequiredService<ILogger>())  
                )
            );
        }
    }
}