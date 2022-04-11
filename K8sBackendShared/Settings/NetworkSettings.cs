using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace K8sBackendShared.Settings
{
    public class NetworkSettings
    {
        private static readonly string RabbitContainerName = "k8sdemorabbitmqcontainer";
        private static readonly int RabbitBootTimeoutSeconds = 30;

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

        //Returns true if rabbit management web page  is available, this meand that boot phase is completed
        public static async Task<bool> IsRabbitUp()
        {

            HttpClient client = new HttpClient();
            if (RunningInDocker())
            {
                client.BaseAddress = new Uri("http://" + RabbitDockerHost + ":15672/");
            }
            else
            {
                client.BaseAddress = new Uri("http://" + RabbitDebugHost + ":15672/");
            }
            Console.WriteLine($"Docker managment: {client.BaseAddress}");
            try
            {
                var response = await client.GetAsync(client.BaseAddress );
                if (response.IsSuccessStatusCode)
                {
                    return true;
                } 
            }
            catch
            {
                //Do nothing
            }
            return false;
        }


        public static void WaitForRabbitDependancy()
        {
                DateTime timeoutTime = DateTime.Now.AddSeconds(RabbitBootTimeoutSeconds);
                while (true)
                {
                    //Wait that RabbitMQ is ready
                    if (IsRabbitUp().Result)
                    {
                        Console.WriteLine($"RabbitMq started");
                        Thread.Sleep(2000);
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Waiting RabbitMq to start ...");
                        Thread.Sleep(1000);
                    }

                    if (DateTime.Now > timeoutTime)
                    {
                        throw new Exception($"RabbitMq docker start timeout");
                    }
                }
        }

    }
}