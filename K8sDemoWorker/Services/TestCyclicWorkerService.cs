using System;
using K8sBackendShared.Jobs;
using K8sBackendShared.RabbitConnector;
using K8sBackendShared.Workers;
using K8sDemoHubWorker.Services;
using K8sDemoWorker.Jobs;
using Microsoft.Extensions.Hosting;

namespace K8sDemoWorker.Services
{
    public class TestCyclicWorkerService:CyclicWorkerService
    {
        public TestCyclicWorkerService(RabbitConnectorService rabbitService, int cycleTime, AbstractWorkerJob workerJob)
        :base(rabbitService,cycleTime, workerJob)
        {
            if (rabbitService is null) throw new Exception("rabbit is null");
        }
    }
}