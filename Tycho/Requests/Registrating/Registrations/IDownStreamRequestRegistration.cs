using Tycho.Modules;

namespace Tycho.Requests.Registrating.Registrations
{
    internal interface IDownStreamRequestRegistration<TRequest, TModule>
        : IRequestRegistration<TRequest>
        where TRequest : class, IRequest
        where TModule : TychoModule
    {
    }

    internal interface IDownStreamRequestRegistration<TRequest, TResponse, TModule>
        : IRequestRegistration<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where TModule : TychoModule
    {
    }
}
