using System.Collections.Generic;
using System.Threading.Tasks;
using K8sBackendShared.Data;
using K8sBackendShared.Entities;
using K8sBackendShared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K8sDemoApi.Controllers
{
    public class DemoDatabaseController:BaseApiController
    {
        private readonly DataContext _context;

        public DemoDatabaseController(DataContext context, ILogger logger):base(logger)
        {
            _context = context;
        }

        [HttpGet("GetTestData")]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<TestDataEntity>>> GetTestData()
        {
            List<TestDataEntity> data = new List<TestDataEntity>();

            return await _context.Data.ToListAsync();
        }


    }
}