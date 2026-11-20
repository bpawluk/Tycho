using Tycho.Persistence.EFCore.TestUtils.Databases;

namespace Tycho.Persistence.EFCore.IntegrationTests.SqlServer;

[CollectionDefinition]
public sealed class SqlServerCollection : ICollectionFixture<SqlServerDatabase>;

public sealed class SqlServerDatabase(DatabaseServers servers) : IntegrationTestsDatabaseBase(servers, DatabaseProvider.SqlServer);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class InboxConsumerTests(SqlServerDatabase database) : Common.Inbox.InboxConsumerTests(database);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class InboxWriterTests(SqlServerDatabase database) : Common.Inbox.InboxWriterTests(database);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class ClaimInboxEntryCommandTests(SqlServerDatabase database) : Common.Operations.ClaimInboxEntryCommandTests(database);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class ClaimOutboxEntryCommandTests(SqlServerDatabase database) : Common.Operations.ClaimOutboxEntryCommandTests(database);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class ClearInboxPayloadsCommandTests(SqlServerDatabase database) : Common.Operations.ClearInboxPayloadsCommandTests(database);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class ClearOutboxPayloadsCommandTests(SqlServerDatabase database) : Common.Operations.ClearOutboxPayloadsCommandTests(database);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class CompleteInboxClaimCommandTests(SqlServerDatabase database) : Common.Operations.CompleteInboxClaimCommandTests(database);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class CompleteOutboxClaimCommandTests(SqlServerDatabase database) : Common.Operations.CompleteOutboxClaimCommandTests(database);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class DeleteInboxEntriesCommandTests(SqlServerDatabase database) : Common.Operations.DeleteInboxEntriesCommandTests(database);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class DeleteOutboxEntriesCommandTests(SqlServerDatabase database) : Common.Operations.DeleteOutboxEntriesCommandTests(database);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class FailInboxClaimCommandTests(SqlServerDatabase database) : Common.Operations.FailInboxClaimCommandTests(database);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class FailOutboxClaimCommandTests(SqlServerDatabase database) : Common.Operations.FailOutboxClaimCommandTests(database);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class FindInboxEntryQueryTests(SqlServerDatabase database) : Common.Operations.FindInboxEntryQueryTests(database);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class InsertInboxEntryCommandTests(SqlServerDatabase database) : Common.Operations.InsertInboxEntryCommandTests(database);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class StageOutboxEntriesCommandTests(SqlServerDatabase database) : Common.Operations.StageOutboxEntriesCommandTests(database);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class OutboxConsumerTests(SqlServerDatabase database) : Common.Outbox.OutboxConsumerTests(database);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class OutboxWriterTests(SqlServerDatabase database) : Common.Outbox.OutboxWriterTests(database);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class PersistenceRetentionServiceTests(SqlServerDatabase database) : Common.Retention.PersistenceRetentionServiceTests(database);

[Collection<SqlServerCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SqlServer))]
public sealed class TransactionTests(SqlServerDatabase database) : Common.Transactions.TransactionTests(database);
