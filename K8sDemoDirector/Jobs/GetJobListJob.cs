using System;
using K8sBackendShared.Jobs;
using K8sBackendShared.Interfaces;
using System.Collections.Concurrent;
using K8sCore.Entities;

namespace K8sDemoDirector.Jobs
{
    public class GetJobListJob : AbstractWorkerJob
    {

        public ConcurrentDictionary<int,JobEntity> _insertedJobs {get; private set;}


        public GetJobListJob(ILogger logger, IRabbitConnector rabbitConnector):base(logger, rabbitConnector)
        {
            _insertedJobs = new ConcurrentDictionary<int,JobEntity>();
        }
    
        public override void DoWork()
        {
            try 
            {
                //TODO Remove, just dummy!
            }     
            catch(Exception e)
            {
                _logger.LogError($"Eror processing: {nameof(GetJobListJob)}",e);
            }
        }

        
    }
}