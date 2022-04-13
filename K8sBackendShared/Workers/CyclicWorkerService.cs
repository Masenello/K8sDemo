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
        private AbstractWorkerJob _workerJob = null;
        private BackgroundWorker _bw = new BackgroundWorker();

        private readonly IRabbitConnector _rabbitConnector;

        private readonly ILogger _logger;

        private int _cycleTime = 500;

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
            _logger.LogInfo($"{nameof(CyclicWorkerService)}: Job Id:{e.Status.JobId} Status: {e.Status.Status} Progress: {e.Status.ProgressPercentage}% Message{e.Status.UserMessage}");
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
                _workerJob.DoWork();
                //Console.WriteLine("Worker run completed");
            } 
        
        }


        //TODO complete and test!
        public void CancelWork()
        {
            _bw.CancelAsync();
            _logger.LogWarning("Job Aborted");
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfo($"{nameof(CyclicWorkerService)} started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}