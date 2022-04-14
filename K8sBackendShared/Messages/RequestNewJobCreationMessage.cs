using System;
using K8sBackendShared.Enums;

namespace K8sBackendShared.Messages
{
    public class RequestNewJobCreationMessage
    {
        public string User { get; set; }
        public DateTime RequestDateTime { get; set; }
        public JobType RequestedJobType { get; set; }
    }
}