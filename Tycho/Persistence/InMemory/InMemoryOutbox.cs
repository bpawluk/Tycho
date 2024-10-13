using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Persistence.InMemory
{
    internal class InMemoryOutbox : IOutbox
    {
        private readonly Queue<OutboxEntry> _entries = new Queue<OutboxEntry>();

        public event EventHandler? NewEntriesAdded;

        public Task Add(IReadOnlyCollection<OutboxEntry> entries, CancellationToken cancellationToken)
        {
            foreach (var entry in entries)
            {
                _entries.Enqueue(entry);
            }
            NewEntriesAdded?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<OutboxEntry>> Read(int count, CancellationToken cancellationToken)
        {
            List<OutboxEntry> entries = new List<OutboxEntry>();

            count = Math.Min(count, _entries.Count);
            for (int i = 0; i < count; i++)
            {
                var entry = _entries.Dequeue();
                entries.Add(entry);
            }

            return Task.FromResult<IReadOnlyCollection<OutboxEntry>>(entries);
        }

        public Task MarkAsFailed(OutboxEntry entry, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task MarkAsProcessed(OutboxEntry entry, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
