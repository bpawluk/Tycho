using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;
using Tycho.Events.Serialization;
using Tycho.Utils;

namespace Tycho.Events.Outbox.InMemory
{
    internal class InMemoryOutbox : IOutboxWriter, IOutboxConsumer
    {
        private readonly IEventSerializer _eventSerializer;
        private readonly OutboxActivity _outboxActivity;
        private readonly ConcurrentQueue<SerializedRoutedEvent> _entries;

        public InMemoryOutbox(IEventSerializer eventSerializer, OutboxActivity outboxActivity)
        {
            _eventSerializer = eventSerializer;
            _outboxActivity = outboxActivity;
            _entries = new ConcurrentQueue<SerializedRoutedEvent>();
        }

        public Task Write(IReadOnlyCollection<RoutedEvent> routedEvents, CancellationToken cancellationToken)
        {
            routedEvents.ThrowIfNull();
            cancellationToken.ThrowIfCancellationRequested();

            if (routedEvents.Count == 0)
            {
                return Task.CompletedTask;
            }

            var serializedEvents = new List<SerializedRoutedEvent>(routedEvents.Count);
            foreach (RoutedEvent routedEvent in routedEvents)
            {
                cancellationToken.ThrowIfCancellationRequested();
                serializedEvents.Add(_eventSerializer.Serialize(routedEvent));
            }

            cancellationToken.ThrowIfCancellationRequested();
            foreach (SerializedRoutedEvent serializedEvent in serializedEvents)
            {
                _entries.Enqueue(serializedEvent);
            }
            _outboxActivity.NotifyNewEntriesAdded();

            return Task.CompletedTask;
        }

        public Task<OutboxEvent?> TryReadAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_entries.TryDequeue(out SerializedRoutedEvent? nextEntry))
            {
                return Task.FromResult<OutboxEvent?>(new OutboxEvent(Guid.Empty, nextEntry));
            }

            return Task.FromResult<OutboxEvent?>(null);
        }

        public Task<bool> MarkAsDeliveredAsync(Guid entryId, Guid claimId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(true);
        }

        public Task<bool> MarkAsFailedAsync(Guid entryId, Guid claimId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(true);
        }
    }
}
