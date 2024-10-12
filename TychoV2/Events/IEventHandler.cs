using System.Threading;
using System.Threading.Tasks;

namespace TychoV2.Events
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IEventHandler { }

    /// <summary>
    /// TODO
    /// </summary>
    public interface IEventHandler<TEvent>
        where TEvent : class, IEvent 
    {
        Task Handle(TEvent eventData, CancellationToken cancellationToken = default);
    }
}
