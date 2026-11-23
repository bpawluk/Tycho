using Tycho.Modules;

namespace Tycho.Requests.Registration.Registrations
{
    internal class DownstreamRequestRegistration<TRequest, THandler, TModule>
        : IDownstreamRequestRegistration<TRequest, TModule>
        where TRequest : class, IRequest
        where THandler : class, IRequestHandler<TRequest>
        where TModule : TychoModule
    {
        public IRequestHandler<TRequest> Handler { get; }

        public DownstreamRequestRegistration(THandler handler)
        {
            Handler = handler;
        }
    }

    internal class DownstreamRequestRegistration<TRequest, TResponse, THandler, TModule>
        : IDownstreamRequestRegistration<TRequest, TResponse, TModule>
        where TRequest : class, IRequest<TResponse>
        where THandler : class, IRequestHandler<TRequest, TResponse>
        where TModule : TychoModule
    {
        public IRequestHandler<TRequest, TResponse> Handler { get; }

        public DownstreamRequestRegistration(THandler handler)
        {
            Handler = handler;
        }
    }
}
