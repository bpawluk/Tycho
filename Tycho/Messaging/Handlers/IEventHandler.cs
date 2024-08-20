using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Handlers
{
    /// <summary>
    /// An interface that represents an event handler
    /// </summary>
    /// <typeparam name="Event">The type of the event being handled</typeparam>
    public interface IEventHandler<in Event> : IMessageHandler
        where Event : class, IEvent
    {
        /// <summary>
        /// A method to be executed when the specified event is published
        /// </summary>
        /// <param name="eventData">An object that represents the event being handled</param>
        /// <param name="cancellationToken">A token that notifies when the operation should be canceled</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task Handle(Event eventData, CancellationToken cancellationToken = default);
    }
}
