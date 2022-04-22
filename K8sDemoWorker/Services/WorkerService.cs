using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using K8sBackendShared.Data;
using K8sBackendShared.Enums;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Jobs;
using K8sBackendShared.Messages;
using K8sBackendShared.Workers;

namespace K8sDemoWorker.Services
{
    public class WorkerService:EventWorkerService
    {

        private string _workerId = "UNDEFINED";
        private readonly JobType _targetJobType ;

        public WorkerService(IRabbitConnector rabbitConnector, ILogger logger, AbstractWorkerJob workerJob, JobType targetJobType):base(rabbitConnector, logger,workerJob)
        {
            _targetJobType = targetJobType;
        }

        public override void Subscribe()
        {
            _rabbitConnector.Subscribe<JobsAvailableMessage>(HandleJobsAvailableMessage);
        }


        public override Task  StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfo($"Worker: {_workerId} unregistering from director");
            _rabbitConnector.Publish<WorkerUnRegisterToDirectorMessage>(new WorkerUnRegisterToDirectorMessage(){WorkerId = _workerId});
            _logger.LogInfo($"{nameof(WorkerService)} stopped");
            return Task.CompletedTask;
        }

        private void HandleJobsAvailableMessage(JobsAvailableMessage msg)
        {
            try
            {
                 //Check if jobs are compatible with worker
                if(msg.JobsList.Count(x=>x.JobType == _targetJobType)==0) return;

                //Check if worker is busy
                if (!_bw.IsBusy)
                {
                    _logger.LogInfo($"Worker: {_workerId} requesting job to director");
                    var response = _rabbitConnector.Request<JobRequestAssignMessage, JobRequestAssignResultMessage>(new JobRequestAssignMessage() { 
                        WorkerId = _workerId,
                        JobType = _targetJobType });

                    if (response.JobId>0)
                    {
                        
                        _workerId = response.WorkerId;
                        _logger.LogInfo($"Worker: {_workerId} received Job with Id: {response.JobId} from director");

                        using (var _context = (new DataContextFactory()).CreateDbContext(null))
                        {
                            var targetJob = _context.Jobs.FirstOrDefault(x=>x.Id== response.JobId);
                            if (targetJob is null) throw new Exception($"Job with Id: {response.JobId} not found on database");
                            //Start the worker with Job id
                            _bw.RunWorkerAsync(response.JobId);
                        }

                    }
                }
            }
            catch (System.Exception e)
            {
                _logger.LogError($"Failed to request job assign", e);
            }
        }
    }
}