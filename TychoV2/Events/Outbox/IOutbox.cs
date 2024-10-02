using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TychoV2.Events.Outbox
{
    internal interface IOutbox
    {
        Task Enqueue<Event>(
            IEnumerable<string> eventHandlerIds,
            Event eventData,
            CancellationToken cancellationToken = default)
            where Event : class, IEvent;
    }
}
