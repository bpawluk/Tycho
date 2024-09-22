using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NewTycho.Events.Persistence
{
    internal interface IPersistentOutbox
    {
        Task Enqueue<Event>(IEnumerable<string> eventHandlerIds, Event eventData, CancellationToken cancellationToken)
            where Event : class, IEvent;
    }
}
