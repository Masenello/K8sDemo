using K8sBackendShared.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace K8sDemoApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration,IWebHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment CurrentEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (CurrentEnvironment.IsDevelopment())
            {
                //Connection string for debug SERVER = host.docker.internal
                services.AddDbContext<DataContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("SqlServerDebugConnection"),
                        sqlServerOptions => sqlServerOptions.CommandTimeout(180));
                });
            }
            else
            {
                //Connection string for production SERVER = k8sDemo-database (name defined in docker-compose.yml)
                services.AddDbContext<DataContext>(options =>
                        {
                            options.UseSqlServer(Configuration.GetConnectionString("SqlServerDockerConnection"),
                                sqlServerOptions => sqlServerOptions.CommandTimeout(180));
                        });
            }



            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDevClient",
                    builder =>
                    {
                        builder
                            .WithOrigins("http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                options.AddPolicy("AllowAngularClient",
                    builder =>
                    {
                        builder
                            .WithOrigins("http://localhost")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "K8sDemoApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("AllowAngularDevClient");
            app.UseCors("AllowAngularClient");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "K8sDemoApi v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
