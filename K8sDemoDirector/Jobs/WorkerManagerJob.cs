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

namespace K8sDemoDirector.Jobs
{
    public class WorkerManagerJob : AbstractWorkerJob
    {

        

        public WorkerManagerJob(ILogger logger):base(logger)
        {
    
        }

        public override void DoWork()
        {
            try 
            {
                // //Console.WriteLine($"{DateTime.Now}: Searching for jobs to process ...");
                // _logger.LogInfo("Searching for jobs to process ...");
                using (var _context = (new DataContextFactory()).CreateDbContext(null))
                {
            
                    foreach(var createdJob in  _context.Jobs.Where(x=>x.Status == JobStatus.created).Include(u=>u.User).ToList())   
                    {
                        try
                        {
                            //Set the job to assigned status
                            createdJob.Status = JobStatus.assigned;
                            _context.SaveChanges();
                            //Start the worker

                            // using(System.Diagnostics.Process pProcess = new System.Diagnostics.Process())
                            // {
                            //     pProcess.StartInfo.FileName = @"D:\Code\K8sDemo\K8sDemoWorker\bin\Debug\net5.0\K8sDemoWorker.exe";
                            //     pProcess.StartInfo.Arguments = $"{createdJob.Id}";
                            //     pProcess.StartInfo.UseShellExecute = false;
                            //     pProcess.StartInfo.RedirectStandardOutput = false;
                            //     pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                            //     pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
                            //     pProcess.Start();
                            //     // string output = pProcess.StandardOutput.ReadToEnd(); //The output result
                            //     // pProcess.WaitForExit();
                            // }

                            //Generate error
                            throw new Exception("Unable to generate worker!");

                            _logger.LogInfo($"Job: {createdJob.Id} of type: {createdJob.Descritpion} created by user: {createdJob.User.UserName} assigned to worker");
                            
                        }
                        catch (Exception e)
                        {
                            createdJob.Status = JobStatus.error;
                            _context.SaveChanges();
                            _logger.LogError($"Error assigning Job: {createdJob.Id} from user: {createdJob.User.UserName} to worker",e);
                            ReportWorkProgress(new JobStatusMessage(){
                                User = createdJob.User.UserName,
                                Status = JobStatus.error,
                                UserMessage = $"Error processing Job: {createdJob.Id} from user: {createdJob.User.UserName}, {e.Message}"
                            });
                        } 
                        
                        
                    }             
                }
            }     
            catch(Exception e)
            {
                _logger.LogError($"Eror processing {nameof(WorkerManagerJob)}",e);
            }
        }
    }
}