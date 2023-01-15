using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;
using Tycho.Structure.Modules;

namespace Tycho.Structure.Submodules
{
    internal class SubmoduleProxy<Definition> : ISubmodule<Definition>
        where Definition : TychoModule
    {
        private readonly IModule _submodule;

        public SubmoduleProxy(IModule supermodule)
        {
            _submodule = (supermodule as Module)!.GetSubmodule<Definition>();
        }

        public void PublishEvent<Event>(Event eventData, CancellationToken cancellationToken)
            where Event : class, IEvent
        {
            _submodule.PublishEvent(eventData, cancellationToken);
        }

        public Task ExecuteCommand<Command>(Command commandData, CancellationToken cancellationToken)
            where Command : class, ICommand
        {
            return _submodule.ExecuteCommand(commandData, cancellationToken);
        }

        public Task<Response> ExecuteQuery<Query, Response>(Query queryData, CancellationToken cancellationToken)
            where Query : class, IQuery<Response>
        {
            return _submodule.ExecuteQuery<Query, Response>(queryData, cancellationToken);
        }
    }
}
