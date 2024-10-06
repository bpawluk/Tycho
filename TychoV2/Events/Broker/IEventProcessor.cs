using System.Threading.Tasks;
using System.Threading;

namespace TychoV2.Events.Broker
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IEventProcessor
    {
        Task<bool> Process<TEvent>(
            string handlerId,
            TEvent eventData,
            CancellationToken cancellationToken = default)
            where TEvent : class, IEvent;
    }
}
