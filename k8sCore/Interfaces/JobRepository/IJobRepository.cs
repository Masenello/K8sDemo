using System.Collections.Generic;
using K8sCore.Entities;
using K8sCore.Enums;
using K8sCore.Messages;

namespace K8sCore.Interfaces.JobRepository
{
    public interface IJobRepository: IGenericRepository<JobEntity>
    {
        public IEnumerable<JobEntity> GetJobsInStatus(JobStatus targetStatus);
        public JobEntity GetJobWithId(int Id);
    }
}