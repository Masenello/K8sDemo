using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using K8sBackendShared.Enums;
using K8sBackendShared.Messages;
using K8sBackendShared.RabbitConnector;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;

namespace K8sDemoLogManager.Services
{
    public class RabbitConnectorServiceDemoLogManager:RabbitConnectorService
    {
        private readonly  Logger _nlogger; 

        public RabbitConnectorServiceDemoLogManager():base()
        {
            _nlogger = NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
        }



        public override async void Subscribe()
        {
            await  _rabbitBus.PubSub.SubscribeAsync<LogMessage>("",HandleLogMessage);  
        }

        private async void HandleLogMessage(LogMessage msg)
        {
                        switch(msg.MessageType)
            {
                case LogType.Error:
                    _nlogger.Log(LogLevel.Error,  msg.ToString());
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogType.Debug:
                    _nlogger.Log(LogLevel.Debug,  msg.ToString());
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case LogType.Info:
                    _nlogger.Log(LogLevel.Info,  msg.ToString());
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogType.Warning:
                    _nlogger.Log(LogLevel.Warn,  msg.ToString());
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
            }
            Console.WriteLine(msg.ToString());

            


            await Task.Delay(0);
        }


                
    }
}