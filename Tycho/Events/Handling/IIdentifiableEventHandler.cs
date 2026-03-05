using Tycho.Identity.Events;

namespace Tycho.Events.Handling
{
    internal interface IIdentifiableEventHandler
    {
        EventHandlerIdentity Identity { get; }
    }
}
