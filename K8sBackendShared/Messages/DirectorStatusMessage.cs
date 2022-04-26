using System;
using System.Collections.Generic;
using System.Linq;
using K8sBackendShared.DTOs;
using K8sBackendShared.Enums;

namespace K8sBackendShared.Messages
{
    public class DirectorStatusMessage
    {

        public DateTime Timestamp { get; set; }
        
        
        public List<WorkerDescriptorDto> RegisteredWorkers { get; set; }

        public List<JobAvailableCount> JobsList{ get; set; } 

        public int GetWorkersNumber(JobType jobType)
        {
            return RegisteredWorkers.Where(x=>x.WorkerJobType == jobType).Count();
        }

        public int GetJobsNumber(JobType jobType)
        {
            var tmp = JobsList.SingleOrDefault(x=>x.JobType == jobType);
            if (tmp is null)
            {
                return 0;
            }
            else
            {
                return tmp.JobCount;
            }
            
        }

        public List<JobType> GetWorkersTypes()
        {
            var tmp = new List<JobType>();
            foreach(var worker in RegisteredWorkers)
            {
                if (!tmp.Contains(worker.WorkerJobType))
                {
                tmp.Add(worker.WorkerJobType);
                }
            }
            return tmp;
        }

        
        
    }
}