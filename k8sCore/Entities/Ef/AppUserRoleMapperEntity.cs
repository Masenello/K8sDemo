using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K8sCore.Entities.Ef
{
    public class AppUserRoleMapperEntity : BaseEfEntity
    {
        [Required]
        [ForeignKey(nameof(AppUserEntity))]
        public int UserId { get; set; }
        public virtual AppUserEntity User { get; set; }
        [Required]
        [ForeignKey(nameof(AppRoleEntity))]
        public int RoleId { get; set; }
        public virtual AppRoleEntity Role { get; set; }


    }
}