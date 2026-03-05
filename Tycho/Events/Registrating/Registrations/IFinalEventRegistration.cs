using Tycho.Identity.Events;

namespace Tycho.Events.Registrating.Registrations
{
    internal interface IFinalEventRegistration<TEvent> : IEventRegistration<TEvent>
        where TEvent : class, IEvent
    {
        IEventHandler<TEvent> Handler { get; }

        EventHandlerIdentity HandlerId { get; }
    }
}
