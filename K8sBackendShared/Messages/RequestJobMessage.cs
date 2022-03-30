using System;

namespace K8sBackendShared.Messages
{
    public class RequestJobMessage
    {
        public string ApplicationGuid { get; set; }
        public DateTime RequestDateTime { get; set; }
    }
}