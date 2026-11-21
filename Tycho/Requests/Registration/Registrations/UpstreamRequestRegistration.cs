namespace Tycho.Requests.Registration.Registrations
{
    internal class UpstreamRequestRegistration<TRequest, THandler>
        : IUpstreamRequestRegistration<TRequest>
        where TRequest : class, IRequest
        where THandler : class, IRequestHandler<TRequest>
    {
        public IRequestHandler<TRequest> Handler { get; }

        public UpstreamRequestRegistration(THandler handler)
        {
            Handler = handler;
        }
    }

    internal class UpstreamRequestRegistration<TRequest, TResponse, THandler>
        : IUpstreamRequestRegistration<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where THandler : class, IRequestHandler<TRequest, TResponse>
    {
        public IRequestHandler<TRequest, TResponse> Handler { get; }

        public UpstreamRequestRegistration(THandler handler)
        {
            Handler = handler;
        }
    }
}
