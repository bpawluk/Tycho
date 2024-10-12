using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TychoV2.Persistence.InMemory
{
    internal class InMemoryOutbox : IOutbox
    {
        private readonly Queue<Entry> _entries = new Queue<Entry>();

        public event EventHandler? NewEntriesAdded;

        public Task Add(IReadOnlyCollection<Entry> entries, CancellationToken cancellationToken)
        {
            foreach (var entry in entries)
            {
                _entries.Enqueue(entry);
            }
            NewEntriesAdded?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<Entry>> Read(int count, CancellationToken cancellationToken)
        {
            List<Entry> entries = new List<Entry>();

            count = Math.Min(count, _entries.Count);
            for (int i = 0; i < count; i++)
            {
                var entry = _entries.Dequeue();
                entries.Add(entry);
            }

            return Task.FromResult<IReadOnlyCollection<Entry>>(entries);
        }

        public Task MarkAsFailed(Entry entry, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task MarkAsProcessed(Entry entry, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
