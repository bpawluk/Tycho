using Tycho.Persistence.EFCore.TestUtils.Databases;

namespace Tycho.Persistence.EFCore.IntegrationTests.SQLite;

[CollectionDefinition]
public sealed class SQLiteCollection : ICollectionFixture<SQLiteDatabase>;

public sealed class SQLiteDatabase(DatabaseServers servers) : IntegrationTestsDatabaseBase(servers, DatabaseProvider.SQLite);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class InboxConsumerTests(SQLiteDatabase database) : Common.Inbox.InboxConsumerTests(database);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class InboxWriterTests(SQLiteDatabase database) : Common.Inbox.InboxWriterTests(database);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class ClaimInboxEntryCommandTests(SQLiteDatabase database) : Common.Operations.ClaimInboxEntryCommandTests(database);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class ClaimOutboxEntryCommandTests(SQLiteDatabase database) : Common.Operations.ClaimOutboxEntryCommandTests(database);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class ClearInboxPayloadsCommandTests(SQLiteDatabase database) : Common.Operations.ClearInboxPayloadsCommandTests(database);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class ClearOutboxPayloadsCommandTests(SQLiteDatabase database) : Common.Operations.ClearOutboxPayloadsCommandTests(database);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class CompleteInboxClaimCommandTests(SQLiteDatabase database) : Common.Operations.CompleteInboxClaimCommandTests(database);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class CompleteOutboxClaimCommandTests(SQLiteDatabase database) : Common.Operations.CompleteOutboxClaimCommandTests(database);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class DeleteInboxEntriesCommandTests(SQLiteDatabase database) : Common.Operations.DeleteInboxEntriesCommandTests(database);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class DeleteOutboxEntriesCommandTests(SQLiteDatabase database) : Common.Operations.DeleteOutboxEntriesCommandTests(database);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class FailInboxClaimCommandTests(SQLiteDatabase database) : Common.Operations.FailInboxClaimCommandTests(database);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class FailOutboxClaimCommandTests(SQLiteDatabase database) : Common.Operations.FailOutboxClaimCommandTests(database);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class FindInboxEntryQueryTests(SQLiteDatabase database) : Common.Operations.FindInboxEntryQueryTests(database);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class InsertInboxEntryCommandTests(SQLiteDatabase database) : Common.Operations.InsertInboxEntryCommandTests(database);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class StageOutboxEntriesCommandTests(SQLiteDatabase database) : Common.Operations.StageOutboxEntriesCommandTests(database);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class OutboxConsumerTests(SQLiteDatabase database) : Common.Outbox.OutboxConsumerTests(database);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class OutboxWriterTests(SQLiteDatabase database) : Common.Outbox.OutboxWriterTests(database);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class PersistenceRetentionServiceTests(SQLiteDatabase database) : Common.Retention.PersistenceRetentionServiceTests(database);

[Collection<SQLiteCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.SQLite))]
public sealed class TransactionTests(SQLiteDatabase database) : Common.Transactions.TransactionTests(database);
