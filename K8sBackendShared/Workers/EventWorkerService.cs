using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Jobs;
using K8sBackendShared.RabbitConnector;
using K8sBackendShared.Settings;
using K8sBackendShared.Utils;
using Microsoft.Extensions.Hosting;

namespace K8sBackendShared.Workers
{
    public abstract class EventWorkerService : IHostedService
    {
        protected string _workerId = "";
        protected readonly IRabbitConnector _rabbitConnector;
        protected readonly ILogger _logger;

        public EventWorkerService(IRabbitConnector rabbitConnector, ILogger logger)
        {

            _rabbitConnector = rabbitConnector;
            _logger = logger;

            if (NetworkSettings.RunningInDocker())
            {
                _workerId = Environment.GetEnvironmentVariable("HOSTNAME");
            }
            else
            {
                _workerId = $"WORKER_{UniqueIdentifiers.GenerateDateTimeUniqueId()}";
            }
        }

        public virtual Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfo($"{nameof(EventWorkerService)} started");
            return Task.CompletedTask;
        }

        public virtual Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfo($"{nameof(EventWorkerService)} stopped");
            return Task.CompletedTask;
        }
    }
}