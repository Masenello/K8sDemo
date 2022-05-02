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

        protected ThreadedQueue<object> _jobQueue = new ThreadedQueue<object>();

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

            Subscribe();
        }

        public abstract void Subscribe();

        private void JobProgressChanged(object sender, JobProgressEventArgs e)
        {
            e.Status.WorkerId = _workerId;
            _rabbitConnector.Publish(e.Status);
            _logger.LogInfo($"{nameof(CyclicWorkerService)}: Job Id:{e.Status.JobId} Status: {e.Status.Status} Progress: {e.Status.ProgressPercentage}% Message: {e.Status.UserMessage}");
        }

        protected async void DoWork(AbstractWorkerJob workerJob)
        {
            //_jobQueue.Enqueue(args);
            workerJob.JobProgressChanged += new AbstractWorkerJob.JobProgressChangedHandler(JobProgressChanged);
            await Task.Run(() => workerJob.DoWork()).ContinueWith(t =>
            {
                //Throw task exceptions (if any)
                if (t.IsFaulted)
                {
                    throw t.Exception;
                }
                //Log cancelation (if occurred)
                if (t.IsCanceled)
                {
                    _logger.LogError($"Task has been canceled");
                }
            });
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