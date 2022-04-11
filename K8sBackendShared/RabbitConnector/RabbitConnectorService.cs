

using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Logging;
using K8sBackendShared.Messages;
using K8sBackendShared.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace K8sBackendShared.RabbitConnector
{
    public abstract class RabbitConnectorService:IHostedService
    {

    
        protected readonly IBus _rabbitBus;

        public RabbitConnectorService()
        {    
            try 
            {
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

        public void Publish<T>(T message)
        {
            _rabbitBus.PubSub.Publish<T>(message);
        }

        public abstract Task StartAsync(CancellationToken stoppingToken);
   

        public Task StopAsync(CancellationToken stoppingToken)
        {

            this._rabbitBus.Dispose();
            return Task.CompletedTask;
        }


        
    }
}