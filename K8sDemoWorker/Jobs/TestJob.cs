using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using K8sBackendShared.Jobs;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Logging;
using K8sCore.Interfaces.JobRepository;
using Microsoft.Extensions.DependencyInjection;
using K8sCore.Entities;
using K8sCore.Enums;
using K8sCore.Messages;

namespace K8sDemoWorker.Jobs
{
    public class TestJob : AbstractWorkerJob
    {
        private readonly IServiceProvider _serviceProvider;
        private int _jobToProcessId;
        private string _workerId;
        public TestJob(IServiceProvider serviceProvider, ILogger logger, IRabbitConnector rabbitConnector) : base(logger, rabbitConnector)
        {
            _serviceProvider = serviceProvider;
        }

        public void InitService(string workerId, int jobToProcessId)
        {
            _workerId = workerId;
            _jobToProcessId = jobToProcessId;
        }

        public async override void DoWork()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _jobUow = scope.ServiceProvider.GetRequiredService<IJobUnitOfWork>();
                JobEntity targetJob = await _jobUow.Jobs.GetJobWithIdAsync(_jobToProcessId);

                try
                {
                    if (targetJob is null) throw new Exception($"Job with Id: {_jobToProcessId} not found on database");
                    if (targetJob.Status != JobStatus.assigned) throw new Exception($"Job with Id: {_jobToProcessId} is in status: {targetJob.Status}, expected status: {JobStatus.assigned}");

                    _logger.LogInfo($"{targetJob.GenerateJobDescriptor()} running");
                    JobStatusMessage statusMsg = await _jobUow.SetJobInRunningStatusAsync(targetJob.Id);
                    ReportWorkProgress(statusMsg);
                    Thread.Sleep(3000);
                    statusMsg.ProgressPercentage = 33.3;
                    ReportWorkProgress(statusMsg);

                    Thread.Sleep(3000);
                    statusMsg.ProgressPercentage = 66.6;
                    ReportWorkProgress(statusMsg);

                    Thread.Sleep(3000);
                    statusMsg.ProgressPercentage = 100.0;
                    statusMsg = await _jobUow.SetJobInCompletedStatusAsync(targetJob.Id);
                    ReportWorkProgress(statusMsg);


                    //Set job to completed status
                    _logger.LogInfo($"{targetJob.GenerateJobDescriptor()} completed");
                }
                catch (Exception e)
                {
                    JobStatusMessage errorStatus = await _jobUow.SetJobInErrorAsync(targetJob.Id, e);
                    _logger.LogError(errorStatus.UserMessage);
                    ReportWorkProgress(errorStatus);
                }
            }


        }
    }
}