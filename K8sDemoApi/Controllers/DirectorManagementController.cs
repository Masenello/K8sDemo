using System.Threading.Tasks;
using K8sBackendShared.Interfaces;
using K8sCore.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace K8sDemoApi.Controllers
{
    public class DirectorManagementController : BaseApiController
    {
        private readonly IRabbitConnector _rabbitConnector;

        public DirectorManagementController(IRabbitConnector rabbitConnector, ILogger logger) : base(logger)
        {
            _rabbitConnector = rabbitConnector;
        }

        [Authorize]
        [HttpPost("SetDirectorParameters")]
        public ActionResult SetDirectorParameters(SetDirectorParametersMessage setParametersMsg)
        {
            _logger.LogInfo($"Received new set of parameters for director: {setParametersMsg.ToString()}");
            _rabbitConnector.Publish<SetDirectorParametersMessage>(setParametersMsg);
            return Ok();

        }


    }
}