
using K8sBackendShared.Interfaces;
using K8sBackendShared.K8s;
using K8sBackendShared.Logging;
using K8sBackendShared.RabbitConnector;
using K8sBackendShared.Utils;
using K8sCore.Interfaces.Ef;
using K8sCore.Interfaces.Mongo;
using K8sData.Data;
using K8sData.Repository;
using K8sData.Settings;
using K8sDataMongo.Repository;
using K8sDataMongo.Repository.JobRepository;
using K8sDemoApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K8sDemoApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment CurrentEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddScoped<ITokenService, TokenService>();
            //EF Repositories
            services.AddDbContext<DataContext>(options =>
                {
                    options.UseSqlServer(NetworkSettings.DatabaseConnectionStringResolver(),
                            sqlServerOptions => sqlServerOptions.CommandTimeout(180));
                });
            services.AddTransient<IUserRepository, UserRepository>();
            //Mongo repositories
            services.AddTransient(typeof(IGenericMongoRepository<>), typeof(GenericMongoRepository<>));
            services.AddTransient<IJobRepository, JobRepository>();
            services.AddSingleton<IK8s, KubernetesConnectorService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "K8sDemoApi", Version = AppProperties.Instance.ApplicationVersion });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @$"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.{Environment.NewLine}
                      Example: 'Bearer oiod29so83cosad2uhai82jfattodjl'estated2scorsa'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            services.AddSingleton<ILogger, RabbitLoggerService>();
            services.AddSingleton<IRabbitConnector, RabbitConnectorService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseCors(builder =>
                    {
                        builder
                            .WithOrigins(new string[]
                                {
                                    "localhost",
                                    "20.23.193.177",
                                }
                            )
                            .SetIsOriginAllowed((x) => true)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "K8sDemoApi v1"));
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
