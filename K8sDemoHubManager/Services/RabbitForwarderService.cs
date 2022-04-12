using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Messages;
using K8sBackendShared.RabbitConnector;
using K8sDemoHubManager.Interfaces;
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
            _rabbitConnector.Subscribe<LogMessage>(HandleLogMessage);
            
        }

        private async void HandleLogMessage(LogMessage msg)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var transientService = scope.ServiceProvider.GetRequiredService<SignalRbrokerService>();
                await transientService.ForwardMessageToGroup<LogMessage>(msg, "logviewers");
            }
        }

        private async void HandleJobStatusMessage(JobStatusMessage msg)
        {
            _logger.LogInfo($"{nameof(RabbitForwarderService)}: Job Id:{msg.JobId} Status: {msg.Status} Progress: {msg.ProgressPercentage}% ");
            using (var scope = _serviceProvider.CreateScope())
            {
                var transientService = scope.ServiceProvider.GetRequiredService<SignalRbrokerService>();
                await transientService.ForwardMessage<JobStatusMessage>(msg, msg.User);
            }
        }

                
    }
}