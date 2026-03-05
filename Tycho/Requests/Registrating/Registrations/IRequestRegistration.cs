namespace Tycho.Requests.Registrating.Registrations
{
    internal interface IRequestRegistration
    {
    }

    internal interface IRequestRegistration<TRequest> : IRequestRegistration
        where TRequest : class, IRequest
    {
        IRequestHandler<TRequest> Handler { get; }
    }

    internal interface IRequestRegistration<TRequest, TResponse> : IRequestRegistration
        where TRequest : class, IRequest<TResponse>
    {
        IRequestHandler<TRequest, TResponse> Handler { get; }
    }
}