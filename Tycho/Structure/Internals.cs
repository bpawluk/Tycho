using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Tycho.Structure
{
    internal class Internals : IServiceProvider, IRunnable, IDisposable
    {
        private HostApplicationBuilder? _hostBuilder;
        private IHost? _host;
        private int _disposed;

        public Type Owner { get; }

        public Internals(Type owner, HostApplicationBuilder hostBuilder)
        {
            Owner = owner;
            _hostBuilder = hostBuilder;
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

            return GetService(serviceType) != null;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            ThrowIfNotBuilt();
            return _host!.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            ThrowIfNotBuilt();
            return _host!.StopAsync(cancellationToken);
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
                throw new ObjectDisposedException(Owner.FullName ?? Owner.Name);
            }
        }
    }
}
