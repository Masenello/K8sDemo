using System;
using K8sBackendShared.Messages;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;

namespace K8sDemoWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("K8sDemoWorker Started!");

            var services = new ServiceCollection();
            using (var bus = RabbitHutch.CreateBus("host=host.docker.internal")) 
            {
                bus.PubSub.Subscribe<TestMessage>("test", HandleTextMessage);
                Console.WriteLine("Listening for messages. Hit <return> to quit.");
                Console.ReadLine();
            }

        }

        static void HandleTextMessage(TestMessage textMessage) 
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"{DateTime.Now.ToString()} Got message: {textMessage.Text}");
            Console.ResetColor();
        }
    }
}
