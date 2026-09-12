using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Tycho.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Hosting.Services
{
    internal class ModuleHostedLifecycleService<TModuleDefinition> : IHostedLifecycleService
        where TModuleDefinition : TychoModule
    {
        private readonly IModule<TModuleDefinition> _module;

        public ModuleHostedLifecycleService(IModule<TModuleDefinition> module)
        {
            _module = module;
        }

        public Task StartingAsync(CancellationToken cancellationToken) => _module.StartAsync(cancellationToken);

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppedAsync(CancellationToken cancellationToken) => _module.StopAsync(cancellationToken);
    }
}
