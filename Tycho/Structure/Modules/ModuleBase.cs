using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging;
using Tycho.Messaging.Payload;

namespace Tycho.Structure.Modules
{
    internal class ModuleBase : IModule
    {
        protected IMessageBroker? _messageBroker;

        public void Publish<Event>(Event eventData, CancellationToken cancellationToken)
            where Event : class, IEvent
        {
            if (_messageBroker is null)
            {
                throw new InvalidOperationException("Could not publish an event " +
                    "because this module does have a message broker defined");
            }
            _messageBroker.Publish(eventData, cancellationToken);
        }

        public Task Execute<Command>(Command commandData, CancellationToken cancellationToken)
            where Command : class, ICommand
        {
            if (_messageBroker is null)
            {
                throw new InvalidOperationException("Could not execute a command " +
                    "because this module does have a message broker defined");
            }
            return _messageBroker.Execute(commandData, cancellationToken);
        }

        public Task<Response> Execute<Query, Response>(Query queryData, CancellationToken cancellationToken)
            where Query : class, IQuery<Response>
        {
            if (_messageBroker is null)
            {
                throw new InvalidOperationException("Could not execute a query " +
                    "because this module does have a message broker defined");
            }
            return _messageBroker.Execute<Query, Response>(queryData, cancellationToken);
        }

        public void SetMessageBroker(IMessageBroker broker)
        {
            if (_messageBroker is null)
            {
                _messageBroker = broker;
            }
            else
            {
                throw new InvalidOperationException("Message broker for this module is already defined");
            }
        }
    }
}
