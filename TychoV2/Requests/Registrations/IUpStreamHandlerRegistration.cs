namespace TychoV2.Requests.Registrations
{
    internal interface IUpStreamHandlerRegistration<TRequest> : IHandlerRegistration<TRequest>
        where TRequest : class, IRequest
    {
    }

    internal interface IUpStreamHandlerRegistration<TRequest, TResponse> : IHandlerRegistration<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
    }
}
