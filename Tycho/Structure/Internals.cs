using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tycho.Events.Delivery;
using Tycho.Identity.Structure;
using Tycho.Logging;
using Tycho.Modules.Instance;

namespace Tycho.Structure
{
    internal abstract class Internals : IServiceProvider, IRunnable, IDisposable
    {
        private HostApplicationBuilder? _hostBuilder;
        private IHost? _host;
        private int _disposed;

        public ControlPlane ControlPlane { get; }
        public InstanceIdentity OwnerId { get; }

        protected Internals(HostApplicationBuilder hostBuilder, ControlPlane controlPlane, InstanceIdentity instanceId)
        {
            _hostBuilder = hostBuilder;
            ControlPlane = controlPlane;
            OwnerId = instanceId;
            hostBuilder.Services.AddSingleton(ControlPlane);
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

                try
                {
                    ControlPlane.RegisterModule(OwnerId, new ModuleReference(new DeliveryEndpoint(this)));
                }
                catch
                {
                    Dispose();
                    throw;
                }
            }
        }

        internal void MaterializeModules(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            ThrowIfNotBuilt();

            cancellationToken.ThrowIfCancellationRequested();

            foreach (IModule module in _host!.Services.GetServices<IModule>())
            {
                cancellationToken.ThrowIfCancellationRequested();
                module.Internals.MaterializeModules(cancellationToken);
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

            PrepareForStart(cancellationToken);

            await _host!.StartAsync(cancellationToken).ConfigureAwait(false);
            _host.Services.GetService<ILogger<Internals>>()?.TychoHostStarted(OwnerId.Value);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            ThrowIfNotBuilt();

            await _host!.StopAsync(cancellationToken).ConfigureAwait(false);
            _host.Services.GetService<ILogger<Internals>>()?.TychoHostStopped(OwnerId.Value);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            _host?.Dispose();
        }

        protected abstract void PrepareForStart(CancellationToken cancellationToken);

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
                throw new ObjectDisposedException(OwnerId.Value);
            }
        }
    }
}
