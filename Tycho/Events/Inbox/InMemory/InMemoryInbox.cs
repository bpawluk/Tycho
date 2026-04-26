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

        public Task<IReadOnlyCollection<RoutedEvent>> Read(int count, CancellationToken cancellationToken = default)
        {
            var events = new List<RoutedEvent>();

            for (var i = 0; i < count; i++)
            {
                if (_entries.TryDequeue(out var nextEntry))
                {
                    var deserializedEvent = _eventSerializer.Deserialize(nextEntry);
                    events.Add(deserializedEvent);
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
