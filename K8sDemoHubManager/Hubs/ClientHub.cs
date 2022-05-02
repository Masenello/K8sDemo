using System.Threading.Tasks;
using K8sBackendShared.Interfaces;
using K8sDemoHubManager.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace K8sDemoHubManager.Hubs
{
    [Authorize]
    public class ClientHub:Hub
    {

        private readonly IConnectedAppsService _connectedAppsService;
        private readonly ILogger _logger;
        public ClientHub(IConnectedAppsService connectedAppsService, ILogger logger)
        {
            _connectedAppsService = connectedAppsService;
            _logger = logger;
        }

        #region Log In 
        
                public void UserAppLogIn(string username)
                {
                    //Notify other users of this user log in
                    Clients.Others.SendAsync("UserIsOnLine",username);
                    _connectedAppsService.AddAppToTable(username, k8sCore.Enums.ApplicationType.client,  Context.ConnectionId);

                    
                    _logger.LogInfo($"User {username} logged in");
                }

                public void UserAppLogOff(string username)
                {
                    //Notify other users of this user log off
                    Clients.Others.SendAsync("UserIsOffLine",username);
                    _connectedAppsService.RemoveAppFromTable(username);

                    _logger.LogInfo($"User {username} logged out");
                }

        #endregion

        #region Logs
            public async Task JoinGroup(string groupName)
            {
                _logger.LogInfo($"User: {_connectedAppsService.GetUser(Context.ConnectionId)} joined group: {groupName}");
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }

            public async Task LeaveGroup(string groupName)
            {
                _logger.LogInfo($"User: {_connectedAppsService.GetUser(Context.ConnectionId)} leaved group: {groupName}");
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            }

        #endregion
    }
}