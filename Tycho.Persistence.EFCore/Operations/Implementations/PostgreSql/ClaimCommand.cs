using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;

namespace Tycho.Persistence.EFCore.Operations.Implementations.PostgreSql;

internal static class ClaimCommand
{
    public static async Task<ClaimResult<TEntity>> ExecuteAsync<TEntity>(
        TychoDbContext dbContext,
        IEntryTable table,
        PersistenceOwner owner,
        ClaimRequest request,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        string sql = CreateSql(table);

        IEntityType entity = dbContext.Model.FindEntityType(typeof(TEntity))!;
        using DbCommand parameterFactory = dbContext.Database.GetDbConnection().CreateCommand();
        object[] parameters = CreateParameters(entity, parameterFactory, owner, request);

        List<TEntity> claimedEntries = await dbContext.Set<TEntity>()
            .FromSqlRaw(sql, parameters)
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        TEntity? claimedEntry = claimedEntries.SingleOrDefault();
        return claimedEntry is null
            ? ClaimResult<TEntity>.NotClaimed()
            : ClaimResult<TEntity>.Claimed(claimedEntry);
    }

    private static string CreateSql(IEntryTable table)
    {
        return $$"""
            WITH candidate AS (
                SELECT {{table.OwnerIdColumn}}, {{table.EntryIdColumn}}
                FROM {{table.Name}}
                WHERE {{table.OwnerIdColumn}} = @ownerId
                    AND (
                        {{table.StateColumn}} = @newState
                        OR ({{table.StateColumn}} = @failedState AND {{table.AttemptsColumn}} < @maxAttempts)
                        OR (
                            {{table.StateColumn}} = @inProcessingState
                            AND {{table.AttemptsColumn}} < @maxAttempts
                            AND {{table.ClaimExpirationColumn}} < @utcNow
                        )
                    )
                ORDER BY {{table.UpdatedColumn}}, {{table.EntryIdColumn}}
                LIMIT 1 FOR UPDATE SKIP LOCKED
            )
            UPDATE {{table.Name}} AS target
            SET {{table.StateColumn}} = @inProcessingState,
                {{table.UpdatedColumn}} = @utcNow,
                {{table.AttemptsColumn}} = target.{{table.AttemptsColumn}} + 1,
                {{table.ClaimIdColumn}} = @claimId,
                {{table.ClaimExpirationColumn}} = @claimExpiration
            FROM candidate
            WHERE target.{{table.OwnerIdColumn}} = candidate.{{table.OwnerIdColumn}}
                AND target.{{table.EntryIdColumn}} = candidate.{{table.EntryIdColumn}}
            RETURNING target.*
            """;
    }

    private static object[] CreateParameters(
        IEntityType entity,
        DbCommand parameterFactory,
        PersistenceOwner owner,
        ClaimRequest request)
    {
        return
        [
            CreateParameter("ownerId", "OwnerId", owner.Identifier),
            CreateParameter("newState", "State", EntryState.New),
            CreateParameter("failedState", "State", EntryState.Failed),
            CreateParameter("inProcessingState", "State", EntryState.InProcessing),
            CreateParameter("utcNow", "Updated", request.UtcNow),
            CreateParameter("claimId", "ClaimId", request.ClaimId),
            CreateParameter("claimExpiration", "ClaimExpiration", request.Expiration),
            CreateParameter("maxAttempts", "Attempts", request.MaxAttempts)
        ];

        DbParameter CreateParameter(string name, string propertyName, object value)
        {
            IProperty property = entity.FindProperty(propertyName)!;
            return property.GetRelationalTypeMapping().CreateParameter(parameterFactory, name, value);
        }
    }
}
