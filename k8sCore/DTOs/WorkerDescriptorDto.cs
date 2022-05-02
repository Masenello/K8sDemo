using k8sCore.Enums;

namespace k8sCore.DTOs
{
    public class WorkerDescriptorDto
    {
        public string WorkerId { get; set; }
        public int CurrentJobs { get; set; }
        
    }
}