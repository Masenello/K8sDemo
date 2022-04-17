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
                _jobToProcessId = (int)workerParameters;
                using (var _context = (new DataContextFactory()).CreateDbContext(null))
                {
                    JobEntity targetJob = _context.Jobs.Where(x=>x.Id == _jobToProcessId).Include(u=>u.User).FirstOrDefault();
                    if (targetJob is null) throw new Exception($"Job with Id: {_jobToProcessId} not found on database");
                    if (targetJob.Status != JobStatus.assigned) throw new Exception($"Job with Id: {_jobToProcessId} is in status: {targetJob.Status}, expected status: {JobStatus.assigned}");


                    try 
                    {
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
                        //Set job to error status
                        _logger.LogError($"{targetJob.GenerateJobDescriptor()} in error", e);
                        targetJob.Status = JobStatus.error;
                        targetJob.EndDate = DateTime.Now;
                        await _context.SaveChangesAsync();
                        throw;
                    }
                }
            

        }
    }
}