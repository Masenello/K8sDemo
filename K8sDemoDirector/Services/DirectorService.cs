using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using K8sBackendShared.Interfaces;
using K8sBackendShared.K8s;
using K8sBackendShared.Workers;
using K8sCore.DTOs;
using K8sCore.Entities;
using K8sCore.Entities.Mongo;
using K8sCore.Enums;
using K8sCore.Interfaces.Mongo;
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

        // ******  Settings *********
        private readonly int _maxJobsPerWorker = 20;
        private readonly int _maxWorkers = 10;
        private bool scalingUpEnabled = true;
        private bool scalingDownEnabled = true;
        //***************************

        private bool systemIsScalingUp = false;
        private bool systemIsScalingDown = false;
        private readonly IK8s _k8sConnector;

        private List<JobEntity> openJobs = new List<JobEntity>();

        public DirectorService(IServiceProvider serviceProvider, IK8s k8sConnector, IRabbitConnector rabbitConnector, ILogger logger, int cycleTime, GetJobListJob workerJob)
        : base(rabbitConnector, logger, cycleTime, workerJob)
        {
            _serviceProvider = serviceProvider;
            _getJobListJob = workerJob;
            _k8sConnector = k8sConnector;

            base.MainCycleCompleted += CyclicWorkerMainCycleCompleted;

            _workersRegistry = new ConcurrentDictionary<int, WorkerDescriptorDto>();

            _rabbitConnector.Subscribe<WorkerRegisterToDirectorMessage>(HandleWorkerRegisterMessage);
            _rabbitConnector.Subscribe<WorkerUnRegisterToDirectorMessage>(HandleWorkerUnregisterMessage);
            _rabbitConnector.Publish<DirectorStartedMessage>(new DirectorStartedMessage());

        }

        private async void CyclicWorkerMainCycleCompleted(object sender, EventArgs e)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //Create Scope to call transient service in singleton
                var jobRepo = scope.ServiceProvider.GetRequiredService<IJobRepository>();
                openJobs = jobRepo.GetOpenJobs();
                foreach (var createdJob in openJobs.Where(x=>x.Status== JobStatus.created))
                {
                    
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
                        await jobRepo.AssignJobAsync(targetWorker.WorkerId, createdJob.Id);
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

                //Monitor active job for timeouts
                foreach (var openJob in openJobs)
                {
                    //var jobToMonitor = await uow.Jobs.GetJobWithIdAsync(activeJob.Key);
                    //TODO variable timeouts set on job creation 

                    //if ((jobToMonitor != null) && (DateTime.UtcNow - jobToMonitor.AssignmentDate).TotalSeconds>jobToMonitor.TimeOutSeconds)
                    if ((DateTime.UtcNow - openJob.CreationDate).TotalSeconds > 30)
                    {
                        var timeoutMsg = await jobRepo.SetJobInTimeOutAsync(openJob.Id);
                        _rabbitConnector.Publish<JobStatusMessage>(timeoutMsg);
                    }
                }
            }

            //Update Director Status
            
            //Update job counts for workers
            foreach (var worker in _workersRegistry.Values)
            {
                worker.CurrentJobs = openJobs.Where(x => x.WorkerId == worker.WorkerId).Count();
                //_logger.LogInfo($"Worker with id: {worker.WorkerId} has: {worker.CurrentJobs} active jobs");
            }
            DirectorStatusMessage newStatus = new DirectorStatusMessage()
            {
                Timestamp = DateTime.UtcNow,
                RegisteredWorkers = _workersRegistry.Values.ToList(),
                TotalJobs = openJobs.Count(),
            };
            _rabbitConnector.Publish<DirectorStatusMessage>(newStatus);

            //Monitor worker scaling
            if (!(systemIsScalingUp || systemIsScalingDown))
            {
                MonitorWorkerLoad();
            }
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
            if (_workersRegistry.Count == 0) return null;
            var targetWorker= _workersRegistry.OrderBy(x=>x.Value.CurrentJobs).First();
            //A worker that is saturated is considered not available
            if (targetWorker.Value.CurrentJobs >= _maxJobsPerWorker) return null;
            return targetWorker.Value;
        }

        #endregion

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
            //scalingTarget = _workersRegistry.Count() + 1;
            scalingTarget = (openJobs.Count() +  _maxJobsPerWorker -1) / _maxJobsPerWorker;
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