using System;
using k8sCore.Repository.JobRepository;
using K8sCore.Messages;

namespace k8sCore.Repository
{
    public interface IJobUnitOfWork : IDisposable
    {
        IJobRepository Jobs { get; }
        int Complete();

        JobStatusMessage AssignJob(string workerId, int jobId);
        JobStatusMessage SetJobInTimeOut(int jobId);
    }
}