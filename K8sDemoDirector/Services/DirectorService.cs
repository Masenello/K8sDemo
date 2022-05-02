using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using k8s.Util.Common;
using K8sBackendShared.Data;
using K8sBackendShared.DTOs;
using K8sBackendShared.Entities;
using K8sBackendShared.Enums;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Jobs;
using K8sBackendShared.K8s;
using K8sBackendShared.Messages;
using K8sBackendShared.Repository;
using K8sBackendShared.Repository.JobRepository;
using K8sBackendShared.Workers;
using K8sDemoDirector.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace K8sDemoDirector.Services
{
    public class DirectorService:CyclicWorkerService
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly GetJobListJob _getJobListJob;     

        private readonly ConcurrentDictionary<int,WorkerDescriptorDto> _workersRegistry;

        private readonly ConcurrentDictionary<int,JobStatusMessage> _activeJobsRegistry;

        private readonly int _maxJobsPerWorker = 20;
        private readonly int _maxWorkers = 3;

        private bool systemIsScalingUp=false;
        private bool systemIsScalingDown=false;
        private bool scalingUpEnabled = true;

        private bool scalingDownEnabled = false;


        private readonly IK8s _k8sConnector;
        
        public DirectorService(IServiceProvider  serviceProvider, IK8s k8sConnector,IRabbitConnector rabbitConnector, ILogger logger, int cycleTime, GetJobListJob workerJob)
        :base(rabbitConnector,logger,cycleTime, workerJob)
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
            switch(msg.Status)
            {
                case(JobStatus.running):
                    UpdateJobStatusInRegistry(msg);
                break;
                case(JobStatus.completed):
                case(JobStatus.error):
                    RemoveFromRegistry(msg);
                break;
                default:
                    throw new Exception($"Unknown job status {msg.Status}");

            }

            //Update job counts for workers
            foreach(var worker in _workersRegistry.Values)
            {
                worker.CurrentJobs = _activeJobsRegistry.Where(x=>x.Value.WorkerId == worker.WorkerId ).Count();
                //_logger.LogInfo($"Worker with id: {worker.WorkerId} has: {worker.CurrentJobs} active jobs");
            }
        }

        
        private async void  CyclicWorkerMainCycleCompleted(object sender, EventArgs e)
        {
            //If system is scaling suspend job assignment
                 //_logger.LogInfo($"Director main cycle");
                //TODO Change to event based worker if possible

            
            using(var scope = _serviceProvider.CreateScope()) 
            {
                var uow = scope.ServiceProvider.GetRequiredService<IJobUnitOfWork>();
                foreach(var createdJob in  uow.Jobs.GetJobsInStatus(JobStatus.created))   
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
                        createdJob.Status = JobStatus.assigned;
                        createdJob.AssignmentDate = DateTime.UtcNow;
                        uow.Complete();
                        //be sure that changes are saved in database before sending message to worker
                        _rabbitConnector.Publish<DirectorAssignJobToWorker>(new DirectorAssignJobToWorker()
                            {
                                WorkerId = targetWorker.WorkerId,
                                JobId = createdJob.Id
                            });
                        AddJobToRegistry(createdJob, targetWorker.WorkerId);
                        _logger.LogInfo($"Director assigned job: {createdJob.Id} to worker: {targetWorker.WorkerId}. Worker jobs: {_activeJobsRegistry.Where(x=>x.Value.WorkerId == targetWorker.WorkerId).Count()}");
                    }
                        
                } 

                //Monitor active job for timeouts
                foreach(var activeJob in _activeJobsRegistry.Where(x=>x.Value.Status== JobStatus.assigned
                        ||x.Value.Status== JobStatus.running))
                {
                    var jobToMonitor = uow.Jobs.GetJobWithId(activeJob.Key);
                            
                    //TODO variable timeouts. Be carefull that cluster time zone is UTC!
                    if ((jobToMonitor != null) && (DateTime.UtcNow - jobToMonitor.AssignmentDate).TotalSeconds>30)
                    {
                        //Job has timed out
                        //update job on database
                        jobToMonitor.EndDate = DateTime.UtcNow;
                        jobToMonitor.Status= JobStatus.error;
                        jobToMonitor.Errors = $"Director job monitoring, timeout on worker {activeJob.Value.WorkerId}";
                        uow.Complete();
                        //notify clients
                        JobStatusMessage msg = new JobStatusMessage()
                        {
                            JobId = jobToMonitor.Id,
                            StatusJobType=jobToMonitor.Type,
                            Status = JobStatus.error,
                            ProgressPercentage = 100,
                            User = jobToMonitor.User.UserName,
                            UserMessage = $"{jobToMonitor.GenerateJobDescriptor()}. Director job monitoring, timeout on worker {activeJob.Value.WorkerId}",
                            WorkerId = activeJob.Value.WorkerId,
                        };
                        _rabbitConnector.Publish<JobStatusMessage>(msg);
                        //remove from active jobs list
                        RemoveFromRegistry(msg);
                    }
                }
            }

            //Update Director Status
            DirectorStatusMessage newStatus = new DirectorStatusMessage()
            {
                Timestamp = DateTime.UtcNow,
                RegisteredWorkers = _workersRegistry.Values.ToList(),
            };
            _rabbitConnector.Publish<DirectorStatusMessage>(newStatus);

            //Monitor worker scaling
            if (!(systemIsScalingUp || systemIsScalingDown))
            {
                MonitorWorkerLoad();
            }
            

            
            cycleCounter+=1;
        }

    #region Worker registry management
        private void HandleWorkerRegisterMessage(WorkerRegisterToDirectorMessage msg)
        { 
            try
            {
                var target = _workersRegistry.FirstOrDefault(x=>x.Value.WorkerId == msg.WorkerId);
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
                
                _logger.LogError($"Failed to add worker: {msg.WorkerId} to worker registry",e);
            }

        }

        private void HandleWorkerUnregisterMessage(WorkerUnRegisterToDirectorMessage msg)
        {
            try
            {
                var target = _workersRegistry.FirstOrDefault(x=>x.Value.WorkerId == msg.WorkerId);
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
                _logger.LogError($"Failed to remove worker: {msg.WorkerId} from worker registry",e);
            }

        }

        private int GenerateKeyForWorkerRegistry()
        {
            if (_workersRegistry.Count()==0)
            {
                return 1;
            }
            else
            {
                return(_workersRegistry.Keys.Max()+1);
            }
        }

        private bool WorkerIsRegistered(string workerId)
        {
            if (_workersRegistry.Count(x=>x.Value.WorkerId == workerId)>0)
            {
                return true;
            } 
            return false;
        }

        private WorkerDescriptorDto GetWorkerWithLessLoad()
        {
            //Important note: jobs enter _activeJobsRegistry when ASSIGNED to worker. Jobs are counted on _workersRegistry when in RUNNING state.
            //Check must be performed on _activeJobsRegistry to vaoid assigning to many jobs to a worker
            if (_workersRegistry.Count==0) return null;
            string targetWorkerId = _workersRegistry.First().Value.WorkerId;
            foreach (var worker in _workersRegistry)
            {   
                if(GetJobsAssignedToWorker(worker.Value.WorkerId)<GetJobsAssignedToWorker(targetWorkerId))
                {
                    targetWorkerId = worker.Value.WorkerId;
                }
            }
            var targetWorker = _workersRegistry.FirstOrDefault(x=>x.Value.WorkerId == targetWorkerId);
            //A worker that is saturated is considered not available
            if (GetJobsAssignedToWorker(targetWorkerId) >= _maxJobsPerWorker) return null;

            return targetWorker.Value;
        }
    
    #endregion

    #region Active Jobs Registry management

        private bool IsJobInActiveJobRegistry(int jobId)
            {
                if (_activeJobsRegistry.Where(x=>x.Key == jobId).Count()>0)
                {
                    return true;
                }
                return false;
            }

        private void AddJobToRegistry(JobEntity targetJob, string workerId)
        {
            if (!IsJobInActiveJobRegistry(targetJob.Id))
                {
                        _activeJobsRegistry.TryAdd(targetJob.Id, new JobStatusMessage(){
                            WorkerId = workerId,
                            JobId = targetJob.Id,
                            StatusJobType = targetJob.Type,
                            User = targetJob.User.UserName,
                            Status = JobStatus.assigned,
                            ProgressPercentage = 0
                        });
                        //_logger.LogInfo($"Job with id: {msg.JobId} from worker: {msg.WorkerId} inserted in active job registry");
                }
        }

        private void UpdateJobStatusInRegistry(JobStatusMessage newStatus)
        {
            if (IsJobInActiveJobRegistry(newStatus.JobId))
                {
                    _activeJobsRegistry.TryUpdate(newStatus.JobId, newStatus,_activeJobsRegistry[newStatus.JobId]);
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
              return _activeJobsRegistry.Where(x=>x.Value.WorkerId == workerId).Count();
          }
        
    #endregion

        int cycleCounter = 0;
  

        private void MonitorWorkerLoad()
        {
            //TODO variable threshold
            if ((_workersRegistry.Count()>0) 
            && (scalingUpEnabled)
            && ((_workersRegistry.Count() < _maxWorkers)) 
            && (_workersRegistry.All(x=>x.Value.CurrentJobs >= _maxJobsPerWorker)))  //All existing workers are saturated
            {
                WorkersScaleUp();
            }

            if (_workersRegistry.All(x=>x.Value.CurrentJobs==0)
            && scalingDownEnabled)
            {
                WorkersScaleDown();
            }
        }

        int scalingTarget = 0;
    
        private void WorkersScaleUp()
        {
            systemIsScalingUp = true;
            scalingTarget = _workersRegistry.Count()+1;
            _k8sConnector.ScaleDeployment(K8sNamespace.defaultNamespace, Deployment.worker, scalingTarget);
            _logger.LogInfo($"Scaling up workers to {scalingTarget}");
        }

        private void WorkersScaleDown()
        {
            

            //Always leave at least one worker 
            if (_workersRegistry.Count()>1)
            {
                {
                    systemIsScalingDown = true;
                    scalingTarget = _workersRegistry.Count()-1;
                    _k8sConnector.ScaleDeployment(K8sNamespace.defaultNamespace, Deployment.worker, scalingTarget);
                    _logger.LogInfo($"Scaling down workers to {scalingTarget}");
                }
            }      
        }
    }
}