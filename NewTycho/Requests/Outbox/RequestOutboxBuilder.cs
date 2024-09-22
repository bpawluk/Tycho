using NewTycho.Requests.Execution;

namespace NewTycho.Requests.Outbox
{
    internal class RequestOutboxBuilder : IRequestOutboxDefinition, IRequestOutboxConsumer
    {
        IRequestOutboxDefinition IRequestOutboxDefinition.Declare<TRequest>()
        {
            throw new System.NotImplementedException();
        }

        IRequestOutboxDefinition IRequestOutboxDefinition.Declare<TRequest, TResponse>()
        {
            throw new System.NotImplementedException();
        }

        IRequestOutboxConsumer IRequestOutboxConsumer.Expose<TRequest>()
        {
            throw new System.NotImplementedException();
        }

        IRequestOutboxConsumer IRequestOutboxConsumer.Expose<TRequest, TResponse>()
        {
            throw new System.NotImplementedException();
        }

        IRequestOutboxConsumer IRequestOutboxConsumer.Forward<TRequest, TModule>()
        {
            throw new System.NotImplementedException();
        }

        IRequestOutboxConsumer IRequestOutboxConsumer.Forward<TRequest, TResponse, TModule>()
        {
            throw new System.NotImplementedException();
        }

        IRequestOutboxConsumer IRequestOutboxConsumer.Handle<TRequest, THandler>()
        {
            throw new System.NotImplementedException();
        }

        IRequestOutboxConsumer IRequestOutboxConsumer.Handle<TRequest, TResponse, THandler>()
        {
            throw new System.NotImplementedException();
        }

        IRequestOutboxConsumer IRequestOutboxConsumer.Ignore<TRequest>()
        {
            throw new System.NotImplementedException();
        }

        IRequestOutboxConsumer IRequestOutboxConsumer.Ignore<TRequest, TResponse>(TResponse response)
        {
            throw new System.NotImplementedException();
        }

        public RequestBroker Build()
        {
            throw new System.NotImplementedException();
        }
    }
}
