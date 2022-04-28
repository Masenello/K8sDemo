using System;
using System.Collections.Generic;
using System.Linq;
using K8sBackendShared.DTOs;
using K8sBackendShared.Enums;

namespace K8sBackendShared.Messages
{
    public class DirectorAssignJobToWorker
    {
        public string WorkerId { get; set; }
        public int JobId{ get; set; } 
    }
}