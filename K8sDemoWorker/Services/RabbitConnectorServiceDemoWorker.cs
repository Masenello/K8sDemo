using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using K8sBackendShared.Messages;
using K8sBackendShared.RabbitConnector;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace K8sDemoHubWorker.Services
{
    public class RabbitConnectorServiceDemoWorker:RabbitConnectorService
    {

        public RabbitConnectorServiceDemoWorker():base()
        {

        }



        public override async void Subscribe()
        {
            //await  _rabbitBus.PubSub.SubscribeAsync<JobStatusMessage>("",HandleJobStatusMessage);  
            Console.WriteLine("Sono nel subscription");
            await Task.Delay(0);
        }

        // private async void HandleJobStatusMessage(JobStatusMessage msg)
        // {
        //     Console.WriteLine($"{DateTime.Now}: {nameof(RabbitConnectorService)}: Job Id:{msg.JobId} Status: {msg.Status} Progress: {msg.ProgressPercentage}% ");

        //     using (var scope = _serviceProvider.CreateScope())
        //     {
        //         var transientService = scope.ServiceProvider.GetRequiredService<SignalRbrokerService>();
        //         await transientService.ForwardJobStatusMessage(msg);
        //     }
        // }

        public override Task StartAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"Service {nameof(RabbitConnectorServiceDemoWorker)} started");
            return Task.CompletedTask;
        }

                
    }
}