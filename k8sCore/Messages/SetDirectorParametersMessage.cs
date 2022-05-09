
using K8sCore.Enums;

namespace K8sCore.Messages
{
    public class SetDirectorParametersMessage
    {
        public int MaxJobsPerWorker { get; set; }
        public int MaxWorkers{ get; set; } 
        public bool  ScalingEnabled { get; set; } 
        public int IdleSecondsBeforeScaleDown{ get; set; }

        public override string ToString()
        {
            return $"{nameof(MaxJobsPerWorker)}:{MaxJobsPerWorker}, {nameof(MaxWorkers)}:{MaxWorkers}, {nameof(ScalingEnabled)}:{ScalingEnabled}, {nameof(IdleSecondsBeforeScaleDown)}:{IdleSecondsBeforeScaleDown}";
        }
    }
}