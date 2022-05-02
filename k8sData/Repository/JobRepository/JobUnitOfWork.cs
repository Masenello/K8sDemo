using System;
using K8sBackendShared.Logging;
using k8sCore.Interfaces.JobRepository;
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
        public int Complete()
        {
            return _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public JobStatusMessage AssignJob(string workerId, int jobId)
        {
            var targetJob = Jobs.GetJobWithId(jobId);
            targetJob.Status = k8sCore.Enums.JobStatus.assigned;
            targetJob.WorkerId = workerId;
            targetJob.AssignmentDate = DateTime.UtcNow;
            Complete();
            return new JobStatusMessage(targetJob);
        }

        public JobStatusMessage SetJobInRunningStatus(int jobId)
        {
            var targetJob = Jobs.GetJobWithId(jobId);
            targetJob.Status = k8sCore.Enums.JobStatus.running;
            targetJob.StartDate = DateTime.UtcNow;
            Complete();
            return new JobStatusMessage(targetJob);
        }

        public JobStatusMessage SetJobInCompletedStatus(int jobId)
        {
            var targetJob = Jobs.GetJobWithId(jobId);
            targetJob.Status = k8sCore.Enums.JobStatus.completed;
            targetJob.EndDate = DateTime.UtcNow;
            Complete();
            return new JobStatusMessage(targetJob);
        }

        public JobStatusMessage SetJobInError(int jobId, Exception ex)
        {
            var targetJob = Jobs.GetJobWithId(jobId);
            targetJob.Status = k8sCore.Enums.JobStatus.error;
            targetJob.EndDate = DateTime.UtcNow;
            targetJob.Errors = $"{targetJob.GenerateJobDescriptor()} in error".AddException(ex);
            Complete();
            return new JobStatusMessage(targetJob, targetJob.Errors);
        }



        public JobStatusMessage SetJobInTimeOut(int jobId)
        {
            var targetJob = Jobs.GetJobWithId(jobId);
            targetJob.Status = k8sCore.Enums.JobStatus.error;
            targetJob.EndDate = DateTime.UtcNow;
            targetJob.Errors = $"Job timeout";
            Complete();
            return new JobStatusMessage(targetJob, $"{targetJob.GenerateJobDescriptor()}. Job Timeout");
        }


    }
}