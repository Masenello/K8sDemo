using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K8sBackendShared.Enums;

namespace K8sBackendShared.Entities
{
    public class TestJobEntity

    {
        public int Id { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
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
        
    }
}