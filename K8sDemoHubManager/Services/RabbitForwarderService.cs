using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Messages;
using K8sBackendShared.RabbitConnector;
using Microsoft.Extensions.DependencyInjection;

namespace K8sDemoHubManager.Services
{
    public class RabbitForwarderService
    {

        private readonly IServiceProvider _serviceProvider;   
        private readonly ILogger _logger; 
        private readonly IRabbitConnector _rabbitConnector;
        public RabbitForwarderService(IServiceProvider serviceProvider, ILogger logger, IRabbitConnector rabbitConnector):base()
        {
            _serviceProvider= serviceProvider;
            _logger = logger;
            _rabbitConnector = rabbitConnector;
            _rabbitConnector.Subscribe<JobStatusMessage>(HandleJobStatusMessage);
        }


        private async void HandleJobStatusMessage(JobStatusMessage msg)
        {
            _logger.LogInfo($"{nameof(RabbitForwarderService)}: Job Id:{msg.JobId} Status: {msg.Status} Progress: {msg.ProgressPercentage}% ");
            using (var scope = _serviceProvider.CreateScope())
            {
                var transientService = scope.ServiceProvider.GetRequiredService<SignalRbrokerService>();
                await transientService.ForwardJobStatusMessage(msg);
            }
        }

                
    }
}