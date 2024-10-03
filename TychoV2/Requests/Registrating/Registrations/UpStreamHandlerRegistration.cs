namespace TychoV2.Requests.Registrating.Registrations
{
    internal class UpStreamHandlerRegistration<TRequest, THandler> : IUpStreamHandlerRegistration<TRequest>
        where TRequest : class, IRequest
        where THandler : class, IHandle<TRequest>
    {
        public IHandle<TRequest> Handler { get; }

        public UpStreamHandlerRegistration(THandler handler)
        {
            Handler = handler;
        }
    }

    internal class UpStreamHandlerRegistration<TRequest, TResponse, THandler> : IUpStreamHandlerRegistration<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where THandler : class, IHandle<TRequest, TResponse>
    {
        public IHandle<TRequest, TResponse> Handler { get; }

        public UpStreamHandlerRegistration(THandler handler)
        {
            Handler = handler;
        }
    }
}
