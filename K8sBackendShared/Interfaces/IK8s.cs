using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using K8sBackendShared.K8s;
using K8sCore.DTOs;

namespace K8sBackendShared.Interfaces
{
    public interface IK8s
    {
        Task ScaleDeployment(K8sNamespace kubernetesNameSpace, Deployment dep, int targetReplicas);
        Task<List<PodInfoDto>> GetPodInfo(K8sNamespace kubernetesNameSpace);
        Task<PodLogDto> GetPodLog(K8sNamespace kubernetesNameSpace, string podName, int sinceSeconds = 24*60*60);
    }
}