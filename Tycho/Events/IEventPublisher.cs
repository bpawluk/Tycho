using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IEventPublisher
    {
        Task Publish<TEvent>(TEvent eventData, CancellationToken cancellationToken = default)
            where TEvent : class, IEvent;
    }
}
