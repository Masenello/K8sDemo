using K8sBackendShared.Interfaces;
using K8sBackendShared.Jobs;
using K8sBackendShared.Workers;

namespace K8sDemoDirector.Services
{
    public class CyclicWorkerManagerService:CyclicWorkerService
    {
        public CyclicWorkerManagerService(IRabbitConnector rabbitConnector, ILogger logger, int cycleTime, AbstractWorkerJob workerJob)
        :base(rabbitConnector,logger,cycleTime, workerJob)
        {

        }
    }
}