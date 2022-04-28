using System;
using System.Collections.Generic;
using System.Linq;
using K8sBackendShared.DTOs;
using K8sBackendShared.Enums;

namespace K8sBackendShared.Messages
{
    public class DirectorStatusMessage
    {

        public DateTime Timestamp { get; set; }
        public List<WorkerDescriptorDto> RegisteredWorkers { get; set; }
    }
}