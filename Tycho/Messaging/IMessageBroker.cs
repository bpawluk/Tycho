using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging;

public interface IMessageBroker
{
    void Publish<Event>(Event eventData, CancellationToken cancellationToken = default) 
        where Event : class, IEvent;

    Task Execute<Command>(Command commandData, CancellationToken cancellationToken = default) 
        where Command : class, ICommand;

    Task<Response> Execute<Query, Response>(Query queryData, CancellationToken cancellationToken = default)
        where Query : class, IQuery<Response>;
}
