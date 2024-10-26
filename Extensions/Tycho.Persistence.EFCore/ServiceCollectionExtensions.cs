using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tycho.Events.Publishing;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.Serialization;
using Tycho.Persistence.EFCore.UoW;
using Tycho.Persistence.InMemory;

namespace Tycho.Persistence.EFCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTychoPersistence<TDbContext>(this IServiceCollection services)
        where TDbContext : TychoDbContext
    {
        services.AddDbContext<TDbContext>();

        var inMemoryOutbox = services.FirstOrDefault(s => s.ServiceType == typeof(InMemoryOutbox));
        if (inMemoryOutbox != null)
        {
            services.Remove(inMemoryOutbox);
        }

        services.TryAddSingleton<OutboxConsumerSettings>();
        services.AddScoped<IUnitOfWork, UnitOfWork>()
                .AddTransient<IOutboxWriter, OutboxWriter>()
                .AddTransient<IOutboxConsumer, OutboxConsumer>()
                .AddTransient<IPayloadSerializer, PayloadSerializer>()
                .AddTransient<IUncommittedEventPublisher, EventPublisher>();

        return services;
    }
}