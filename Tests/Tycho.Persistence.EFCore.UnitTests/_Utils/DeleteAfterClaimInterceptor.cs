using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Tycho.Persistence.EFCore.UnitTests._Utils;

internal sealed class DeleteAfterClaimInterceptor(string tableName) : DbCommandInterceptor
{
    public bool WasTriggered { get; private set; }

    public override async ValueTask<int> NonQueryExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (!WasTriggered && result == 1 && command.CommandText.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase))
        {
            WasTriggered = true;
            await using DbCommand delete = command.Connection!.CreateCommand();
            delete.Transaction = command.Transaction;
            delete.CommandText = $"DELETE FROM \"{tableName}\"";
            await delete.ExecuteNonQueryAsync(cancellationToken);
        }

        return result;
    }
}
