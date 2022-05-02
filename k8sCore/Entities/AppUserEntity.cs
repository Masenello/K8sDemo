using System.Collections.Generic;

namespace k8sCore.Entities
{
    public class AppUserEntity
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public virtual ICollection<JobEntity> UserJobs { get; set; }
    }
}