using Tycho.Events.Routing;
using Tycho.Requests.Broker;

namespace Tycho.Structure.External
{
    /// <summary>
    /// Represents a parent that owns the current module.
    /// </summary>
    public interface IParent
    {
        internal IEventRouter EventRouter { get; }
        internal IRequestBroker RequestBroker { get; }
    }
}
