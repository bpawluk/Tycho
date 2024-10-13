namespace Tycho.Requests.Registrating.Registrations
{
    internal class UpStreamHandlerRegistration<TRequest, THandler> : IUpStreamHandlerRegistration<TRequest>
        where TRequest : class, IRequest
        where THandler : class, IRequestHandler<TRequest>
    {
        public IRequestHandler<TRequest> Handler { get; }

        public UpStreamHandlerRegistration(THandler handler)
        {
            Handler = handler;
        }
    }

    internal class UpStreamHandlerRegistration<TRequest, TResponse, THandler> : IUpStreamHandlerRegistration<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where THandler : class, IRequestHandler<TRequest, TResponse>
    {
        public IRequestHandler<TRequest, TResponse> Handler { get; }

        public UpStreamHandlerRegistration(THandler handler)
        {
            Handler = handler;
        }
    }
}
