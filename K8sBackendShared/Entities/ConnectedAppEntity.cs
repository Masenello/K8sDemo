using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K8sBackendShared.Enums;

namespace K8sBackendShared.Entities
{
    public class ConnectedAppEntity
    {
        public int Id { get; set; }
        [Required]
        public string ConnectionId { get; set; }
        public virtual AppUserEntity User {get; set;}
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId {get; set;}  
        public ApplicationType AppType { get; set; }
    }
}