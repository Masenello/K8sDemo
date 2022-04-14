using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Jobs;
using K8sBackendShared.RabbitConnector;
using Microsoft.Extensions.Hosting;

namespace K8sBackendShared.Workers
{
    public abstract class EventWorkerService:IHostedService
    {
        protected AbstractWorkerJob _workerJob = null;
        protected BackgroundWorker _bw = new BackgroundWorker();

        protected readonly IRabbitConnector _rabbitConnector;

        protected readonly ILogger _logger;


        public EventWorkerService(IRabbitConnector rabbitConnector, ILogger logger, AbstractWorkerJob workerJob)
        {
            _rabbitConnector = rabbitConnector;
            _logger=logger;

            _bw.WorkerReportsProgress = false;
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += BackgroundWorkerOnDoWork;
            _bw.RunWorkerCompleted += BackgroundWorkerOnWorkCompleted;

            _workerJob = workerJob;
            _workerJob.JobProgressChanged += new AbstractWorkerJob.JobProgressChangedHandler(JobProgressChanged);

            Subscribe();
        }

        public abstract void Subscribe();

        private void JobProgressChanged(object sender, JobProgressEventArgs e)
        { 
            
            _rabbitConnector.Publish(e.Status);
            _logger.LogInfo($"{nameof(CyclicWorkerService)}: Job Id:{e.Status.JobId} Status: {e.Status.Status} Progress: {e.Status.ProgressPercentage}% Message: {e.Status.UserMessage}");
        }

        private void BackgroundWorkerOnWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            // while (!_bw.CancellationPending)
            // {
                _workerJob.DoWork(e.Argument);
            // } 
        
        }


        //TODO complete and test!
        public void CancelWork()
        {
            _bw.CancelAsync();
            _logger.LogWarning("Job Aborted");
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