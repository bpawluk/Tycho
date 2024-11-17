using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tycho.Events.Publishing;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.Serialization;
using Tycho.Persistence.EFCore.UoW;

namespace Tycho.Persistence.EFCore;

/// <summary>
///     TODO
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTychoPersistence<TDbContext>(this IServiceCollection services)
        where TDbContext : TychoDbContext
    {
        services.TryAddSingleton<OutboxConsumerSettings>();
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