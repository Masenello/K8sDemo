

using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Logging;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Logging;
using K8sBackendShared.Messages;
using K8sBackendShared.Settings;
using K8sBackendShared.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace K8sBackendShared.RabbitConnector
{
    public class RabbitConnectorService:IRabbitConnector,IDisposable
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
            await  _rabbitBus.PubSub.SubscribeAsync<T>(UniqueIdentifiers.GenerateDateTimeUniqueId(),subscribeAction);  
        }

        public async void Publish<T>(T message)
        {
            await _rabbitBus.PubSub.PublishAsync<T>(message);
        }



        public Tresp Request<Treq, Tresp>(Treq message)
        {
            return _rabbitBus.Rpc.Request<Treq, Tresp>(message);
        }

        public void Respond<Treq, Tresp>(Func<Treq, Tresp> respondToRequestFunction)
        {
            _rabbitBus.Rpc.Respond<Treq, Tresp>(respondToRequestFunction);
        }

                public void Dispose()
        {
            _rabbitBus.Dispose(); 
        }

    }
}