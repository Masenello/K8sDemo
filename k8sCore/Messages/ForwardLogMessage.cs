using System;
using k8sCore.Enums;

namespace K8sCore.Messages
{
    public class ForwardLogMessage
    {
        public string Program {get; set;}
        public LogType MessageType { get; set; } 
        public string Message { get; set; } 


        public ForwardLogMessage(){}
        
        public ForwardLogMessage(LogMessage originalLog)
        {
            Program =originalLog.Program;
            MessageType = originalLog.MessageType;
            Message = originalLog.ToString();
        }
    }
}