
using System.Threading.Tasks;
using EasyNetQ;
using K8sBackendShared.Interfaces;
using K8sCore.Messages;

namespace K8sBackendShared.Jobs
{
    public abstract class AbstractWorkerJob
    {

        protected readonly ILogger _logger;

        protected readonly IRabbitConnector _rabbitConnector;

        public AbstractWorkerJob(ILogger logger, IRabbitConnector rabbitConnector)
        {
            _logger = logger;
            _rabbitConnector = rabbitConnector;
        }

        public abstract Task DoWorkAsync();

        protected void ReportWorkProgress(JobStatusMessage newStatus) 
        {
            _rabbitConnector.Publish(newStatus);
            _logger.LogInfo($"{newStatus.StatusJobType}: Job Id:{newStatus.JobId} Status: {newStatus.Status} Progress: {newStatus.ProgressPercentage}% Message: {newStatus.UserMessage}");
        }

    }
    


}