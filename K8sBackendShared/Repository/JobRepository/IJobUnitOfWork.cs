using System;
using K8sBackendShared.Repository.JobRepository;

namespace K8sBackendShared.Repository
{
    public interface IJobUnitOfWork : IDisposable
    {
        IJobRepository Jobs { get; }
        int Complete();
    }
}