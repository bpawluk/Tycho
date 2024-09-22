using System.Threading;
using System.Threading.Tasks;

namespace NewTycho.Events.Publishing
{
    internal class EventPublisher<TEvent> : IPublish<TEvent> where TEvent : class, IEvent
    {
        private readonly EventBroker _eventBroker;

        public EventPublisher(EventBroker eventBroker)
        {
            _eventBroker = eventBroker;
        }

        public Task Publish(TEvent eventData, CancellationToken cancellationToken)
        {
            return _eventBroker.Publish(eventData, cancellationToken);
        }
    }
}
