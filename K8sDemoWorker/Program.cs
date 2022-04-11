using System;
using K8sBackendShared.Messages;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using K8sBackendShared.Settings;
using System.Threading;
using K8sBackendShared.Workers;
using K8sBackendShared.Data;
using K8sBackendShared.Entities;
using System.Linq;
using K8sBackendShared.Enums;
using Microsoft.EntityFrameworkCore;
using K8sDemoWorker.Jobs;

namespace K8sDemoWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("K8sDemoWorker Started!");
            Console.WriteLine($"Rabbit Host: {NetworkSettings.RabbitHostResolver()}");

            var services = new ServiceCollection();

            var cyclicWorker = new CyclicWorker(3000, new TestJob());



            //Keep main running
            while(true)
            {
                Thread.Sleep(100);
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
