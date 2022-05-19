using System.Collections.Generic;
using K8sCore.Entities.Ef;

namespace K8sCore.DTOs
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public List<RoleDto> Roles { get; set; }
        public string Mail { get; set; }

        public UserDto()
        {

        }

        public UserDto(AppUserEntity userEntity, List<RoleDto> roles)
        {
            Username = userEntity.UserName;
            Mail = userEntity.Mail;
            FirstName = userEntity.FirstName;
            LastName = userEntity.LastName;
            Department = userEntity.Department;
            Roles = roles;
        }

    }
}