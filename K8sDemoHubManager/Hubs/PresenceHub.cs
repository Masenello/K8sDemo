using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace K8sDemoHubManager.Hubs
{
    [Authorize]
    public class PresenceHub:Hub
    {

  
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

        public void UserAppLogIn(string username)
        {
            Clients.Others.SendAsync("UserIsOnLine",username);
            Console.WriteLine($"User {username} connected");
        }

        public void UserAppLogOff(string username)
        {
            Clients.Others.SendAsync("UserIsOffLine",username);
            Console.WriteLine($"User {username} disconnected");
        }


    }
}