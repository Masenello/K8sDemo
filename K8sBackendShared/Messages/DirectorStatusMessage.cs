using System.Collections.Generic;
using K8sBackendShared.DTOs;

namespace K8sBackendShared.Messages
{
    public class DirectorStatusMessage
    {
        public List<WorkerDescriptorDto> RegisteredWorkers { get; set; }

        public List<JobAvailableCount> JobsList{ get; set; } 
        
        
    }
}