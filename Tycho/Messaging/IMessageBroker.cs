using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging
{
    public interface IMessageBroker
    {
        void PublishEvent<Event>(Event eventData, CancellationToken cancellationToken = default)
            where Event : class, IEvent;

        Task ExecuteCommand<Command>(Command commandData, CancellationToken cancellationToken = default)
            where Command : class, ICommand;

        Task<Response> ExecuteQuery<Query, Response>(Query queryData, CancellationToken cancellationToken = default)
            where Query : class, IQuery<Response>;
    }
}
