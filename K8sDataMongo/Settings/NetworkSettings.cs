using System;

namespace K8sDataMongo.Settings
{
    public class NetworkSettings
    {
        private static readonly string MongoServerDockerHost = "k8sDemo-mongodatabase";
        //private static readonly string SqlServerDebugHost = "host.docker.internal";
        private static readonly string MongoServerDebugHost = "127.0.0.1";

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
            string hostName = MongoServerDockerHost;
            if (!RunningInDocker())
            {
                hostName = MongoServerDebugHost;
            }

            return $"mongodb://sa:Pass%40Word1@{hostName}:27017/?authMechanism=DEFAULT";
            
        }
    }
}