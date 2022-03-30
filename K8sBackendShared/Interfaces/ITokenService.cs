using K8sBackendShared.Entities;

namespace K8sBackendShared.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}