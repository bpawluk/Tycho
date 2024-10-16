using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events.Handling
{
    internal class WrappingHandler<TEvent, TTargetEvent> : IEventHandler<TEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        private readonly IEventHandler<TTargetEvent> _wrappedHandler;
        private readonly Func<TEvent, TTargetEvent> _map;

        public WrappingHandler(IEventHandler<TTargetEvent> wrappedHandler, Func<TEvent, TTargetEvent> map)
        {
            _wrappedHandler = wrappedHandler;
            _map = map;
        }

        public Task Handle(TEvent eventData, CancellationToken cancellationToken)
        {
            var mappedEventData = _map(eventData);
            return _wrappedHandler.Handle(mappedEventData, cancellationToken);
        }
    }
}