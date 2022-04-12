using System;
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
            return new LogMessage()
            {
                Message = message, 
                MessageType = messageType, 
                Program = System.Diagnostics.Process.GetCurrentProcess().ProcessName
            };
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