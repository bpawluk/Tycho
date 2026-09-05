using System;
using System.Collections.Concurrent;
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

        public Task<InboxEvent?> TryReadAsync(CancellationToken cancellationToken = default)
        {
            if (_entries.TryDequeue(out SerializedRoutedEvent? nextEntry))
            {
                RoutedEvent deserializedEvent = _eventSerializer.Deserialize(nextEntry);
                return Task.FromResult<InboxEvent?>(new InboxEvent(Guid.Empty, deserializedEvent));
            }

            return Task.FromResult<InboxEvent?>(null);
        }

        public Task<bool> MarkAsHandledAsync(Guid claimId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        public Task<bool> MarkAsFailedAsync(Guid claimId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }
    }
}
