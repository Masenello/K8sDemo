

using K8sCore.Enums;

namespace K8sCore.DTOs
{
    public class JobStatusDto
    {
        public int JobId { get; set; }
        public JobType JobType { get; set; }
        public JobStatus Status { get; set; }
        public string User { get; set; }
        public int ProgressPercentage { get; set; }
        public string UserMessage { get; set; }
        public string WorkerId { get; set; }
    }
}
