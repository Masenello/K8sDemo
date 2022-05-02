

using k8sCore.Entities;
using k8sCore.Enums;

namespace K8sDemoHubManager.Interfaces
{
    public interface IConnectedAppsService
    {
        void AddAppToTable(string username, ApplicationType appType, string connectionId);
        void RemoveAppFromTable(string username);
        ConnectedAppEntity GetApp(string username);

        string GetUser(string connectionId);

    }
}