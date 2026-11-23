namespace Tycho.Requests.Registration.Registrations
{
    internal interface IUpstreamRequestRegistration<TRequest>
        : IRequestRegistration<TRequest>
        where TRequest : class, IRequest
    {
    }

    internal interface IUpstreamRequestRegistration<TRequest, TResponse>
        : IRequestRegistration<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
    }
}
