using System;
using EasyNetQ;
using K8sBackendShared.Messages;
using K8sBackendShared.RabbitConnector;
using Microsoft.Extensions.DependencyInjection;

namespace K8sDemoHubManager.Services
{
    public class RabbitConnectorServiceHub:RabbitConnectorService
    {

        public RabbitConnectorServiceHub(IServiceProvider serviceProvider):base(serviceProvider)
        {

        }

    
        public override async void Subscribe()
        {
            await  _rabbitBus.PubSub.SubscribeAsync<JobStatusMessage>("",HandleJobStatusMessage);  
        }

        private async void HandleJobStatusMessage(JobStatusMessage msg)
        {
            Console.WriteLine($"{DateTime.Now}: {nameof(RabbitConnectorService)}: Job Id:{msg.JobId} Status: {msg.Status} Progress: {msg.ProgressPercentage}% ");

            using (var scope = _serviceProvider.CreateScope())
            {
                var transientService = scope.ServiceProvider.GetRequiredService<SignalRbrokerService>();
                await transientService.ForwardJobStatusMessage(msg);
            }
        }
        
    }
}