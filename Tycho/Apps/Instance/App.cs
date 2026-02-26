using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Registry;
using Tycho.Requests.Broker;
using Tycho.Structure.Internal;

namespace Tycho.Apps.Instance
{
    internal class App<TAppDefinition> : IApp<TAppDefinition>
        where TAppDefinition : TychoApp
    {
        private readonly Internals _internals;
        private readonly IRequestBroker _requestBroker;

        private readonly Func<IServiceProvider, Task> _cleanup;

        Internals IApp.Internals => _internals;
        IRequestBroker IApp.RequestBroker => _requestBroker;

        public App(Internals internals, Func<IServiceProvider, Task> cleanup)
        {
            _internals = internals;
            _requestBroker = new UpStreamBroker(_internals);
            _cleanup = cleanup;
        }

        public async ValueTask DisposeAsync()
        {
            var moduleRegistry = _internals.GetRequiredService<IModuleRegistry>();

            try
            {
                await _cleanup(_internals).ConfigureAwait(false);
            }
            catch { }

            foreach (var module in moduleRegistry.GetAllModules())
            {
                try
                {
                    await module.DisposeAsync().ConfigureAwait(false);
                }
                catch { }
            }

            _internals.Dispose();
        }
    }
}
