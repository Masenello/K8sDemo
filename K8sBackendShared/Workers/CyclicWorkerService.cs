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
    public abstract class CyclicWorkerService:IHostedService
    {
        protected AbstractWorkerJob _workerJob = null;
        protected BackgroundWorker _bw = new BackgroundWorker();

        protected readonly IRabbitConnector _rabbitConnector;

        protected readonly ILogger _logger;

        private int _cycleTime = 500;

        public delegate void MainCycleCompletedHandler(object sender, EventArgs e);
        public event MainCycleCompletedHandler MainCycleCompleted;

        public CyclicWorkerService(IRabbitConnector rabbitConnector, ILogger logger, int cycleTime, AbstractWorkerJob workerJob)
        {
            _rabbitConnector = rabbitConnector;
            _logger=logger;

            _cycleTime = cycleTime;
            _bw.WorkerReportsProgress = false;
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += BackgroundWorkerOnDoWork;
            _bw.RunWorkerCompleted += BackgroundWorkerOnWorkCompleted;

            _workerJob = workerJob;
            _workerJob.JobProgressChanged += new AbstractWorkerJob.JobProgressChangedHandler(JobProgressChanged);

            _bw.RunWorkerAsync();


        }

        private void JobProgressChanged(object sender, JobProgressEventArgs e)
        { 
            
            _rabbitConnector.Publish(e.Status);
            _logger.LogInfo($"{nameof(CyclicWorkerService)}: Job Id:{e.Status.JobId} Status: {e.Status.Status} Progress: {e.Status.ProgressPercentage}% Message: {e.Status.UserMessage}");
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
                //Console.WriteLine("Worker run started");
                //Do Work!
                _workerJob.DoWork(null);
                //Console.WriteLine("Worker run completed");
                if (MainCycleCompleted != null)
                {
                    MainCycleCompleted(this,new EventArgs());
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