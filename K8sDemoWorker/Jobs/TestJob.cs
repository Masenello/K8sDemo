using System;
using System.Linq;
using K8sBackendShared.Data;
using K8sBackendShared.Enums;
using Microsoft.EntityFrameworkCore;
using K8sBackendShared.Messages;
using System.Threading;
using K8sBackendShared.Jobs;
using K8sBackendShared.Entities;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Logging;
using System.Collections.Concurrent;
using K8sBackendShared.Utils;

namespace K8sDemoWorker.Jobs
{
    public class TestJob : AbstractWorkerJob
    {
        private int _jobToProcessId;
        public TestJob(ILogger logger):base(logger)
        {     
        }

        public async override void DoWork(object workerParameters)
        {
                ThreadedQueue<object> argsQueue = workerParameters as ThreadedQueue<object>;

                _jobToProcessId = (int)workerParameters;
                using (var _context = (new DataContextFactory()).CreateDbContext(null))
                {
                    JobEntity targetJob = _context.Jobs.Where(x=>x.Id == _jobToProcessId).Include(u=>u.User).FirstOrDefault();
                    try 
                    {
                        if (targetJob is null) throw new Exception($"Job with Id: {_jobToProcessId} not found on database");
                        if (targetJob.Status != JobStatus.assigned) throw new Exception($"Job with Id: {_jobToProcessId} is in status: {targetJob.Status}, expected status: {JobStatus.assigned}");

                        _logger.LogInfo($"{targetJob.GenerateJobDescriptor()} running");
                        targetJob.Status = JobStatus.running;
                        targetJob.StartDate = DateTime.Now;
                        _context.SaveChanges();

                        //Main Action and progess report
                        JobStatusMessage jobStatus = new JobStatusMessage();
                        jobStatus.JobId = targetJob.Id;
                        jobStatus.Status = JobStatus.running;
                        jobStatus.User = targetJob.User.UserName;
                        jobStatus.StatusJobType = targetJob.Type;
                        jobStatus.ProgressPercentage = 0.0;
                        ReportWorkProgress(jobStatus);

                        Thread.Sleep(3000);
                        jobStatus.ProgressPercentage = 33.3;
                        ReportWorkProgress(jobStatus);

                        Thread.Sleep(3000);
                        jobStatus.ProgressPercentage = 66.6;
                        ReportWorkProgress(jobStatus);

                        Thread.Sleep(3000);
                        jobStatus.ProgressPercentage = 100.0;
                        jobStatus.Status = JobStatus.completed;
                        ReportWorkProgress(jobStatus);


                        //Set job to completed status
                        _logger.LogInfo($"{targetJob.GenerateJobDescriptor()} completed");
                        targetJob.Status = JobStatus.completed;
                        targetJob.EndDate = DateTime.Now;
                        await _context.SaveChangesAsync();

                    }     
                    catch(Exception e)
                    {
                        _logger.LogError($"{targetJob.GenerateJobDescriptor()} in error", e);
                        //Set job to error status on database
                        targetJob.Status = JobStatus.error;
                        targetJob.EndDate = DateTime.Now;
                        await _context.SaveChangesAsync();
                        //Report error to status monitoring
                        JobStatusMessage jobStatus = new JobStatusMessage();
                        jobStatus.JobId = targetJob.Id;
                        jobStatus.Status = JobStatus.error;
                        jobStatus.User = targetJob.User.UserName;
                        jobStatus.StatusJobType = targetJob.Type;
                        jobStatus.ProgressPercentage = 100.0;
                        jobStatus.UserMessage = $"{targetJob.GenerateJobDescriptor()} in error".AddException(e);
                        ReportWorkProgress(jobStatus);
                    }
                }
            

        }
    }
}