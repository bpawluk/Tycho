namespace Tycho.Requests.Registrating.Registrations
{
    internal interface IUpStreamRequestRegistration<TRequest>
        : IRequestRegistration<TRequest>
        where TRequest : class, IRequest
    {
    }

    internal interface IUpStreamRequestRegistration<TRequest, TResponse>
        : IRequestRegistration<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
    }
}
