namespace TychoV2.Requests.Registrating.Registrations
{
    internal interface IHandlerRegistration
    {
    }

    internal interface IHandlerRegistration<TRequest> : IHandlerRegistration
        where TRequest : class, IRequest
    {
        IHandle<TRequest> Handler { get; }
    }

    internal interface IHandlerRegistration<TRequest, TResponse> : IHandlerRegistration
        where TRequest : class, IRequest<TResponse>
    {
        IHandle<TRequest, TResponse> Handler { get; }
    }
}
