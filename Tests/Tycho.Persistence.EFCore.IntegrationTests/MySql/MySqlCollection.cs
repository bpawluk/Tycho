using Tycho.Persistence.EFCore.TestUtils.Databases;

namespace Tycho.Persistence.EFCore.IntegrationTests.MySql;

[CollectionDefinition]
public sealed class MySqlCollection : ICollectionFixture<MySqlDatabase>;

public sealed class MySqlDatabase(DatabaseServers servers) : IntegrationTestsDatabaseBase(servers, DatabaseProvider.MySql);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class InboxConsumerTests(MySqlDatabase database) : Common.Inbox.InboxConsumerTests(database);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class InboxWriterTests(MySqlDatabase database) : Common.Inbox.InboxWriterTests(database);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class ClaimInboxEntryCommandTests(MySqlDatabase database) : Common.Operations.ClaimInboxEntryCommandTests(database);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class ClaimOutboxEntryCommandTests(MySqlDatabase database) : Common.Operations.ClaimOutboxEntryCommandTests(database);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class ClearInboxPayloadsCommandTests(MySqlDatabase database) : Common.Operations.ClearInboxPayloadsCommandTests(database);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class ClearOutboxPayloadsCommandTests(MySqlDatabase database) : Common.Operations.ClearOutboxPayloadsCommandTests(database);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class CompleteInboxClaimCommandTests(MySqlDatabase database) : Common.Operations.CompleteInboxClaimCommandTests(database);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class CompleteOutboxClaimCommandTests(MySqlDatabase database) : Common.Operations.CompleteOutboxClaimCommandTests(database);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class DeleteInboxEntriesCommandTests(MySqlDatabase database) : Common.Operations.DeleteInboxEntriesCommandTests(database);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class DeleteOutboxEntriesCommandTests(MySqlDatabase database) : Common.Operations.DeleteOutboxEntriesCommandTests(database);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class FailInboxClaimCommandTests(MySqlDatabase database) : Common.Operations.FailInboxClaimCommandTests(database);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class FailOutboxClaimCommandTests(MySqlDatabase database) : Common.Operations.FailOutboxClaimCommandTests(database);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class FindInboxEntryQueryTests(MySqlDatabase database) : Common.Operations.FindInboxEntryQueryTests(database);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class InsertInboxEntryCommandTests(MySqlDatabase database) : Common.Operations.InsertInboxEntryCommandTests(database);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class StageOutboxEntriesCommandTests(MySqlDatabase database) : Common.Operations.StageOutboxEntriesCommandTests(database);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class OutboxConsumerTests(MySqlDatabase database) : Common.Outbox.OutboxConsumerTests(database);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class OutboxWriterTests(MySqlDatabase database) : Common.Outbox.OutboxWriterTests(database);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class PersistenceRetentionServiceTests(MySqlDatabase database) : Common.Retention.PersistenceRetentionServiceTests(database);

[Collection<MySqlCollection>]
[Trait("DatabaseProvider", nameof(DatabaseProvider.MySql))]
public sealed class TransactionTests(MySqlDatabase database) : Common.Transactions.TransactionTests(database);
