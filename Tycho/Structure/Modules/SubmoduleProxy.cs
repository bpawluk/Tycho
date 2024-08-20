using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Structure.Modules
{
    internal class SubmoduleProxy<Definition> : IModule<Definition>
        where Definition : TychoModule
    {
        private readonly IModule _submodule;

        public SubmoduleProxy(IModule module)
        {
            _submodule = (module as ModuleInternals)!.GetSubmodule<Definition>();
        }

        public void Publish<Event>(Event eventData, CancellationToken cancellationToken)
            where Event : class, IEvent
        {
            _submodule.Publish(eventData, cancellationToken);
        }

        public Task Execute<Request>(Request requestData, CancellationToken cancellationToken)
            where Request : class, IRequest
        {
            return _submodule.Execute(requestData, cancellationToken);
        }

        public Task<Response> Execute<Request, Response>(Request requestData, CancellationToken cancellationToken)
            where Request : class, IRequest<Response>
        {
            return _submodule.Execute<Request, Response>(requestData, cancellationToken);
        }
    }
}
