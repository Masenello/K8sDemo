using System;
using System.Threading.Tasks;
using K8sBackendShared.Logging;
using K8sCore.Interfaces.JobRepository;
using K8sCore.Messages;
using K8sData.Data;

namespace K8sBackendShared.Repository.JobRepository
{
    public class JobUnitOfWork : IJobUnitOfWork
    {
        private readonly DataContext _context;

        public IJobRepository Jobs { get; private set; }

        public JobUnitOfWork(DataContext context)
        {
            _context = context;
            Jobs = new JobRepository(_context);
        }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<JobStatusMessage> AssignJobAsync(string workerId, int jobId)
        {
            var targetJob = Jobs.GetJobWithIdAsync(jobId).Result;
            targetJob.Status = K8sCore.Enums.JobStatus.assigned;
            targetJob.WorkerId = workerId;
            targetJob.AssignmentDate = DateTime.UtcNow;
            await CompleteAsync();
            return new JobStatusMessage(targetJob);
        }

        public async Task<JobStatusMessage> SetJobInRunningStatusAsync(int jobId)
        {
            var targetJob = Jobs.GetJobWithIdAsync(jobId).Result;
            targetJob.Status = K8sCore.Enums.JobStatus.running;
            targetJob.StartDate = DateTime.UtcNow;
            await CompleteAsync();
            return new JobStatusMessage(targetJob);
        }

        public async Task<JobStatusMessage> SetJobInCompletedStatusAsync(int jobId)
        {
            var targetJob = Jobs.GetJobWithIdAsync(jobId).Result;
            targetJob.Status = K8sCore.Enums.JobStatus.completed;
            targetJob.EndDate = DateTime.UtcNow;
            await CompleteAsync();
            return new JobStatusMessage(targetJob);
        }

        public async Task<JobStatusMessage> SetJobInErrorAsync(int jobId, Exception ex)
        {
            var targetJob = Jobs.GetJobWithIdAsync(jobId).Result;
            targetJob.Status = K8sCore.Enums.JobStatus.error;
            targetJob.EndDate = DateTime.UtcNow;
            targetJob.Errors = $"{targetJob.GenerateJobDescriptor()} in error".AddException(ex);
            await CompleteAsync();
            return new JobStatusMessage(targetJob, targetJob.Errors);
        }



        public async Task<JobStatusMessage> SetJobInTimeOutAsync(int jobId)
        {
            var targetJob = Jobs.GetJobWithIdAsync(jobId).Result;
            targetJob.Status = K8sCore.Enums.JobStatus.error;
            targetJob.EndDate = DateTime.UtcNow;
            targetJob.Errors = $"Job timeout";
            await CompleteAsync();
            return new JobStatusMessage(targetJob, $"{targetJob.GenerateJobDescriptor()}. Job Timeout");
        }


    }
}