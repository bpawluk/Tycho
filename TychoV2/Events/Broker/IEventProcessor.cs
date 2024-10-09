using System.Threading.Tasks;
using System.Threading;
using TychoV2.Events.Routing;

namespace TychoV2.Events.Broker
{
    internal interface IEventProcessor
    {
        Task<bool> Process<TEvent>(
            HandlerIdentity handlerIdentity,
            TEvent eventData,
            CancellationToken cancellationToken = default)
            where TEvent : class, IEvent;
    }
}
