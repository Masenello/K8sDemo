using System.Collections.Generic;
using System.Threading.Tasks;
using K8sBackendShared.Interfaces;
using K8sCore.DTOs;
using K8sCore.Entities;
using K8sCore.Entities.Ef;
using K8sCore.Enums;
using K8sCore.Interfaces.Ef;
using K8sData.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K8sDemoApi.Controllers
{
    public class UsersController : BaseApiController
    {

        private readonly IUserRepository _userRepo;
        public UsersController(IUserRepository userRepo, ILogger logger) : base(logger)
        {
            _userRepo = userRepo;
        }


        [HttpGet("GetUsers")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            return Ok(await _userRepo.GetUsersAsync());
        }

        [Authorize(Roles = "1")]
        [HttpDelete("DeleteUser/{username}")]
        public async Task<ActionResult> DeleteUser(string username)
        {
            await _userRepo.DeleteUserAsync(username);
            _logger.LogInfo($"User: {username} deleted");
            return Ok();
        }

        [Authorize(Roles = "1")]
        [HttpPost("AddRoleToUser/{username},{role}")]
        public async Task<ActionResult> AddRoleToUser(string username, RolesEnum role)
        {
            await _userRepo.AddRoleToUser(role, username);
            _logger.LogInfo($"Role: {role} assigned to user:{username}");
            return Ok();
        }

        [Authorize(Roles = "1")]
        [HttpPost("RemoveRoleFromUser/{username},{role}")]
        public async Task<ActionResult> RemoveRoleFromUser(string username, RolesEnum role)
        {
            await _userRepo.RemoveRoleFromUser(role, username);
            _logger.LogInfo($"Role: {role} removed from user:{username}");
            return Ok();
        }

        [Authorize(Roles = "1")]
        [HttpPost("CreateRole/{rolename}")]
        public async Task<ActionResult> CreateRole(string rolename)
        {
            await _userRepo.CreateRole(rolename);
            _logger.LogInfo($"Role: {rolename} created");
            return Ok();
        }

        [Authorize(Roles = "1")]
        [HttpGet("GetRoles")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetRoles()
        {
            return Ok(await _userRepo.GetRolesAsync());
        }
    }
}