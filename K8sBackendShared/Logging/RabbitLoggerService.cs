using System;
using K8sBackendShared.Interfaces;
using k8sCore.Enums;
using K8sCore.Messages;

namespace K8sBackendShared.Logging
{
    public class RabbitLoggerService : ILogger
    {
        private readonly IRabbitConnector _rabbitConnector;

        public RabbitLoggerService(IRabbitConnector rabbitConnector)
        {
            _rabbitConnector = rabbitConnector;
        }

        private void SendLogMessage (LogMessage logmessage)
        {
            _rabbitConnector.Publish<LogMessage>(logmessage);
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