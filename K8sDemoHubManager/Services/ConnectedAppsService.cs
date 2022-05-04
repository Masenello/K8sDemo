
using System.Collections.Generic;
using System.Linq;
using K8sBackendShared.Interfaces;
using K8sCore.Entities;
using K8sCore.Entities.Ef;
using K8sCore.Enums;
using K8sData.Data;
using K8sDemoHubManager.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace K8sDemoHubManager.Services
{
    public class ConnectedAppsService : IConnectedAppsService
    {

        private Dictionary<string, string> _usersRequestingLogForwardingOnBrowser;
        private readonly ILogger _logger;

        private readonly DataContext _context;
        public ConnectedAppsService(DataContext context, ILogger logger)
        {
            _context = context;
            _usersRequestingLogForwardingOnBrowser = new Dictionary<string, string>();
            _logger = logger;
        }
        public void AddAppToTable(string username, ApplicationType appType, string connectionId)
        {
            AppUserEntity targetUser =  _context.Users.FirstOrDefault(x=>x.UserName == username);
            if (targetUser is null)
            {
                throw new System.Exception($"User: {username} not found on database");
            }

            ConnectedAppEntity targetUserConnection = _context.ConnectedApps.FirstOrDefault(x=>x.UserId == targetUser.Id);

            //User connection not found -> Add
            if (targetUserConnection is null)
            {
                _context.ConnectedApps.Add(new ConnectedAppEntity(){
                    UserId= targetUser.Id,
                    ConnectionId = connectionId,
                    AppType = appType
                });
                
            }
            else
            {
                targetUserConnection.ConnectionId = connectionId;
            }
            _context.SaveChanges();
        }

        public  ConnectedAppEntity GetApp(string username)
        {
            return  _context.ConnectedApps.Include(u=>u.User).FirstOrDefault(x=>x.User.UserName == username);
        }

        public string GetUser(string connectionId)
        {
            return  _context.ConnectedApps.Include(u=>u.User).FirstOrDefault(x=>x.ConnectionId == connectionId).User.UserName;
        }

        public  void RemoveAppFromTable(string username)    
        {
            ConnectedAppEntity targetApp =  _context.ConnectedApps.Include(u=>u.User).FirstOrDefault(x=>x.User.UserName == username);

            if (targetApp != null)
            {
                _context.ConnectedApps.Remove(targetApp);
                _context.SaveChanges();

            }
        }

    }
}