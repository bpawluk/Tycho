using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Broker;
using Tycho.Identity.Modules;
using Tycho.Requests.Broker;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Modules.Instance
{
    [ReferencedByReflection]
    internal class Module<TModuleDefinition> : IModule<TModuleDefinition> where TModuleDefinition : TychoModule
    {
        private readonly ModuleIdentity _identity;
        private readonly Internals _internals;
        private readonly IRequestBroker _requestBroker;
        private readonly IEventBroker _eventBroker;

        ModuleIdentity IModule.Identity => _identity;
        Internals IModule.Internals => _internals;
        IEventBroker IModule.EventBroker => _eventBroker;
        IRequestBroker IModule.RequestBroker => _requestBroker;

        [ReferencedByReflection]
        public Module(Internals internals)
        {
            _identity = ModuleIdentity.Create<TModuleDefinition>();
            _internals = internals;
            _eventBroker = new EventBroker(_internals);
            _requestBroker = new UpStreamBroker(_internals);
        }

        public Task StartAsync(CancellationToken cancellationToken = default) => _internals.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken = default) => _internals.StopAsync(cancellationToken);

        public void Dispose() => _internals.Dispose();
    }
}
