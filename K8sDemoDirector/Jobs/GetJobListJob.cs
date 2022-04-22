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
using K8sBackendShared.Utils;
using System.Collections.Concurrent;

namespace K8sDemoDirector.Jobs
{
    public class GetJobListJob : AbstractWorkerJob
    {

        public ConcurrentDictionary<int,JobEntity> _insertedJobs {get; private set;}
        private readonly IRabbitConnector _rabbitConnector;

        public GetJobListJob(ILogger logger, IRabbitConnector rabbitConnector):base(logger)
        {
            _rabbitConnector = rabbitConnector;
            _insertedJobs = new ConcurrentDictionary<int,JobEntity>();
        }

        public JobsAvailableMessage BuildJobsAvailableMessage()
        {
            JobsAvailableMessage msg = new JobsAvailableMessage();
            foreach(var jobType  in _insertedJobs.ToList().GroupBy(x=>x.Value.Type))
                {
                    //Console.WriteLine($"{jobType.Key}:{jobType.Count()}");
                    msg.JobsList.Add(new JobAvailableCount()
                    {
                        JobType = jobType.Key,
                        JobCount = jobType.Count()
                    });
                } 
                return msg;
        }

        public override void DoWork(object workerParameters)
        {
            try 
            {
                using (var _context = (new DataContextFactory()).CreateDbContext(null))
                {
                    foreach(var createdJob in  _context.Jobs.Where(x=>x.Status == JobStatus.created).Include(u=>u.User).ToList())   
                    {
                        _insertedJobs.TryAdd(createdJob.Id, createdJob);
                    } 

                    if(_insertedJobs.Count()>0)
                    {
                        _rabbitConnector.Publish<JobsAvailableMessage>(BuildJobsAvailableMessage());
                    }
                }
            }     
            catch(Exception e)
            {
                _logger.LogError($"Eror processing: {nameof(GetJobListJob)}",e);
            }
        }

        
    }
}