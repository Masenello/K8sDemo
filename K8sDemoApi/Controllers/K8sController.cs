using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Messages;
using K8sBackendShared.Settings;
using K8sCore.DTOs;
using K8sCore.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace K8sDemoApi.Controllers
{

    public class K8sController :BaseApiController
    {

        private readonly IK8s _K8sController;
        public K8sController(ILogger logger, IK8s k8SController):base(logger)
        {
            _K8sController = k8SController;
        }

        [Authorize]
        [HttpGet ("GetPodInfo")]
        public Task<List<PodInfoDto>> GetPodInfo()
        {
            return _K8sController.GetPodInfo(K8sBackendShared.K8s.K8sNamespace.defaultNamespace);
        }

        
        //[Authorize]
        [HttpGet ("GetPodLog/{podName}")]
        public Task<PodLogDto> GetPodLog(string podName)
        {
            _logger.LogInfo($"Received log request for pod: {podName}");
            return _K8sController.GetPodLog(K8sBackendShared.K8s.K8sNamespace.defaultNamespace,podName);
        }
    }

}
