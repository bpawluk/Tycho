using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Tycho.Events.Inbox;
using Tycho.Events.Outbox;
using Tycho.Persistence.EFCore.Common;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.Retention;
using Tycho.Persistence.EFCore.Transactions;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore;

/// <summary>
/// Extension methods for setting up Tycho persistence with Entity Framework Core.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Sets up Tycho persistence in the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The service collection to add the persistence functionality to.</param>
    /// <typeparam name="TDbContext">The type of the TychoDbContext to be used.</typeparam>
    public static IServiceCollection AddTychoPersistence<TDbContext>(this IServiceCollection services)
        where TDbContext : TychoDbContext
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddDbContext<TDbContext>()
                .AddSingleton<PersistenceOwner>()
                .AddScoped<TychoDbContext>(sp => sp.GetRequiredService<TDbContext>())
                .AddScoped<ITransaction, Transaction>()
                .AddTransient<IOutboxWriter, OutboxWriter>()
                .AddTransient<IOutboxConsumer, OutboxConsumer>()
                .AddTransient<IInboxWriter, InboxWriter>()
                .AddTransient<IInboxConsumer, InboxConsumer>();
        return services;
    }

    /// <summary>
    /// Enables automatic cleanup of processed inbox and outbox entries in the specified IServiceCollection.
    /// By default, clears completed inbox payloads and deletes completed outbox entries after seven days.
    /// </summary>
    /// <param name="services">The service collection to add the retention functionality to.</param>
    /// <param name="configure">An optional callback overriding the retention defaults.</param>
    public static IServiceCollection AddTychoPersistenceRetention(
        this IServiceCollection services,
        Action<PersistenceRetentionOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new PersistenceRetentionOptions();
        configure?.Invoke(options);
        options.Validate();
        services.Replace(ServiceDescriptor.Singleton(options));

        services.TryAddScoped<IInboxCleaner, InboxCleaner>();
        services.TryAddScoped<IOutboxCleaner, OutboxCleaner>();

        // Repeated registration replaces configuration without scheduling duplicate workers.
        for (int i = services.Count - 1; i >= 0; i--)
        {
            if (services[i].ServiceType == typeof(IHostedService) &&
                services[i].ImplementationType == typeof(PersistenceRetentionService))
            {
                services.RemoveAt(i);
            }
        }

        if (options.IsEnabled)
        {
            services.AddHostedService<PersistenceRetentionService>();
        }

        return services;
    }
}
