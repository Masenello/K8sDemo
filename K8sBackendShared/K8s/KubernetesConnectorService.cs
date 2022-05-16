

using System;
using System.Collections.Generic;
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
using K8sCore.DTOs;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.IO;

namespace K8sBackendShared.K8s
{
    public class KubernetesConnectorService : IK8s
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
                _logger.LogError($"Failed to scale deployment {dep.Value} in namespace {kubernetesNameSpace.Value}".AddException(e));
            }

        }

        public async Task<List<PodInfoDto>> GetPodInfo(K8sNamespace kubernetesNameSpace)
        {
            try
            {
                List<PodInfoDto> podInfoList = new List<PodInfoDto>();
                var podList = await _client.ListNamespacedPodAsync(kubernetesNameSpace.Value);
                foreach (var item in podList.Items)
                {
                    podInfoList.Add(new PodInfoDto()
                    {
                        Name = item.Metadata.Name,
                        Status = item.Status.Phase,
                        Image = item.Spec.Containers[0].Image,
                        Node = item.Spec.NodeName

                    });
                }
                return podInfoList;
            }
            catch (System.Exception e)
            {
                _logger.LogError($"Failed get info on pod in namespace {kubernetesNameSpace.Value}".AddException(e));
                return null;
            }

        }

        public async Task<PodLogDto> GetPodLog(K8sNamespace kubernetesNameSpace, string podName)
        {
            try
            {

                V1PodList podList = await _client.ListNamespacedPodAsync(kubernetesNameSpace.Value);
                var myPod = podList.Items.FirstOrDefault(x => x.Name() == podName);
                if (myPod is null)
                {
                    throw new Exception($"Pod: {podName} not found in cluster");
                }

                var response = await _client.ReadNamespacedPodLogWithHttpMessagesAsync(
                    myPod.Metadata.Name,
                    myPod.Metadata.NamespaceProperty, 
                    follow: false).ConfigureAwait(false);

                //_logger.LogInfo($"Log stream acquired for pod {podName}");

                string log = "";
                using (var sr = new StreamReader(response.Body))
                {
                    string tmp = "";
                    do
                    {
                        tmp = await sr.ReadLineAsync();
                        if (!string.IsNullOrEmpty(tmp))
                        {
                            //_logger.LogInfo($"Logline :  {tmp}");
                            log += $"{tmp}{Environment.NewLine}";
                        }
                        else
                        {
                            break;
                        }
                    }
                    while (!sr.EndOfStream);
                }



                PodLogDto podLog = new PodLogDto()
                {
                    PodName = podName,
                    Log = log,
                };
                _logger.LogInfo($"Log content acquired from pod :  {podName}");
                return podLog;

            }
            catch (System.Exception e)
            {
                _logger.LogError($"Failed to retrieve log for pod: {podName} in namespace: {kubernetesNameSpace.Value}".AddException(e));
                return null;
            }

        }







    }
}