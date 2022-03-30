using K8sBackendShared.Enums;

namespace K8sBackendShared.Messages
{
    public class JobStatusMessage
    {
        public JobStatus Status { get; set; }
        public string ApplicationGuid { get; set; }
        public double ProgressPercentage { get; set; }
    }
}