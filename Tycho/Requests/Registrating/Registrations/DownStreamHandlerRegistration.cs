using Tycho.Modules;

namespace Tycho.Requests.Registrating.Registrations
{
    internal class DownStreamHandlerRegistration<TRequest, THandler, TModule> 
        : IDownStreamHandlerRegistration<TRequest, TModule>
        where TRequest : class, IRequest
        where THandler : class, IRequestHandler<TRequest>
        where TModule : TychoModule
    {
        public IRequestHandler<TRequest> Handler { get; }

        public DownStreamHandlerRegistration(THandler handler)
        {
            Handler = handler;
        }
    }

    internal class DownStreamHandlerRegistration<TRequest, TResponse, THandler, TModule> 
        : IDownStreamHandlerRegistration<TRequest, TResponse, TModule>
        where TRequest : class, IRequest<TResponse>
        where THandler : class, IRequestHandler<TRequest, TResponse>
        where TModule : TychoModule
    {
        public IRequestHandler<TRequest, TResponse> Handler { get; }

        public DownStreamHandlerRegistration(THandler handler)
        {
            Handler = handler;
        }
    }
}