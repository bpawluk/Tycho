using Tycho.Identity.Events;

namespace Tycho.Events.Registrating
{
    internal interface IHandlerRegistration<TEvent> : IEventHandlerIdentity
        where TEvent : class, IEvent
    {
        IEventHandler<TEvent> Handler { get; }
    }
}
