using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using K8sBackendShared.Jobs;
using K8sBackendShared.Interfaces;
using K8sBackendShared.Logging;
using k8sCore.Interfaces.JobRepository;
using Microsoft.Extensions.DependencyInjection;
using k8sCore.Entities;
using k8sCore.Enums;
using K8sCore.Messages;

namespace K8sDemoWorker.Jobs
{
    public class TestJob : AbstractWorkerJob
    {
        private readonly IServiceProvider _serviceProvider;
        private int _jobToProcessId;
        private string _workerId;
        public TestJob(IServiceProvider serviceProvider, ILogger logger, IRabbitConnector rabbitConnector, string workerId) : base(logger, rabbitConnector)
        {
            _serviceProvider = serviceProvider;
            _workerId = workerId;
        }

        public async override void DoWork(object workerParameters)
        {
            _jobToProcessId = (int)workerParameters;
            using (var scope = _serviceProvider.CreateScope())
            {
                var uow = scope.ServiceProvider.GetRequiredService<IJobUnitOfWork>();
                JobEntity targetJob = uow.Jobs.GetJobWithId(_jobToProcessId);

                try
                {
                    if (targetJob is null) throw new Exception($"Job with Id: {_jobToProcessId} not found on database");
                    if (targetJob.Status != JobStatus.assigned) throw new Exception($"Job with Id: {_jobToProcessId} is in status: {targetJob.Status}, expected status: {JobStatus.assigned}");

                    _logger.LogInfo($"{targetJob.GenerateJobDescriptor()} running");
                    JobStatusMessage statusMsg = uow.SetJobInRunningStatus(targetJob.Id);
                    ReportWorkProgress(statusMsg);
                    Thread.Sleep(3000);
                    statusMsg.ProgressPercentage = 33.3;
                    ReportWorkProgress(statusMsg);

                    Thread.Sleep(3000);
                    statusMsg.ProgressPercentage = 66.6;
                    ReportWorkProgress(statusMsg);

                    Thread.Sleep(3000);
                    statusMsg.ProgressPercentage = 100.0;
                    statusMsg = uow.SetJobInCompletedStatus(targetJob.Id);
                    ReportWorkProgress(statusMsg);


                    //Set job to completed status
                    _logger.LogInfo($"{targetJob.GenerateJobDescriptor()} completed");
                }
                catch (Exception e)
                {
                    JobStatusMessage errorStatus = uow.SetJobInError(targetJob.Id,e);
                    _logger.LogError(errorStatus.UserMessage);
                    ReportWorkProgress(errorStatus);
                }
            }
        }
    }
}