using System;
using K8sBackendShared.Enums;

namespace K8sBackendShared.Messages
{
    public class LogMessage
    {
        public string Program {get; set;}
        public LogType MessageType { get; set; } 
        public string Message { get; set; } 

        public override string ToString()
        {
            return ($"{DateTime.Now.Day.ToString("00")}/{DateTime.Now.Month.ToString("00")}/{DateTime.Now.Year.ToString("00")} {DateTime.Now.Hour.ToString("00")}:{DateTime.Now.Minute.ToString("00")}:{DateTime.Now.Second.ToString("00")}.{DateTime.Now.Millisecond.ToString("000")}|{this.Program}|{this.MessageType}|{this.Message}");
        }

    }
}