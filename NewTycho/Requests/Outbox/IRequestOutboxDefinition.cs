namespace NewTycho.Requests.Outbox
{
    /// <summary>
    /// Lets you define all the outgoing <b>requests</b> that your module will send out.
    /// </summary>
    public interface IRequestOutboxDefinition
    {
        /// <summary>
        /// Declares that the specified <b>request</b> is sent by your module
        /// </summary>
        /// <typeparam name="TRequest">The type of the request being sent</typeparam>
        IRequestOutboxDefinition Declare<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Declares that the specified <b>request</b> is sent by your module
        /// </summary>
        /// <typeparam name="TRequest">The type of the request being sent</typeparam>
        /// <typeparam name="TResponse">The type of the response</typeparam>
        IRequestOutboxDefinition Declare<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }
}
