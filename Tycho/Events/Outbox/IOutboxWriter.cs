using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events.Outbox
{
    internal interface IOutboxWriter
    {
        Task Write(IReadOnlyCollection<OutboxEntry> entries, CancellationToken cancellationToken = default);
    }
}
