using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Publishing;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.Serialization;
using Tycho.Persistence.EFCore.UoW;

namespace Tycho.Persistence.EFCore;

/// <summary>
/// Extension methods for setting up Tycho persistence using Entity Framework Core
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Sets up Tycho persistence in the specified IServiceCollection
    /// </summary>
    /// <typeparam name="TDbContext">The type of the TychoDbContext to be used</typeparam>
    public static IServiceCollection AddTychoPersistence<TDbContext>(this IServiceCollection services)
        where TDbContext : TychoDbContext
    {
        services.AddDbContext<TDbContext>()
                .AddScoped<TychoDbContext, TDbContext>()
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddTransient<IOutboxWriter, OutboxWriter>()
                .AddTransient<IOutboxConsumer, OutboxConsumer>()
                .AddTransient<IPayloadSerializer, PayloadSerializer>()
                .AddTransient<IUncommittedEventPublisher, EventPublisher>();
        return services;
    }
}