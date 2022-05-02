using System;
using k8sCore.Repository;
using k8sCore.Repository.JobRepository;
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
            var targetJob = Jobs.GetById(jobId);
            targetJob.Status = k8sCore.Enums.JobStatus.assigned;
            targetJob.WorkerId = workerId;
            targetJob.AssignmentDate = DateTime.UtcNow;
            Complete();
            return new JobStatusMessage(targetJob);
        }

        public JobStatusMessage SetJobInTimeOut(int jobId)
        {
            var targetJob = Jobs.GetById(jobId);
            targetJob.Status = k8sCore.Enums.JobStatus.error;
            targetJob.EndDate = DateTime.UtcNow;
            targetJob.Errors = $"Job timeout";
            Complete();
            return new JobStatusMessage(targetJob, $"{targetJob.GenerateJobDescriptor()}. Job Timeout");
        }


    }
}