using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Persistence;

internal class OrderingDbContext : UseCaseDbContext<OrderingDbContext>
{
    public DbSet<Order> Orders { get; set; } = null!;
}
