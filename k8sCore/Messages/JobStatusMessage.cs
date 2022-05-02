using System;
using K8sCore.Entities;
using K8sCore.Enums;

namespace K8sCore.Messages
{
    public class JobStatusMessage
    {
        public int JobId { get; set; }
        public JobType StatusJobType { get; set; }
        public JobStatus Status { get; set; }
        public string User { get; set; }
        public double ProgressPercentage { get; set; }
        public string UserMessage { get; set; }
        public string WorkerId { get; set; }

        public JobStatusMessage()
        {

        }

        public JobStatusMessage(JobEntity jobEntity, string userMessage = "", double progressPercentage = -1)
        {
            JobId = jobEntity.Id;
            StatusJobType = jobEntity.Type;
            User = jobEntity.User.UserName;
            UserMessage = userMessage;
            WorkerId = jobEntity.WorkerId;
            Status = jobEntity.Status;

            if (progressPercentage != -1)
            {
                ProgressPercentage = progressPercentage;
            }
            else
            {
                //Automatic percentage
                switch (Status)
                {
                    case JobStatus.completed:
                    case JobStatus.error:
                        ProgressPercentage = 100.0;
                        break;
                    case JobStatus.assigned:
                    case JobStatus.created:
                        ProgressPercentage = 0.0;
                        break;
                    case JobStatus.running:
                        break;
                    default:
                        throw new Exception($"Unknown job status:{Status}");
                }
            }


        }

    }
}