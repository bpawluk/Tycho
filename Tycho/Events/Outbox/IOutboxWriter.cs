using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;

namespace Tycho.Events.Outbox
{
    internal interface IOutboxWriter
    {
        Task Write(IReadOnlyCollection<RoutedEvent> entries, CancellationToken cancellationToken = default);
    }
}
