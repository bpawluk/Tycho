using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.Transactions;
using System.Threading;
using System.Threading.Tasks;
using Default = Tycho.Persistence.EFCore.Operations.Implementations.Default;
using PostgreSql = Tycho.Persistence.EFCore.Operations.Implementations.PostgreSql;
using SqlServer = Tycho.Persistence.EFCore.Operations.Implementations.SqlServer;
using MySql = Tycho.Persistence.EFCore.Operations.Implementations.MySql;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Database;
using Tycho.Persistence.EFCore.Entries;

namespace Tycho.Persistence.EFCore.Operations;

internal static class ClaimOutboxEntryCommand
{
    private delegate Task<ClaimResult<OutboxEntry>> Command(
        TychoDbContext dbContext,
        PersistenceOwner owner,
        ClaimRequest request,
        CancellationToken cancellationToken);

    private static readonly ProviderDispatcher<Command> s_dispatcher = new(
        Default.ClaimOutboxEntryCommand.ExecuteAsync,
        new Dictionary<string, Command>
        {
            [Providers.MySql] = MySql.ClaimOutboxEntryCommand.ExecuteAsync,
            [Providers.PostgreSql] = PostgreSql.ClaimOutboxEntryCommand.ExecuteAsync,
            [Providers.SqlServer] = SqlServer.ClaimOutboxEntryCommand.ExecuteAsync
        });

    internal static Task<ClaimResult<OutboxEntry>> ExecuteAsync(
        TychoDbContext dbContext,
        PersistenceOwner owner,
        ClaimRequest request,
        CancellationToken cancellationToken = default)
    {
        if (ExecutionStrategy.Current?.RetriesOnFailure == true)
        {
            throw new InvalidOperationException("Claiming cannot execute inside an active retrying EF Core execution strategy.");
        }

        return new NonRetryingScopeExecutionStrategy(dbContext)
            .ExecuteAsync(
                token => s_dispatcher.Resolve(dbContext)(dbContext, owner, request, token),
                cancellationToken);
    }
}
