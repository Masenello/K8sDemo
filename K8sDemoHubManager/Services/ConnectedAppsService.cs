using System.Linq;
using System.Threading.Tasks;
using K8sBackendShared.Data;
using K8sBackendShared.Entities;
using K8sBackendShared.Enums;
using K8sDemoHubManager.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace K8sDemoHubManager.Services
{
    public class ConnectedAppsService : IConnectedAppsService
    {

        private readonly DataContext _context;
        public ConnectedAppsService(DataContext context)
        {
            _context = context;
        }
        public void AddAppToTable(string username, ApplicationType appType, string connectionId)
        {
            AppUserEntity targetUser =  _context.Users.FirstOrDefault(x=>x.UserName == username);
            if (targetUser is null)
            {
                throw new System.Exception($"User: {username} not found on database");
            }

            if (_context.ConnectedApps.Include(u=>u.User).Where(x=>x.User.UserName == targetUser.UserName).Count()== 0)
            {
                _context.ConnectedApps.Add(new ConnectedAppEntity(){
                    UserId= targetUser.Id,
                    ConnectionId = connectionId,
                    AppType = appType
                });
                _context.SaveChanges();
            }
        }

        public  ConnectedAppEntity GetApp(string username)
        {
            return  _context.ConnectedApps.Include(u=>u.User).FirstOrDefault(x=>x.User.UserName == username);
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