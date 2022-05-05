using System;
using K8sBackendShared.Jobs;
using K8sBackendShared.Interfaces;
using System.Collections.Concurrent;
using K8sCore.Entities;
using K8sCore.Entities.Mongo;
using K8sCore.Interfaces.Mongo;
using K8sDemoDirector.Interfaces;
using K8sCore.Messages;
using System.Collections.Generic;
using System.Linq;
using K8sCore.Enums;
using System.Threading.Tasks;

namespace K8sDemoDirector.Jobs
{
    public class DirectorCycleJob : AbstractWorkerJob, IDirectorCycleJob
    {
        private readonly IWorkersScaler _workersScaler;
        private readonly IWorkersRegistryManager _registryManager;
        private readonly IJobRepository _jobRepo;

        private List<JobEntity> openJobs = new List<JobEntity>();

        public DirectorCycleJob(IJobRepository jobRepo, ILogger logger, IWorkersScaler workersScaler, IWorkersRegistryManager registryManager, IRabbitConnector rabbitConnector) : base(logger, rabbitConnector)
        {
            _jobRepo = jobRepo;
            _workersScaler = workersScaler;
            _registryManager = registryManager;
        }

        public override async Task DoWorkAsync()
        {
            openJobs = _jobRepo.GetOpenJobs();
            await AssignJobsToWorkersAsync();
            _registryManager.UpdateJobCounts(openJobs);
            _workersScaler.MonitorWorkersScaling(openJobs.Count);
            await MonitorJobsForTimeoutsAsync();
            UpdateDirectorStatus();

        }

        private async Task AssignJobsToWorkersAsync()
        {
            foreach (var createdJob in openJobs.Where(x => x.Status == JobStatus.created))
            {

                //If system is scaling DOWN or max workers is reached, avoid assigning jobs
                if (_workersScaler.SystemIsScalingDown) break;
                //Assign job to worker
                var targetWorker = _workersScaler.GetWorkerWithLessLoad();
                if (targetWorker is null)
                {
                    //No workers available
                    break;
                }
                else
                {
                    await _jobRepo.AssignJobAsync(targetWorker.WorkerId, createdJob.Id);
                    //be sure that changes are saved in database before sending message to worker
                    _rabbitConnector.Publish<DirectorAssignJobToWorker>(new DirectorAssignJobToWorker()
                    {
                        WorkerId = targetWorker.WorkerId,
                        JobId = createdJob.Id,
                        JobType = createdJob.Type,
                    });
                    _logger.LogInfo($"Director assigned job: {createdJob.Id} to worker: {targetWorker.WorkerId}");
                }
            }
        }

        private async Task MonitorJobsForTimeoutsAsync()
        {
            //Monitor active job for timeouts
            foreach (var openJob in openJobs)
            {
                //var jobToMonitor = await uow.Jobs.GetJobWithIdAsync(activeJob.Key);
                //TODO variable timeouts set on job creation 
                //if ((jobToMonitor != null) && (DateTime.UtcNow - jobToMonitor.AssignmentDate).TotalSeconds>jobToMonitor.TimeOutSeconds)
                if ((DateTime.UtcNow - openJob.CreationDate).TotalSeconds > 30)
                {
                    var timeoutMsg = await _jobRepo.SetJobInTimeOutAsync(openJob.Id);
                    _rabbitConnector.Publish<JobStatusMessage>(timeoutMsg);
                }
            }
        }

        private void UpdateDirectorStatus()
        {
            DirectorStatusMessage newStatus = new DirectorStatusMessage()
            {
                Timestamp = DateTime.UtcNow,
                RegisteredWorkers = _registryManager.WorkersRegistry.Values.ToList(),
                TotalJobs = openJobs.Count(),
                MaxWorkers = _workersScaler.MaxWorkers,
                MaxConcurrentTasks = _registryManager.WorkersRegistry.Count * _workersScaler.MaxJobsPerWorker
            };
            _rabbitConnector.Publish<DirectorStatusMessage>(newStatus);
        }


    }
}