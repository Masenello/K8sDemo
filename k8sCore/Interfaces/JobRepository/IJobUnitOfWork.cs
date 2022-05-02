using System;
using K8sCore.Messages;

namespace K8sCore.Interfaces.JobRepository
{
    public interface IJobUnitOfWork : IDisposable
    {
        IJobRepository Jobs { get; }
        int Complete();

        JobStatusMessage AssignJob(string workerId, int jobId);
        JobStatusMessage SetJobInRunningStatus(int jobId);
        JobStatusMessage SetJobInCompletedStatus(int jobId);
        JobStatusMessage SetJobInTimeOut(int jobId);
        JobStatusMessage SetJobInError(int jobId, Exception ex);
    }
}