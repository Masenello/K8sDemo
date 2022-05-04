using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Messages;
using K8sBackendShared.Settings;
using K8sCore.DTOs;
using K8sCore.Entities;
using K8sCore.Entities.Ef;
using K8sCore.Entities.Mongo;
using K8sCore.Enums;
using K8sCore.Interfaces.Mongo;
using K8sCore.Messages;
using K8sData.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K8sDemoApi.Controllers
{

    public class JobController :BaseApiController
    {
        private readonly IJobRepository _jobRepo;

        

        //TODO Do with unito of work!!!
        public JobController( IJobRepository jobRepo, ILogger logger):base(logger)
        {
            _jobRepo = jobRepo;
        }
        
        [Authorize]
        [HttpPost("RequestNewJob")]
        public async Task<ActionResult<RequestNewJobCreationResultMessage>> RequestNewJob(RequestNewJobCreationMessage requestFromClient)
        {

            //Add new job in Jobs database table with status INSERTED
            JobEntity newJob = new JobEntity();
            newJob.CreationDate = DateTime.Now;
            newJob.Descritpion= requestFromClient.RequestedJobType.ToString();
            newJob.Status = JobStatus.created;
            newJob.UserName = requestFromClient.User;
            newJob.Type = requestFromClient.RequestedJobType;
            await _jobRepo.AddAsync(newJob);
            _logger.LogInfo($"Job: {newJob.Id} of type: {newJob.Descritpion} created by user: {newJob.UserName} inserted in database");

            //Reply to Client
            RequestNewJobCreationResultMessage newJobResult = new RequestNewJobCreationResultMessage();
            newJobResult.CreationTime = DateTime.Now;
            newJobResult.JobId = newJob.Id;
            newJobResult.User = newJob.UserName;
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

            foreach (var job in _jobRepo.GetOpenJobs())
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

            foreach (var job in _jobRepo.Find(x=>x.UserName == username))
            {
                JobDto jobDto = new JobDto(job);
                userPendingJobs.Add(jobDto);
            }
            return userPendingJobs;
        }
    }

}
