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
            return Ok($"User: {username} deleted");
        }
    }
}