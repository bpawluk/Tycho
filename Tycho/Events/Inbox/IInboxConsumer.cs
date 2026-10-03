using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events.Inbox
{
    internal interface IInboxConsumer
    {
        Task<InboxEvent?> TryReadAsync(CancellationToken cancellationToken = default);

        Task<bool> MarkAsHandledAsync(Guid claimId, CancellationToken cancellationToken = default);

        Task<bool> MarkAsFailedAsync(Guid claimId, CancellationToken cancellationToken = default);
    }
}
