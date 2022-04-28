using System;
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
    public abstract class EventWorkerService:IHostedService
    {
        protected  string _workerId ="";
        protected AbstractWorkerJob _workerJob = null;
        protected readonly IRabbitConnector _rabbitConnector;
        protected readonly ILogger _logger;


        public EventWorkerService(IRabbitConnector rabbitConnector, ILogger logger, AbstractWorkerJob workerJob)
        {
            _rabbitConnector = rabbitConnector;
            _logger=logger;

            _workerJob = workerJob;
            _workerJob.JobProgressChanged += new AbstractWorkerJob.JobProgressChangedHandler(JobProgressChanged);

            if (NetworkSettings.RunningInDocker())
            {
                _workerId = Environment.GetEnvironmentVariable("HOSTNAME");
            }
            else
            {
                _workerId = $"WORKER_{UniqueIdentifiers.GenerateDateTimeUniqueId()}";
            }

            Subscribe();
        }

        public abstract void Subscribe();

        private void JobProgressChanged(object sender, JobProgressEventArgs e)
        { 
            e.Status.WorkerId = _workerId;
            _rabbitConnector.Publish(e.Status);
            _logger.LogInfo($"{nameof(CyclicWorkerService)}: Job Id:{e.Status.JobId} Status: {e.Status.Status} Progress: {e.Status.ProgressPercentage}% Message: {e.Status.UserMessage}");
        }

        protected void DoWork(object args)
        {

            Task.Run(() => _workerJob.DoWork(args));

            
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