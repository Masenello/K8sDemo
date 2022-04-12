using System;

namespace K8sBackendShared.Interfaces
{
    public interface IRabbitConnector
    {
        void Publish<T>(T message);
        void Subscribe<T>(Action<T> subscribeAction);
    }
}