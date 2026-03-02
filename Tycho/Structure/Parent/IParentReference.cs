using Tycho.Events.Routing;
using Tycho.Requests.Broker;

namespace Tycho.Structure.Parent
{
    /// <summary>
    /// Represents a parent that owns the current module.
    /// </summary>
    public interface IParentReference
    {
        internal IEventRouter EventRouter { get; }
        internal IRequestBroker RequestBroker { get; }
    }
}
