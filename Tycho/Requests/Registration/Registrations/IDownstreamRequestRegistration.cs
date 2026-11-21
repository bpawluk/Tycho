using Tycho.Modules;

namespace Tycho.Requests.Registration.Registrations
{
    internal interface IDownstreamRequestRegistration<TRequest, TModule>
        : IRequestRegistration<TRequest>
        where TRequest : class, IRequest
        where TModule : TychoModule
    {
    }

    internal interface IDownstreamRequestRegistration<TRequest, TResponse, TModule>
        : IRequestRegistration<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where TModule : TychoModule
    {
    }
}
