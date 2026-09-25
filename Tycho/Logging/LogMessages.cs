using System;
using Microsoft.Extensions.Logging;

namespace Tycho.Logging
{
    // Event IDs use PSNN format:
    // Project (P): 1 for Tycho, 2 for Tycho.Persistence.EfCore
    // Subsystem (S): 0 Lifecycle, 1 Structure, 2 Requests, 3 Events, 4 Inbox, 5 Outbox
    // Event Number (NN): 01-99
    internal static partial class LogMessages
    {
        #region Lifecycle (0)

        // Not defined

        #endregion

        #region Structure (1)

        // Not defined

        #endregion

        #region Requests (2)

        // Not defined

        #endregion

        #region Events (3)

        // Not defined

        #endregion

        #region Inbox (4)

        [LoggerMessage(
            EventId = 1401,
            EventName = "InboxNotificationFailed",
            Level = LogLevel.Error,
            Message = "An error occurred while handling inbox activity notifications.")]
        internal static partial void InboxNotificationFailed(this ILogger logger, Exception exception);

        [LoggerMessage(
            EventId = 1402,
            EventName = "InboxProcessingFailed",
            Level = LogLevel.Error,
            Message = "An error occurred while processing inbox messages.")]
        internal static partial void InboxProcessingFailed(this ILogger logger, Exception exception);

        [LoggerMessage(
            EventId = 1403,
            EventName = "InboxMessageProcessingFailed",
            Level = LogLevel.Error,
            Message = "An error occurred while processing inbox message with ID {EntryId}.")]
        internal static partial void InboxMessageProcessingFailed(this ILogger logger, Guid entryId, Exception exception);

        [LoggerMessage(
            EventId = 1404,
            EventName = "InboxMessageStatusUpdateFailed",
            Level = LogLevel.Warning,
            Message = "An error occurred while updating the status of inbox message with ID {EntryId} for claim {ClaimId}.")]
        internal static partial void InboxMessageStatusUpdateFailed(this ILogger logger, Guid entryId, Guid claimId);

        [LoggerMessage(
            EventId = 1405,
            EventName = "InboxJobIsMissing",
            Level = LogLevel.Warning,
            Message = "No event assigned for processing. Skipping execution.")]
        internal static partial void InboxJobIsMissing(this ILogger logger);

        #endregion

        #region Outbox (5)

        [LoggerMessage(
            EventId = 1501,
            EventName = "OutboxNotificationFailed",
            Level = LogLevel.Error,
            Message = "Failed to notify an outbox activity subscriber about new entries.")]
        internal static partial void OutboxNotificationFailed(this ILogger logger, Exception exception);

        [LoggerMessage(
            EventId = 1502,
            EventName = "OutboxProcessingFailed",
            Level = LogLevel.Error,
            Message = "An error occurred while processing outbox messages.")]
        internal static partial void OutboxProcessingFailed(this ILogger logger, Exception exception);

        [LoggerMessage(
            EventId = 1503,
            EventName = "OutboxMessageDeliveryFailed",
            Level = LogLevel.Error,
            Message = "An error occurred while delivering outbox message with ID {EntryId}.")]
        internal static partial void OutboxMessageDeliveryFailed(this ILogger logger, Guid entryId, Exception exception);

        [LoggerMessage(
            EventId = 1504,
            EventName = "OutboxMessageStatusUpdateFailed",
            Level = LogLevel.Warning,
            Message = "An error occurred while updating the status of outbox message with ID {EntryId} for claim {ClaimId}.")]
        internal static partial void OutboxMessageStatusUpdateFailed(this ILogger logger, Guid entryId, Guid claimId);

        [LoggerMessage(
            EventId = 1505,
            EventName = "OutboxJobIsMissing",
            Level = LogLevel.Warning,
            Message = "No event assigned for processing. Skipping execution.")]
        internal static partial void OutboxJobIsMissing(this ILogger logger);

        #endregion
    }
}
