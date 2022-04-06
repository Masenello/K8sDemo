using System;
using K8sBackendShared.Messages;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using K8sBackendShared.Settings;
using System.Threading;
using K8sBackendShared.Workers;
using K8sBackendShared.Data;
using K8sBackendShared.Entities;
using System.Linq;
using K8sBackendShared.Enums;
using Microsoft.EntityFrameworkCore;

namespace K8sDemoWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("K8sDemoWorker Started!");
            Console.WriteLine($"Rabbit Host: {NetworkSettings.RabbitHostResolver()}");
            //Console.WriteLine(NetworkSettings.DatabaseConnectionStringResolver());

            var services = new ServiceCollection();
            //using (var bus = RabbitHutch.CreateBus(NetworkSettings.RabbitHostResolver())) 
            //{
            //    bus.PubSub.Subscribe<TestMessage>("test", HandleTextMessage);
            //    Console.WriteLine("Listening for messages. Hit <return> to quit.");
            //    Console.ReadLine();
            //}

            var cyclicWorker = new CyclicWorker(3000, JobManagementAction);



            //Keep main running
            Console.ReadLine();
        }


        private static void JobManagementAction()
        {
            try 
            {
                Console.WriteLine($"{DateTime.Now}: Searching for jobs to process ...");
                using (var _context = (new DataContextFactory()).CreateDbContext(null))
                {
                    //Get the oldest job found in CREATED status
                    TestJob targetJob = _context.Jobs.Where(x=>x.Status == JobStatus.created).OrderBy(y=>y.CreationDate).Include(u=>u.User).FirstOrDefault();
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
                        ReportJobProgress(jobStatus);

                        Thread.Sleep(3000);
                        jobStatus.ProgressPercentage = 33.3;
                        ReportJobProgress(jobStatus);

                        Thread.Sleep(3000);
                        jobStatus.ProgressPercentage = 66.6;
                        ReportJobProgress(jobStatus);

                        Thread.Sleep(3000);
                        jobStatus.ProgressPercentage = 100.0;
                        ReportJobProgress(jobStatus);


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

        private static void ReportJobProgress(JobStatusMessage jobStatus)
        {
            using (var bus = RabbitHutch.CreateBus(NetworkSettings.RabbitHostResolver())) 
            {
                bus.PubSub.Publish(jobStatus);
            }
        }

        static void HandleTextMessage(TestMessage textMessage) 
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"{DateTime.Now.ToString()} Got message: {textMessage.Text}");
            Console.ResetColor();
        }
    }
}
