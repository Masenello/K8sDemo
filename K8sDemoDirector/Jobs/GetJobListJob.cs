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


        public GetJobListJob(ILogger logger, IRabbitConnector rabbitConnector):base(logger, rabbitConnector)
        {
            _insertedJobs = new ConcurrentDictionary<int,JobEntity>();
        }
    
        public override void DoWork(object workerParameters)
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