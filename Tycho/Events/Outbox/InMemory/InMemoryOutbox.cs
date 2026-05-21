using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;
using Tycho.Events.Serialization;

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
            if (routedEvents.Count == 0)
            {
                return Task.CompletedTask;
            }

            foreach (RoutedEvent routedEvent in routedEvents)
            {
                SerializedRoutedEvent serializedEvent = _eventSerializer.Serialize(routedEvent);
                _entries.Enqueue(serializedEvent);
            }
            _outboxActivity.NotifyNewEntriesAdded();

            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<OutboxEvent>> Read(int count, CancellationToken cancellationToken)
        {
            var events = new List<OutboxEvent>();

            for (int i = 0; i < count; i++)
            {
                if (_entries.TryDequeue(out SerializedRoutedEvent? nextEntry))
                {
                    events.Add(new OutboxEvent(Guid.Empty, nextEntry));
                }
                else
                {
                    break;
                }
            }

            return Task.FromResult<IReadOnlyCollection<OutboxEvent>>(events);
        }

        public Task<bool> MarkAsDelivered(Guid eventId, Guid claimId, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<bool> MarkAsFailed(Guid eventId, Guid claimId, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
