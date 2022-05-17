using System;

namespace K8sDataMongo.Settings
{
    public class NetworkSettings
    {

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
            string hostName = "";
            string user = "";
            string password = "";
            if (RunningInDocker())
            {
                hostName = "k8sDemo-mongodatabase";
                user = Environment.GetEnvironmentVariable("MONGO_INITDB_ROOT_USERNAME") ;
                password = Environment.GetEnvironmentVariable("MONGO_INITDB_ROOT_PASSWORD") ;
            }
            else
            {
                hostName = "127.0.0.1";
                user="sa";
                password="PassWord1";
            }

            return $"mongodb://{user}:{password}@{hostName}:27017/";
        }
    }
}