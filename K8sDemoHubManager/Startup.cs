using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using K8sDemoHubManager.Hubs;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using K8sDemoHubManager.Interfaces;
using K8sDemoHubManager.Services;
using K8sBackendShared.Utils;
using K8sBackendShared.Logging;
using K8sBackendShared.Interfaces;
using K8sBackendShared.RabbitConnector;
using K8sData.Data;
using K8sData.Settings;

namespace K8sDemoHubManager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<DataContext>(options =>
                {
                    options.UseSqlServer(NetworkSettings.DatabaseConnectionStringResolver(),
                            sqlServerOptions => sqlServerOptions.CommandTimeout(180));
                });
            services.AddScoped<IConnectedAppsService, ConnectedAppsService>();
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "K8sDemoHubManager", Version = "v1" });
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options=>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
                //Add query string authorization
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddSignalR();
            services.AddTransient<SignalRbrokerService>();

            services.AddSingleton<ILogger,RabbitLoggerService>();
            services.AddSingleton<IRabbitConnector, RabbitConnectorService>();
            services.AddSingleton<RabbitForwarderService>();
            services.AddHostedService<BackgroundServiceStarter<RabbitForwarderService>>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            app.UseCors(builder =>
            {
                builder.WithOrigins("localhost")
                    .AllowAnyHeader()
                    .WithMethods("GET", "POST")
                    .SetIsOriginAllowed((x) => true)
                    .AllowCredentials();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "K8sDemoHubManager v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ClientHub>("hubs/client");
            });

            //Register event of application shutdown
            //applicationLifetime.ApplicationStopped.Register(()=>OnShutdown(app.ApplicationServices));
            applicationLifetime.ApplicationStopping.Register(()=>OnStopping(app.ApplicationServices));
        }

        private void OnStopping(IServiceProvider serviceProvider)
        {
            //NON VA!
            // Console.WriteLine($"{nameof(RabbitConnectorService)} cleaning connections table");
            // using (var scope = serviceProvider.CreateScope())
            // {
            //     var transientService = scope.ServiceProvider.GetRequiredService<DataBaseSpecialOperationsService>();
            //     transientService.CleanConnectionsTable();
            // }
            // Console.WriteLine($"Cleaned my shit");
        
        }
    }
}
