using Microsoft.Extensions.DependencyInjection;
using System;

namespace NewTycho.Structure
{
    internal class ModuleInternals : IServiceProvider
    {
        private IServiceCollection? _serviceCollection = new ServiceCollection();
        private IServiceProvider? _serviceProvider = null;

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
        }

        public object GetService(Type serviceType)
        {
            if (_serviceProvider == null)
            {
                throw new InvalidOperationException("Service provider has not been built yet.");
            }
            return _serviceProvider!.GetService(serviceType)!;
        }
    }
}
