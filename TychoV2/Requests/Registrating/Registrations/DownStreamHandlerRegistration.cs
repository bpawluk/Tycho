using TychoV2.Modules;

namespace TychoV2.Requests.Registrating.Registrations
{
    internal class DownStreamHandlerRegistration<TRequest, THandler, TModule> : IDownStreamHandlerRegistration<TRequest, TModule>
        where TRequest : class, IRequest
        where THandler : class, IHandle<TRequest>
        where TModule : TychoModule
    {
        public IHandle<TRequest> Handler { get; }

        public DownStreamHandlerRegistration(THandler handler)
        {
            Handler = handler;
        }
    }

    internal class DownStreamHandlerRegistration<TRequest, TResponse, THandler, TModule> : IDownStreamHandlerRegistration<TRequest, TResponse, TModule>
        where TRequest : class, IRequest<TResponse>
        where THandler : class, IHandle<TRequest, TResponse>
        where TModule : TychoModule
    {
        public IHandle<TRequest, TResponse> Handler { get; }

        public DownStreamHandlerRegistration(THandler handler)
        {
            Handler = handler;
        }
    }
}
