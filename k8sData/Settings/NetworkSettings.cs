using System;

namespace K8sData.Settings
{
    public class NetworkSettings
    {
        private static readonly string SqlServerDockerHost = "k8sDemo-database";
        //private static readonly string SqlServerDebugHost = "host.docker.internal";
        //LocalHost
        private static readonly string SqlServerDebugHost = "127.0.0.1";
        //Azure cluster
        //private static readonly string SqlServerDebugHost = "20.71.23.87";
        

        public static bool RunningInDocker()
        {
            if ( Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") is null)
            {
                return false;
            }
            return true;
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