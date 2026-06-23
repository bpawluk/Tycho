using Tycho.Events.Broker;
using Tycho.Requests.Broker;
using Tycho.Utils;

namespace Tycho.Structure.Parent
{
    /// <summary>
    /// Represents a parent that owns the current Module.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IParentReference
    {
        internal IEventBroker EventBroker { get; }
        internal IRequestBroker RequestBroker { get; }
    }
}
