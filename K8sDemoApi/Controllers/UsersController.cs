using System.Collections.Generic;
using System.Threading.Tasks;
using K8sBackendShared.Data;
using K8sBackendShared.Entities;
using K8sBackendShared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K8sDemoApi.Controllers
{
    public class UsersController:BaseApiController
    {

        private readonly DataContext _context;
        public UsersController(DataContext context, ILogger logger):base(logger)
        {
            _context = context;
        } 

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUserEntity>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUserEntity>> GetUser(int id)
        {
            return await _context.Users.FindAsync(id);
        }

    }
}