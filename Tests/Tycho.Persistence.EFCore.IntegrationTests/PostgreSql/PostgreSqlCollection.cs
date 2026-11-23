using Tycho.Persistence.EFCore.TestUtils.Databases;

namespace Tycho.Persistence.EFCore.IntegrationTests.PostgreSql;

[CollectionDefinition]
public sealed class PostgreSqlCollection : ICollectionFixture<PostgreSqlDatabase>;

public sealed class PostgreSqlDatabase(DatabaseServers servers) : IntegrationTestsDatabaseBase(servers, DatabaseProvider.PostgreSql);

[Collection<PostgreSqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.PostgreSql))]
public sealed class InboxConsumerTests(PostgreSqlDatabase database) : Common.Inbox.InboxConsumerTests(database);

[Collection<PostgreSqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.PostgreSql))]
public sealed class InboxWriterTests(PostgreSqlDatabase database) : Common.Inbox.InboxWriterTests(database);

[Collection<PostgreSqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.PostgreSql))]
public sealed class ClaimInboxEntryCommandTests(PostgreSqlDatabase database) : Common.Operations.ClaimInboxEntryCommandTests(database);

[Collection<PostgreSqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.PostgreSql))]
public sealed class ClaimOutboxEntryCommandTests(PostgreSqlDatabase database) : Common.Operations.ClaimOutboxEntryCommandTests(database);

[Collection<PostgreSqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.PostgreSql))]
public sealed class ClearInboxPayloadsCommandTests(PostgreSqlDatabase database) : Common.Operations.ClearInboxPayloadsCommandTests(database);

[Collection<PostgreSqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.PostgreSql))]
public sealed class ClearOutboxPayloadsCommandTests(PostgreSqlDatabase database) : Common.Operations.ClearOutboxPayloadsCommandTests(database);

[Collection<PostgreSqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.PostgreSql))]
public sealed class CompleteInboxClaimCommandTests(PostgreSqlDatabase database) : Common.Operations.CompleteInboxClaimCommandTests(database);

[Collection<PostgreSqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.PostgreSql))]
public sealed class CompleteOutboxClaimCommandTests(PostgreSqlDatabase database) : Common.Operations.CompleteOutboxClaimCommandTests(database);

[Collection<PostgreSqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.PostgreSql))]
public sealed class DeleteInboxEntriesCommandTests(PostgreSqlDatabase database) : Common.Operations.DeleteInboxEntriesCommandTests(database);

[Collection<PostgreSqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.PostgreSql))]
public sealed class DeleteOutboxEntriesCommandTests(PostgreSqlDatabase database) : Common.Operations.DeleteOutboxEntriesCommandTests(database);

[Collection<PostgreSqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.PostgreSql))]
public sealed class FailInboxClaimCommandTests(PostgreSqlDatabase database) : Common.Operations.FailInboxClaimCommandTests(database);

[Collection<PostgreSqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.PostgreSql))]
public sealed class FailOutboxClaimCommandTests(PostgreSqlDatabase database) : Common.Operations.FailOutboxClaimCommandTests(database);

[Collection<PostgreSqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.PostgreSql))]
public sealed class FindInboxEntryQueryTests(PostgreSqlDatabase database) : Common.Operations.FindInboxEntryQueryTests(database);

[Collection<PostgreSqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.PostgreSql))]
public sealed class InsertInboxEntryCommandTests(PostgreSqlDatabase database) : Common.Operations.InsertInboxEntryCommandTests(database);

[Collection<PostgreSqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.PostgreSql))]
public sealed class OutboxConsumerTests(PostgreSqlDatabase database) : Common.Outbox.OutboxConsumerTests(database);

[Collection<PostgreSqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.PostgreSql))]
public sealed class OutboxWriterTests(PostgreSqlDatabase database) : Common.Outbox.OutboxWriterTests(database);

[Collection<PostgreSqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.PostgreSql))]
public sealed class PersistenceRetentionServiceTests(PostgreSqlDatabase database) : Common.Retention.PersistenceRetentionServiceTests(database);

[Collection<PostgreSqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.PostgreSql))]
public sealed class TransactionTests(PostgreSqlDatabase database) : Common.Transactions.TransactionTests(database);
