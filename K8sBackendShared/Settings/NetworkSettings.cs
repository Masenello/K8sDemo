using System;

namespace K8sBackendShared.Settings
{
    public class NetworkSettings
    {
        private static readonly string RabbitDockerHost = "k8sDemo-rabbitMq";
        private static readonly string RabbitDebugHost = "host.docker.internal";

        private static readonly string SqlServerDockerHost = "k8sDemo-database";
        private static readonly string SqlServerDebugHost = "host.docker.internal";

        private static bool RunningInDocker()
        {
            if ( Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") is null)
            {
                return false;
            }
            return true;
        }

        public static string RabbitHostResolver()
        {
            string hostName = RabbitDockerHost;
            if ( !RunningInDocker())
            {
                hostName = RabbitDebugHost;
            }
            return $"host={hostName}";
        }

        public static string DatabaseConnectionStringResolver()
        {
            string hostName = SqlServerDockerHost;
            if (!RunningInDocker())
            {
                hostName = SqlServerDebugHost;
            }

            return $"server={hostName};initial catalog=TestDatabase;persist security info=True;user id=sa;password=Pass@Word1;MultipleActiveResultSets=True;App=EntityFramework";
            
        }
    }
}