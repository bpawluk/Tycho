using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events.Publishing
{
    internal interface IUncommittedEventPublisher
    {
        Task PublishWithoutCommitting<TEvent>(
            TEvent eventPayload, 
            CancellationToken cancellationToken = default) 
            where TEvent : class, IEvent;
    }
}
