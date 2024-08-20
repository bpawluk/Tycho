using Tycho.Messaging.Payload;

namespace Tycho.Contract.Outbox
{
    /// <summary>
    /// Lets you define all the outgoing <b>events</b> that your module will send out.
    /// </summary>
    public interface IEventOutboxDefinition
    {
        /// <summary>
        /// Declares that the specified <b>event</b> is published by your module
        /// </summary>
        /// <typeparam name="Event">The type of the event being published</typeparam>
        IOutboxDefinition Declare<Event>()
            where Event : class, IEvent;
    }
}
