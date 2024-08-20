using Tycho.Messaging.Payload;

namespace Tycho.Contract.Outbox
{
    /// <summary>
    /// Lets you define all the outgoing <b>requests</b> that your module will send out.
    /// </summary>
    public interface IRequestOutboxDefinition
    {
        /// <summary>
        /// Declares that the specified <b>request</b> is sent by your module
        /// </summary>
        /// <typeparam name="Request">The type of the request being sent</typeparam>
        IOutboxDefinition Declare<Request>()
            where Request : class, IRequest;

        /// <summary>
        /// Declares that the specified <b>request</b> is sent by your module
        /// </summary>
        /// <typeparam name="Request">The type of the request being sent</typeparam>
        /// <typeparam name="Response">The type of the response</typeparam>
        IOutboxDefinition Declare<Request, Response>()
            where Request : class, IRequest<Response>;
    }
}
