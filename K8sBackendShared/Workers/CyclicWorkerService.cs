using System;
using System.ComponentModel;
using System.Threading;
using System.Timers;
using System.Threading.Tasks;
using K8sBackendShared.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace K8sBackendShared.Workers
{
    public abstract class CyclicWorkerService<T> : IHostedService where T : IJob
    {

        protected readonly IRabbitConnector _rabbitConnector;
        private readonly IServiceProvider _serviceProvider;
        protected readonly ILogger _logger;
        //IMPORTANT: this value must be intended as a MAXIMUM frequency for the cycle.
        //The actual time interval between two calls of the main cycle will always be >=
        //this value.
        private int _cycleTime = 500;
        private readonly System.Timers.Timer _mainCycleTimer;

        public CyclicWorkerService(IServiceProvider serviceProvider, IRabbitConnector rabbitConnector, ILogger logger, int cycleTime)
        {
            _rabbitConnector = rabbitConnector;
            _logger = logger;
            _serviceProvider = serviceProvider;

            _cycleTime = cycleTime;

            _mainCycleTimer = new System.Timers.Timer(_cycleTime);
            _mainCycleTimer.Elapsed += mainCycleTimer_Elapsed;
            _mainCycleTimer.Enabled = false;
            _mainCycleTimer.AutoReset = false;

        }

        private void mainCycleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ExecuteMainCycle();
        }

        internal void ExecuteMainCycle()
        {
            try
            {
                //Disable timer for main cycle (main cycle must not be interrupted)
                _mainCycleTimer.Enabled = false;

                //Operations executed withing the main cycle. Must be implemented by inheriting class. 
                using (var scope = _serviceProvider.CreateScope())
                {
                    var transientService = scope.ServiceProvider.GetRequiredService<T>();
                    //Force sync execution to be sure DoWork is completed before timer restart
                    transientService.DoWorkAsync().GetAwaiter().GetResult();
                    //_logger.LogInfo($"Ending cycle operations. Duration {(DateTime.Now - start).TotalMilliseconds} [ms]");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error on cyclic worker main cycle:", ex);
            }
            finally
            {
                //If worker change set cycle frequency set again on timer
                _mainCycleTimer.Interval = _cycleTime;
                //Enable main cycle timer
                _mainCycleTimer.Enabled = true;
            }
        }

        public virtual Task StartAsync(CancellationToken stoppingToken)
        {
            _mainCycleTimer.Enabled = true;
            _logger.LogInfo($"{nameof(CyclicWorkerService<T>)} started");
            return Task.CompletedTask;
        }

        public virtual Task StopAsync(CancellationToken stoppingToken)
        {
            _mainCycleTimer.Enabled = false;
            _mainCycleTimer.Dispose();
            _logger.LogInfo($"{nameof(CyclicWorkerService<T>)} stopped");
            return Task.CompletedTask;
        }
    }
}