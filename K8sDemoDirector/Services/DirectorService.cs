using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using K8sBackendShared.Interfaces;
using K8sBackendShared.K8s;
using K8sBackendShared.Workers;
using K8sCore.DTOs;
using K8sCore.Entities;
using K8sCore.Enums;
using K8sCore.Interfaces.JobRepository;
using K8sCore.Messages;
using K8sDemoDirector.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace K8sDemoDirector.Services
{
    public class DirectorService : CyclicWorkerService
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly GetJobListJob _getJobListJob;

        private readonly ConcurrentDictionary<int, WorkerDescriptorDto> _workersRegistry;

        private readonly ConcurrentDictionary<int, JobStatusMessage> _activeJobsRegistry;

        // ******  Settings *********
        private readonly int _maxJobsPerWorker = 20;
        private readonly int _maxWorkers = 3;
        private bool scalingUpEnabled = false;
        private bool scalingDownEnabled = false;
        //***************************

        private bool systemIsScalingUp = false;
        private bool systemIsScalingDown = false;
        private readonly IK8s _k8sConnector;

        public DirectorService(IServiceProvider serviceProvider, IK8s k8sConnector, IRabbitConnector rabbitConnector, ILogger logger, int cycleTime, GetJobListJob workerJob)
        : base(rabbitConnector, logger, cycleTime, workerJob)
        {
            _serviceProvider = serviceProvider;
            _getJobListJob = workerJob;
            _k8sConnector = k8sConnector;

            base.MainCycleCompleted += CyclicWorkerMainCycleCompleted;

            _workersRegistry = new ConcurrentDictionary<int, WorkerDescriptorDto>();
            _activeJobsRegistry = new ConcurrentDictionary<int, JobStatusMessage>();

            _rabbitConnector.Subscribe<WorkerRegisterToDirectorMessage>(HandleWorkerRegisterMessage);
            _rabbitConnector.Subscribe<WorkerUnRegisterToDirectorMessage>(HandleWorkerUnregisterMessage);
            _rabbitConnector.Subscribe<JobStatusMessage>(HandleJobStatusMessage);

            _rabbitConnector.Publish<DirectorStartedMessage>(new DirectorStartedMessage());

        }

        private void HandleJobStatusMessage(JobStatusMessage msg)
        {
            //_logger.LogInfo($"Received job status message");
            switch (msg.Status)
            {
                case (JobStatus.running):
                    UpdateJobStatusInRegistry(msg);
                    break;
                case (JobStatus.completed):
                case (JobStatus.error):
                    RemoveFromRegistry(msg);
                    break;
                default:
                    throw new Exception($"Unknown job status {msg.Status}");

            }

            //Update job counts for workers
            foreach (var worker in _workersRegistry.Values)
            {
                worker.CurrentJobs = _activeJobsRegistry.Where(x => x.Value.WorkerId == worker.WorkerId).Count();
                //_logger.LogInfo($"Worker with id: {worker.WorkerId} has: {worker.CurrentJobs} active jobs");
            }
        }


        private async void CyclicWorkerMainCycleCompleted(object sender, EventArgs e)
        {
            int jobsToAssign = 0;
            using (var scope = _serviceProvider.CreateScope())
            {

                var uow = scope.ServiceProvider.GetRequiredService<IJobUnitOfWork>();
                
                foreach (var createdJob in await uow.Jobs.GetJobsInStatusAsync(JobStatus.created))
                {
                    jobsToAssign +=1;
                    //If system is scaling down or max workers is reached, avoid assigning jobs
                    if (systemIsScalingDown) break;
                    //Assign job to worker
                    var targetWorker = GetWorkerWithLessLoad();
                    if (targetWorker is null)
                    {
                        //No workers available
                        break;
                    }
                    else
                    {
                        await uow.AssignJobAsync(targetWorker.WorkerId, createdJob.Id);
                        //be sure that changes are saved in database before sending message to worker
                        _rabbitConnector.Publish<DirectorAssignJobToWorker>(new DirectorAssignJobToWorker()
                        {
                            WorkerId = targetWorker.WorkerId,
                            JobId = createdJob.Id,
                            JobType = createdJob.Type,
                        });
                        AddJobToRegistry(createdJob, targetWorker.WorkerId);
                        _logger.LogInfo($"Director assigned job: {createdJob.Id} to worker: {targetWorker.WorkerId}. Worker jobs: {_activeJobsRegistry.Where(x => x.Value.WorkerId == targetWorker.WorkerId).Count()}");
                    }

                }

                //Monitor active job for timeouts
                foreach (var activeJob in _activeJobsRegistry.Where(x => x.Value.Status == JobStatus.assigned
                        || x.Value.Status == JobStatus.running))
                {
                    //var jobToMonitor = await uow.Jobs.GetJobWithIdAsync(activeJob.Key);
                    //TODO variable timeouts set on job creation 

                    //if ((jobToMonitor != null) && (DateTime.UtcNow - jobToMonitor.AssignmentDate).TotalSeconds>jobToMonitor.TimeOutSeconds)
                    if ((DateTime.UtcNow - activeJob.Value.CreationDate).TotalSeconds > 30)
                    {
                        var timeoutMsg = await uow.SetJobInTimeOutAsync(activeJob.Value.JobId);
                        _rabbitConnector.Publish<JobStatusMessage>(timeoutMsg);
                        //remove from active jobs list
                        RemoveFromRegistry(timeoutMsg);
                    }
                }
            }

            //Update Director Status
            DirectorStatusMessage newStatus = new DirectorStatusMessage()
            {
                Timestamp = DateTime.UtcNow,
                RegisteredWorkers = _workersRegistry.Values.ToList(),
                TotalJobs = jobsToAssign + _activeJobsRegistry.Count(),
            };
            _rabbitConnector.Publish<DirectorStatusMessage>(newStatus);

            //Monitor worker scaling
            if (!(systemIsScalingUp || systemIsScalingDown))
            {
                MonitorWorkerLoad();
            }



            cycleCounter += 1;
        }

        #region Worker registry management
        private void HandleWorkerRegisterMessage(WorkerRegisterToDirectorMessage msg)
        {
            try
            {
                var target = _workersRegistry.FirstOrDefault(x => x.Value.WorkerId == msg.WorkerId);
                if (!WorkerIsRegistered(msg.WorkerId))
                {
                    _workersRegistry.TryAdd(GenerateKeyForWorkerRegistry(), new WorkerDescriptorDto()
                    {
                        WorkerId = msg.WorkerId
                    });
                    _logger.LogInfo($"Worker {msg.WorkerId} registered, total workers: {_workersRegistry.Count}");
                    systemIsScalingUp = false;
                }
            }
            catch (System.Exception e)
            {

                _logger.LogError($"Failed to add worker: {msg.WorkerId} to worker registry", e);
            }

        }

        private void HandleWorkerUnregisterMessage(WorkerUnRegisterToDirectorMessage msg)
        {
            try
            {
                var target = _workersRegistry.FirstOrDefault(x => x.Value.WorkerId == msg.WorkerId);
                if (WorkerIsRegistered(msg.WorkerId))
                {
                    WorkerDescriptorDto tmp;
                    _workersRegistry.TryRemove(target.Key, out tmp);
                    _logger.LogInfo($"Worker {msg.WorkerId} unregistered, total workers: {_workersRegistry.Count}");
                    systemIsScalingDown = false;
                }

            }
            catch (System.Exception e)
            {
                _logger.LogError($"Failed to remove worker: {msg.WorkerId} from worker registry", e);
            }

        }

        private int GenerateKeyForWorkerRegistry()
        {
            if (_workersRegistry.Count() == 0)
            {
                return 1;
            }
            else
            {
                return (_workersRegistry.Keys.Max() + 1);
            }
        }

        private bool WorkerIsRegistered(string workerId)
        {
            if (_workersRegistry.Count(x => x.Value.WorkerId == workerId) > 0)
            {
                return true;
            }
            return false;
        }

        private WorkerDescriptorDto GetWorkerWithLessLoad()
        {
            //Important note: jobs enter _activeJobsRegistry when ASSIGNED to worker. Jobs are counted on _workersRegistry when in RUNNING state.
            //Check must be performed on _activeJobsRegistry to vaoid assigning to many jobs to a worker
            if (_workersRegistry.Count == 0) return null;
            string targetWorkerId = _workersRegistry.First().Value.WorkerId;
            foreach (var worker in _workersRegistry)
            {
                if (GetJobsAssignedToWorker(worker.Value.WorkerId) < GetJobsAssignedToWorker(targetWorkerId))
                {
                    targetWorkerId = worker.Value.WorkerId;
                }
            }
            var targetWorker = _workersRegistry.FirstOrDefault(x => x.Value.WorkerId == targetWorkerId);
            //A worker that is saturated is considered not available
            if (GetJobsAssignedToWorker(targetWorkerId) >= _maxJobsPerWorker) return null;

            return targetWorker.Value;
        }

        #endregion

        #region Active Jobs Registry management

        private bool IsJobInActiveJobRegistry(int jobId)
        {
            if (_activeJobsRegistry.Where(x => x.Key == jobId).Count() > 0)
            {
                return true;
            }
            return false;
        }

        private void AddJobToRegistry(JobEntity targetJob, string workerId)
        {
            if (!IsJobInActiveJobRegistry(targetJob.Id))
            {
                JobStatusMessage addMsg = new JobStatusMessage(targetJob);
                addMsg.WorkerId = workerId;
                _activeJobsRegistry.TryAdd(targetJob.Id, addMsg);
                //_logger.LogInfo($"Job with id: {msg.JobId} from worker: {msg.WorkerId} inserted in active job registry");
            }
        }

        private void UpdateJobStatusInRegistry(JobStatusMessage newStatus)
        {
            if (IsJobInActiveJobRegistry(newStatus.JobId))
            {
                _activeJobsRegistry.TryUpdate(newStatus.JobId, newStatus, _activeJobsRegistry[newStatus.JobId]);
                //_logger.LogInfo($"Job with id: {newStatus.JobId} from worker: {newStatus.WorkerId} status update to {newStatus.Status} in active job registry");
            }
        }

        private void RemoveFromRegistry(JobStatusMessage newStatus)
        {
            if (IsJobInActiveJobRegistry(newStatus.JobId))
            {
                JobStatusMessage tmp;
                _activeJobsRegistry.Remove(newStatus.JobId, out tmp);
                //_logger.LogInfo($"Job with id: {newStatus.JobId} from worker: {newStatus.WorkerId} removed from active job registry");
                if (newStatus.Status == JobStatus.error)
                {
                    _logger.LogError($"Job with id: {newStatus.JobId} from worker: {newStatus.WorkerId} in error. {newStatus.UserMessage}");
                }
                else
                {
                    _logger.LogInfo($"Job with id: {newStatus.JobId} from worker: {newStatus.WorkerId} completed");
                }

            }
        }

        private int GetJobsAssignedToWorker(string workerId)
        {
            return _activeJobsRegistry.Where(x => x.Value.WorkerId == workerId).Count();
        }

        #endregion

        int cycleCounter = 0;

        private void MonitorWorkerLoad()
        {
            //TODO variable threshold
            if ((_workersRegistry.Count() > 0)
            && (scalingUpEnabled)
            && ((_workersRegistry.Count() < _maxWorkers))
            && (_workersRegistry.All(x => x.Value.CurrentJobs >= _maxJobsPerWorker)))  //All existing workers are saturated
            {
                WorkersScaleUp();
            }

            if (_workersRegistry.All(x => x.Value.CurrentJobs == 0)
            && scalingDownEnabled)
            {
                WorkersScaleDown();
            }
        }

        int scalingTarget = 0;

        private void WorkersScaleUp()
        {
            systemIsScalingUp = true;
            scalingTarget = _workersRegistry.Count() + 1;
            _k8sConnector.ScaleDeployment(K8sNamespace.defaultNamespace, Deployment.worker, scalingTarget);
            _logger.LogInfo($"Scaling up workers to {scalingTarget}");
        }

        private void WorkersScaleDown()
        {


            //Always leave at least one worker 
            if (_workersRegistry.Count() > 1)
            {
                {
                    systemIsScalingDown = true;
                    scalingTarget = _workersRegistry.Count() - 1;
                    _k8sConnector.ScaleDeployment(K8sNamespace.defaultNamespace, Deployment.worker, scalingTarget);
                    _logger.LogInfo($"Scaling down workers to {scalingTarget}");
                }
            }
        }
    }
}