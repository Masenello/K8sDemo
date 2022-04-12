using System;
using K8sBackendShared.Interfaces;
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
        public TestCyclicWorkerService(IRabbitPublisher rabbitSender, ILogger logger, int cycleTime, AbstractWorkerJob workerJob)
        :base(rabbitSender,logger,cycleTime, workerJob)
        {

        }
    }
}