using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using K8sCore.DTOs;
using K8sCore.Entities.Ef;
using K8sCore.Interfaces.Ef;
using K8sData.Data;
using Microsoft.EntityFrameworkCore;

namespace K8sData.Repository
{
    public class UserRepository : GenericEfRepository<AppUserEntity>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {

        }

        public async Task<int> RegisterUserAsync(RegisterDto registerDto)
        {
            using var hmac = new HMACSHA512();
            var user = new AppUserEntity
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Add(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task<UserDto> LoginUserAsync(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.UserName == loginDto.Username.ToLower());
            if (user is null) return null;
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i=0; i< computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return null;
            }

            return await GetUserByIdAsync(user.Id);

        }

        public async Task<List<UserDto>> GetUsersAsync()
        {
            List<UserDto> userList = new List<UserDto>();
            foreach (var user in await _context.Users.ToListAsync())
            {
                userList.Add(new UserDto()
                {
                    Username = user.UserName,
                });
            }
            return userList;
        }


        public async Task DeleteUserAsync(string username)
        {
            var targetUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == username);
            if (targetUser is null) throw new System.Exception($"User with username: {username} not found in database");
            _context.Users.Remove(targetUser);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckIfUserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var targetUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (targetUser is null) return null;
            return new UserDto(){
                Username = targetUser.UserName,
            };
        }




    }
}