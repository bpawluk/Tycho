using Tycho.Messaging.Payload;

namespace Tycho.Contract
{
    /// <summary>
    /// Lets you define all the outgoing messages that your module will send out.
    /// These messages make up the contract that your module requires its clients to fulfill
    /// </summary>
    public interface IOutboxDefinition
    {
        /// <summary>
        /// Declares that the specified <b>event</b> message is published by your module
        /// </summary>
        /// <typeparam name="Event">The type of the event being published</typeparam>
        IOutboxDefinition Publishes<Event>()
            where Event : class, IEvent;

        /// <summary>
        /// Declares that the specified <b>command</b> message is sent by your module
        /// </summary>
        /// <typeparam name="Command">The type of the command being sent</typeparam>
        IOutboxDefinition Sends<Command>()
            where Command : class, ICommand;

        /// <summary>
        /// Declares that the specified <b>query</b> message is sent by your module
        /// </summary>
        /// <typeparam name="Query">The type of the query being sent</typeparam>
        /// <typeparam name="Response">The type of the query response</typeparam>
        IOutboxDefinition Sends<Query, Response>()
            where Query : class, IQuery<Response>;
    }
}
