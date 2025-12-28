using Tycho.Events;

namespace Tycho.Identities.Providers
{
    internal interface IEventHandlerProvider
    {
        IEventHandler GetHandler(EventHandlerIdentity eventHandlerId);
    }
}
