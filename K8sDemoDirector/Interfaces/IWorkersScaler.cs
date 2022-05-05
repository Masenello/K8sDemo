using System.Collections.Concurrent;
using K8sCore.DTOs;

namespace K8sDemoDirector.Interfaces
{
    public interface IWorkersScaler
    {

        public bool SystemIsScaling { get; }
        public void MonitorWorkersScaling(int openJobsCount);

        public WorkerDescriptorDto GetWorkerWithLessLoad();
    }
}