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
    /// <param name="configure">An optional callback configuring inbox and outbox consumer settings.</param>
    /// <typeparam name="TDbContext">The type of the TychoDbContext to be used.</typeparam>
    public static IServiceCollection AddTychoPersistence<TDbContext>(
        this IServiceCollection services,
        Action<PersistenceOptions>? configure = null)
        where TDbContext : TychoDbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new PersistenceOptions();
        configure?.Invoke(options);
        PersistenceOptions snapshot = options.Copy();
        snapshot.Validate();
        services.Replace(ServiceDescriptor.Transient(_ => snapshot.InboxConsumer.Copy()));
        services.Replace(ServiceDescriptor.Transient(_ => snapshot.OutboxConsumer.Copy()));

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
        PersistenceRetentionOptions snapshot = options.Copy();
        snapshot.Validate();
        services.Replace(ServiceDescriptor.Transient(_ => snapshot.Copy()));

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

        if (snapshot.IsRetentionEnabled)
        {
            services.AddHostedService<PersistenceRetentionService>();
        }

        return services;
    }
}
