using System.Collections.Generic;

namespace K8sBackendShared.Entities
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