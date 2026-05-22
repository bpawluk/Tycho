using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;
using Tycho.Events.Serialization;

namespace Tycho.Events.Inbox.InMemory
{
    internal class InMemoryInbox : IInboxWriter, IInboxConsumer
    {
        private readonly IEventSerializer _eventSerializer;
        private readonly InboxActivity _inboxActivity;
        private readonly ConcurrentQueue<SerializedRoutedEvent> _entries;

        public InMemoryInbox(IEventSerializer eventSerializer, InboxActivity inboxActivity)
        {
            _eventSerializer = eventSerializer;
            _inboxActivity = inboxActivity;
            _entries = new ConcurrentQueue<SerializedRoutedEvent>();
        }

        public Task Write(SerializedRoutedEvent routedEvent, CancellationToken cancellationToken = default)
        {
            _entries.Enqueue(routedEvent);
            _inboxActivity.NotifyNewEntriesAdded();
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<InboxEvent>> Read(int count, CancellationToken cancellationToken = default)
        {
            var events = new List<InboxEvent>();

            for (int i = 0; i < count; i++)
            {
                if (_entries.TryDequeue(out SerializedRoutedEvent? nextEntry))
                {
                    RoutedEvent deserializedEvent = _eventSerializer.Deserialize(nextEntry);
                    events.Add(new InboxEvent(Guid.Empty, deserializedEvent));
                }
                else
                {
                    break;
                }
            }

            return Task.FromResult<IReadOnlyCollection<InboxEvent>>(events);
        }

        public Task<bool> MarkAsHandled(Guid eventId, Guid claimId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        public Task<bool> MarkAsFailed(Guid eventId, Guid claimId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }
    }
}
