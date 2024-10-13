using Tycho.Modules;

namespace Tycho.Requests.Registrating.Registrations
{
    internal interface IDownStreamHandlerRegistration<TRequest, TModule> : IHandlerRegistration<TRequest>
        where TRequest : class, IRequest
        where TModule : TychoModule
    {
    }

    internal interface IDownStreamHandlerRegistration<TRequest, TResponse, TModule> : IHandlerRegistration<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where TModule : TychoModule
    {
    }
}
