using System.Threading;
using System.Threading.Tasks;

namespace TychoV2.Events
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IHandle { }

    /// <summary>
    /// TODO
    /// </summary>
    public interface IHandle<TEvent>
        where TEvent : class, IEvent 
    {
        Task Handle(TEvent eventData, CancellationToken cancellationToken = default);
    }
}
