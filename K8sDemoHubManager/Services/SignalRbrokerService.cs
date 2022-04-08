using System;
using System.Linq;
using System.Threading.Tasks;
using K8sBackendShared.Data;
using K8sBackendShared.Entities;
using K8sBackendShared.Messages;
using K8sDemoHubManager.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace K8sDemoHubManager.Services
{
    public class SignalRbrokerService
    {
        private readonly IHubContext<ClientHub> _hub;
        private readonly DataContext _context;

        public SignalRbrokerService(IHubContext<ClientHub> hub, DataContext context)
        {
            _hub = hub;
            _context = context;
        }

        public async Task ForwardJobStatusMessage(JobStatusMessage msg)
        {
            Console.WriteLine($"{DateTime.Now}: {nameof(SignalRbrokerService)} Job Id:{msg.JobId} Status: {msg.Status} Progress: {msg.ProgressPercentage}% ");

        
                ConnectedAppEntity connectedClient = await _context.ConnectedApps.FirstOrDefaultAsync(x=>x.User.UserName == msg.User);
                if (connectedClient != null)
                {
                    await 
                    _hub.Clients.Clients(connectedClient.ConnectionId).SendAsync("ReportJobProgress", msg);
                }
            
        }
    }
}