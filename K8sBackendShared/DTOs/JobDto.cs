using System;
using K8sBackendShared.Entities;
using K8sBackendShared.Enums;

namespace K8sBackendShared.DTOs
{
    public class JobDto
    {
        public int JobId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public JobStatus Status { get; set; }
        public string Description { get; set; }
        public string Errors { get; set; }
        public JobType JobType { get; set; }

        public JobDto()
        {
            
        }

        public JobDto(JobEntity jobEntity)
        {
            JobId = jobEntity.Id;
            CreationDate = jobEntity.CreationDate;
            StartDate = jobEntity.StartDate;
            EndDate = jobEntity.EndDate;
            Status = jobEntity.Status;
            Description = jobEntity.Descritpion;
            Errors = jobEntity.Errors;
            JobType = jobEntity.Type;

        }
    }

}