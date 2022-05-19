using System.Collections.Generic;

namespace K8sCore.DTOs
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public List<string> Roles { get; set; }
        public string Mail { get; set; }

    }
}