using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Persistence;

internal class OrderingDbContext : TestDbContext<OrderingDbContext>
{
    public DbSet<Order> Orders { get; set; } = null!;
}
