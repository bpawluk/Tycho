using Tycho.Events.Routing;
using Tycho.Requests;

namespace Tycho.Structure
{
    /// <summary>
    ///     TODO
    /// </summary>
    public interface IParent : IRequestExecutor
    {
        internal IEventRouter EventRouter { get; }
    }
}