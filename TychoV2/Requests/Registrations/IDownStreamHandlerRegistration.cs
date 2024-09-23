using TychoV2.Modules;

namespace TychoV2.Requests.Registrations
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
