using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Outbox;
using Tycho.Events.Routing;
using Tycho.Events.Serialization;

namespace Tycho.Persistence.EFCore.Outbox;

internal class OutboxWriter(
    TychoDbContext dbContext,
    IEventSerializer eventSerializer,
    OutboxActivity outboxActivity) : IOutboxWriter
{
    private readonly TychoDbContext _dbContext = dbContext;
    private readonly OutboxActivity _outboxActivity = outboxActivity;

    public async Task Write(IReadOnlyCollection<RoutedEvent> events, CancellationToken cancellationToken = default)
    {
        var outboxEntries = events.Select(@event => 
        {
            var serializedEvent = eventSerializer.Serialize(@event);
            return new OutboxEntry
            {
                Id = serializedEvent.Id,
                Event = serializedEvent.EventId,
                Handler = serializedEvent.HandlerId,
                Route = serializedEvent.Route,
                Payload = serializedEvent.Payload
            };
        });
        _dbContext.Set<OutboxEntry>().AddRange(outboxEntries);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        _outboxActivity.NotifyNewEntriesAdded();
    }
}