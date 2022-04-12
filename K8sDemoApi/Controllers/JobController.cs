using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using K8sBackendShared.Data;
using K8sBackendShared.Entities;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Messages;
using K8sBackendShared.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K8sDemoApi.Controllers
{

    public class JobController :BaseApiController
    {
        private readonly DataContext _context;

        public JobController(DataContext context, ILogger logger):base(logger)
        {
            _context = context;
        }
        
        [HttpPost("RequestNewJob")]
        public async Task<ActionResult<RequestJobMessageResult>> RequestNewJob(RequestJobMessage requestFromClient)
        {
            //Retrieve user entity from database
            AppUserEntity jobUser =await _context.Users.FirstOrDefaultAsync(x=>x.UserName== requestFromClient.User);
            if (jobUser is null)
            {
                return BadRequest($"User:{requestFromClient.User} not found on database");
            }

            //Add new job in Jobs database table with status INSERTED
            TestJobEntity newJob = new TestJobEntity();
            newJob.CreationDate = DateTime.Now;
            newJob.Descritpion= requestFromClient.RequestedJobType.ToString();
            newJob.Status = K8sBackendShared.Enums.JobStatus.created;
            newJob.UserId = jobUser.Id;
            _context.Jobs.Add(newJob);
            await _context.SaveChangesAsync();

            //Reply to Client
            RequestJobMessageResult newJobResult = new RequestJobMessageResult();
            newJobResult.CreationTime = DateTime.Now;
            newJobResult.JobId = newJob.Id;
            newJobResult.User = jobUser.UserName;
            newJobResult.UserMessage = $"Job: {newJob.Id} of type: {newJob.Descritpion} created by user: {jobUser.UserName}";
            return Ok(newJobResult);

        }
    }

}
