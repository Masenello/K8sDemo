using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using K8sBackendShared.Logging;
using K8sCore.Entities.Mongo;
using K8sCore.Enums;
using K8sCore.Interfaces.Mongo;
using K8sCore.Messages;

namespace K8sDataMongo.Repository.JobRepository
{
    public class JobRepository : GenericMongoRepository<JobEntity>, IJobRepository
    {
        public JobRepository() : base()
        {

        }

        public List<JobEntity> GetJobsInStatus(JobStatus targetStatus)
        {
            return Find(x => x.Status == targetStatus);
        }

        public List<JobEntity> GetOpenJobs()
        {
            //return FindAsync(x=>x.EndDate == null);

            return Find(x => x.EndDate == null);
        }

        public async Task<JobStatusMessage> AssignJobAsync(string workerId, string jobId)
        {
            var targetJob = await GetByIdAsync(jobId);
            targetJob.Status = K8sCore.Enums.JobStatus.assigned;
            targetJob.WorkerId = workerId;
            targetJob.AssignmentDate = DateTime.UtcNow;
            await UpdateAsync(jobId, targetJob);
            return new JobStatusMessage(targetJob);
        }

        public async Task<JobStatusMessage> SetJobInRunningStatusAsync(string jobId)
        {
            var targetJob = await GetByIdAsync(jobId);
            targetJob.Status = K8sCore.Enums.JobStatus.running;
            targetJob.StartDate = DateTime.UtcNow;
            await UpdateAsync(jobId, targetJob);
            return new JobStatusMessage(targetJob);
        }

        public async Task<JobStatusMessage> SetJobInCompletedStatusAsync(string jobId)
        {
            var targetJob = await GetByIdAsync(jobId);
            targetJob.Status = K8sCore.Enums.JobStatus.completed;
            targetJob.EndDate = DateTime.UtcNow;
            await UpdateAsync(jobId, targetJob);
            return new JobStatusMessage(targetJob);
        }

        public async Task<JobStatusMessage> SetJobInErrorAsync(string jobId, string workerId, Exception ex)
        {
            var targetJob = await GetByIdAsync(jobId);
            targetJob.Status = K8sCore.Enums.JobStatus.error;
            targetJob.EndDate = DateTime.UtcNow;
            targetJob.Errors = $"{targetJob.GenerateJobDescriptor()} in error".AddException(ex);
            await UpdateAsync(jobId, targetJob);
            return new JobStatusMessage(targetJob, targetJob.Errors);
        }



        public async Task<JobStatusMessage> SetJobInTimeOutAsync(string jobId, string workerId)
        {
            var targetJob = await GetByIdAsync(jobId);
            targetJob.Status = K8sCore.Enums.JobStatus.error;
            targetJob.EndDate = DateTime.UtcNow;
            targetJob.Errors = $"Job timeout";
            targetJob.WorkerId = workerId;
            await UpdateAsync(jobId, targetJob);
            return new JobStatusMessage(targetJob, $"{targetJob.GenerateJobDescriptor()}. Job Timeout");
        }

        public async Task UnAssignOpenWorkerJobs(string workerId)
        {
            var targetJobs = GetOpenJobs();
            foreach (JobEntity job in GetOpenJobs().Where(x=>x.WorkerId == workerId))
            {
                job.AssignmentDate = null;
                job.StartDate = null;
                job.Status = JobStatus.created;
                job.WorkerId = null;
                await UpdateAsync(job.Id, job);
            }
        }
    }
}