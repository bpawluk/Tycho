using Tycho.Identity.Events;

namespace Tycho.Events.Registration.Registrations
{
    internal interface IFinalEventRegistration<TEvent> : IEventRegistration<TEvent>
        where TEvent : class, IEvent
    {
        EventHandlerIdentity HandlerId { get; }
    }
}
