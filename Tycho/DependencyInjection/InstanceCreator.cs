using Microsoft.Extensions.DependencyInjection;
using System;

namespace Tycho.DependencyInjection;

internal class InstanceCreator : IInstanceCreator
{
    private readonly IServiceProvider _serviceProvider;

    public InstanceCreator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T CreateInstance<T>() where T : class => ActivatorUtilities.CreateInstance<T>(_serviceProvider);
}
