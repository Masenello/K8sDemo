using System;
using System.Threading.Tasks;

namespace K8sBackendShared.Interfaces
{
    public interface IRabbitConnector
    {
        void Publish<T>(T message);
        void Subscribe<T>(Action<T> handleMessageAction);

        Tresp Request<Treq, Tresp>(Treq message);

        void Respond<Treq, Tresp>(Func<Treq, Tresp> respondToRequestAction);

    }
}