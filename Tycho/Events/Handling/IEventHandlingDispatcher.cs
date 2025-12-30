using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events.Handling
{
    public interface IEventHandlingDispatcher
    {
        Task Dispatch(
            Guid eventId, 
            object eventPayload, 
            IEventHandler eventHandler, 
            CancellationToken cancellationToken);
    }
}
