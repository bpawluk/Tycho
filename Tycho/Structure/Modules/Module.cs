using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging;
using Tycho.Messaging.Payload;

namespace Tycho.Structure.Modules
{
    internal class Module : IModule
    {
        public void PublishEvent<Event>(Event eventData, CancellationToken cancellationToken)
            where Event : class, IEvent
        {

        }

        public Task ExecuteCommand<Command>(Command commandData, CancellationToken cancellationToken)
            where Command : class, ICommand
        {
            return Task.CompletedTask;
        }

        public Task<Response> ExecuteQuery<Query, Response>(Query queryData, CancellationToken cancellationToken)
            where Query : class, IQuery<Response>
        {
            return Task.FromResult<Response>(default!);
        }

        public IModule GetSubmodule<T>()
        {
            throw new System.NotImplementedException();
        }

        internal void AddSubmodules(IEnumerable<IModule> submodules)
        {

        }

        internal void SetInternalBroker(IMessageBroker internalBroker)
        {

        }

        internal void SetExternalBroker(IMessageBroker internalBroker)
        {

        }
    }
}
