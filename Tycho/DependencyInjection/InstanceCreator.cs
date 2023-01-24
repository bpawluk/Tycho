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

        public T CreateInstance<T>(params object[] parameters) where T : class
        {
            return ActivatorUtilities.CreateInstance<T>(_serviceProvider, parameters);
        }
    }
}
