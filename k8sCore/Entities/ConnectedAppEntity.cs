using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K8sCore.Enums;

namespace K8sCore.Entities
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