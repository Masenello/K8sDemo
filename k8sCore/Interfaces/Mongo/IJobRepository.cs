using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using K8sCore.Entities;
using K8sCore.Entities.Mongo;
using K8sCore.Enums;
using K8sCore.Messages;

namespace K8sCore.Interfaces.Mongo
{
    public interface IJobRepository : IGenericMongoRepository<JobEntity>
    {
        public List<JobEntity> GetJobsInStatus(JobStatus targetStatus);
        public List<JobEntity> GetOpenJobs();
        public Task<JobStatusMessage> AssignJobAsync(string workerId, string jobId);

        public Task<JobStatusMessage> SetJobInRunningStatusAsync(string jobId);

        public Task<JobStatusMessage> SetJobInCompletedStatusAsync(string jobId);

        public Task<JobStatusMessage> SetJobInErrorAsync(string jobId, string workerId, Exception ex);
        public Task<JobStatusMessage> SetJobInTimeOutAsync(string jobId, string workerId);

    }
}