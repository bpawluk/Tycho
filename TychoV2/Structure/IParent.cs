using TychoV2.Events.Routing;
using TychoV2.Requests;

namespace TychoV2.Structure
{
    public interface IParent : IExecute
    {
        internal IEventRouter EventRouter { get; }
    }
}
