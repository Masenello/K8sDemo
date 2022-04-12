using K8sBackendShared.Interfaces;
using K8sBackendShared.Logging;
using K8sBackendShared.RabbitConnector;
using K8sBackendShared.Utils;
using K8sDemoLogManager.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace K8sDemoLogManager
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
            services.AddSingleton<RabbitLogReceiverService>();
            services.AddHostedService<BackgroundServiceStarter<RabbitLogReceiverService>>();
        }
    }
    
}