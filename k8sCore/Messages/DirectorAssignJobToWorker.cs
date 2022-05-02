
using K8sCore.Enums;

namespace K8sCore.Messages
{
    public class DirectorAssignJobToWorker
    {
        public string WorkerId { get; set; }
        public int JobId{ get; set; } 
        public JobType JobType { get; set; } 

    }
}