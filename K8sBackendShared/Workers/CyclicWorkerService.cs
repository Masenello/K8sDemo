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
    public abstract class CyclicWorkerService<T> : IHostedService where T : IJob
    {
        protected BackgroundWorker _bw = new BackgroundWorker();
        protected readonly IRabbitConnector _rabbitConnector;
        private readonly IServiceProvider _serviceProvider;
        protected readonly ILogger _logger;
        private int _cycleTime = 500;

        public CyclicWorkerService(IServiceProvider serviceProvider, IRabbitConnector rabbitConnector, ILogger logger, int cycleTime)
        {
            _rabbitConnector = rabbitConnector;
            _logger = logger;
            _serviceProvider = serviceProvider;

            _cycleTime = cycleTime;
            _bw.WorkerReportsProgress = false;
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += BackgroundWorkerOnDoWork;
            _bw.RunWorkerCompleted += BackgroundWorkerOnWorkCompleted;

            _bw.RunWorkerAsync();
        }

        private void BackgroundWorkerOnWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Run again after previous run is complete
            _bw.RunWorkerAsync();
        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (!_bw.CancellationPending)
                {
                    // _logger.LogInfo($"Starting cycle");
                    // DateTime startCycle = DateTime.Now;
                    Thread.Sleep(_cycleTime);
                    //To make pauses
                    //Create Scope to call transient service in singleton hosted service
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        // _logger.LogInfo($"Starting cycle operations");
                        // DateTime start = DateTime.Now;
                        //Do Work!
                        var transientService = scope.ServiceProvider.GetRequiredService<T>();
                        transientService.DoWorkAsync();
                        //_logger.LogInfo($"Ending cycle operations. Duration {(DateTime.Now - start).TotalMilliseconds} [ms]");
                    }
                    //_logger.LogInfo($"Ending cycle. Duration {(DateTime.Now - startCycle).TotalMilliseconds} [ms]");

                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error on cyclic worker main cycle:", ex);
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
            _logger.LogInfo($"{nameof(CyclicWorkerService<T>)} started");
            return Task.CompletedTask;
        }

        public virtual Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfo($"{nameof(CyclicWorkerService<T>)} stopped");
            return Task.CompletedTask;
        }
    }
}