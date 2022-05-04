using System;
using System.Linq;
using System.Threading;
using K8sBackendShared.Jobs;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Logging;
using Microsoft.Extensions.DependencyInjection;
using K8sCore.Entities;
using K8sCore.Enums;
using K8sCore.Messages;
using K8sCore.Interfaces.Mongo;
using K8sCore.Entities.Mongo;

namespace K8sDemoWorker.Jobs
{
    public class TestJob : AbstractWorkerJob
    {
        private readonly IServiceProvider _serviceProvider;
        private string _jobToProcessId;
        private string _workerId;
        public TestJob(string workerId, string jobToProcessId, IServiceProvider serviceProvider, ILogger logger, IRabbitConnector rabbitConnector) : base(logger, rabbitConnector)
        {
            _serviceProvider = serviceProvider;
            _workerId = workerId;
            _jobToProcessId = jobToProcessId;
        }

        public async override void DoWork()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _jobRepo = scope.ServiceProvider.GetRequiredService<IJobRepository>();
                JobEntity targetJob = await _jobRepo.GetByIdAsync(_jobToProcessId);

                try
                {
                    if (targetJob is null) throw new Exception($"Job with Id: {_jobToProcessId} not found on database");
                    //If already set in error return (timeout by director)
                    if (targetJob.Status == JobStatus.error) return;
                    if (targetJob.Status != JobStatus.assigned) throw new Exception($"Job with Id: {_jobToProcessId} is in status: {targetJob.Status}, expected status: {JobStatus.assigned}");

                    _logger.LogInfo($"{targetJob.GenerateJobDescriptor()} running");
                    
                    JobStatusMessage statusMsg = await _jobRepo.SetJobInRunningStatusAsync(targetJob.Id);
                    ReportWorkProgress(statusMsg);
                    Thread.Sleep(3000);
                    statusMsg.ProgressPercentage = 33.3;
                    ReportWorkProgress(statusMsg);

                    Thread.Sleep(3000);
                    statusMsg.ProgressPercentage = 66.6;
                    ReportWorkProgress(statusMsg);

                    Thread.Sleep(3000);
                    statusMsg.ProgressPercentage = 100.0;
                    statusMsg = await _jobRepo.SetJobInCompletedStatusAsync(targetJob.Id);
                    ReportWorkProgress(statusMsg);


                    //Set job to completed status
                    _logger.LogInfo($"{targetJob.GenerateJobDescriptor()} completed");
                }
                catch (Exception e)
                {
                    JobStatusMessage errorStatus = await _jobRepo.SetJobInErrorAsync(targetJob.Id, e);
                    _logger.LogError(errorStatus.UserMessage);
                    ReportWorkProgress(errorStatus);
                }
            }


        }
    }
}