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

        public AbstractWorkerJob(ILogger logger)
        {
            _logger = logger;
        }

        public abstract void DoWork();

        protected void ReportWorkProgress(JobStatusMessage newStatus) 
        {
            if (JobProgressChanged != null)
            {
                JobProgressEventArgs myArgs = new JobProgressEventArgs(newStatus);
                JobProgressChanged(this, myArgs);
            }
        }

    }
    


}