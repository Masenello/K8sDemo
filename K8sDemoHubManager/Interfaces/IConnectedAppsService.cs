using System.Threading.Tasks;
using K8sBackendShared.Entities;
using K8sBackendShared.Enums;

namespace K8sDemoHubManager.Interfaces
{
    public interface IConnectedAppsService
    {
        void AddAppToTableAsync(string username, ApplicationType appType, string connectionId);
        void RemoveAppFromTableAsync(string username);
        Task<ConnectedAppEntity> GetAppAsync(string username);
    }
}