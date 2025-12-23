using Tycho.Identities;

namespace Tycho.Events.Handling
{
    internal interface IEventHandlerProvider
    {
        IEventHandler GetHandler(EventHandlerIdentity eventHandlerId);
    }
}
