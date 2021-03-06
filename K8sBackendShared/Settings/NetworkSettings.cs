using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace K8sBackendShared.Settings
{
    public class NetworkSettings
    {
        private static readonly int RabbitBootTimeoutSeconds = 30;

        private static readonly string RabbitDockerHost = "k8sDemo-rabbitMq";
        private static readonly string RabbitDebugHost = "127.0.0.1";

        

        public static bool RunningInDocker()
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

        //Returns true if rabbit management web page  is available, this means that boot phase is completed
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
                        Console.WriteLine($"RabbitMq is running");
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