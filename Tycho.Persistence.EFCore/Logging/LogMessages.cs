using System;
using Microsoft.Extensions.Logging;

namespace Tycho.Persistence.EFCore.Logging;

// Event IDs use PSNN format:
// Project (P): 1 for Tycho, 2 for Tycho.Persistence.EfCore
// Subsystem (S): 0 Lifecycle, 1 Transactions, 2 Retention, 3 Inbox, 4 Outbox
// Event Number (NN): 01-99
internal static partial class LogMessages
{
    #region Lifecycle (0)

    [LoggerMessage(
        EventId = 2001,
        EventName = "PersistenceOwnerConfigured",
        Level = LogLevel.Information,
        Message = "{OwnerInstanceId} persistence configured with key {Key}.")]
    internal static partial void PersistenceOwnerConfigured(this ILogger logger, string ownerInstanceId, string key);

    #endregion

    #region Transactions (1)

    [LoggerMessage(
        EventId = 2101,
        EventName = "TransactionRollbackFailed",
        Level = LogLevel.Error,
        Message = "An error occurred while rolling back the transaction.")]
    internal static partial void TransactionRollbackFailed(this ILogger logger, Exception exception);

    [LoggerMessage(
        EventId = 2102,
        EventName = "TransactionDisposalFailed",
        Level = LogLevel.Error,
        Message = "An error occurred while disposing the transaction.")]
    internal static partial void TransactionDisposalFailed(this ILogger logger, Exception exception);

    #endregion

    #region Retention (2)

    [LoggerMessage(
        EventId = 2201,
        EventName = "RetentionCleanupFailed",
        Level = LogLevel.Error,
        Message = "An error occurred while cleaning up expired messages. Retrying in {RetryDelay}.")]
    internal static partial void RetentionCleanupFailed(this ILogger logger, TimeSpan retryDelay, Exception exception);

    [LoggerMessage(
        EventId = 2202,
        EventName = "RetentionCleanupAffectedEntries",
        Level = LogLevel.Information,
        Message = "{Operation} affected {AffectedCount} {Store} entries before {Cutoff}.")]
    internal static partial void RetentionCleanupAffectedEntries(this ILogger logger, string operation, int affectedCount, string store, DateTime cutoff);

    #endregion

    #region Inbox (3)

    [LoggerMessage(
        EventId = 2301,
        EventName = "InboxClaimedEntryMissing",
        Level = LogLevel.Warning,
        Message = "Inbox claim {ClaimId} succeeded, but the entry could not be read.")]
    internal static partial void InboxClaimedEntryMissing(this ILogger logger, Guid claimId);

    [LoggerMessage(
        EventId = 2302,
        EventName = "InboxMessageStatusUpdateFailed",
        Level = LogLevel.Warning,
        Message = "An error occurred while updating the status of inbox message with ID {EntryId} for claim {ClaimId}.")]
    internal static partial void InboxMessageStatusUpdateFailed(this ILogger logger, Guid entryId, Guid claimId);

    #endregion

    #region Outbox (4)

    [LoggerMessage(
        EventId = 2401,
        EventName = "OutboxClaimedEntryMissing",
        Level = LogLevel.Warning,
        Message = "Outbox claim {ClaimId} succeeded, but the entry could not be read.")]
    internal static partial void OutboxClaimedEntryMissing(this ILogger logger, Guid claimId);

    #endregion
}
