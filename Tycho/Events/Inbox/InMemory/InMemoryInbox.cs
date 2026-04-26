using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;
using Tycho.Events.Serialization;

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

        public Task Write(RoutedEvent routedEvent, CancellationToken cancellationToken = default)
        {
            _entries.Enqueue(routedEvent);
            _inboxActivity.NotifyNewEntriesAdded();
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<RoutedEvent>> Read(int count, CancellationToken cancellationToken = default)
        {
            var events = new List<RoutedEvent>();

            for (var i = 0; i < count; i++)
            {
                if (_entries.TryDequeue(out var nextEntry))
                {
                    events.Add(nextEntry);
                }
                else
                {
                    break;
                }
            }

            return Task.FromResult<IReadOnlyCollection<RoutedEvent>>(events);
        }

        public Task MarkAsHandled(Guid eventId, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task MarkAsFailed(Guid eventId, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
