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
        public async void AddAppToTableAsync(string username, ApplicationType appType, string connectionId)
        {
            AppUserEntity targetUser = await _context.Users.FirstOrDefaultAsync(x=>x.UserName == username);
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
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ConnectedAppEntity> GetAppAsync(string username)
        {
            return await _context.ConnectedApps.Include(u=>u.User).FirstOrDefaultAsync(x=>x.User.UserName == username);
        }

        public async void RemoveAppFromTableAsync(string username)
        {
            ConnectedAppEntity targetApp = await _context.ConnectedApps.Include(u=>u.User).FirstOrDefaultAsync(x=>x.User.UserName == username);

            if (targetApp != null)
            {
                _context.ConnectedApps.Remove(targetApp);
                await _context.SaveChangesAsync();

            }
        }
    }
}