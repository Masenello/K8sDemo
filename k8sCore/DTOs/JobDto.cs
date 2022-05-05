using System;
using K8sCore.Entities;
using K8sCore.Entities.Mongo;
using K8sCore.Enums;

namespace K8sCore.DTOs
{
    public class JobDto
    {
        public string JobId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public JobStatus Status { get; set; }
        public string Description { get; set; }
        public string Errors { get; set; }
        public JobType JobType { get; set; }
        public string WorkerId { get; set; }

        public JobDto()
        {
            
        }

        public JobDto(JobEntity jobEntity)
        {
            JobId = jobEntity.Id;
            CreationDate = jobEntity.CreationDate;
            AssignmentDate = jobEntity.AssignmentDate;
            StartDate = jobEntity.StartDate;
            EndDate = jobEntity.EndDate;
            Status = jobEntity.Status;
            Description = jobEntity.Descritpion;
            Errors = jobEntity.Errors;
            JobType = jobEntity.Type;
            WorkerId = jobEntity.WorkerId;

        }
    }

}
