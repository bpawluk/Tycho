using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;
using Tycho.Requests.Broker;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Modules.Instance
{
    [ReferencedByReflection]
    internal class Module<TModuleDefinition> : IModule<TModuleDefinition> where TModuleDefinition : TychoModule
    {
        private readonly Internals _internals;
        private readonly IRequestBroker _requestBroker;
        private readonly IEventRouter _eventRouter;

        Internals IModule.Internals => _internals;
        IEventRouter IModule.EventRouter => _eventRouter;
        IRequestBroker IModule.RequestBroker => _requestBroker;

        [ReferencedByReflection]
        public Module(Internals internals)
        {
            _internals = internals;
            _eventRouter = new EventRouter(_internals);
            _requestBroker = new UpStreamBroker(_internals);
        }

        public Task StartAsync(CancellationToken cancellationToken) => _internals.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => _internals.StopAsync(cancellationToken);

        public void Dispose() => _internals.Dispose();
    }
}
