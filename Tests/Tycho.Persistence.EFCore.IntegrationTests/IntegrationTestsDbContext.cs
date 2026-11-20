using Microsoft.EntityFrameworkCore;

namespace Tycho.Persistence.EFCore.IntegrationTests;

internal sealed class IntegrationTestsDbContext(DbContextOptions options) : TychoDbContext(options)
{
    public override string InboxTableName => "TestInbox";
    public override string OutboxTableName => "TestOutbox";
}
