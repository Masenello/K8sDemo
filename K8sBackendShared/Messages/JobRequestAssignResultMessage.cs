namespace K8sBackendShared.Messages
{
    public class JobRequestAssignResultMessage
    {
        public int JobId { get; set; }

        public string WorkerId { get; set; }
        
    }
}