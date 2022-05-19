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
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
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
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task<UserDto> LoginUserAsync(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());
            if (user is null) return null;
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < computedHash.Length; i++)
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
                    Mail = user.Mail,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Department = user.Department,
                    Roles = await GetUserRolesAsync(user.UserName),
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
            var targetUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == username.ToLower());
            if (targetUser is null)
            {
                return false;
            }
            return true;
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var targetUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (targetUser is null) return null;
            return new UserDto()
            {
                Username = targetUser.UserName,
            };
        }

        public async Task CreateRole(string roleName)
        {
            await _context.Roles.AddAsync(new AppRoleEntity()
            {
                Role = roleName.ToLower(),
            });
            await _context.SaveChangesAsync();
        }

        public async Task AddRoleToUser(string roleName, string userName)
        {
            var user = await GetUserByNameAsync(userName);
            var role = await GetRoleByNameAsync(roleName);

            await _context.UserRolesMapper.AddAsync(new AppUserRoleMapperEntity()
            {
                UserId = user.Id,
                RoleId = role.Id,
            });
            await _context.SaveChangesAsync();
        }


        public async Task RemoveRoleFromUser(string roleName, string userName)
        {

            var roleMap = await GetRoleMapAsync(roleName, userName);
            if (roleMap is null) throw new System.Exception($"User: {userName} does not have role: {roleName}");

            _context.UserRolesMapper.Remove(roleMap);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RoleDto>> GetRolesAsync()
        {
            List<RoleDto> roleList = new List<RoleDto>();
            foreach (var role in await _context.Roles.ToListAsync())
            {
                roleList.Add(new RoleDto()
                {
                    RoleName = role.Role,
                });

            }
            return roleList;
        }



        private async Task<AppUserEntity> GetUserByNameAsync(string username)
        {
            var targetUser = await _context.Users.Include(m=>m.UserRoles).ThenInclude(r=>r.Role).FirstOrDefaultAsync(x => x.UserName == username.ToLower());
            if (targetUser is null) throw new System.Exception($"User: {username} not found in database");
            return targetUser;
        }


        private async Task<AppRoleEntity> GetRoleByNameAsync(string roleName)
        {
            var targetRole = await _context.Roles.FirstOrDefaultAsync(x => x.Role == roleName.ToLower());
            if (targetRole is null) throw new System.Exception($"Role: {roleName} not found in database");
            return targetRole;
        }

        private async Task<AppUserRoleMapperEntity> GetRoleMapAsync(string roleName, string username)
        {
            var roleMap = await _context.UserRolesMapper.Include(u => u.User).Include(r => r.Role).FirstOrDefaultAsync(x => x.User.UserName == username.ToLower() && x.Role.Role == roleName.ToLower());
            return roleMap;
        }

        private async Task<List<RoleDto>> GetUserRolesAsync(string username)
        {
            var targetUser = await GetUserByNameAsync(username);
            List<RoleDto> userRolesList = new List<RoleDto>();
            foreach (var map in targetUser.UserRoles)
            {
                userRolesList.Add(new RoleDto(){
                    RoleName = map.Role.Role,
                });
            }
            return userRolesList;
        }

    }
}