using System.Threading.Tasks;
using System.Threading;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Interceptors
{
    /// <summary>
    /// Lets you run additional logic before and after forwarding an event
    /// </summary>
    /// <typeparam name="Event">The type of the event being intercepted</typeparam>
    public interface IEventInterceptor<Event> where Event : class, IEvent
    {
        /// <summary>
        /// A method to be executed before the specified event is forwarded
        /// </summary>
        /// <param name="eventData">An object that represents the event being intercepted</param>
        /// <param name="cancellationToken">A token that notifies when the operation should be canceled</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ExecuteBefore(Event eventData, CancellationToken cancellationToken = default);

        /// <summary>
        /// A method to be executed after the specified event is forwarded
        /// </summary>
        /// <param name="eventData">An object that represents the event being intercepted</param>
        /// <param name="cancellationToken">A token that notifies when the operation should be canceled</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ExecuteAfter(Event eventData, CancellationToken cancellationToken = default);
    }
}
