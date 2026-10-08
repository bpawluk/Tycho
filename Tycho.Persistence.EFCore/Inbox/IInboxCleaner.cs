using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Persistence.EFCore.Inbox;

internal interface IInboxCleaner
{
    Task<int> CleanPayloadsAsync(DateTime cutoff, CancellationToken cancellationToken);

    Task<int> CleanEntriesAsync(DateTime cutoff, CancellationToken cancellationToken);
}
