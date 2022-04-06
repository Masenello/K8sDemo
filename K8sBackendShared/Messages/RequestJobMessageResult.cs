using System;

namespace K8sBackendShared.Messages
{
    public class RequestJobMessageResult
    {
        public int JobId { get; set; }
        public DateTime CreationTime { get; set; }
        public string User { get; set; }
        public string UserMessage { get; set; }
    }
}