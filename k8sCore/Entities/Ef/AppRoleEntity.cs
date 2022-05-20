using System.Collections.Generic;
using K8sCore.Enums;

namespace K8sCore.Entities.Ef
{
    public class AppRoleEntity:BaseEfEntity
    {
        public RolesEnum Role { get; set; }

        public virtual ICollection<AppUserRoleMapperEntity> UserRoles { get; set; }

    }
}