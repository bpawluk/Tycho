using Microsoft.Extensions.DependencyInjection;

namespace Tycho.Persistence.EFCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTychoPersistence<TDbContext>(this IServiceCollection services)
        where TDbContext : TychoDbContext
    {
        services.AddDbContext<TDbContext>();
        return services;
    }
}