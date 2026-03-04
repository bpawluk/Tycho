using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Routing;
using Tycho.Identity.Modules;
using Tycho.Requests.Broker;
using Tycho.Structure;

namespace Tycho.Modules.Instance
{
    internal class Module<TTychoDefinition> : IModule<TTychoDefinition>
        where TTychoDefinition : TychoModule
    {
        private readonly ModuleIdentity _identity;
        private readonly Internals _internals;
        private readonly IRequestBroker _requestBroker;
        private readonly IEventRouter _eventRouter;

        private readonly Func<IServiceProvider, Task> _cleanup;

        ModuleIdentity IModule.Identity => _identity;
        Internals IModule.Internals => _internals;
        IEventRouter IModule.EventRouter => _eventRouter;
        IRequestBroker IModule.RequestBroker => _requestBroker;

        public Module(Internals internals, Func<IServiceProvider, Task> cleanup)
        {
            _identity = ModuleIdentity.Create<TTychoDefinition>();
            _internals = internals;
            _eventRouter = new EventRouter(_internals);
            _requestBroker = new UpStreamBroker(_internals);
            _cleanup = cleanup;
        }

        public async ValueTask DisposeAsync()
        {
            var moduleProvider = _internals.GetRequiredService<IModuleProvider>();

            try
            {
                await _cleanup(_internals).ConfigureAwait(false);
            }
            catch { }

            foreach (var module in moduleProvider.GetAllModules())
            {
                try
                {
                    await module.DisposeAsync().ConfigureAwait(false);
                }
                catch { }
            }

            await _internals.DisposeAsync().ConfigureAwait(false);
        }
    }
}
