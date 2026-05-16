using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Identity.Modules;
using Tycho.Modules.Instance;
using Tycho.Requests.Broker;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Apps.Instance
{
    [ReferencedByReflection]
    internal class App<TAppDefinition> : IApp<TAppDefinition>
        where TAppDefinition : TychoApp
    {
        private readonly Internals _internals;
        private readonly IRequestBroker _requestBroker;

        private readonly Func<IServiceProvider, Task> _cleanup;

        Internals IApp.Internals => _internals;
        IRequestBroker IApp.RequestBroker => _requestBroker;

        [ReferencedByReflection]
        public App(Internals internals, Func<IServiceProvider, Task> cleanup)
        {
            _internals = internals;
            _requestBroker = new UpStreamBroker(_internals);
            _cleanup = cleanup;
        }

        public async ValueTask DisposeAsync()
        {
            IModuleProvider moduleProvider = _internals.GetRequiredService<IModuleProvider>();

            try
            {
                await _cleanup(_internals).ConfigureAwait(false);
            }
            catch { }

            foreach (IModule module in moduleProvider.GetAllModules())
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
