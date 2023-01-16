﻿using Microsoft.Extensions.DependencyInjection;
using Tycho.Structure.Modules;
using Tycho.Structure.Submodules;

namespace Tycho.DependencyInjection
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddModuleInternals(this ServiceCollection services, Module module)
        {
            services.AddSingleton<IModule>(module.Internals);
            return services;
        }

        public static IServiceCollection AddSubmodules(this ServiceCollection services)
        {
            services.AddSingleton(typeof(ISubmodule<>), typeof(SubmoduleProxy<>));
            return services;
        }
    }
}
