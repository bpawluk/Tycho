namespace TychoV2.Requests.Broker
{
    internal interface IRequestBroker : IRequestExecutor
    {
        public bool CanExecute<TRequest>()
            where TRequest : class, IRequest;

        public bool CanExecute<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }
}
