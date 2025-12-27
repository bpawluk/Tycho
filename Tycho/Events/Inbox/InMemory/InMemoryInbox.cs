using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events.Inbox.InMemory
{
    internal class InMemoryInbox : IInboxWriter, IInboxConsumer
    {
        private readonly InboxActivity _inboxActivity;
        private readonly Queue<InboxEntry> _entries;

        public InMemoryInbox(InboxActivity inboxActivity)
        {
            _inboxActivity = inboxActivity;
            _entries = new Queue<InboxEntry>();
        }

        public Task Write(InboxEntry entry, CancellationToken cancellationToken = default)
        {
            _entries.Enqueue(entry);
            _inboxActivity.NotifyNewEntriesAdded();
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<InboxEntry>> Read(int count, CancellationToken cancellationToken = default)
        {
            var entries = new List<InboxEntry>();

            count = Math.Min(count, _entries.Count);
            for (var i = 0; i < count; i++)
            {
                var entry = _entries.Dequeue();
                entries.Add(entry);
            }

            return Task.FromResult<IReadOnlyCollection<InboxEntry>>(entries);
        }

        public Task MarkAsHandled(Guid entryId, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task MarkAsFailed(Guid entryId, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
