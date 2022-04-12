using K8sBackendShared.Enums;

namespace K8sBackendShared.Messages
{
    public class LogMessage
    {
        public string Program {get; set;}
        public LogType MessageType { get; set; } 
        public string Message { get; set; } 
    }
}