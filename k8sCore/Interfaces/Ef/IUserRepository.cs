using System.Collections.Generic;
using System.Threading.Tasks;
using K8sCore.DTOs;


namespace K8sCore.Interfaces.Ef
{
    public interface IUserRepository
    {
        public  Task<int> RegisterUserAsync(RegisterDto registerDto);

        public  Task<List<UserDto>> GetUsersAsync();
        public  Task DeleteUserAsync(string username);
        public Task<bool> CheckIfUserExistsAsync(string username);
        public Task<UserDto> GetUserByIdAsync(int id);

        public Task<UserDto> LoginUserAsync(LoginDto loginDto);
    }
}