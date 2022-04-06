using System;
using System.Linq;
using K8sBackendShared.Data;
using K8sBackendShared.Enums;
using Microsoft.EntityFrameworkCore;
using K8sBackendShared.Messages;
using System.Threading;
using K8sBackendShared.Jobs;
using K8sBackendShared.Entities;

namespace K8sDemoWorker.Jobs
{
    public class TestJob : AbstractWorkerJob
    {

        public override void DoWork()
        {
            try 
            {
                Console.WriteLine($"{DateTime.Now}: Searching for jobs to process ...");
                using (var _context = (new DataContextFactory()).CreateDbContext(null))
                {
                    //Get the oldest job found in CREATED status
                    TestJobEntity targetJob = _context.Jobs.Where(x=>x.Status == JobStatus.created).OrderBy(y=>y.CreationDate).Include(u=>u.User).FirstOrDefault();
                    if (targetJob != null)
                    {
                        Console.WriteLine($"Processing Job: {targetJob.Id} from user: {targetJob.User.UserName}");
                        //Set job to running status
                        targetJob.Status = JobStatus.running;
                        targetJob.StartDate = DateTime.Now;
                        _context.SaveChanges();

                        //Main Action and progess report
                        JobStatusMessage jobStatus = new JobStatusMessage();
                        jobStatus.JobId = targetJob.Id;
                        jobStatus.Status = JobStatus.running;
                        jobStatus.User = targetJob.User.UserName;
                        jobStatus.StatusJobType = (JobType)Enum.Parse(typeof(JobType), targetJob.Descritpion);
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
                        ReportWorkProgress(jobStatus);


                        //Set job to completed status
                        targetJob.Status = JobStatus.completed;
                        targetJob.EndDate = DateTime.Now;
                        _context.SaveChanges();
                        Console.WriteLine($"Job: {targetJob.Id} from user: {targetJob.User.UserName} processing completed");
                    }
                }
            }     
            catch(Exception e)
            {
                Console.WriteLine($"Eror processing job: {e.Message}");
            }
        }
    }
}