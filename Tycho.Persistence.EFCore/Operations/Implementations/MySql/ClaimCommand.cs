using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;

namespace Tycho.Persistence.EFCore.Operations.Implementations.MySql;

internal static class ClaimCommand
{
    public static Task<int> ExecuteAsync(
        TychoDbContext dbContext,
        IEntryTable table,
        PersistenceOwner owner,
        ClaimRequest request,
        CancellationToken cancellationToken)
    {
        string commandText = $$"""
            UPDATE {{table.Name}}
            SET {{table.StateColumn}} = {1},
                {{table.UpdatedColumn}} = {4},
                {{table.AttemptsColumn}} = {{table.AttemptsColumn}} + 1,
                {{table.ClaimIdColumn}} = {5},
                {{table.ClaimExpirationColumn}} = {6}
            WHERE {{table.OwnerIdColumn}} = {0}
                AND ({{table.StateColumn}} = {2}
                    OR ({{table.StateColumn}} = {3} AND {{table.AttemptsColumn}} < {7})
                    OR ({{table.StateColumn}} = {1} AND {{table.AttemptsColumn}} < {7} AND {{table.ClaimExpirationColumn}} < {4}))
            ORDER BY {{table.UpdatedColumn}}, {{table.EntryIdColumn}}
            LIMIT 1
            """;

        return dbContext.Database.ExecuteSqlRawAsync(commandText,
            new object[]
            {
                owner.Identifier,
                (int)EntryState.InProcessing,
                (int)EntryState.New,
                (int)EntryState.Failed,
                request.UtcNow,
                request.ClaimId,
                request.Expiration,
                request.MaxAttempts
            }, cancellationToken);
    }
}
