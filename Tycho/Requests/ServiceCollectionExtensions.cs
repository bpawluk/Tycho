using System;
using Microsoft.Extensions.DependencyInjection;

namespace Tycho.Requests
{
    /// <summary>
    /// Extension methods for registering request related services in an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a request interceptor in the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection to register the interceptor in.</param>
        /// <param name="interceptorType">The type of the interceptor to register.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddRequestInterceptor(this IServiceCollection services, Type interceptorType)
        {
            services.AddScoped(typeof(IRequestInterceptor<,>), interceptorType);
            return services;
        }
    }
}
