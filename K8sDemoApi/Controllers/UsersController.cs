using System.Collections.Generic;
using System.Threading.Tasks;
using K8sBackendShared.Interfaces;
using K8sCore.DTOs;
using K8sCore.Entities;
using K8sCore.Entities.Ef;
using K8sCore.Interfaces.Ef;
using K8sData.Data;
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

        [HttpPost("DeleteUser{username}")]
        public async Task<ActionResult> DeleteUser(string username)
        {
            await _userRepo.DeleteUserAsync(username);
            _logger.LogInfo($"User: {username} deleted");
            return Ok($"User: {username} deleted");
        }

        [HttpPost("AddRoleToUSer{username},{rolename}")]
        public async Task<ActionResult> AddRoleToUSer(string username, string rolename)
        {
            await _userRepo.AddRoleToUser(rolename, username);
            _logger.LogInfo($"Role: {rolename} assigned to user:{username}");
            return Ok($"Role: {rolename} added to user: {username}");
        }

        [HttpPost("RemoveRoleFromUser{username},{rolename}")]
        public async Task<ActionResult> RemoveRoleFromUser(string username, string rolename)
        {
            await _userRepo.RemoveRoleFromUser(rolename, username);
            _logger.LogInfo($"Role: {rolename} removed from user:{username}");
            return Ok($"Role: {rolename} removed from user: {username}");
        }

        [HttpPost("CreateRole{rolename}")]
        public async Task<ActionResult> CreateRole(string rolename)
        {
            await _userRepo.CreateRole(rolename);
            _logger.LogInfo($"Role: {rolename} created");
            return Ok($"Role: {rolename} created");
        }

        [HttpGet("GetRoles")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetRoles()
        {
            return Ok(await _userRepo.GetRolesAsync());
        }
    }
}