using System;
using K8sBackendShared.Messages;
using K8sBackendShared.Repository.JobRepository;

namespace K8sBackendShared.Repository
{
    public interface IJobUnitOfWork : IDisposable
    {
        IJobRepository Jobs { get; }
        int Complete();

        JobStatusMessage AssignJob(string workerId, int jobId);
        JobStatusMessage SetJobInTimeOut(int jobId);
    }
}