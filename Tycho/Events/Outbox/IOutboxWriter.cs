using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;

namespace Tycho.Events.Outbox
{
    internal interface IOutboxWriter
    {
        Task Write(IReadOnlyCollection<RoutedEvent> routedEvents, CancellationToken cancellationToken = default);
    }
}
