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
using K8sDemoWorker.Interfaces;
using System.Threading.Tasks;

namespace K8sDemoWorker.Jobs
{
    public class TestJob : AbstractWorkerJob, ITestJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IJobRepository _jobRepo;
        public TestJob(IServiceProvider serviceProvider, IJobRepository jobRepo, ILogger logger, IRabbitConnector rabbitConnector) : base(logger, rabbitConnector)
        {
            _serviceProvider = serviceProvider;
            _jobRepo = jobRepo;
        }

        public string WorkerId { get; set; }
        public string DatabaseJobId { get; set; }

        public async override Task DoWorkAsync()
        {
            JobEntity targetJob = await _jobRepo.GetByIdAsync(DatabaseJobId);

            try
            {
                if (targetJob is null) throw new Exception($"Job with Id: {DatabaseJobId} not found on database");
                //If already set in error return (timeout by director)
                if (targetJob.Status == JobStatus.error) return;
                if (targetJob.Status != JobStatus.assigned) throw new Exception($"Job with Id: {DatabaseJobId} is in status: {targetJob.Status}, expected status: {JobStatus.assigned}");

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
                JobStatusMessage errorStatus = await _jobRepo.SetJobInErrorAsync(targetJob.Id,targetJob.WorkerId, e);
                _logger.LogError(errorStatus.UserMessage);
                ReportWorkProgress(errorStatus);
            }
        }
    }
}