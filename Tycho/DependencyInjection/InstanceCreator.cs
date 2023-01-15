using Microsoft.Extensions.DependencyInjection;
using System;

namespace Tycho.DependencyInjection
{
    internal class InstanceCreator : IInstanceCreator
    {
        private readonly IServiceProvider _serviceProvider;

        public InstanceCreator(IServiceProvider? serviceProvider = null)
        {
            _serviceProvider = serviceProvider ?? new StubServiceProvider();
        }

        public T CreateInstance<T>() where T : class => ActivatorUtilities.CreateInstance<T>(_serviceProvider);
    }
}
