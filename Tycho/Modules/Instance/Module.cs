using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Broker;
using Tycho.Identity.Modules;
using Tycho.Requests.Broker;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Modules.Instance
{
    [ReferencedByReflection]
    internal class Module<TModuleDefinition> : IModule<TModuleDefinition>
        where TModuleDefinition : TychoModule
    {
        private readonly ModuleIdentity _identity;
        private readonly Internals _internals;
        private readonly IRequestBroker _requestBroker;
        private readonly IEventBroker _eventBroker;

        private readonly Func<IServiceProvider, Task> _cleanup;

        ModuleIdentity IModule.Identity => _identity;
        Internals IModule.Internals => _internals;
        IEventBroker IModule.EventBroker => _eventBroker;
        IRequestBroker IModule.RequestBroker => _requestBroker;

        [ReferencedByReflection]
        public Module(Internals internals, Func<IServiceProvider, Task> cleanup)
        {
            _identity = ModuleIdentity.Create<TModuleDefinition>();
            _internals = internals;
            _eventBroker = new EventBroker(_internals);
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
