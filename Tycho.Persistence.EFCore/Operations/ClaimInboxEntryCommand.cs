using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tycho.Persistence.EFCore.Database;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Transactions;
using Default = Tycho.Persistence.EFCore.Operations.Implementations.Default;
using MySql = Tycho.Persistence.EFCore.Operations.Implementations.MySql;
using PostgreSql = Tycho.Persistence.EFCore.Operations.Implementations.PostgreSql;
using SqlServer = Tycho.Persistence.EFCore.Operations.Implementations.SqlServer;

namespace Tycho.Persistence.EFCore.Operations;

internal static class ClaimInboxEntryCommand
{
    private delegate Task<ClaimResult<InboxEntry>> Command(
        TychoDbContext dbContext,
        PersistenceOwner owner,
        ClaimRequest request,
        CancellationToken cancellationToken);

    private static readonly ProviderDispatcher<Command> s_dispatcher = new(
        Default.ClaimInboxEntryCommand.ExecuteAsync,
        new Dictionary<string, Command>
        {
            [Providers.MySql] = MySql.ClaimInboxEntryCommand.ExecuteAsync,
            [Providers.PostgreSql] = PostgreSql.ClaimInboxEntryCommand.ExecuteAsync,
            [Providers.SqlServer] = SqlServer.ClaimInboxEntryCommand.ExecuteAsync
        });

    public static Task<ClaimResult<InboxEntry>> ExecuteAsync(
        TychoDbContext dbContext,
        PersistenceOwner owner,
        ClaimRequest request,
        CancellationToken cancellationToken = default)
    {
        if (ExecutionStrategy.Current?.RetriesOnFailure == true)
            throw new InvalidOperationException("Claiming cannot execute inside an active retrying EF Core execution strategy.");

        return new NonRetryingScopeExecutionStrategy(dbContext).ExecuteAsync(
            token => s_dispatcher.Resolve(dbContext)(dbContext, owner, request, token),
            cancellationToken);
    }
}
