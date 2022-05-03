using System.Collections.Generic;
using System.Threading.Tasks;
using K8sCore.Entities;
using K8sCore.Enums;
using K8sCore.Messages;

namespace K8sCore.Interfaces.JobRepository
{
    public interface IJobRepository: IGenericRepository<JobEntity>
    {
        public Task<IEnumerable<JobEntity>> GetJobsInStatusAsync(JobStatus targetStatus);

        public Task<IEnumerable<JobEntity>> GetOpenJobs();
        public Task<JobEntity> GetJobWithIdAsync(int Id);
    }
}