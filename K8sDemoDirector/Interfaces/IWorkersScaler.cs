using System.Collections.Concurrent;
using K8sCore.DTOs;

namespace K8sDemoDirector.Interfaces
{
    public interface IWorkersScaler
    {

        public int MaxJobsPerWorker { get;}
        public int MaxWorkers { get;}
        public bool SystemIsScaling { get; }
        public bool SystemIsScalingUp { get; }
        public bool SystemIsScalingDown { get; }
        public void MonitorWorkersScaling(int openJobsCount);

        public WorkerDescriptorDto GetWorkerWithLessLoad();
    }
}