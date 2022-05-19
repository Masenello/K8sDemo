using System.Collections.Generic;

namespace K8sCore.Entities.Ef
{
    public class AppUserEntity : BaseEfEntity
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string Mail { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public virtual ICollection<AppUserRoleMapperEntity> UserRoles { get; set; }

    }
}