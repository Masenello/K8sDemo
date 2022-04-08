using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using K8sBackendShared.Data;
using K8sBackendShared.Entities;
using K8sBackendShared.Messages;
using K8sBackendShared.Settings;
using K8sDemoHubManager.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace K8sDemoHubManager.Services
{
    public class RabbitConnectorService:IHostedService
    {

    
        private readonly IBus _rabbitBus;
        private readonly IServiceProvider _serviceProvider;
        public RabbitConnectorService(IServiceProvider serviceProvider)
        {          

            _serviceProvider = serviceProvider;
            _rabbitBus = RabbitHutch.CreateBus(NetworkSettings.RabbitHostResolver());
            _rabbitBus.PubSub.SubscribeAsync<JobStatusMessage>("",HandleJobStatusMessage);             
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

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"{nameof(RabbitConnectorService)} started");
            // _logger.LogInformation("Timed Hosted Service running.");

            // _timer = new Timer(DoWork, null, TimeSpan.Zero, 
            //     TimeSpan.FromSeconds(5));
            await Task.Delay(0);
            return;
        }

        private void DoWork(object? state)
        {
            // var count = Interlocked.Increment(ref executionCount);

            // _logger.LogInformation(
            //     "Timed Hosted Service is working. Count: {Count}", count);
        }

        public async Task StopAsync(CancellationToken stoppingToken)
        {
            // Console.WriteLine($"{nameof(RabbitConnectorService)} cleaning connections table");

            // using (var _context = (new DataContextFactory()).CreateDbContext(null))
            // {
            //     foreach(var connection in _context.ConnectedApps)
            //         {
            //             Console.WriteLine($"Sono nel for each");
            //             _context.ConnectedApps.Remove(connection);
            //         }
            //         await _context.SaveChangesAsync();
            // }

    
            // Console.WriteLine($"{nameof(RabbitConnectorService)} stopped");
            await Task.Delay(0);
            return;
        }


        
    }
}