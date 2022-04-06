using System;
using EasyNetQ;
using K8sBackendShared.Messages;
using K8sBackendShared.Settings;

namespace K8sBackendShared.Jobs
{
    public abstract class AbstractWorkerJob
    {
        
        public delegate void JobProgressChangedHandler(object sender, JobProgressEventArgs e);
        public event JobProgressChangedHandler JobProgressChanged;

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