using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using K8sBackendShared.Interfaces;
using K8sBackendShared.K8s;
using K8sBackendShared.Workers;
using K8sCore.DTOs;
using K8sCore.Entities;
using K8sCore.Entities.Mongo;
using K8sCore.Enums;
using K8sCore.Interfaces.Mongo;
using K8sCore.Messages;
using K8sDemoDirector.Interfaces;
using K8sDemoDirector.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace K8sDemoDirector.Services
{
    public class DirectorService : CyclicWorkerService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly GetJobListJob _getJobListJob;
        private readonly IWorkersScaler _workersScaler;
        private readonly IWorkersRegistryManager _registryManager;
        private List<JobEntity> openJobs = new List<JobEntity>();

        public DirectorService(IServiceProvider serviceProvider, IWorkersScaler workersScaler,IWorkersRegistryManager registryManager, IRabbitConnector rabbitConnector, ILogger logger, int cycleTime, GetJobListJob workerJob)
        : base(rabbitConnector, logger, cycleTime, workerJob)
        {
            _serviceProvider = serviceProvider;
            _getJobListJob = workerJob;
            _workersScaler=workersScaler;
            _registryManager = registryManager;
            

            base.MainCycleCompleted += CyclicWorkerMainCycleCompleted;


            _rabbitConnector.Publish<DirectorStartedMessage>(new DirectorStartedMessage());

        }

        private async void CyclicWorkerMainCycleCompleted(object sender, EventArgs e)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //Create Scope to call transient service in singleton
                var jobRepo = scope.ServiceProvider.GetRequiredService<IJobRepository>();
                openJobs = jobRepo.GetOpenJobs();
                foreach (var createdJob in openJobs.Where(x=>x.Status== JobStatus.created))
                {
                    
                    //If system is scaling DOWN or max workers is reached, avoid assigning jobs
                    //TODO evaulate if necessary to use scaling UP and DOWN separetely
                    if (_workersScaler.SystemIsScaling) break;
                    //Assign job to worker
                    var targetWorker = _workersScaler.GetWorkerWithLessLoad();
                    if (targetWorker is null)
                    {
                        //No workers available
                        break;
                    }
                    else
                    {
                        await jobRepo.AssignJobAsync(targetWorker.WorkerId, createdJob.Id);
                        //be sure that changes are saved in database before sending message to worker
                        _rabbitConnector.Publish<DirectorAssignJobToWorker>(new DirectorAssignJobToWorker()
                        {
                            WorkerId = targetWorker.WorkerId,
                            JobId = createdJob.Id,
                            JobType = createdJob.Type,
                        });
                        _logger.LogInfo($"Director assigned job: {createdJob.Id} to worker: {targetWorker.WorkerId}");
                    }

                }

                //Monitor active job for timeouts
                foreach (var openJob in openJobs)
                {
                    //var jobToMonitor = await uow.Jobs.GetJobWithIdAsync(activeJob.Key);
                    //TODO variable timeouts set on job creation 

                    //if ((jobToMonitor != null) && (DateTime.UtcNow - jobToMonitor.AssignmentDate).TotalSeconds>jobToMonitor.TimeOutSeconds)
                    if ((DateTime.UtcNow - openJob.CreationDate).TotalSeconds > 30)
                    {
                        var timeoutMsg = await jobRepo.SetJobInTimeOutAsync(openJob.Id);
                        _rabbitConnector.Publish<JobStatusMessage>(timeoutMsg);
                    }
                }
            }

            //Update job counts for workers
            _registryManager.UpdateJobCounts(openJobs);
            
            //Update Director Status
            DirectorStatusMessage newStatus = new DirectorStatusMessage()
            {
                Timestamp = DateTime.UtcNow,
                RegisteredWorkers = _registryManager.WorkersRegistry.Values.ToList(),
                TotalJobs = openJobs.Count(),
            };
            _rabbitConnector.Publish<DirectorStatusMessage>(newStatus);

            //Monitor worker scaling
            _workersScaler.MonitorWorkersLoad(openJobs.Count);

        }


    }
}