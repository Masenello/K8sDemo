
using K8sBackendShared.Interfaces;
using K8sBackendShared.K8s;
using K8sBackendShared.Logging;
using K8sBackendShared.RabbitConnector;
using K8sCore.Interfaces.Mongo;
using K8sDataMongo.Repository;
using K8sDataMongo.Repository.JobRepository;
using K8sDemoDirector.Interfaces;
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

            services.AddTransient(typeof(IGenericMongoRepository<>), typeof(GenericMongoRepository<>));
            services.AddTransient<IJobRepository, JobRepository>();
            //***********************************************************

            services.AddSingleton<ILogger,RabbitLoggerService>();
            services.AddSingleton<IRabbitConnector, RabbitConnectorService>();
            services.AddSingleton<IK8s, KubernetesConnectorService>();
            services.AddSingleton<IWorkersRegistryManager, WorkersRegistryManagerService>();
            services.AddSingleton<IWorkersScaler, WorkersScalerService>();

            services.AddTransient<IDirectorCycleJob, DirectorCycleJob>();

            services.AddHostedService<DirectorService>(x =>
                new DirectorService(
                        services.BuildServiceProvider(),
                        x.GetRequiredService<IRabbitConnector>(),
                        x.GetRequiredService<ILogger>(),
                        1000)
                );
            
        }
    }
}