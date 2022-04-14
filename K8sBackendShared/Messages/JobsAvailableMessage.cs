using System;
using System.Collections.Generic;
using System.Linq;
using K8sBackendShared.Enums;

namespace K8sBackendShared.Messages
{
    public class JobsAvailableMessage
    {
        public List<JobAvailableCount> JobsList{ get; set; } 

        public JobsAvailableMessage()
        {
            JobsList = new List<JobAvailableCount>();
        }

        
        public int GetJobsNumber(JobType targetJobType)
        {
            return JobsList.Where(x => x.JobType == targetJobType).Count();
        }
    }

    public class JobAvailableCount
    {
        public JobType JobType { get; set; }
        public int JobCount { get; set; }
        

    }
}