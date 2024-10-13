using Tycho.Events.Routing;
using Tycho.Requests;

namespace Tycho.Structure
{
    public interface IParent : IRequestExecutor
    {
        internal IEventRouter EventRouter { get; }
    }
}
