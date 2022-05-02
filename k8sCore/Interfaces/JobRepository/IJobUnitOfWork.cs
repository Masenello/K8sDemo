using System;
using System.Threading.Tasks;
using K8sCore.Messages;

namespace K8sCore.Interfaces.JobRepository
{
    public interface IJobUnitOfWork : IDisposable
    {
        IJobRepository Jobs { get; }
        Task<int> CompleteAsync();
        Task<JobStatusMessage> AssignJobAsync(string workerId, int jobId);
        Task<JobStatusMessage> SetJobInRunningStatusAsync(int jobId);
        Task<JobStatusMessage> SetJobInCompletedStatusAsync(int jobId);
        Task<JobStatusMessage> SetJobInTimeOutAsync(int jobId);
        Task<JobStatusMessage> SetJobInErrorAsync(int jobId, Exception ex);
    }
}