using K8sBackendShared.Enums;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Logging;
using K8sBackendShared.RabbitConnector;
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
            services.AddSingleton<IRabbitConnector, RabbitConnectorService>();
            services.AddHostedService<WorkerService>(x =>
                new WorkerService(
                        x.GetRequiredService<IRabbitConnector>(),
                        x.GetRequiredService<ILogger>()
                        )
                
            );
        }
    }
}