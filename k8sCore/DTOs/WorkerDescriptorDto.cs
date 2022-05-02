using K8sCore.Enums;

namespace K8sCore.DTOs
{
    public class WorkerDescriptorDto
    {
        public string WorkerId { get; set; }
        public int CurrentJobs { get; set; }
        
    }
}