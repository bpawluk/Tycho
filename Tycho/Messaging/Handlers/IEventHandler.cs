using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Handlers;

public interface IEventHandler { }

public interface IEventHandler<in Event> 
    : IEventHandler
    where Event : class, IEvent 
{ 
    Task Handle(Event eventData, CancellationToken cancellationToken = default);
}
