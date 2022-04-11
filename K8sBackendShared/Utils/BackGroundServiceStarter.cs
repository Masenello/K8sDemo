using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace K8sBackendShared.Utils
{
public class BackgroundServiceStarter<T> : IHostedService where T:IHostedService
{
    readonly T backgroundService;

    public BackgroundServiceStarter(T backgroundService)
    {
        this.backgroundService = backgroundService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return backgroundService.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return backgroundService.StopAsync(cancellationToken);
    }
}
}