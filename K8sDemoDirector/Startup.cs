using K8sBackendShared.Interfaces;
using K8sBackendShared.Logging;
using K8sBackendShared.RabbitConnector;
using K8sDemoDirector.Jobs;
using K8sDemoDirector.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace K8sDemoDirector
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
            services.AddSingleton<IRabbitConnector, RabbitConnectorService>();
            services.AddHostedService<CyclicWorkerManagerService>(x =>
                new CyclicWorkerManagerService(
                        x.GetRequiredService<IRabbitConnector>(),
                        x.GetRequiredService<ILogger>(),
                        1000,
                        new WorkerManagerJob(x.GetRequiredService<ILogger>())  
                )
            );
        }
    }
}