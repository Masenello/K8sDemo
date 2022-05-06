using System.Collections.Concurrent;
using System.Linq;
using K8sBackendShared.Interfaces;
using K8sBackendShared.K8s;
using K8sCore.DTOs;
using K8sDemoDirector.Interfaces;

namespace K8sDemoDirector.Services
{
    public class WorkersScalerService : IWorkersScaler
    {
        public int MaxJobsPerWorker { get; private set;} = 20;
        public int MaxWorkers { get; private set; } = 10;
        private bool ScalingEnabled { get; set; } = true;


        public bool SystemIsScaling { get; private set; }
        public bool SystemIsScalingDown { get
        {
            if ((SystemIsScaling) && (_registryManager.WorkersRegistry.Count > scalingTarget))
            {
                return true;
            }
            return false;
        } }
        public bool SystemIsScalingUp { get
        {
            if ((SystemIsScaling) && (_registryManager.WorkersRegistry.Count < scalingTarget))
            {
                return true;
            }
            return false;
        } }

        public int SystemCurrentJobsCapacity{ get
        {
            return _registryManager.WorkersRegistry.Count * MaxJobsPerWorker;
        } }

        int scalingTarget = 1;

        private readonly IK8s _k8sConnector;
        private readonly ILogger _logger;
        private readonly IWorkersRegistryManager _registryManager;

        public WorkersScalerService(IWorkersRegistryManager registryManager, IK8s k8sConnector, ILogger logger)
        {
            _k8sConnector = k8sConnector;
            _logger = logger;
            _registryManager = registryManager;
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
                && (SystemCurrentJobsCapacity<openJobsCount)) 
                {
                    WorkersScaleUp(openJobsCount);
                }

                if (_registryManager.WorkersRegistry.All(x => x.Value.CurrentJobs == 0)) //All workers are idle
                {
                    WorkersScaleDown(currentWorkers);
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
            int tmpScalingTarget= (openJobsCount + MaxJobsPerWorker - 1) / MaxJobsPerWorker;

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

        private void WorkersScaleDown(int currentWorkers)
        {

            //Always leave at least one worker 
            if (currentWorkers > 1)
            {
                {
                    SystemIsScaling = true;
                    scalingTarget = currentWorkers - 1;
                    _k8sConnector.ScaleDeployment(K8sNamespace.defaultNamespace, Deployment.worker, scalingTarget);
                    _logger.LogInfo($"Scaling down workers to: {scalingTarget}");
                }
            }
        }
    }


}