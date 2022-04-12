using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using K8sBackendShared.Enums;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Messages;
using K8sBackendShared.RabbitConnector;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;

namespace K8sDemoLogManager.Services
{
    public class RabbitLogReceiverService
    {
        private readonly  Logger _nlogger; 
        private readonly  IRabbitConnector _rabbitConnector; 

        public RabbitLogReceiverService(IRabbitConnector rabbitConnector):base()
        {
            _rabbitConnector = rabbitConnector;
            _nlogger = NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
            _rabbitConnector.Subscribe<LogMessage>(HandleLogMessage);
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