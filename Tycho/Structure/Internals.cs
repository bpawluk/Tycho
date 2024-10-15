using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Tycho.Structure
{
    internal class Internals : IServiceProvider
    {
        private IServiceCollection? _serviceCollection = new ServiceCollection();
        private IServiceProvider? _serviceProvider;

        public Type Owner { get; }

        public event EventHandler? InternalsBuilt;

        public Internals(Type owner)
        {
            Owner = owner;
        }

        public object GetService(Type serviceType)
        {
            if (_serviceProvider == null)
            {
                throw new InvalidOperationException("Service provider has not been built yet.");
            }

            return _serviceProvider!.GetService(serviceType)!;
        }

        public IServiceCollection GetServiceCollection()
        {
            if (_serviceCollection == null)
            {
                throw new InvalidOperationException("Service provider has already been built.");
            }

            return _serviceCollection;
        }

        public void Build()
        {
            _serviceProvider ??= _serviceCollection!.BuildServiceProvider();
            _serviceCollection = null;
            InternalsBuilt?.Invoke(this, EventArgs.Empty);
        }

        public bool HasService<TServiceInterface>()
        {
            var serviceType = typeof(TServiceInterface);

            if (_serviceCollection != null)
            {
                return _serviceCollection.Any(descriptor => descriptor.ServiceType == serviceType);
            }

            return GetService(serviceType) != null;
        }
    }
}