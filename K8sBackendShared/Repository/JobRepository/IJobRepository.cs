using System.Collections.Generic;
using K8sBackendShared.Entities;
using K8sBackendShared.Enums;

namespace K8sBackendShared.Repository.JobRepository
{
    public interface IJobRepository: IGenericRepository<JobEntity>
    {
        public IEnumerable<JobEntity> GetJobsInStatus(JobStatus targetStatus);

        public JobEntity GetJobWithId(int Id);
        
    }
}