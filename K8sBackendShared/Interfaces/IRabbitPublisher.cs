namespace K8sBackendShared.Interfaces
{
    public interface IRabbitPublisher
    {
        void Publish<T>(T message);
    }
}