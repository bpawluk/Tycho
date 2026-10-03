using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tycho.Events;

namespace Tycho
{
    /// <summary>
    /// Extension methods for configuring Tycho.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures Tycho event processing.
        /// </summary>
        /// <param name="services">The application or module service collection.</param>
        /// <param name="configure">The callback configuring inbox, outbox, and serialization settings.</param>
        public static IServiceCollection ConfigureTychoEventProcessing(
            this IServiceCollection services,
            Action<EventProcessingOptions> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var options = new EventProcessingOptions();
            configure(options);
            EventProcessingOptions snapshot = options.Copy();
            snapshot.Validate();
            services.Replace(ServiceDescriptor.Transient(_ => snapshot.Inbox.Copy()));
            services.Replace(ServiceDescriptor.Transient(_ => snapshot.Outbox.Copy()));
            services.Replace(ServiceDescriptor.Transient(_ => snapshot.PayloadSerializer.Copy()));

            return services;
        }
    }
}
