namespace Tycho.Requests.Broker
{
    internal interface IRequestBroker : IRequestExecutor
    {
        bool CanExecute<TRequest>()
            where TRequest : class, IRequest;

        bool CanExecute<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }
}