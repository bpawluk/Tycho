using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Persistence.InMemory
{
    internal class InMemoryOutbox : IOutboxWriter, IOutboxConsumer
    {
        private readonly OutboxActivity _outboxActivity;
        private readonly Queue<OutboxEntry> _entries;

        public InMemoryOutbox(OutboxActivity outboxActivity)
        {
            _outboxActivity = outboxActivity;
            _entries = new Queue<OutboxEntry>();
        }

        public Task Write(IReadOnlyCollection<OutboxEntry> entries, bool _, CancellationToken cancellationToken)
        {
            foreach (var entry in entries)
            {
                _entries.Enqueue(entry);
            }
            _outboxActivity.NotifyNewEntriesAdded();
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<OutboxEntry>> Read(int count, CancellationToken cancellationToken)
        {
            var entries = new List<OutboxEntry>();

            count = Math.Min(count, _entries.Count);
            for (var i = 0; i < count; i++)
            {
                var entry = _entries.Dequeue();
                entries.Add(entry);
            }

            return Task.FromResult<IReadOnlyCollection<OutboxEntry>>(entries);
        }

        public Task MarkAsFailed(OutboxEntry entry, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task MarkAsProcessed(OutboxEntry entry, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}