using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Messages;
using K8sBackendShared.Settings;
using k8sCore.DTOs;
using k8sCore.Entities;
using k8sCore.Enums;
using K8sCore.Messages;
using K8sData.Data;
using Microsoft.AspNetCore.Authorization;
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
        
        [Authorize]
        [HttpPost("RequestNewJob")]
        public async Task<ActionResult<RequestNewJobCreationResultMessage>> RequestNewJob(RequestNewJobCreationMessage requestFromClient)
        {
            //Retrieve user entity from database
            AppUserEntity jobUser =await _context.Users.FirstOrDefaultAsync(x=>x.UserName== requestFromClient.User);
            if (jobUser is null)
            {
                return BadRequest($"User:{requestFromClient.User} not found on database");
            }

            //Add new job in Jobs database table with status INSERTED
            JobEntity newJob = new JobEntity();
            newJob.CreationDate = DateTime.Now;
            newJob.Descritpion= requestFromClient.RequestedJobType.ToString();
            newJob.Status = JobStatus.created;
            newJob.UserId = jobUser.Id;
            newJob.Type = requestFromClient.RequestedJobType;
            _context.Jobs.Add(newJob);
            await _context.SaveChangesAsync();
            _logger.LogInfo($"Job: {newJob.Id} of type: {newJob.Descritpion} created by user: {jobUser.UserName} inserted in database");

            //Reply to Client
            RequestNewJobCreationResultMessage newJobResult = new RequestNewJobCreationResultMessage();
            newJobResult.CreationTime = DateTime.Now;
            newJobResult.JobId = newJob.Id;
            newJobResult.User = jobUser.UserName;
            newJobResult.JobType = newJob.Type;
            newJobResult.JobStatus = newJob.Status;
            newJobResult.UserMessage = "";
            return Ok(newJobResult);

        }

        [Authorize]
        [HttpGet("GetUserPendingJobs/{username}")]
        public async Task<ActionResult<List<JobStatusDto>>> GetUserPendingJobs(string username)
        {
            List<JobStatusDto> userPendingJobs = new List<JobStatusDto>();

            foreach (var job in await _context.Jobs.Where(x=>x.User.UserName == username 
            && (x.Status == JobStatus.created 
            || x.Status == JobStatus.assigned 
            ||x.Status == JobStatus.running)).ToListAsync())
            {
                JobStatusDto jobStatus = new JobStatusDto()
                {
                    JobId = job.Id,
                    JobType = job.Type,
                    Status = job.Status,
                    User = username,
                    ProgressPercentage = 0,
                    UserMessage="",
                };
                userPendingJobs.Add(jobStatus);
            }
            return userPendingJobs;
        }

        [Authorize]
        [HttpGet("GetUserJobs/{username}")]
        public async Task<ActionResult<List<JobDto>>> GetUserJobs(string username)
        {
            List<JobDto> userPendingJobs = new List<JobDto>();

            foreach (var job in await _context.Jobs.Where(x=>x.User.UserName == username).ToListAsync())
            {
                JobDto jobDto = new JobDto(job);
                userPendingJobs.Add(jobDto);
            }
            return userPendingJobs;
        }
    }

}
