namespace Tycho.Requests.Registrating.Registrations
{
    internal class UpStreamRequestRegistration<TRequest, THandler>
        : IUpStreamRequestRegistration<TRequest>
        where TRequest : class, IRequest
        where THandler : class, IRequestHandler<TRequest>
    {
        public IRequestHandler<TRequest> Handler { get; }

        public UpStreamRequestRegistration(THandler handler)
        {
            Handler = handler;
        }
    }

    internal class UpStreamRequestRegistration<TRequest, TResponse, THandler>
        : IUpStreamRequestRegistration<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where THandler : class, IRequestHandler<TRequest, TResponse>
    {
        public IRequestHandler<TRequest, TResponse> Handler { get; }

        public UpStreamRequestRegistration(THandler handler)
        {
            Handler = handler;
        }
    }
}
