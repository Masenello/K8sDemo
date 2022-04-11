

using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Logging;
using K8sBackendShared.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace K8sBackendShared.RabbitConnector
{
    public abstract class RabbitConnectorService:IHostedService
    {

    
        protected readonly IBus _rabbitBus;
        protected readonly IServiceProvider _serviceProvider;
        public RabbitConnectorService(IServiceProvider serviceProvider)
        {    
            try 
            {
                _serviceProvider = serviceProvider;
                LogProvider.SetCurrentLogProvider(ConsoleLogProvider.Instance);
                NetworkSettings.WaitForRabbitDependancy();
                _rabbitBus = RabbitHutch.CreateBus(NetworkSettings.RabbitHostResolver());
                Subscribe();

            }     
            catch (Exception e)
            {
                Console.WriteLine($"Failed to start {nameof(RabbitConnectorService)} {e.Message}");
                throw;
            }

        }

        public abstract void Subscribe();

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"{nameof(RabbitConnectorService)} started");
            await Task.Delay(0);
            return;
        }

        public async Task StopAsync(CancellationToken stoppingToken)
        {

            this._rabbitBus.Dispose();
            await Task.Delay(0);
            Console.WriteLine($"{nameof(RabbitConnectorService)} stopped");
            return;
        }


        
    }
}