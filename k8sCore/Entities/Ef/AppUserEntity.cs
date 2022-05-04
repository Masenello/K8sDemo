using System.Collections.Generic;

namespace K8sCore.Entities.Ef
{
    public class AppUserEntity:BaseEfEntity
    {
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}