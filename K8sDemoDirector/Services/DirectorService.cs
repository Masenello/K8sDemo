using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using K8sBackendShared.Data;
using K8sBackendShared.DTOs;
using K8sBackendShared.Entities;
using K8sBackendShared.Enums;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Jobs;
using K8sBackendShared.Messages;
using K8sBackendShared.Workers;
using K8sDemoDirector.Jobs;

namespace K8sDemoDirector.Services
{
    public class DirectorService:CyclicWorkerService
    {
        private readonly GetJobListJob _getJobListJob;     

        private readonly ConcurrentDictionary<int,WorkerDescriptorDto> _workersRegistry;
        
        public DirectorService(IRabbitConnector rabbitConnector, ILogger logger, int cycleTime, GetJobListJob workerJob)
        :base(rabbitConnector,logger,cycleTime, workerJob)
        {
            _getJobListJob = workerJob;
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
                //Console.WriteLine($"received request for job of type: {request.JobType}");
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
                WorkerId =$"WORKER_{workerJobType}_{_workersRegistry.Count(x=>x.Value.WorkerJobType == workerJobType)+1}" 
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

    }
}