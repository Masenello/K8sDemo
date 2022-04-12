using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace K8sBackendShared.Utils
{

//This class is used to forse creation an instance of a singleton service in case the service is not injected in any other.
public class BackgroundServiceStarter<T> : IHostedService
{
    readonly T backgroundService;

    public BackgroundServiceStarter(T backgroundService)
    {
        this.backgroundService = backgroundService;
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