using System.Collections.Generic;
using k8sCore.Entities;
using k8sCore.Enums;

namespace k8sCore.Repository.JobRepository
{
    public interface IJobRepository: IGenericRepository<JobEntity>
    {
        public IEnumerable<JobEntity> GetJobsInStatus(JobStatus targetStatus);

        public JobEntity GetJobWithId(int Id);
        
    }
}