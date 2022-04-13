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
                    //Get the oldest job found in CREATED status from database
                    TestJobEntity targetJob = _context.Jobs.Where(x=>x.Status == JobStatus.created).OrderBy(y=>y.CreationDate).Include(u=>u.User).FirstOrDefault();
                    if (targetJob != null)
                    {
                        //Set the job to assigned status
                        targetJob.Status = JobStatus.assigned;
                        _context.SaveChanges();
                        //Start the worker
                        using(System.Diagnostics.Process pProcess = new System.Diagnostics.Process())
                        {
                            pProcess.StartInfo.FileName = @"D:\Code\K8sDemo\K8sDemoWorker\bin\Debug\net5.0\K8sDemoWorker.exe";
                            pProcess.StartInfo.Arguments = $"{targetJob.Id}";
                            pProcess.StartInfo.UseShellExecute = false;
                            pProcess.StartInfo.RedirectStandardOutput = false;
                            pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                            pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
                            pProcess.Start();
                            // string output = pProcess.StandardOutput.ReadToEnd(); //The output result
                            // pProcess.WaitForExit();
                        }

                        _logger.LogInfo($"Job: {targetJob.Id} from user: {targetJob.User.UserName} assigned to worker");

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