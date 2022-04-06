using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace K8sDemoHubManager.Hubs
{
    [Authorize]
    public class JobStatusHub:Hub
    {
        
    }
}