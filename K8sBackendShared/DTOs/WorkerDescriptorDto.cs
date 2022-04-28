using K8sBackendShared.Enums;

namespace K8sBackendShared.DTOs
{
    public class WorkerDescriptorDto
    {
        public string WorkerId { get; set; }
        public int CurrentJobs { get; set; }
        
    }
}