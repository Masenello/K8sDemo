using System;
using K8sCore.Enums;

namespace K8sCore.DTOs
{
    public class PodLogDto
    {
        public string PodName { get; set; }
        public string Log { get; set; }    

        public DateTime LogsStartDate  { get; set; }  
    }
}