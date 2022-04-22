using System;
using System.Threading.Tasks;
using K8sBackendShared.K8s;

namespace K8sBackendShared.Interfaces
{
    public interface IK8s
    {
        Task ScaleDeployment(K8sNamespace kubernetesNameSpace, Deployment dep, int targetReplicas);

    }
}