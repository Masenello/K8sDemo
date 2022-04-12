using System.Collections.Generic;
using System.Threading.Tasks;
using K8sBackendShared.Entities;
using K8sBackendShared.Enums;

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