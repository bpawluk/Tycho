using System.Threading;
using System.Threading.Tasks;

namespace NewTycho.Events
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IHandle<TEvent> : Messaging.IHandle<TEvent>
        where TEvent : class, IEvent 
    {
        Task Handle(TEvent eventData, CancellationToken cancellationToken = default);
    }
}
