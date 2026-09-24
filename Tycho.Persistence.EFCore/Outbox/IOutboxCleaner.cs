using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Persistence.EFCore.Outbox;

internal interface IOutboxCleaner
{
    Task<int> CleanPayloadsAsync(DateTime cutoff, CancellationToken cancellationToken);

    Task<int> CleanEntriesAsync(DateTime cutoff, CancellationToken cancellationToken);
}
