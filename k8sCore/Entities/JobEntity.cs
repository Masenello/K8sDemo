using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K8sCore.Enums;

namespace K8sCore.Entities
{
    public class JobEntity:BaseEntity

    {
        public int Id { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        public DateTime AssignmentDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Required]
        public string Descritpion { get; set; }
        [Required]
        public JobStatus Status { get; set; }
        public string Errors { get; set; }
        public virtual AppUserEntity User {get; set;}
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId {get; set;}

        public JobType Type {get; set;}

        public string WorkerId {get; set;}
        public int TimeOutSeconds { get; set; }
        
        


        public string GenerateJobDescriptor()
        {
            return ($"Job: {Id} of type: {Type} user: {User.UserName} worker: {WorkerId} status: {Status}");
        }
        
    }
}