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

namespace K8sDemoWorker.Services
{
    public class WorkerService:EventWorkerService
    {
        private readonly JobType _targetJobType ;

        public WorkerService(IRabbitConnector rabbitConnector, ILogger logger, AbstractWorkerJob workerJob, JobType targetJobType):base(rabbitConnector, logger,workerJob)
        {
            _targetJobType = targetJobType;
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
            //Discard jobs for other workers
            if (msg.WorkerId != _workerId) return;

            _logger.LogInfo($"Worker: {_workerId} received Job with Id: {msg.JobId} from director");
            //Start thread for job
            DoWork(msg.JobId);
        }


    }
}