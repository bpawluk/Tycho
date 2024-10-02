using System.Threading.Tasks;
using System.Threading;

namespace TychoV2.Events.Outbox
{
    internal interface IProcess<TEvent>
        where TEvent : class, IEvent
    {
        Task<bool> Process(
            string handlerId,
            TEvent eventData,
            CancellationToken cancellationToken = default);
    }
}
