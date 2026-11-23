using System;
using System.Linq;
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
        /// <param name="interceptorType">A concrete open generic class with two type parameters implementing IRequestInterceptor with those parameters in the same order.</param>
        /// <exception cref="ArgumentNullException">A required argument is null.</exception>
        /// <exception cref="ArgumentException">The interceptor type does not have the required generic shape.</exception>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddRequestInterceptor(this IServiceCollection services, Type interceptorType)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (interceptorType == null)
            {
                throw new ArgumentNullException(nameof(interceptorType));
            }

            if (!IsValidInterceptorType(interceptorType))
            {
                throw new ArgumentException("The interceptor must be a concrete open generic class implementing IRequestInterceptor<TRequest, TResponse> with its two type parameters in that order.", nameof(interceptorType));
            }

            services.AddScoped(typeof(IRequestInterceptor<,>), interceptorType);
            return services;
        }

        private static bool IsValidInterceptorType(Type interceptorType)
        {
            if (!interceptorType.IsClass || interceptorType.IsAbstract || !interceptorType.IsGenericTypeDefinition)
            {
                return false;
            }

            Type[] typeParameters = interceptorType.GetGenericArguments();
            if (typeParameters.Length != 2)
            {
                return false;
            }

            return interceptorType.GetInterfaces().Any(contract =>
                contract.IsGenericType &&
                contract.GetGenericTypeDefinition() == typeof(IRequestInterceptor<,>) &&
                contract.GetGenericArguments().SequenceEqual(typeParameters));
        }
    }
}
