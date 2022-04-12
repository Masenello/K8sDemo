

using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Logging;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Logging;
using K8sBackendShared.Messages;
using K8sBackendShared.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace K8sBackendShared.RabbitConnector
{
    public class RabbitConnectorService:IRabbitConnector
    {

        protected readonly IBus _rabbitBus;

        public RabbitConnectorService()
        {    
            try 
            {
                
                //LogProvider.SetCurrentLogProvider(ConsoleLogProvider.Instance);
                NetworkSettings.WaitForRabbitDependancy();
                _rabbitBus = RabbitHutch.CreateBus(NetworkSettings.RabbitHostResolver());

            }     
            catch (Exception e)
            {
                Console.WriteLine($"Failed to start {nameof(RabbitConnectorService)}".AddException(e));
                throw;
            }

        }

        public async void Subscribe<T>(Action<T> subscribeAction)
        {
            await  _rabbitBus.PubSub.SubscribeAsync<T>("",subscribeAction);  
        }

        public async void Publish<T>(T message)
        {
            await _rabbitBus.PubSub.PublishAsync<T>(message);
        }
        
    }
}