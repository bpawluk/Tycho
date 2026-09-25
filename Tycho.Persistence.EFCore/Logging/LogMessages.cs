using System;
using Microsoft.Extensions.Logging;

namespace Tycho.Persistence.EFCore.Logging;

// Event IDs use PSNN format:
// Project (P): 1 for Tycho, 2 for Tycho.Persistence.EfCore
// Subsystem (S): 0 Lifecycle, 1 Transactions, 2 Retention
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

    // Not defined

    #endregion
}
