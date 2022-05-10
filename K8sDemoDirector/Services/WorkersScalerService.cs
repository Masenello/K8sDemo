using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Timers;
using K8sBackendShared.Interfaces;
using K8sBackendShared.K8s;
using K8sCore.DTOs;
using K8sCore.Messages;
using K8sDemoDirector.Interfaces;

namespace K8sDemoDirector.Services
{
    public class WorkersScalerService : IWorkersScaler
    {
        public int MaxJobsPerWorker { get; private set; } = 20;
        public int MaxWorkers { get; private set; } = 5;
        private bool ScalingEnabled { get; set; } = true;
        private int IdleSecondsBeforeScaleDown { get; set; } = 30;


        public bool SystemIsScaling { get; private set; }
        public bool SystemIsScalingDown
        {
            get
            {
                if ((SystemIsScaling) && (_registryManager.WorkersRegistry.Count > scalingTarget))
                {
                    return true;
                }
                return false;
            }
        }
        public bool SystemIsScalingUp
        {
            get
            {
                if ((SystemIsScaling) && (_registryManager.WorkersRegistry.Count < scalingTarget))
                {
                    return true;
                }
                return false;
            }
        }

        public int SystemCurrentJobsCapacity
        {
            get
            {
                return _registryManager.WorkersRegistry.Count * MaxJobsPerWorker;
            }
        }

        int scalingTarget = 1;

        private readonly IK8s _k8sConnector;
        private readonly ILogger _logger;
        private readonly IWorkersRegistryManager _registryManager;
        private readonly IRabbitConnector _rabbitConnector;

        private Timer _scaleDownFilterTimer = new Timer();

        public WorkersScalerService(IWorkersRegistryManager registryManager, IRabbitConnector rabbitConnector, IK8s k8sConnector, ILogger logger)
        {
            _k8sConnector = k8sConnector;
            _logger = logger;
            _registryManager = registryManager;
            _rabbitConnector = rabbitConnector;

            _scaleDownFilterTimer.AutoReset = false;
            _scaleDownFilterTimer.Enabled = false;
            _scaleDownFilterTimer.Elapsed += new ElapsedEventHandler(ScaleDownFilterTimerElapsed);

            _rabbitConnector.Subscribe<SetDirectorParametersMessage>(SetDirectorParametersMessageHandler);
        }



        private void SetDirectorParametersMessageHandler(SetDirectorParametersMessage msg)
        {
            MaxJobsPerWorker = msg.MaxJobsPerWorker;
            MaxWorkers = msg.MaxWorkers;
            ScalingEnabled = msg.ScalingEnabled;
            IdleSecondsBeforeScaleDown = msg.IdleSecondsBeforeScaleDown;
        }

        public void MonitorWorkersScaling(int openJobsCount)
        {
            if (SystemIsScaling)
            {
                if (_registryManager.WorkersRegistry.Count == scalingTarget)
                {
                    SystemIsScaling = false;
                }
            }

            //if ((!SystemIsScaling) && ScalingEnabled)
            if (ScalingEnabled)
            {
                int currentWorkers = _registryManager.WorkersRegistry.Count;
                if ((currentWorkers > 0)
                && (currentWorkers < MaxWorkers)
                && (SystemCurrentJobsCapacity < openJobsCount))
                {
                    WorkersScaleUp(openJobsCount);
                }

                // _logger.LogInfo($"Open jobs count: {openJobsCount} current workers {currentWorkers} timer enabled: {_scaleDownFilterTimer.Enabled}");
                if ((openJobsCount == 0) && (currentWorkers > 1) && (!_scaleDownFilterTimer.Enabled))
                {
                    _scaleDownFilterTimer.Interval = IdleSecondsBeforeScaleDown * 1000;
                    _scaleDownFilterTimer.Start();
                }
            
                if ((openJobsCount != 0) || (currentWorkers == 1))
                {
                    //If load changes while timer is running, disable filter timer
                    _scaleDownFilterTimer.Stop();
                }
                
            }
        }

        public WorkerDescriptorDto GetWorkerWithLessLoad()
        {
            if (_registryManager.WorkersRegistry.Count == 0) return null;
            var targetWorker = _registryManager.WorkersRegistry.OrderBy(x => x.Value.CurrentJobs).First();
            //A worker that is saturated is considered not available
            if (targetWorker.Value.CurrentJobs >= MaxJobsPerWorker) return null;
            return targetWorker.Value;
        }

        private void WorkersScaleUp(int openJobsCount)
        {
            SystemIsScaling = true;
            //If current workers requirement is higher than the current scaling target use the highest one
            int tmpScalingTarget = (openJobsCount + MaxJobsPerWorker - 1) / MaxJobsPerWorker;

            if (tmpScalingTarget > MaxWorkers)
            {
                tmpScalingTarget = MaxWorkers;
            }
            if (tmpScalingTarget > scalingTarget)
            {
                scalingTarget = tmpScalingTarget;
                _k8sConnector.ScaleDeployment(K8sNamespace.defaultNamespace, Deployment.worker, scalingTarget);
                _logger.LogInfo($"Scaling up workers to: {scalingTarget}");
            }

        }

        private void ScaleDownFilterTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _logger.LogInfo($"Scale down timer elapsed");
            WorkersScaleDown();
        }

        private void WorkersScaleDown()
        {
            SystemIsScaling = true;
            scalingTarget = _registryManager.WorkersRegistry.Count - 1;
            _k8sConnector.ScaleDeployment(K8sNamespace.defaultNamespace, Deployment.worker, scalingTarget);
            _logger.LogInfo($"Scaling down workers to: {scalingTarget}");
        }
    }


}