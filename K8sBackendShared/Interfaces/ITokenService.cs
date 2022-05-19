using K8sCore.DTOs;
using K8sCore.Entities;
using K8sCore.Entities.Ef;

namespace K8sBackendShared.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(UserDto userDto);
    }
}