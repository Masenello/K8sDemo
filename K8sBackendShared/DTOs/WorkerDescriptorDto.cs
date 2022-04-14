using K8sBackendShared.Enums;

namespace K8sBackendShared.DTOs
{
    public class WorkerDescriptorDto
    {
        public JobType  WorkerJobType { get; set; }
        public string WorkerId { get; set; }
        
    }
}