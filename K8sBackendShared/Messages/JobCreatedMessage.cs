using System;

namespace K8sBackendShared.Messages
{
    public class JobCreatedMessage
    {
        public int JobId { get; set; }
        public DateTime CreationTime { get; set; }
        public string ApplicationGuid { get; set; }
    }
}