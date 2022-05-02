using System;
using System.Threading.Tasks;
using K8sBackendShared.Interfaces;
using k8sCore.Enums;
using K8sCore.Messages;
using NLog;

namespace K8sDemoLogManager.Services
{
    public class RabbitLogReceiverService
    {
        private readonly  Logger _nlogger; 
        private readonly  IRabbitConnector _rabbitConnector; 

        public RabbitLogReceiverService(IRabbitConnector rabbitConnector)
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
            _rabbitConnector.Publish<ForwardLogMessage>(new ForwardLogMessage(msg));
            await Task.Delay(0);
        }


                
    }
}