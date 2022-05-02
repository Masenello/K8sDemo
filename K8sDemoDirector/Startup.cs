using K8sBackendShared.Data;
using K8sBackendShared.Interfaces;
using K8sBackendShared.K8s;
using K8sBackendShared.Logging;
using K8sBackendShared.RabbitConnector;
using K8sBackendShared.Repository;
using K8sBackendShared.Repository.JobRepository;
using K8sBackendShared.Settings;
using K8sDemoDirector.Jobs;
using K8sDemoDirector.Services;
using Microsoft.EntityFrameworkCore;
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
            services.AddDbContext<DataContext>(options =>
                {
                    options.UseSqlServer(NetworkSettings.DatabaseConnectionStringResolver(),
                            sqlServerOptions => sqlServerOptions.CommandTimeout(180));
                });
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IJobRepository, JobRepository>();
            services.AddTransient<IJobUnitOfWork, JobUnitOfWork>();

            services.AddSingleton<ILogger,RabbitLoggerService>();
            services.AddSingleton<IRabbitConnector, RabbitConnectorService>();
            services.AddSingleton<IK8s, KubernetesConnectorService>();

            services.AddHostedService<DirectorService>(x =>
                new DirectorService(
                        services.BuildServiceProvider(),
                        x.GetRequiredService<IK8s>(),
                        x.GetRequiredService<IRabbitConnector>(),
                        x.GetRequiredService<ILogger>(),
                        1000,
                        new GetJobListJob(x.GetRequiredService<ILogger>(), x.GetRequiredService<IRabbitConnector>())  
                )
            );
        }
    }
}