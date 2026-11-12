using Microsoft.Extensions.Logging;
using Tycho.Events.Inbox;
using Tycho.Events.Outbox;
using Tycho.Events.Serialization;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.Transactions;

namespace Tycho.Persistence.EFCore.TestUtils.Other;

// Keep the existing behavioral tests on real EF operations after extracting them
// from consumers and writers; no database operation is mocked here.
internal static class PersistenceTestOperations
{
    internal static InboxConsumer CreateInboxConsumer(
        IEventSerializer serializer, TychoDbContext context, PersistenceOwner owner,
        InboxConsumerSettings settings, ILogger<InboxConsumer>? logger = null) =>
        new(serializer, context, owner, settings, logger);

    internal static OutboxConsumer CreateOutboxConsumer(
        TychoDbContext context, PersistenceOwner owner, OutboxConsumerSettings settings,
        ILogger<OutboxConsumer>? logger = null) =>
        new(context, owner, settings, logger);

    internal static InboxWriter CreateInboxWriter(InboxActivity activity, TychoDbContext context, PersistenceOwner owner) =>
        new(activity, context, owner);

    internal static OutboxWriter CreateOutboxWriter(
        IEventSerializer serializer, OutboxActivity activity, TychoDbContext context, PersistenceOwner owner) =>
        new(serializer, activity, context, owner);

    internal static Transaction CreateTransaction(TychoDbContext context, ILogger<Transaction>? logger = null) =>
        new(context, logger);
}
