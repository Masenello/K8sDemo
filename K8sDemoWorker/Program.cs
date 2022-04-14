using System;
using K8sBackendShared.Messages;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using K8sBackendShared.Settings;
using System.Threading;
using K8sBackendShared.Workers;
using K8sBackendShared.Data;
using K8sBackendShared.Entities;
using System.Linq;
using K8sBackendShared.Enums;
using Microsoft.EntityFrameworkCore;
using K8sDemoWorker.Jobs;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using K8sBackendShared.Extensions;

namespace K8sDemoWorker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseStartup<Startup>(); 
    }
}
