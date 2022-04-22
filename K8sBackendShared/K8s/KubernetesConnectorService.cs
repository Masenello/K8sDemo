

using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Logging;
using k8s;
using k8s.Models;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Logging;
using K8sBackendShared.Messages;
using K8sBackendShared.Settings;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

namespace K8sBackendShared.K8s
{
    public class KubernetesConnectorService:IK8s
    {

        private readonly Kubernetes _client;
        private readonly ILogger _logger;
        

        public KubernetesConnectorService(ILogger logger)
        {    
            _logger = logger;
            try 
            {
                
                 // Load from the default kubeconfig on the machine.
                var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
                _client = new Kubernetes(config);
    
            }     
            catch (Exception e)
            {
                Console.WriteLine($"Failed to build Kubernetes API client".AddException(e));
                throw;
            }

        }

        public async Task ScaleDeployment(K8sNamespace kubernetesNameSpace, Deployment dep, int targetReplicas)
        {
            try
            {
                var jsonPatch = new JsonPatchDocument<V1Scale>();
                jsonPatch.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
                jsonPatch.Replace(e => e.Spec.Replicas, targetReplicas);
                var jsonPatchString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonPatch);
                var patch = new V1Patch(jsonPatchString, V1Patch.PatchType.JsonPatch);
                await _client.PatchNamespacedDeploymentScaleAsync(patch, dep.Value, kubernetesNameSpace.Value);
            }
            catch (System.Exception e)
            {
                _logger.LogError($"Failed to scale deployment {dep.Value} in namespace {kubernetesNameSpace}".AddException(e));
            }

        }

        

    

    }
}