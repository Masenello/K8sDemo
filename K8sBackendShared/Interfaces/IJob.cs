using System;
using System.Threading.Tasks;
using K8sBackendShared.K8s;

namespace K8sBackendShared.Interfaces
{
    public interface IJob
    {
        public void DoWork();
    }
}