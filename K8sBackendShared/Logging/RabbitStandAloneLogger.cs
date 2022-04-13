using System;
using EasyNetQ;
using K8sBackendShared.Enums;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Messages;

namespace K8sBackendShared.Logging
{
    public class RabbitStandAloneLogger : ILogger
    {

        protected readonly IBus _rabbitBus;
        
        public RabbitStandAloneLogger(IBus rabbitBus)
        {
            _rabbitBus = rabbitBus;
        }

        private void SendLogMessage (LogMessage logmessage)
        {
            _rabbitBus.PubSub.Publish<LogMessage>(logmessage);
        }

        public void LogError(string errorMessage)
        {
            SendLogMessage(LoggingUtilities.BuildLogMessage(errorMessage, LogType.Error));
        }

        public void LogError(string errorMessage, Exception e)
        {
            SendLogMessage(LoggingUtilities.BuildLogMessage(
                    $"{errorMessage.AddException(e)}", LogType.Error));
        }

        public void LogInfo(string infoMessage)
        {
            SendLogMessage(LoggingUtilities.BuildLogMessage(infoMessage, LogType.Info));
        }

        public void LogWarning(string warningMessage)
        {
            SendLogMessage(LoggingUtilities.BuildLogMessage(warningMessage, LogType.Warning));
        }

        public void LogDebug(string debugMessage)
        {
            SendLogMessage(LoggingUtilities.BuildLogMessage(debugMessage, LogType.Debug));
        }
    }
}