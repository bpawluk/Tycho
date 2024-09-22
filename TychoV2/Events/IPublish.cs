using System.Threading;
using System.Threading.Tasks;

namespace TychoV2.Events
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IPublish<TEvent> 
        where TEvent : class, IEvent 
    {
        Task Publish(TEvent eventData, CancellationToken cancellationToken = default);
    }
}
