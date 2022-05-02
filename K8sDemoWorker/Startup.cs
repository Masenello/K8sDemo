using K8sBackendShared.Interfaces;
using K8sBackendShared.Logging;
using K8sBackendShared.RabbitConnector;
using K8sBackendShared.Repository.JobRepository;
using k8sCore.Interfaces;
using k8sCore.Interfaces.JobRepository;
using K8sData;
using K8sData.Data;
using K8sData.Settings;
using K8sDemoWorker.Services;
using Microsoft.EntityFrameworkCore;
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
            //Director can access database through K8sData
            services.AddDbContext<DataContext>(options =>
                {
                    options.UseSqlServer(NetworkSettings.DatabaseConnectionStringResolver(),
                            sqlServerOptions => sqlServerOptions.CommandTimeout(180));
                });
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IJobUnitOfWork, JobUnitOfWork>();
            services.AddTransient<IJobRepository, JobRepository>();
            //***********************************************************    


            services.AddSingleton<ILogger,RabbitLoggerService>();
            services.AddSingleton<IRabbitConnector, RabbitConnectorService>();
            services.AddHostedService<WorkerService>(x =>
                new WorkerService(
                        services.BuildServiceProvider(),
                        x.GetRequiredService<IRabbitConnector>(),
                        x.GetRequiredService<ILogger>()
                        )
                
            );
        }
    }
}