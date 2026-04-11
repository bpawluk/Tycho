using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;

namespace Tycho.Events.Outbox.InMemory
{
    internal class InMemoryOutbox : IOutboxWriter, IOutboxConsumer
    {
        private readonly OutboxActivity _outboxActivity;
        private readonly ConcurrentQueue<RoutedEvent> _entries;

        public InMemoryOutbox(OutboxActivity outboxActivity)
        {
            _outboxActivity = outboxActivity;
            _entries = new ConcurrentQueue<RoutedEvent>();
        }

        public Task Write(IReadOnlyCollection<RoutedEvent> entries, CancellationToken cancellationToken)
        {
            if (entries.Count == 0)
            {
                return Task.CompletedTask;
            }

            foreach (var entry in entries)
            {
                _entries.Enqueue(entry);
            }
            _outboxActivity.NotifyNewEntriesAdded();

            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<RoutedEvent>> Read(int count, CancellationToken cancellationToken)
        {
            var entries = new List<RoutedEvent>();

            for (var i = 0; i < count; i++)
            {
                if (_entries.TryDequeue(out var nextEntry))
                {
                    entries.Add(nextEntry);
                }
                else
                {
                    break;
                }
            }

            return Task.FromResult<IReadOnlyCollection<RoutedEvent>>(entries);
        }

        public Task MarkAsDelivered(Guid entryId, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task MarkAsFailed(Guid entryId, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
