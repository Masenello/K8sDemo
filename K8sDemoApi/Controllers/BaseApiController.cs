using K8sBackendShared.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace K8sDemoApi.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]

    

    public class BaseApiController:ControllerBase
    {
        protected readonly ILogger _logger;

        public BaseApiController(ILogger logger)
        {
            _logger = logger;
        }
    }
}