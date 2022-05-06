using System;
using System.Threading;
using System.Threading.Tasks;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Workers;
using K8sCore.Messages;
using K8sDemoWorker.Interfaces;
using K8sDemoWorker.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace K8sDemoWorker.Services
{

    public class WorkerService : EventWorkerService
    {

        private readonly IServiceProvider _serviceProvider;
        public WorkerService(IServiceProvider serviceProvider, IRabbitConnector rabbitConnector, ILogger logger) : base(rabbitConnector, logger)
        {
            _serviceProvider = serviceProvider;

            _rabbitConnector.Subscribe<DirectorAssignJobToWorker>(HandleDirectorAssignJobToWorker);
            _rabbitConnector.Subscribe<DirectorStartedMessage>(HandleDirectorStartedMessage);

            _logger.LogInfo($"Worker started with id: {_workerId}");
            WorkerRegisterToDirector();
        }

        private void HandleDirectorStartedMessage(DirectorStartedMessage obj)
        {
            _logger.LogInfo($"Received director alive worker");
            WorkerRegisterToDirector();
        }

        public void WorkerRegisterToDirector()
        {
            _logger.LogInfo($"Worker: {_workerId} registering to director");
            _rabbitConnector.Publish<WorkerRegisterToDirectorMessage>(new WorkerRegisterToDirectorMessage() { WorkerId = _workerId });
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfo($"Worker: {_workerId} unregistering from director");
            _rabbitConnector.Publish<WorkerUnRegisterToDirectorMessage>(new WorkerUnRegisterToDirectorMessage() { WorkerId = _workerId });
            _logger.LogInfo($"{nameof(WorkerService)} stopped");
            return Task.CompletedTask;
        }

        private void HandleDirectorAssignJobToWorker(DirectorAssignJobToWorker msg)
        {
            try
            {
                //Discard jobs for other workers
                if (msg.WorkerId != _workerId) return;
                _logger.LogInfo($"Worker: {_workerId} received Job with Id: {msg.JobId} from director");
                using (var scope = _serviceProvider.CreateScope())
                {
                    switch (msg.JobType)
                    {
                        case K8sCore.Enums.JobType.TestJob:
                            var transientService = scope.ServiceProvider.GetRequiredService<ITestJob>();
                            transientService.WorkerId = _workerId;
                            transientService.DatabaseJobId = msg.JobId;
                            transientService.DoWorkAsync();
                            break;
                        default:
                            throw new Exception($"Unknown job type: {msg.JobType}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Failed to start job with id: {msg.JobId} main task", ex);
            }
        }


    }
}