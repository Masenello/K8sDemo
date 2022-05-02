using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using k8sCore.Enums;

namespace k8sCore.Entities
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