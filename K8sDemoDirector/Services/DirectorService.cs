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
using Newtonsoft.Json.Serialization;

namespace K8sDemoDirector.Services
{
    public class DirectorService:CyclicWorkerService
    {
        private readonly GetJobListJob _getJobListJob;     

        private readonly ConcurrentDictionary<int,WorkerDescriptorDto> _workersRegistry;


        private readonly IK8s _k8sConnector;
        
        public DirectorService(IK8s k8sConnector,IRabbitConnector rabbitConnector, ILogger logger, int cycleTime, GetJobListJob workerJob)
        :base(rabbitConnector,logger,cycleTime, workerJob)
        {
            _getJobListJob = workerJob;
            _k8sConnector = k8sConnector;

            base.MainCycleCompleted += CyclicWorkerMainCycleCompleted;

            _workersRegistry = new ConcurrentDictionary<int, WorkerDescriptorDto>();
            _rabbitConnector.Respond<JobRequestAssignMessage, JobRequestAssignResultMessage>(RespondToJobAssignRequest);
            _rabbitConnector.Subscribe<WorkerUnRegisterToDirectorMessage>(HandleWorkerUnregisterMessage);
        
        }



        private void HandleWorkerUnregisterMessage(WorkerUnRegisterToDirectorMessage msg)
        {
            try
            {
                var target = _workersRegistry.FirstOrDefault(x=>x.Value.WorkerId == msg.WorkerId);
                WorkerDescriptorDto tmp;
                _workersRegistry.TryRemove(target.Key, out tmp);
                _logger.LogInfo($"Worker {msg.WorkerId} unregistered");
            }
            catch (System.Exception e)
            {
                _logger.LogError($"Failed to remove worker: {msg.WorkerId} from registry",e);
            }

        }

        
        private JobRequestAssignResultMessage RespondToJobAssignRequest(JobRequestAssignMessage request)
        {
            try
            {
                if (!WorkerIsRegistered(request.WorkerId))
                {
                    request.WorkerId = RegisterWorker(request.JobType).WorkerId;
                }
                Console.WriteLine($"received request for job of type: {request.JobType}");
                if (_getJobListJob._insertedJobs.ToList().Where(x=>x.Value.Type == request.JobType).Count()==0)
                {
                    return new JobRequestAssignResultMessage(){
                        JobId = 0,
                        WorkerId = request.WorkerId
                    };
                }
                var targetJob = _getJobListJob._insertedJobs.ToList().Where(x=>x.Value.Type == request.JobType).OrderBy(y=>y.Value.CreationDate).FirstOrDefault();
                JobEntity tmp;
                _getJobListJob._insertedJobs.TryRemove(targetJob.Key, out tmp);
                using (var _context = (new DataContextFactory()).CreateDbContext(null))
                {
                    //Set the job to assigned status
                    JobEntity targetJobOnDb = _context.Jobs.FirstOrDefault(x=>x.Id == targetJob.Value.Id);
                    targetJobOnDb.Status = JobStatus.assigned;
                    _context.SaveChanges();
                    
                }
                _logger.LogInfo($"Job: {targetJob.Value.Type} with Id: {targetJob.Value.Id} assigned to worker: {request.WorkerId}");
            
                return  new JobRequestAssignResultMessage(){
                    JobId = targetJob.Value.Id,
                    WorkerId = request.WorkerId,
                };
            }
            catch (System.Exception e)
            {
                _logger.LogError($"Failed to reply to request from worker: {request.WorkerId} for job of type: {request.JobType}",e);
                return null;
            }
        }

        private WorkerDescriptorDto RegisterWorker(JobType workerJobType)
        {
            WorkerDescriptorDto newWorkerDescriptor = new WorkerDescriptorDto()
            {
                WorkerJobType = workerJobType,
                WorkerId =$"WORKER_{workerJobType}_{DateTime.Now.Ticks}" 
            };
            _workersRegistry.TryAdd(_workersRegistry.Count+1,newWorkerDescriptor);
            _logger.LogInfo($"New worker registered with Id: {newWorkerDescriptor.WorkerId}");
            return newWorkerDescriptor;
        }

        private bool WorkerIsRegistered(string workerId)
        {
            if (_workersRegistry.Count(x=>x.Value.WorkerId == workerId)>0)
            {
                return true;
            } 
            return false;
        }


        int cycleCounter = 0;
        private void CyclicWorkerMainCycleCompleted(object sender, EventArgs e)
        {
            DirectorStatusMessage newStatus = new DirectorStatusMessage()
            {
                RegisteredWorkers = _workersRegistry.Values.ToList(),
                JobsList = _getJobListJob.BuildJobsAvailableMessage().JobsList,
            };
            _rabbitConnector.Publish<DirectorStatusMessage>(newStatus);

            if(cycleCounter % 10 == 0)
            {
                MonitorWorkNumber(newStatus);
            }
            


            cycleCounter+=1;
        }



        private void MonitorWorkNumber(DirectorStatusMessage directorStatus)
        {
            foreach(var jobType in directorStatus.GetWorkersTypes())
            {
                _logger.LogInfo($"Job count: {directorStatus.GetJobsNumber(jobType)}");
                //TODO parametrize treshold
                if (directorStatus.GetJobsNumber(jobType) > 4)
                {
                    //Spawn 1 worker
                    WorkersScaleUp(directorStatus,jobType);
                }

                if (directorStatus.GetJobsNumber(jobType) == 0)
                {
                    //Kill worker
                    WorkersScaleDown(directorStatus,jobType);
                }
            }
        }

        int scalingTarget = 0;
        
        


        //TODO parametric on worker pod deployment
        private void WorkersScaleUp(DirectorStatusMessage directorStatus, JobType jobType)
        {
            scalingTarget = directorStatus.GetWorkersNumber(jobType)+1;
            _k8sConnector.ScaleDeployment(K8sNamespace.defaultNamespace, Deployment.worker, scalingTarget);
            _logger.LogInfo($"Scaling up workers for job: {jobType} to {scalingTarget}");
        }

   
        private void WorkersScaleDown(DirectorStatusMessage directorStatus, JobType jobType)
        {
            //Always leave at least one worker 
            if (directorStatus.GetWorkersNumber(jobType)>1)
            {
                //No jobs of that type must be running
                using (var _context = (new DataContextFactory()).CreateDbContext(null))
                {
                    if (_context.Jobs.Where(x=>x.Type == jobType && x.Status==JobStatus.running).Count() == 0)
                    {
                        scalingTarget = directorStatus.GetWorkersNumber(jobType)-1;
                        _k8sConnector.ScaleDeployment(K8sNamespace.defaultNamespace, Deployment.worker, scalingTarget);
                        _logger.LogInfo($"Scaling down workers for job: {jobType} to {scalingTarget}");
                    }
                }
            }      
        }
    }
}