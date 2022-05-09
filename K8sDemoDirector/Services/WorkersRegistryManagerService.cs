using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Utils;
using K8sCore.DTOs;
using K8sCore.Entities.Mongo;
using K8sCore.Interfaces.Mongo;
using K8sCore.Messages;
using K8sDemoDirector.Interfaces;

namespace K8sDemoDirector.Services
{
    public class WorkersRegistryManagerService : IWorkersRegistryManager
    {
        public ConcurrentDictionary<int, WorkerDescriptorDto> WorkersRegistry { get; private set; } = new ConcurrentDictionary<int, WorkerDescriptorDto>();

        private readonly IRabbitConnector _rabbitConnector;
        private readonly ILogger _logger;

        public ThreadedQueue<string> RestartedWorkers { get; private set; } = new ThreadedQueue<string>();


        public WorkersRegistryManagerService(IRabbitConnector rabbitConnector, ILogger logger)
        {
            _rabbitConnector = rabbitConnector;
            _logger = logger;


            _rabbitConnector.Subscribe<WorkerRegisterToDirectorMessage>(HandleWorkerRegisterMessage);
            _rabbitConnector.Subscribe<WorkerUnRegisterToDirectorMessage>(HandleWorkerUnregisterMessage);

            _rabbitConnector.Publish<DirectorStartedMessage>(new DirectorStartedMessage());
        }

        public void UpdateJobCounts(List<JobEntity> openJobs)
        {
            foreach (var worker in WorkersRegistry.Values)
            {
                worker.CurrentJobs = openJobs.Where(x => x.WorkerId == worker.WorkerId).Count();
                //_logger.LogInfo($"Worker with id: {worker.WorkerId} has: {worker.CurrentJobs} active jobs");
            }
        }

        public void AddJobToWorkerJobCount(string workerId)
        {
            WorkersRegistry.Values.FirstOrDefault(x => x.WorkerId == workerId).CurrentJobs += 1;
        }

        public void ResetWorkerJobCount(string workerId)
        {
            WorkersRegistry.Values.FirstOrDefault(x => x.WorkerId == workerId).CurrentJobs = 0;
        }


        private void HandleWorkerRegisterMessage(WorkerRegisterToDirectorMessage msg)
        {
            try
            {
                var target = WorkersRegistry.FirstOrDefault(x => x.Value.WorkerId == msg.WorkerId);
                if (WorkerIsRegistered(msg.WorkerId))
                {
                    _logger.LogWarning($"Worker with id: {msg.WorkerId} restarted after error. Pending jobs will be reassigned");
                    //If worker is restarted after errors add to workers restarted queue
                    RestartedWorkers.Enqueue(msg.WorkerId);
                }
                else
                {
                    WorkersRegistry.TryAdd(GenerateKeyForWorkerRegistry(), new WorkerDescriptorDto()
                    {
                        WorkerId = msg.WorkerId
                    });
                    _logger.LogInfo($"Worker {msg.WorkerId} registered, total workers: {WorkersRegistry.Count}");
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
                var target = WorkersRegistry.FirstOrDefault(x => x.Value.WorkerId == msg.WorkerId);
                if (WorkerIsRegistered(msg.WorkerId))
                {
                    WorkerDescriptorDto tmp;
                    WorkersRegistry.TryRemove(target.Key, out tmp);
                    _logger.LogInfo($"Worker {msg.WorkerId} unregistered, total workers: {WorkersRegistry.Count}");
                }

            }
            catch (System.Exception e)
            {
                _logger.LogError($"Failed to remove worker: {msg.WorkerId} from worker registry", e);
            }

        }

        private int GenerateKeyForWorkerRegistry()
        {
            if (WorkersRegistry.Count() == 0)
            {
                return 1;
            }
            else
            {
                return (WorkersRegistry.Keys.Max() + 1);
            }
        }

        private bool WorkerIsRegistered(string workerId)
        {
            if (WorkersRegistry.Count(x => x.Value.WorkerId == workerId) > 0)
            {
                return true;
            }
            return false;
        }

    }
}