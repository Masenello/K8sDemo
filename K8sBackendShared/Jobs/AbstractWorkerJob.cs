using System;
using EasyNetQ;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Messages;
using K8sBackendShared.Settings;

namespace K8sBackendShared.Jobs
{
    public abstract class AbstractWorkerJob
    {
        
        public delegate void JobProgressChangedHandler(object sender, JobProgressEventArgs e);
        public event JobProgressChangedHandler JobProgressChanged;

        protected readonly ILogger _logger;

        protected readonly IRabbitConnector _rabbitConnector;

        public AbstractWorkerJob(ILogger logger, IRabbitConnector rabbitConnector)
        {
            _logger = logger;
            _rabbitConnector = rabbitConnector;
        }

        public abstract void DoWork(object workerParameters);

        protected void ReportWorkProgress(JobStatusMessage newStatus) 
        {
            _rabbitConnector.Publish(newStatus);
            _logger.LogInfo($"{newStatus.StatusJobType}: Job Id:{newStatus.JobId} Status: {newStatus.Status} Progress: {newStatus.ProgressPercentage}% Message: {newStatus.UserMessage}");
        }

    }
    


}