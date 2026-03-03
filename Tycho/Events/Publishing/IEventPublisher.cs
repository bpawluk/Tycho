using System.Threading;
using System.Threading.Tasks;
using Tycho.Utils;

namespace Tycho.Events.Publishing
{
    /// <summary>
    /// An interface for publishing events.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IEventPublisher
    {
        internal Task PublishAsync<TEvent>(TEvent eventPayload, CancellationToken cancellationToken = default)
            where TEvent : class, IEvent;
    }
}
