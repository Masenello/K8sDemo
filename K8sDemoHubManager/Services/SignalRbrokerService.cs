using System;
using System.Linq;
using System.Threading.Tasks;
using K8sBackendShared.Data;
using K8sBackendShared.Entities;
using K8sBackendShared.Interfaces;
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
        private readonly ILogger _logger;

        public SignalRbrokerService(IHubContext<ClientHub> hub, DataContext context, ILogger logger)
        {
            _hub = hub;
            _context = context;
            _logger = logger;
        }

        public async Task ForwardMessage<T>(T msg, string targetUser)
        {        
                ConnectedAppEntity connectedClient = await _context.ConnectedApps.FirstOrDefaultAsync(x=>x.User.UserName == targetUser);
                if (connectedClient != null)
                {
                    _logger.LogInfo($"{nameof(SignalRbrokerService)}: Forwarding message: {typeof(T).Name} to client with connection id: {connectedClient.Id}");
                    await _hub.Clients.Clients(connectedClient.ConnectionId).SendAsync(typeof(T).Name, msg);
                    
                }
            
        }

        public async Task ForwardMessageToGroup<T>(T msg, string groupName)
        {             
            //_logger.LogInfo($"{nameof(SignalRbrokerService)}: Forwarding message: {typeof(T).Name} to group: {groupName}");
            await _hub.Clients.Group(groupName).SendAsync(typeof(T).Name, msg);
                
        }
    }
}