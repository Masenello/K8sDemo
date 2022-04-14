using K8sBackendShared.Enums;

namespace K8sBackendShared.Messages
{
    public class JobRequestAssignMessage
    {
        public string WorkerId { get; set; }

        public JobType JobType { get; set; }

    }
}