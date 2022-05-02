using k8sCore.Entities;

namespace K8sBackendShared.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUserEntity user);
    }
}