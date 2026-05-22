using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;

namespace Tycho.Events.Inbox
{
    internal interface IInboxConsumer
    {
        Task<IReadOnlyCollection<InboxEvent>> Read(int count, CancellationToken cancellationToken = default);

        Task<bool> MarkAsHandled(Guid eventId, Guid claimId, CancellationToken cancellationToken = default);

        Task<bool> MarkAsFailed(Guid eventId, Guid claimId, CancellationToken cancellationToken = default);
    }
}
