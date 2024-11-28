using System.Threading.Tasks;
using System.Threading;

namespace Tycho.Events.Publishing
{
    internal interface IUncommittedEventPublisher
    {
        Task PublishWithoutCommitting<TEvent>(TEvent eventData, CancellationToken cancellationToken = default)
            where TEvent : class, IEvent;
    }
}
