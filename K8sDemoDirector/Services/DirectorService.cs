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
using K8sBackendShared.Workers;
using K8sDemoDirector.Jobs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;

namespace K8sDemoDirector.Services
{
    public class DirectorService:CyclicWorkerService
    {
        private readonly GetJobListJob _getJobListJob;     

        private readonly ConcurrentDictionary<int,WorkerDescriptorDto> _workersRegistry;

        private readonly ConcurrentDictionary<int,JobStatusMessage> _activeJobsRegistry;


        private readonly IK8s _k8sConnector;
        
        public DirectorService(IK8s k8sConnector,IRabbitConnector rabbitConnector, ILogger logger, int cycleTime, GetJobListJob workerJob)
        :base(rabbitConnector,logger,cycleTime, workerJob)
        {
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
                    
                    if (!isJobInActiveJobRegistry(msg.JobId))
                    {
                        _activeJobsRegistry.TryAdd(msg.JobId, msg);
                        //_logger.LogInfo($"Job with id: {msg.JobId} from worker: {msg.WorkerId} inserted in active job registry");
                    }
                break;
                case(JobStatus.completed):
                case(JobStatus.error):
                    if (isJobInActiveJobRegistry(msg.JobId))
                    {
                        JobStatusMessage tmp;
                        _activeJobsRegistry.Remove(msg.JobId, out tmp);
                        //_logger.LogInfo($"Job with id: {msg.JobId} from worker: {msg.WorkerId} removed from active job registry");
                        _logger.LogInfo($"Job with id: {msg.JobId} from worker: {msg.WorkerId} completed");
                    }
                break;
                default:
                    throw new Exception($"Unknown job status {msg.Status}");

            }

            //Update job counts for workers
            foreach(var worker in _workersRegistry.Values)
            {
                worker.CurrentJobs = _activeJobsRegistry.Where(x=>x.Value.WorkerId == worker.WorkerId).Count();
                //_logger.LogInfo($"Worker with id: {worker.WorkerId} has: {worker.CurrentJobs} active jobs");
            }
        }

        
        private async void CyclicWorkerMainCycleCompleted(object sender, EventArgs e)
        {
            //_logger.LogInfo($"Director main cycle");
            //TODO Change to event based

            //Search for jobs
            using (var _context = (new DataContextFactory()).CreateDbContext(null))
            {
                foreach(var createdJob in  _context.Jobs.Where(x=>x.Status == JobStatus.created).Include(u=>u.User).ToList())   
                {
                    //Assign job to worker
                    var targetWorker = GetWorkerWithLessLoad();
                    if (targetWorker is null)
                    {
                        //No workers registered
                        break;
                    }
                    else
                    {
                        _logger.LogInfo($"Director assigning job with id: {createdJob.Id} to worker: {targetWorker.WorkerId}");
                        createdJob.Status = JobStatus.assigned;
                        await _context.SaveChangesAsync();
                        _rabbitConnector.Publish<DirectorAssignJobToWorker>(new DirectorAssignJobToWorker()
                        {
                            WorkerId = targetWorker.WorkerId,
                            JobId = createdJob.Id
                        });
                    }
                    
                } 
            }

            //Update Director Status
            DirectorStatusMessage newStatus = new DirectorStatusMessage()
            {
                Timestamp = DateTime.Now,
                RegisteredWorkers = _workersRegistry.Values.ToList(),
            };
            _rabbitConnector.Publish<DirectorStatusMessage>(newStatus);

            //Monitor worker scaling
            if(cycleCounter % 10 == 0)
            {
                MonitorWorkNumber(newStatus);
            }
            
            cycleCounter+=1;
        }

    #region Worker registration
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
                }
            }
            catch (System.Exception e)
            {
                _logger.LogError($"Failed to remove worker: {msg.WorkerId} from worker registry",e);
            }

        }
    
    #endregion

    #region Aux
        private bool WorkerIsRegistered(string workerId)
        {
            if (_workersRegistry.Count(x=>x.Value.WorkerId == workerId)>0)
            {
                return true;
            } 
            return false;
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

        private WorkerDescriptorDto GetWorkerWithLessLoad()
        {
            if (_workersRegistry.Count()==0)
            {
                return null;
            }
            else
            {
                return _workersRegistry.OrderBy(x=>x.Value.CurrentJobs).First().Value;
            }
        }

        private bool isJobInActiveJobRegistry(int jobId)
        {
            if (_activeJobsRegistry.Where(x=>x.Key == jobId).Count()>0)
            {
                return true;
            }
            return false;
        }
    
    #endregion


        int cycleCounter = 0;



        private void MonitorWorkNumber(DirectorStatusMessage directorStatus)
        {
            // foreach(var jobType in directorStatus.GetWorkersTypes())
            // {
            //     _logger.LogInfo($"Job count: {directorStatus.GetJobsNumber(jobType)}");
            //     //TODO parametrize treshold
            //     if (directorStatus.GetJobsNumber(jobType) > 4)
            //     {
            //         //Spawn 1 worker
            //         WorkersScaleUp(directorStatus,jobType);
            //     }

            //     if (directorStatus.GetJobsNumber(jobType) == 0)
            //     {
            //         //Kill worker
            //         WorkersScaleDown(directorStatus,jobType);
            //     }
            // }
        }

        int scalingTarget = 0;
        
        


        //TODO parametric on worker pod deployment
        private void WorkersScaleUp(DirectorStatusMessage directorStatus, JobType jobType)
        {
            // scalingTarget = directorStatus.GetWorkersNumber(jobType)+1;
            // _k8sConnector.ScaleDeployment(K8sNamespace.defaultNamespace, Deployment.worker, scalingTarget);
            // _logger.LogInfo($"Scaling up workers for job: {jobType} to {scalingTarget}");
        }

        private void WorkersScaleDown(DirectorStatusMessage directorStatus, JobType jobType)
        {
            // //Always leave at least one worker 
            // if (directorStatus.GetWorkersNumber(jobType)>1)
            // {
            //     //No jobs of that type must be running
            //     using (var _context = (new DataContextFactory()).CreateDbContext(null))
            //     {
            //         if (_context.Jobs.Where(x=>x.Type == jobType && x.Status==JobStatus.running).Count() == 0)
            //         {
            //             scalingTarget = directorStatus.GetWorkersNumber(jobType)-1;
            //             _k8sConnector.ScaleDeployment(K8sNamespace.defaultNamespace, Deployment.worker, scalingTarget);
            //             _logger.LogInfo($"Scaling down workers for job: {jobType} to {scalingTarget}");
            //         }
            //     }
            // }      
        }
    }
}