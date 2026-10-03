using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tycho.Identity.Structure;
using Tycho.Logging;

namespace Tycho.Structure
{
    internal class Internals : IServiceProvider, IRunnable, IDisposable
    {
        private HostApplicationBuilder? _hostBuilder;
        private IHost? _host;
        private int _disposed;

        public DefinitionIdentity OwnerDefinitionId { get; }
        public InstanceIdentity OwnerInstanceId { get; }

        public Internals(HostApplicationBuilder hostBuilder, Type ownerDefinition)
        {
            _hostBuilder = hostBuilder;
            OwnerDefinitionId = DefinitionIdentity.Create(ownerDefinition);
            OwnerInstanceId = InstanceIdentity.CreateRoot(OwnerDefinitionId);
        }

        public Internals(HostApplicationBuilder hostBuilder, Type ownerDefinition, InstanceIdentity parentIdentity)
        {
            _hostBuilder = hostBuilder;
            OwnerDefinitionId = DefinitionIdentity.Create(ownerDefinition);
            OwnerInstanceId = parentIdentity.CreateChild(OwnerDefinitionId);
        }

        public HostApplicationBuilder GetHostBuilder()
        {
            ThrowIfDisposed();
            ThrowIfBuilt();
            return _hostBuilder!;
        }

        public object GetService(Type serviceType)
        {
            ThrowIfDisposed();
            ThrowIfNotBuilt();
            return _host!.Services.GetService(serviceType)!;
        }

        public void Build()
        {
            ThrowIfDisposed();
            if (_host == null)
            {
                _host = _hostBuilder!.Build();
                _hostBuilder = null;
            }
        }

        public bool HasService<TServiceInterface>()
        {
            ThrowIfDisposed();

            Type serviceType = typeof(TServiceInterface);
            if (_hostBuilder != null)
            {
                return _hostBuilder.Services.Any(descriptor => descriptor.ServiceType == serviceType);
            }

            IServiceProviderIsService serviceProviderIsService = _host!.Services.GetRequiredService<IServiceProviderIsService>();
            return serviceProviderIsService.IsService(serviceType);
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            ThrowIfNotBuilt();
            await _host!.StartAsync(cancellationToken).ConfigureAwait(false);
            _host.Services.GetService<ILogger<Internals>>()?.TychoHostStarted(OwnerInstanceId.Value);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            ThrowIfNotBuilt();
            await _host!.StopAsync(cancellationToken).ConfigureAwait(false);
            _host.Services.GetService<ILogger<Internals>>()?.TychoHostStopped(OwnerInstanceId.Value);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            _host?.Dispose();
        }

        private void ThrowIfNotBuilt()
        {
            if (_host == null)
            {
                throw new InvalidOperationException("Internal host has not been built yet.");
            }
        }

        private void ThrowIfBuilt()
        {
            if (_hostBuilder == null)
            {
                throw new InvalidOperationException("Internal host has already been built.");
            }
        }

        private void ThrowIfDisposed()
        {
            if (Volatile.Read(ref _disposed) != 0)
            {
                throw new ObjectDisposedException(OwnerDefinitionId.Value);
            }
        }
    }
}
