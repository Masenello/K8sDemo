using System;
using System.Reflection;
using K8sBackendShared.Enums;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Messages;
using K8sBackendShared.RabbitConnector;

namespace K8sBackendShared.Logging
{
    public class RabbitLoggerService : ILogger
    {
        private readonly IRabbitPublisher _rabbitSender;

        public RabbitLoggerService(IRabbitPublisher rabbitSender)
        {
            _rabbitSender = rabbitSender;
        }

        private LogMessage BuildLogMessage(string message, LogType messageType)
        {
            var logMessage = new LogMessage()
            {
                Message = message, 
                MessageType = messageType, 
                Program = Assembly.GetEntryAssembly().GetName().Name
            };
            WriteToConsole(logMessage);
            return logMessage;
        }

        private void WriteToConsole(LogMessage logmessage)
        {
            
            switch(logmessage.MessageType)
            {
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogType.Debug:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case LogType.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
            }
            Console.WriteLine(logmessage.ToString());
        }

        private void SendLogMessage (LogMessage logmessage)
        {
            _rabbitSender.Publish<LogMessage>(logmessage);
        }

        public void LogError(string errorMessage)
        {
            SendLogMessage(BuildLogMessage(errorMessage, LogType.Error));
        }

        public void LogError(string errorMessage, Exception e)
        {
            SendLogMessage(BuildLogMessage(
                    $"{errorMessage.AddException(e)}", LogType.Error));
        }

        public void LogInfo(string infoMessage)
        {
            SendLogMessage(BuildLogMessage(infoMessage, LogType.Info));
        }

        public void LogWarning(string warningMessage)
        {
            SendLogMessage(BuildLogMessage(warningMessage, LogType.Warning));
        }

        public void LogDebug(string debugMessage)
        {
            SendLogMessage(BuildLogMessage(debugMessage, LogType.Debug));
        }
    }
}