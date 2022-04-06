using System;
using K8sBackendShared.Messages;

namespace K8sBackendShared.Jobs
{
    public class JobProgressEventArgs:EventArgs
    {
        public JobStatusMessage Status { get; set; }


        public JobProgressEventArgs(JobStatusMessage newStatus)
        {
            Status = newStatus;
        }
    }
}