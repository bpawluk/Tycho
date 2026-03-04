using Tycho.Identity.Events;

namespace Tycho.Events.Registrating
{
    internal interface IHandlerRegistration
    {
        EventHandlerIdentity Identity { get; }
    }

    internal interface IHandlerRegistration<TEvent> : IHandlerRegistration
        where TEvent : class, IEvent
    {
        IEventHandler<TEvent> Handler { get; }
    }
}
