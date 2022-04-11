using System.Threading;
using System.Threading.Tasks;
using K8sDemoHubWorker.Services;
using Microsoft.Extensions.Hosting;

namespace K8sDemoWorker.Services
{
    public class culoService:IHostedService
    {

        protected readonly RabbitConnectorServiceDemoWorker _coniglio;
        public culoService(RabbitConnectorServiceDemoWorker rabbitservice)
        {
            _coniglio=rabbitservice;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}