using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Tycho.Persistence.EFCore.Database;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore.UnitTests;

public sealed class TychoDbContextTests
{
    [Theory]
    [InlineData(Providers.Sqlite)]
    [InlineData(Providers.MySql)]
    [InlineData(Providers.PostgreSql)]
    [InlineData(Providers.SqlServer)]
    public void Model_ForProvider_ConfiguresRequiredIndexes(string provider)
    {
        // Arrange
        using var context = new TestDbContext(CreateOptions(provider));

        // Act & Assert
        AssertIndexes(context.Model, needsClaimLookup: provider is Providers.Sqlite or Providers.MySql);
    }

    [Fact]
    public void Model_ForUnknownProvider_KeepsClaimLookupIndex()
    {
        // Arrange
        using var context = new FallbackDbContext(CreateOptions(Providers.Sqlite));

        // Act & Assert
        AssertIndexes(context.Model, needsClaimLookup: true);
    }

    private static DbContextOptions CreateOptions(string provider)
    {
        var builder = new DbContextOptionsBuilder();
        return provider switch
        {
            Providers.Sqlite => builder.UseSqlite("Data Source=:memory:").Options,
            Providers.MySql => builder.UseMySQL("Server=localhost;Database=TychoModel;User Id=root").Options,
            Providers.PostgreSql => builder.UseNpgsql("Host=localhost;Database=TychoModel;Username=postgres").Options,
            Providers.SqlServer => builder.UseSqlServer("Server=localhost;Database=TychoModel;Integrated Security=True").Options,
            _ => throw new ArgumentOutOfRangeException(nameof(provider))
        };
    }

    private static void AssertIndexes(IModel model, bool needsClaimLookup)
    {
        foreach (Type entryType in new[] { typeof(InboxEntry), typeof(OutboxEntry) })
        {
            IEntityType entity = model.FindEntityType(entryType)!;
            Assert.NotNull(entity);
            Assert.Equal(["OwnerId", "EntryId"], entity.FindPrimaryKey()!.Properties.Select(property => property.Name));
            Assert.True(HasIndex(entity, "OwnerId", "State", "Updated"));
            Assert.Equal(needsClaimLookup, HasIndex(entity, "OwnerId", "ClaimId"));
        }
    }

    private static bool HasIndex(IEntityType entity, params string[] properties) =>
        entity.GetIndexes().Any(index => index.Properties.Select(property => property.Name).SequenceEqual(properties));

    private sealed class TestDbContext(DbContextOptions options) : TychoDbContext(options);

    private sealed class FallbackDbContext(DbContextOptions options) : TychoDbContext(options)
    {
        public override DatabaseFacade Database => new FallbackDatabaseFacade(this);
    }

    private sealed class FallbackDatabaseFacade(DbContext context) : DatabaseFacade(context)
    {
        public override string ProviderName => "UnknownProvider";
    }
}
