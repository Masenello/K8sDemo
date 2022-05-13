using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Messages;
using K8sBackendShared.Settings;
using K8sCore.Messages;
using Microsoft.AspNetCore.Mvc;

namespace K8sDemoApi.Controllers
{


    public class DemoController :BaseApiController
    {
        private readonly IRabbitConnector _rabbitConnector;
        public DemoController(ILogger logger, IRabbitConnector rabbitConnector):base(logger)
        {
            _rabbitConnector = rabbitConnector;
        }

        //Test communication with API
        [HttpGet]
        public String Get()
        {
            return "Hello World";
        }

        //Test communication between API and Rabbit
        [HttpPost("SendTestRabbitMessage")]
        public ActionResult SendTestRabbitMessage()
        {
            _rabbitConnector.Publish(new TestMessage { Text = "Test message content" });
            _logger.LogInfo("Message published!");
            return Ok();

        }
    }

}
