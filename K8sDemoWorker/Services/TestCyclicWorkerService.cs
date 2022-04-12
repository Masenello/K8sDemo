using System;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Jobs;
using K8sBackendShared.RabbitConnector;
using K8sBackendShared.Workers;
using K8sDemoWorker.Jobs;
using Microsoft.Extensions.Hosting;

namespace K8sDemoWorker.Services
{
    public class TestCyclicWorkerService:CyclicWorkerService
    {
        public TestCyclicWorkerService(IRabbitConnector rabbitConnector, ILogger logger, int cycleTime, AbstractWorkerJob workerJob)
        :base(rabbitConnector,logger,cycleTime, workerJob)
        {

        }
    }
}