using System;
using K8sCore.Enums;

namespace K8sBackendShared.Messages
{
    public class RequestNewJobCreationMessage
    {
        public string User { get; set; }
        public DateTime RequestDateTime { get; set; }
        public JobType RequestedJobType { get; set; }
    }
}