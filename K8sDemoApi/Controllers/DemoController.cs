using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using K8sBackendShared.Messages;
using K8sBackendShared.Settings;
using Microsoft.AspNetCore.Mvc;

namespace K8sDemoApi.Controllers
{

    [ApiController]
    [Route("demo")]
    public class DemoController : ControllerBase
    {
        [HttpGet]
        public String Get()
        {
            return "Hello World";
        }

        [HttpPost("SendTestRabbitMessage")]
        public async Task<ActionResult> SendTestRabbitMessage()
        {
            using (var bus = RabbitHutch.CreateBus(NetworkSettings.RabbitHostResolver())) 
            {
                await bus.PubSub.PublishAsync(new TestMessage { Text = "Test message content" });
                Console.WriteLine("Message published!");
                return Ok("Message published");
            }
        }
    }

}
