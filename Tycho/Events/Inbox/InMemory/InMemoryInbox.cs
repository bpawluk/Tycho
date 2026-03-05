using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;

namespace Tycho.Events.Inbox.InMemory
{
    internal class InMemoryInbox : IInboxWriter, IInboxConsumer
    {
        private readonly InboxActivity _inboxActivity;
        private readonly ConcurrentQueue<RoutedEvent> _entries;

        public InMemoryInbox(InboxActivity inboxActivity)
        {
            _inboxActivity = inboxActivity;
            _entries = new ConcurrentQueue<RoutedEvent>();
        }

        public Task Write(RoutedEvent entry, CancellationToken cancellationToken = default)
        {
            _entries.Enqueue(entry);
            _inboxActivity.NotifyNewEntriesAdded();
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<RoutedEvent>> Read(int count, CancellationToken cancellationToken = default)
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
