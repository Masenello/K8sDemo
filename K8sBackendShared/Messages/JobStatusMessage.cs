using K8sBackendShared.Entities;
using K8sBackendShared.Enums;

namespace K8sBackendShared.Messages
{
    public class JobStatusMessage
    {
        public int JobId { get; set; }
        public JobType StatusJobType { get; set; }
        public JobStatus Status { get; set; }
        public string User { get; set; }
        public double ProgressPercentage { get; set; }
        public string UserMessage { get; set; }

        public string WorkerId { get; set; }

    }
}