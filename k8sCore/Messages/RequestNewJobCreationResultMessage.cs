using System;
using K8sCore.Enums;

namespace K8sCore.Messages
{
    public class RequestNewJobCreationResultMessage
    {
        public int JobId { get; set; }
        public DateTime CreationTime { get; set; }
        public string User { get; set; }
        public string UserMessage { get; set; }

        public JobType JobType { get; set; }

        public JobStatus JobStatus { get; set; }


    }
}