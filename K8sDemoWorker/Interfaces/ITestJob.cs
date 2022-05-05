using K8sBackendShared.Interfaces;

namespace K8sDemoWorker.Interfaces
{
    public interface ITestJob : IJob
    {
        public string WorkerId { get; set; }
        public string DatabaseJobId { get; set; }
    }
}