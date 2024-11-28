using Tycho.Events.Routing;
using Tycho.Requests;

namespace Tycho.Structure
{
    /// <summary>
    /// An interface for a parent that owns the current module
    /// </summary>
    public interface IParent : IRequestExecutor
    {
        internal IEventRouter EventRouter { get; }
    }
}