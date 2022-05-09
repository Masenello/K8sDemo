using System.Collections.Concurrent;
using System.Collections.Generic;
using K8sBackendShared.Utils;
using K8sCore.DTOs;
using K8sCore.Entities.Mongo;

namespace K8sDemoDirector.Interfaces
{
    public interface IWorkersRegistryManager
    {
        public ConcurrentDictionary<int, WorkerDescriptorDto> WorkersRegistry { get; }

        public ThreadedQueue<string> RestartedWorkers { get;}

        public void UpdateJobCounts(List<JobEntity> openJobs);
        public void AddJobToWorkerJobCount(string workerId);
        public void ResetWorkerJobCount(string workerId);
    }
}