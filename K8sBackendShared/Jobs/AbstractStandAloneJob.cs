using System;
using EasyNetQ;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Logging;
using K8sBackendShared.Messages;
using K8sBackendShared.Settings;

namespace K8sBackendShared.Jobs
{
    public abstract class AbstractStandAloneJob:IDisposable
    {
        protected readonly IBus _rabbitBus;

        protected readonly RabbitStandAloneLogger _logger;

        public AbstractStandAloneJob()
        {
            
            _rabbitBus = RabbitHutch.CreateBus(NetworkSettings.RabbitHostResolver());
            _logger = new RabbitStandAloneLogger(_rabbitBus);
        }

        public void Dispose()
        {
            _rabbitBus.Dispose();
        }

        public abstract void DoWork();

    
        protected void ReportWorkProgress(JobStatusMessage newStatus) 
        {
            _rabbitBus.PubSub.Publish<JobStatusMessage>(newStatus);
        }




    }
}