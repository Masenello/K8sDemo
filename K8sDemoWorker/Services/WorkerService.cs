using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using K8sBackendShared.Data;
using K8sBackendShared.Enums;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Jobs;
using K8sBackendShared.Messages;
using K8sBackendShared.Settings;
using K8sBackendShared.Utils;
using K8sBackendShared.Workers;
using K8sDemoWorker.Jobs;

namespace K8sDemoWorker.Services
{
    public class WorkerService:EventWorkerService
    {

        private object lockvariable;

        public WorkerService(IRabbitConnector rabbitConnector, ILogger logger):base(rabbitConnector, logger)
        {
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
            _rabbitConnector.Publish<WorkerRegisterToDirectorMessage>(new WorkerRegisterToDirectorMessage(){WorkerId = _workerId});
        }

        public override Task  StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfo($"Worker: {_workerId} unregistering from director");
            _rabbitConnector.Publish<WorkerUnRegisterToDirectorMessage>(new WorkerUnRegisterToDirectorMessage(){WorkerId = _workerId});
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
                    DoWork(new TestJob(_logger,_rabbitConnector, _workerId), msg.JobId);
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Failed to start job with id: {msg.JobId} main task",ex);
            }
        }


    }
}