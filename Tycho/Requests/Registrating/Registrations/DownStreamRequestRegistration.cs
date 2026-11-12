using Tycho.Modules;

namespace Tycho.Requests.Registrating.Registrations
{
    internal class DownStreamRequestRegistration<TRequest, THandler, TModule>
        : IDownStreamRequestRegistration<TRequest, TModule>
        where TRequest : class, IRequest
        where THandler : class, IRequestHandler<TRequest>
        where TModule : TychoModule
    {
        public IRequestHandler<TRequest> Handler { get; }

        public DownStreamRequestRegistration(THandler handler)
        {
            Handler = handler;
        }
    }

    internal class DownStreamRequestRegistration<TRequest, TResponse, THandler, TModule>
        : IDownStreamRequestRegistration<TRequest, TResponse, TModule>
        where TRequest : class, IRequest<TResponse>
        where THandler : class, IRequestHandler<TRequest, TResponse>
        where TModule : TychoModule
    {
        public IRequestHandler<TRequest, TResponse> Handler { get; }

        public DownStreamRequestRegistration(THandler handler)
        {
            Handler = handler;
        }
    }
}
