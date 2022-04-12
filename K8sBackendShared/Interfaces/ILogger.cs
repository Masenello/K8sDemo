using System;

namespace K8sBackendShared.Interfaces
{
    public interface ILogger
    {
        void LogInfo(string infoMessage);
        void LogWarning(string warningMessage);
        void LogError(string errorMessage);
        void LogError(string errorMessage, Exception e);
        void LogDebug(string debugMessage);
    }
}