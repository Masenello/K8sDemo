using System.Collections.Generic;
using k8sCore.Entities;
using k8sCore.Enums;
using K8sCore.Messages;

namespace k8sCore.Interfaces.JobRepository
{
    public interface IJobRepository: IGenericRepository<JobEntity>
    {
        public IEnumerable<JobEntity> GetJobsInStatus(JobStatus targetStatus);
        public JobEntity GetJobWithId(int Id);
    }
}