using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Jobs;
using K8sBackendShared.RabbitConnector;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace K8sBackendShared.Workers
{
    public abstract class CyclicWorkerService : IHostedService
    {
        protected IJob _job = null;
        protected BackgroundWorker _bw = new BackgroundWorker();
        protected readonly IRabbitConnector _rabbitConnector;
        private readonly IServiceProvider _serviceProvider;
        protected readonly ILogger _logger;
        private int _cycleTime = 500;

        public CyclicWorkerService(IServiceProvider serviceProvider, IRabbitConnector rabbitConnector, ILogger logger, int cycleTime, IJob job)
        {
            _rabbitConnector = rabbitConnector;
            _logger = logger;
            _serviceProvider = serviceProvider;

            _cycleTime = cycleTime;
            _bw.WorkerReportsProgress = false;
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += BackgroundWorkerOnDoWork;
            _bw.RunWorkerCompleted += BackgroundWorkerOnWorkCompleted;

            _job = job;

            _bw.RunWorkerAsync();
        }

        private void BackgroundWorkerOnWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Run again after previous run is complete
            _bw.RunWorkerAsync();
        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            while (!_bw.CancellationPending)
            {
                Thread.Sleep(_cycleTime);   // If you need to make a pause between runs
                //Create Scope to call transient service in singleton hosted service
                using (var scope = _serviceProvider.CreateScope())
                {
                    //Do Work!
                    _job.DoWorkAsync();
                }
            }

        }


        //TODO complete and test!
        public void CancelWork()
        {
            _bw.CancelAsync();
            _logger.LogWarning("Job Aborted");
        }

        public virtual Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfo($"{nameof(CyclicWorkerService)} started");
            return Task.CompletedTask;
        }

        public virtual Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfo($"{nameof(CyclicWorkerService)} stopped");
            return Task.CompletedTask;
        }
    }
}