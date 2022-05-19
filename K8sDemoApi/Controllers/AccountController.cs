using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using K8sBackendShared.Interfaces;
using K8sCore.DTOs;
using K8sCore.Entities.Ef;
using K8sCore.Interfaces.Ef;
using K8sData.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K8sDemoApi.Controllers
{
    
    public class AccountController:BaseApiController
    {
        private readonly IUserRepository _userRepo;
        private readonly ITokenService _tokenService;
        

        public AccountController(IUserRepository userRepo, ITokenService tokenService, ILogger logger):base(logger)
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            if (await _userRepo.CheckIfUserExistsAsync(registerDto.Username)) return BadRequest($"Username: {registerDto.Username} is taken");

            await _userRepo.RegisterUserAsync(registerDto);

            return Ok($"Username: {registerDto.Username} created");
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var targetUser = await _userRepo.LoginUserAsync(loginDto);
            if (targetUser is null) return BadRequest("Login failed, wrong user or password");

            targetUser.Token = _tokenService.CreateToken(targetUser.Username);

            return Ok(targetUser);
            
        }


    }
}