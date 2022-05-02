using System;
using System.Collections.Generic;
using K8sCore.DTOs;

namespace K8sCore.Messages
{
    public class DirectorStatusMessage
    {

        public DateTime Timestamp { get; set; }
        public List<WorkerDescriptorDto> RegisteredWorkers { get; set; }
    }
}