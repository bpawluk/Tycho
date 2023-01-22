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
            _submodule = (supermodule as ModuleInternals)!.GetSubmodule<Definition>();
        }

        public void Publish<Event>(Event eventData, CancellationToken cancellationToken)
            where Event : class, IEvent
        {
            _submodule.Publish(eventData, cancellationToken);
        }

        public Task Execute<Command>(Command commandData, CancellationToken cancellationToken)
            where Command : class, ICommand
        {
            return _submodule.Execute(commandData, cancellationToken);
        }

        public Task<Response> Execute<Query, Response>(Query queryData, CancellationToken cancellationToken)
            where Query : class, IQuery<Response>
        {
            return _submodule.Execute<Query, Response>(queryData, cancellationToken);
        }
    }
}
