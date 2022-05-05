using K8sBackendShared.Interfaces;
using K8sBackendShared.Logging;
using K8sBackendShared.RabbitConnector;
using K8sCore.Interfaces.Mongo;
using K8sDataMongo.Repository;
using K8sDataMongo.Repository.JobRepository;
using K8sDemoWorker.Interfaces;
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

            //***********   Database access layer *********************
            //Director can access database through K8sData, register as transient to grant multi threading
            // services.AddDbContext<DataContext>(options =>
            //     {
            //         options.UseSqlServer(NetworkSettings.DatabaseConnectionStringResolver(),
            //                 sqlServerOptions => sqlServerOptions.CommandTimeout(180));

            //     });

            services.AddTransient(typeof(IGenericMongoRepository<>), typeof(GenericMongoRepository<>));
            services.AddTransient<IJobRepository, JobRepository>();
            //***********************************************************    

            services.AddSingleton<ILogger, RabbitLoggerService>();
            services.AddSingleton<IRabbitConnector, RabbitConnectorService>();

            services.AddTransient<ITestJob, TestJob>();

            services.AddHostedService<WorkerService>();

        }
    }
}