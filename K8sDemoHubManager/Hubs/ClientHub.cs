using System;
using System.Threading.Tasks;
using K8sBackendShared.Enums;
using K8sDemoHubManager.Interfaces;
using K8sDemoHubManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace K8sDemoHubManager.Hubs
{
    [Authorize]
    public class ClientHub:Hub
    {

        private readonly IConnectedAppsService _connectedAppsService;
        public ClientHub(IConnectedAppsService connectedAppsService)
        {
            _connectedAppsService = connectedAppsService;
        }




        //public override async Task OnConnectedAsync()
        //{
            //await Clients.Others.SendAsync("UserIsOnLine",Context.User.Identity.Name);
            //await Clients.Others.SendAsync("UserIsOnLine","Ciccio");
            //Console.WriteLine($"User {Context.User.Identity.Name} connected");
            
        //}

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
            //await Clients.Others.SendAsync("UserIsOffLine",Context.User.Identity.Name);
        //    await Clients.Others.SendAsync("UserIsOffLine","Ciccio");
        //    await base.OnDisconnectedAsync(exception);
        //    Console.WriteLine($"User {Context.User.Identity.Name} disconnected");
        //}

        #region Log In 
        
                public void UserAppLogIn(string username)
                {
                    //Notify other users of this user log in
                    Clients.Others.SendAsync("UserIsOnLine",username);
                    _connectedAppsService.AddAppToTable(username, ApplicationType.client,  Context.ConnectionId);

                    
                    Console.WriteLine($"User {username} logged in");
                }

                public void UserAppLogOff(string username)
                {
                    //Notify other users of this user log off
                    Clients.Others.SendAsync("UserIsOffLine",username);
                    _connectedAppsService.RemoveAppFromTable(username);

                    Console.WriteLine($"User {username} logged out");
                }

        #endregion

        #region Jobs

        

        #endregion
    }
}