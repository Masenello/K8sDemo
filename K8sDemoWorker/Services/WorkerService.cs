using System;
using System.Threading;
using System.Threading.Tasks;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Workers;
using K8sCore.Messages;
using K8sDemoWorker.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace K8sDemoWorker.Services
{
    public class WorkerService : EventWorkerService
    {

        private readonly TestJob _testJob;
        public WorkerService(TestJob testJob, IServiceProvider serviceProvider, IRabbitConnector rabbitConnector, ILogger logger) : base(rabbitConnector, logger)
        {
            _testJob = testJob;
            _logger.LogInfo($"Worker started with id: {_workerId}");
            WorkerRegisterToDirector();
        }

        public override void Subscribe()
        {
            _rabbitConnector.Subscribe<DirectorAssignJobToWorker>(HandleDirectorAssignJobToWorker);
            _rabbitConnector.Subscribe<DirectorStartedMessage>(HandleDirectorStartedMessage);
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
                //TODO Manage multiple job types

                _testJob.InitService(_workerId,msg.JobId);
                DoWork(_testJob);

            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Failed to start job with id: {msg.JobId} main task", ex);
            }
        }


    }
}