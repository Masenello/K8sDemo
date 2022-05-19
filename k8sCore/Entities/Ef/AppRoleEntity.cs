using System.Collections.Generic;

namespace K8sCore.Entities.Ef
{
    public class AppRoleEntity:BaseEfEntity
    {
        public string Role { get; set; }

        public virtual ICollection<AppUserRoleMapperEntity> UserRoles { get; set; }

    }
}