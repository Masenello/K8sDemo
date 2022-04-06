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
    public class DemoController :BaseApiController
    {
        //Test communication with API
        [HttpGet]
        public String Get()
        {
            return "Hello World";
        }

        //Test communication between API and Rabbit
        [HttpPost("SendTestRabbitMessage")]
        public async Task<ActionResult> SendTestRabbitMessage()
        {
            using (var bus = RabbitHutch.CreateBus(NetworkSettings.RabbitHostResolver())) 
            {
                await bus.PubSub.PublishAsync(new TestMessage { Text = "Test message content" });
                Console.WriteLine("Message published!");
                return Ok();
            }
        }
    }

}
